using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AssetBundles;
using UnityEngine.SceneManagement;
using AssetUX;
using System.IO;

public class DemoLoadAsset : MonoBehaviour {

    public string assetBundleModelName;
    public string assetModelName;

    public string assetBundleTextName;
    public string assetTextName;
    public Text showText;

    public string assetBundleImageName;
    public string assetImageName;
    public Image showImage;

    public string assetBundleSceneName;
    public string assetSceneName;
    
    Texture2D texture_ = null;

    // Use this for initialization
    IEnumerator Start()//原语句;
    {
        yield return StartCoroutine(Initialize());
	}


    protected IEnumerator Initialize()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
        DontDestroyOnLoad(gameObject);

        string localUrl="";

        /*
#if UNITY_ANDROID                //目录分隔符  Windows用"\"，Mac OS用"/"。
       localUrl = "jar:file://"+path+"/"+name;  
#elif UNITY_IPHONE  
       localUrl = path+"/"+name;  
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
        string temp = Application.persistentDataPath + "/" + "AssetUXDemo/Windows";
        localUrl = "file://" +   temp.Replace("/","\\");
        //localUrl = "file://" + "C:\\Users\\B15\\AppData\\LocalLow\\DefaultCompany\\AssetUX\\AssetUXDemo\\Windows"; //例子路径;
#endif */


        //判断persistentDataPath路径有没有文件，有从这个路径读；没有从streamingAssets目录读；
        string temp = "";
        temp = Application.persistentDataPath + "/" + Settings.ProjectName + "/" + Settings.Platform.ToString();


        //获取指定路径下面的所有资源文件  
        if (System.IO.Directory.Exists(temp))
        {
            DirectoryInfo direction = new DirectoryInfo(temp);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            Debug.Log(files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                Debug.Log("Name:" + files[i].Name);
                //Debug.Log( "FullName:" + files[i].FullName );  
                //Debug.Log( "DirectoryName:" + files[i].DirectoryName );  
            }
        }
        else
        {
            Debug.Log(temp + "目录不存在");
        }

        temp = Application.persistentDataPath + "/" + Settings.ProjectName + "/" + Settings.Platform.ToString() + "/" + Settings.Platform.ToString();
       if (System.IO.File.Exists(temp)) //如果persistentDataPath路径下文件不存在，找streamingAssets目录下文件;
        {
            Debug.Log("persistentDataPath路径找到文件");
            temp = "file://" + Application.persistentDataPath + "/" + Settings.ProjectName + "/" + Settings.Platform.ToString();//重新赋值路径;
                   
        }
        else  //从新赋值路径;
        {
            Debug.Log("查找streamingAssetPath路径");

            temp =  Application.streamingAssetsPath + "/" + Settings.ProjectName + "/" + Settings.Platform.ToString() + "/" + Settings.Platform.ToString();
            if (System.IO.File.Exists(temp)) //还是不存在，退出;
            {
                Debug.Log("streamingAssetPath目录下找不到文件");
                StopCoroutine(Initialize()); //协程没有停止;
                yield break;
                Debug.Log("停止协程");
            }

            temp = Application.streamingAssetsPath + "/" + Settings.ProjectName + "/" + Settings.Platform.ToString();//重新赋值路径;
            //localUrl = temp;
        }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR

        localUrl = "file://" + temp.Replace("/", "\\");
#elif UNITY_ANDROID || UNITY_IOS
        localUrl = temp;
       // Debug.Log("demo load persistentDataPath = " + Application.streamingAssetsPath);

#endif

       

       AssetBundleManager.SetSourceAssetBundleURL(localUrl);
        var request = AssetBundleManager.Initialize(); //初始化;

        if (request != null)
            yield return StartCoroutine(request);
    }

    public void LoadText()
    {
        StartCoroutine(InstantiateTextAsync(assetBundleTextName,assetTextName));
    }
    protected IEnumerator InstantiateTextAsync(string assetBundleName, string assetName)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);//开启协程;

        string error;
        LoadedAssetBundle bundle =  AssetBundleManager.GetLoadedAssetBundle(assetBundleName, out error);
        if (bundle != null)
        {
            showText.text = bundle.m_AssetBundle.LoadAsset<TextAsset>(assetName).text; 
        }


        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (bundle == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");

        //yield return null;
    }

    public void LoadImage()
    {
        StartCoroutine(InstantiateImageAsync(assetBundleImageName, assetImageName));
    }

    protected IEnumerator InstantiateImageAsync(string assetBundleName, string assetName)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);//开启协程;

        string error;
        LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(assetBundleName, out error);
        if (bundle != null)
        {
            texture_ = bundle.m_AssetBundle.LoadAsset<Texture2D>(assetName);                 //用GUI显示;
            
            Sprite st = Sprite.Create(texture_,new Rect(0,0,texture_.width,texture_.height), Vector2.zero);
            showImage.sprite = st;                                                           //用UI Image显示;    
        }


        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (bundle == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");

    }

    void OnGUI()
    {
        OnGUI_LoadTexture();
    }

    void OnGUI_LoadTexture()
    {
        if (texture_ != null)
        {
            GUI.DrawTexture(new Rect(Screen.width/2, Screen.height/2,200f, 200f), texture_);
        }
    }

    public void LoadModel()
    {
        StartCoroutine(InstantiateGameObjectAsync(assetBundleModelName, assetModelName));
    }


    protected IEnumerator InstantiateGameObjectAsync(string assetBundleName, string assetName)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
        if (request == null)
            yield break;
        yield return StartCoroutine(request);//开启协程;

        // Get the asset.
        GameObject prefab = request.GetAsset<GameObject>();//转为GameObject对象;

        if (prefab != null)
            GameObject.Instantiate(prefab);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");


    }

    public void LoadScene()
    {
        StartCoroutine(InitializeLevelAsync(assetSceneName, false));
    }
    protected IEnumerator InitializeLevelAsync(string levelName, bool isAdditive)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load level from assetBundle.
        AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(assetBundleSceneName, levelName, isAdditive);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");
    }


    public void LoadXLuaScene()
    {
        Application.LoadLevel("DemoXLua_03");
    }

}
