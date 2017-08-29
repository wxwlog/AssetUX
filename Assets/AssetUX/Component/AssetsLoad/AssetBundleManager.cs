using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;


/*
    In this demo, we demonstrate:
    1.	Automatic asset bundle dependency resolving & loading.
        It shows how to use the manifest assetbundle like how to get the dependencies etc.
    2.	Automatic unloading of asset bundles (When an asset bundle or a dependency thereof is no longer needed, the asset bundle is unloaded)
    3.	Editor simulation. A bool defines if we load asset bundles from the project or are actually using asset bundles(doesn't work with assetbundle variants for now.)
        With this, you can player in editor mode without actually building the assetBundles.
    4.	Optional setup where to download all asset bundles
    5.	Build pipeline build postprocessor, integration so that building a player builds the asset bundles and puts them into the player data (Default implmenetation for loading assetbundles from disk on any platform)
    6.	Use WWW.LoadFromCacheOrDownload and feed 128 bit hash to it when downloading via web
        You can get the hash from the manifest assetbundle.
    7.	AssetBundle variants. A prioritized list of variants that should be used if the asset bundle with that variant exists, first variant in the list is the most preferred etc.
*/


/*
 这个demo，我们演示了：;
 * 1.自动的 资源捆绑 依赖解析和加载;
 *   它线索如何使用manifest assetbundle来如何获取依赖等等;
 * 2.自动卸载asset bundles (当一个asset bundle 或一个依赖不再需要，这个asset bundle被卸载;)
 * 3.编辑器模拟。你可以在编辑器模式下玩，不用真实建立这个assetBundles;
 * 4.可以设置在哪里下载所有的资源包;
 * 5.构建后台处理程序，集成以便一个玩家建立这个asset bundles和把它们放到玩家数据上（默认从任意平台的磁盘上加载;）
 * 6.使用WWW.loadFromCacheOrDowload 和feed 128位哈希通过web下载;
 *   你可以从manifest assetbundle获取hash;
 * 7.多样的AssetBundle. 优先列表变体被使用，如果asset bundle变体存在,  列表中的第一个变体是最优先等;
 */

namespace AssetBundles
{	
	// Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
    //加载assetBundle引用计数，可以用来自动卸载依赖的assetBundles
	public class LoadedAssetBundle
	{
		public AssetBundle m_AssetBundle;
		public int m_ReferencedCount;
		
		public LoadedAssetBundle(AssetBundle assetBundle)
		{
			m_AssetBundle = assetBundle;
			m_ReferencedCount = 1;
		}
	}
	
	// Class takes care of loading assetBundle and its dependencies automatically, loading variants automatically.
    // 类负责自动的加载assetBundle和它的依赖，自动加载variants
	public class AssetBundleManager : MonoBehaviour
	{
		public enum LogMode { All, JustErrors };
		public enum LogType { Info, Warning, Error };
	
		static LogMode m_LogMode = LogMode.All;
		static string m_BaseDownloadingURL = "";
		static string[] m_ActiveVariants =  {  };
		static AssetBundleManifest m_AssetBundleManifest = null;  //AssetBundle载货单;
	#if UNITY_EDITOR	
		static int m_SimulateAssetBundleInEditor = -1;
		const string kSimulateAssetBundles = "SimulateAssetBundles";
	#endif
	
		static Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle> ();
		static Dictionary<string, WWW> m_DownloadingWWWs = new Dictionary<string, WWW> ();
		static Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string> ();
		static List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation> ();//在进行的操作;
		static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]> ();
        static public bool isLoadingAsset = false;  //自定义，是否已经加载资源;
		public static LogMode logMode
		{
			get { return m_LogMode; }
			set { m_LogMode = value; }
		}
	
		// The base downloading url which is used to generate the full downloading url with the assetBundle names.
        //基本下载URL;
		public static string BaseDownloadingURL
		{
			get { return m_BaseDownloadingURL; }
			set { m_BaseDownloadingURL = value; }
		}
	
