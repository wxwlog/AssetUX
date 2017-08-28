using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;
using UnityEngine.SceneManagement;

public class DemoLoadAsset : MonoBehaviour {

    public string assetBundleName;
    public string assetName;
    
    // Use this for initialization
    IEnumerator Start()
    {

        yield return StartCoroutine(Initialize());


        // Load asset.
        yield return StartCoroutine(InstantiateGameObjectAsync(assetBundleName, assetName));

	}

    protected IEnumerator Initialize()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
        DontDestroyOnLoad(gameObject);

        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project. 
        // 	Another approach would be to make this configurable in the standalone player.)

		// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
        string localUrl;
#if UNITY_ANDROID                //目录分隔符  Windows用"\"，Mac OS用"/"。
       localUrl = "jar:file://"+path+"/"+name;  
#elif UNITY_IPHONE  
       localUrl = path+"/"+name;  
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
        string temp = Application.persistentDataPath + "/" + "AssetUXDemo/Windows";
        localUrl = "file://" +   temp.Replace("/","\\");
        //localUrl = "file://" + "C:\\Users\\B15\\AppData\\LocalLow\\DefaultCompany\\AssetUX\\AssetUXDemo\\Windows"; //例子路径;
#endif 
        
        AssetBundleManager.SetSourceAssetBundleURL(localUrl);
       // AssetBundleManager.isLoadingAsset = true;//设置已经加载资源;

		// Or customize the URL based on your deployment or configuration
		//AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");


        // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();



        if (request != null)
            yield return StartCoroutine(request);
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

}