		// Variants which is used to define the active variants.
        // 变量用于定义活跃的variants;
		public static string[] ActiveVariants
		{
			get { return m_ActiveVariants; }
			set { m_ActiveVariants = value; }
		}
	
		// AssetBundleManifest object which can be used to load the dependecies and check suitable assetBundle variants.
        // AssetBundle载货单 对象用来加载依赖和检查相配的assetBundle variants
		public static AssetBundleManifest AssetBundleManifestObject
		{
			set {m_AssetBundleManifest = value; }
		}
	
		private static void Log(LogType logType, string text)
		{
			if (logType == LogType.Error)
				Debug.LogError("[AssetBundleManager] " + text);
			else if (m_LogMode == LogMode.All)
				Debug.Log("[AssetBundleManager] " + text);
		}
	
        
	#if UNITY_EDITOR
		// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        // Flag表示我们想在编辑器下模拟assetBundles
		public static bool SimulateAssetBundleInEditor 
		{
			get
			{
				if (m_SimulateAssetBundleInEditor == -1)
					m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;
				
				return m_SimulateAssetBundleInEditor != 0;
			}
			set
			{
				int newValue = value ? 1 : 0;
				if (newValue != m_SimulateAssetBundleInEditor)
				{
					m_SimulateAssetBundleInEditor = newValue;
					EditorPrefs.SetBool(kSimulateAssetBundles, value);
				}
			}
		}
		
	
		#endif 
	
        //获取Streaming资源路径，可读不可写;
		private static string GetStreamingAssetsPath()
		{
			if (Application.isEditor)
				return "file://" +  System.Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
			else if (Application.isWebPlayer)
				return System.IO.Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/")+ "/StreamingAssets";
			else if (Application.isMobilePlatform || Application.isConsolePlatform)
				return Application.streamingAssetsPath;
			else // For standalone player.
				return "file://" +  Application.streamingAssetsPath;
		}
	    
        //设置资源目录;
		public static void SetSourceAssetBundleDirectory(string relativePath)
		{
			BaseDownloadingURL = GetStreamingAssetsPath() + relativePath;
		}
		
        //设置资源URL;
		public static void SetSourceAssetBundleURL(string absolutePath)
		{
            /*
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            BaseDownloadingURL = absolutePath + Utility.GetPlatformName() + "/";//例如  http://192.168.10.15:7888/Android/
#else
            BaseDownloadingURL = absolutePath;//自定义语句
#endif */
            BaseDownloadingURL = absolutePath;//自定义语句
            Debug.Log("Set URL:"+BaseDownloadingURL);
		}
	
        //设置开发资源服务;
		public static void SetDevelopmentAssetBundleServer()
		{
			/* #if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to setup a download URL
            // 如果我们在编辑器模式下，我们不用设置下载URL
			if (SimulateAssetBundleInEditor)
				return;
			#endif  */

            //从Resources目录下加载AssetBundleServerURL文件
			TextAsset urlFile = Resources.Load("AssetBundleServerURL") as TextAsset;
			string url = (urlFile != null) ? urlFile.text.Trim() : null;
			if (url == null || url.Length == 0)
			{
				Debug.LogError("Development Server URL could not be found.");
				//AssetBundleManager.SetSourceAssetBundleURL("http://localhost:7888/" + UnityHelper.GetPlatformName() + "/");
			}
			else
			{
				AssetBundleManager.SetSourceAssetBundleURL(url);
			}
		}
		
		// Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
        //获取加载的AssetBundle， 仅仅当所有依赖下载成功，返回vaild对象;
		static public LoadedAssetBundle GetLoadedAssetBundle (string assetBundleName, out string error)
		{
			if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
				return null;
		
			LoadedAssetBundle bundle = null;
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle == null)
				return null;
			
			// No dependencies are recorded, only the bundle itself is required.
            // 没有依赖记录，只需要bundle本身;
			string[] dependencies = null;
			if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
				return bundle;
			
			// Make sure all dependencies are loaded
            // 确认所有依赖已经加载;
			foreach(var dependency in dependencies)
			{
				if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
					return bundle;
	
				// Wait all the dependent assetBundles being loaded.
                // 等待所有依赖assetBundle被加载
				LoadedAssetBundle dependentBundle;
				m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
				if (dependentBundle == null)
					return null;
			}
	
			return bundle;
		}
        
	    //初始化;
		static public AssetBundleLoadManifestOperation Initialize ()
		{
			return Initialize(Utility.GetPlatformName());
		}
			
	
		// Load AssetBundleManifest.
        // 加载AssetBundle载货单;
		static public AssetBundleLoadManifestOperation Initialize (string manifestAssetBundleName)
		{
	/*#if UNITY_EDITOR
			Log (LogType.Info, "Simulation Mode: " + (SimulateAssetBundleInEditor ? "Enabled" : "Disabled"));
	#endif */
	
			var go = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
			DontDestroyOnLoad(go); //创建AssetBundleManager对象，加载不销毁;

            Debug.Log("manifest assetBundle name: " + manifestAssetBundleName);
/*
	#if UNITY_EDITOR	
			// If we're in Editor simulation mode, we don't need the manifest assetBundle.
            //如果我们在编辑器模拟模式，我们不需要assetBundle载货单;
			if (SimulateAssetBundleInEditor)
				return null;
	#endif */
            

			LoadAssetBundle(manifestAssetBundleName, true);
			var operation = new AssetBundleLoadManifestOperation (manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
			m_InProgressOperations.Add (operation);
			return operation;
		}
		
		// Load AssetBundle and its dependencies.
        // 加载AssetBundle和它的依赖
		static protected void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest = false)
		{
			Log(LogType.Info, "Loading Asset Bundle " + (isLoadingAssetBundleManifest ? "Manifest: " : ": ") + assetBundleName);
	
			if (!isLoadingAssetBundleManifest)
			{
				if (m_AssetBundleManifest == null)
				{
					Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
					return;
				}
			}
	
			// Check if the assetBundle has already been processed.
            // 检查assetBundle是否已经被处理。
			bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);
	
			// Load dependencies.
            // 加载依赖;
			if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
				LoadDependencies(assetBundleName);
		}
		
		// Remaps the asset bundle name to the best fitting asset bundle variant.
        //再交换assetBundleName来装配asset bundle variant
		static protected string RemapVariantName(string assetBundleName)
		{
			string[] bundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();

			string[] split = assetBundleName.Split('.');

			int bestFit = int.MaxValue;
			int bestFitIndex = -1;
			// Loop all the assetBundles with variant to find the best fit variant assetBundle.
            //循环所有assetBundle variant找到最适合的variant assetBundle
			for (int i = 0; i < bundlesWithVariant.Length; i++)
			{
				string[] curSplit = bundlesWithVariant[i].Split('.');
				if (curSplit[0] != split[0])
					continue;
				
				int found = System.Array.IndexOf(m_ActiveVariants, curSplit[1]);
				
				// If there is no active variant found. We still want to use the first 
                //如果没有活动的variant找到，我们仍然使用第一个;
				if (found == -1)
					found = int.MaxValue-1;
						
				if (found < bestFit)
				{
					bestFit = found;
					bestFitIndex = i;
				}
			}
			
			if (bestFit == int.MaxValue-1)
			{
				Debug.LogWarning("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
			}
			
			if (bestFitIndex != -1)
			{
				return bundlesWithVariant[bestFitIndex];
			}
			else
			{
				return assetBundleName;
			}
		}
	
		// Where we actuall call WWW to download the assetBundle.
        //这里我们事实调用WWW下载assetBundle
		static protected bool LoadAssetBundleInternal (string assetBundleName, bool isLoadingAssetBundleManifest)
		{

            Debug.Log("load asset:" + assetBundleName);


			// Already loaded.
            //已经加载过;
			LoadedAssetBundle bundle = null;
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle != null)
			{
				bundle.m_ReferencedCount++;
				return true;
			}
	
			// @TODO: Do we need to consider the referenced count of WWWs?
			// In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
			// But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.

            //我们需要考虑www的引用计数?;
            //在这个demo，我们从来没有重复WWWs ，我们等LoadAssetAsync()/LoadLevelAsync()完成之前，调用其他LoadAssetAsync()/LoadLevelAsync();
            //但实际情况，用户可能调用LoadAssetAsync()/LoadLevelAsync()几次，然后等待它们完成这可能有重复的www。

            if (m_DownloadingWWWs.ContainsKey(assetBundleName) )
				return true;
	
			WWW download = null;
         
            string url = "";
                               //从www是下载;
            url = m_BaseDownloadingURL + "\\" + assetBundleName; //自定义语句;
            // For manifest assetbundle, always download it as we don't have hash for it.
            //因为 manifest assetbundle，已经下载它所以我们不对它hash;


            if (isLoadingAssetBundleManifest )
				download = new WWW(url);
			else
				download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(assetBundleName), 0); 
	

			m_DownloadingWWWs.Add(assetBundleName, download);

            Debug.Log("download RUL: " + url);

            return false;
		}

        //测试保存文件
       public static void SaveAsset2LocalFile(string path, string name, byte[] info, int length)
        {
         /*
            Stream sw = null;
            FileInfo fileInfo = new FileInfo(path + "/" + name);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }

            //如果此文件不存在则创建  
            sw = fileInfo.Create();
            //写入  
            sw.Write(info, 0, length);

            sw.Flush();
            //关闭流  
            sw.Close();
            //销毁流  
            sw.Dispose();
           */

            // 创建文件
            FileStream fs = new FileStream( Application.persistentDataPath+ "/tackTest.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite); //可以指定盘符，也可以指定任意文件名，还可以为word等文件
            StreamWriter sw = new StreamWriter(fs); // 创建写入流
            sw.WriteLine("bob hu"); // 写入Hello World
            sw.Close(); //关闭文件


            Debug.Log(name + "成功保存到本地~");
        }  



		// Where we get all the dependencies and load them all.
        //这里我们获取所有依赖并加载它们;
		static protected void LoadDependencies(string assetBundleName)
		{
			if (m_AssetBundleManifest == null)
			{
				Debug.LogError("Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
				return;
			}
	
			// Get dependecies from the AssetBundleManifest object.
            //从AssetBundle载货单中获取依赖;
			string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
			if (dependencies.Length == 0)
				return;
				
			for (int i=0;i<dependencies.Length;i++)
				dependencies[i] = RemapVariantName (dependencies[i]);//用映射依赖;
				
			// Record and load all dependencies.
            //记录和加载所有依赖;
			m_Dependencies.Add(assetBundleName, dependencies);
			for (int i=0;i<dependencies.Length;i++)
				LoadAssetBundleInternal(dependencies[i], false);
		}
	
		// Unload assetbundle and its dependencies.
        // 卸载assetbundle和它的依赖;
		static public void UnloadAssetBundle(string assetBundleName)
		{
	/*#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
            //如果是编辑模式返回;
			if (SimulateAssetBundleInEditor)
				return;
	#endif */
	
			//Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + assetBundleName);
	
			UnloadAssetBundleInternal(assetBundleName);
			UnloadDependencies(assetBundleName);
	
			//Debug.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + assetBundleName);
		}
	
        //卸载依赖;
		static protected void UnloadDependencies(string assetBundleName)
		{
			string[] dependencies = null;
			if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
				return;
	
			// Loop dependencies.循环依赖
			foreach(var dependency in dependencies)
			{
				UnloadAssetBundleInternal(dependency);
			}
	
			m_Dependencies.Remove(assetBundleName);
		}
	
        //卸载AssetBundle
		static protected void UnloadAssetBundleInternal(string assetBundleName)
		{
			string error;
			LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
			if (bundle == null)
				return;
	
			if (--bundle.m_ReferencedCount == 0)//引用计数自减1
			{
				bundle.m_AssetBundle.Unload(false);//资源卸载;
				m_LoadedAssetBundles.Remove(assetBundleName);//移除这个assetBundleName
	
				Log(LogType.Info, assetBundleName + " has been unloaded successfully");
			}
		}
	
		void Update()
		{
			// Collect all the finished WWWs.
            // 收集所用完成的WWW;
			var keysToRemove = new List<string>();
			foreach (var keyValue in m_DownloadingWWWs)
			{
				WWW download = keyValue.Value;
	
				// If downloading fails.
                // 如果下载失败;
				if (download.error != null)
				{
                    //增加下载错误列表;
                    //keyValue增加到移除列表里;
					m_DownloadingErrors.Add(keyValue.Key, string.Format("Failed downloading bundle {0} from {1}: {2}", keyValue.Key, download.url, download.error));
					keysToRemove.Add(keyValue.Key);
					continue;
				}
	
				// If downloading succeeds.
                // 如果下载成功;
				if(download.isDone)
				{
					AssetBundle bundle = download.assetBundle;
					if (bundle == null)//值为空;
					{
                        //增加下载错误列表;
                        //keyValue增加到移除列表里;
						m_DownloadingErrors.Add(keyValue.Key, string.Format("{0} is not a valid asset bundle.", keyValue.Key));
						keysToRemove.Add(keyValue.Key);
						continue;
					}

                    Debug.Log("down Done :" + bundle.name);

                    /*if (bundle.name == "hello_text") //测试使用
                    {
                        TextAsset t = bundle.LoadAsset<TextAsset>("hello.txt");
                        Debug.Log(t.text);
                    }*/

                    //Debug.Log("Downloading " + keyValue.Key + " is done at frame " + Time.frameCount);
					
                    //把下载的assetBundle加到LoadedAssetBundle字典里;
                    //keyValue增加到移除列表里;
                    m_LoadedAssetBundles.Add(keyValue.Key, new LoadedAssetBundle(download.assetBundle) );
					keysToRemove.Add(keyValue.Key);
				}
			}
	
			// Remove the finished WWWs.
            // 移除完成的WWW
			foreach( var key in keysToRemove)
			{
				WWW download = m_DownloadingWWWs[key];
				m_DownloadingWWWs.Remove(key);
				download.Dispose(); //处理现有的www对象;
			}
	
			// Update all in progress operations
            // 更新所有的程序操作;
			for (int i=0;i<m_InProgressOperations.Count;)
			{
				if (!m_InProgressOperations[i].Update())//没有其他调用;
				{
					m_InProgressOperations.RemoveAt(i);//移除第i个程序操作;
				}
				else
					i++;
			}
		}
	
		// Load asset from the given assetBundle.
        // 从给定的assetBundle中加载资源;
		static public AssetBundleLoadAssetOperation LoadAssetAsync (string assetBundleName, string assetName, System.Type type)
		{
			Log(LogType.Info, "Loading " + assetName + " from " + assetBundleName + " bundle");
	
			AssetBundleLoadAssetOperation operation = null;


			assetBundleName = RemapVariantName (assetBundleName);
			LoadAssetBundle (assetBundleName);
			operation = new AssetBundleLoadAssetOperationFull (assetBundleName, assetName, type);
	
			m_InProgressOperations.Add (operation);

	
			return operation;
		}
	
		// Load level from the given assetBundle.
        // 从指定的assetBundle中加载level
		static public AssetBundleLoadOperation LoadLevelAsync (string assetBundleName, string levelName, bool isAdditive)
		{
			Log(LogType.Info, "Loading " + levelName + " from " + assetBundleName + " bundle");
	
			AssetBundleLoadOperation operation = null;
	/*#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				operation = new AssetBundleLoadLevelSimulationOperation(assetBundleName, levelName, isAdditive);
			}
			else
	#endif */
			{
				assetBundleName = RemapVariantName(assetBundleName);
				LoadAssetBundle (assetBundleName);
				operation = new AssetBundleLoadLevelOperation (assetBundleName, levelName, isAdditive);
	
				m_InProgressOperations.Add (operation);
			}
	
			return operation;
		}
	} // End of AssetBundleManager.
}