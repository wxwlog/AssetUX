using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using AssetBundles;


public class TestXLua : MonoBehaviour {

    public string abundleScriptName;
    public string scriptName;
    public LuaEnv luaenv = null;

	void Start () {
        luaenv = new LuaEnv();
        luaenv.DoString("CS.UnityEngine.Debug.Log('Xlua scene say hello world')");
        //luaenv.Dispose();

        StartCoroutine(LoadScript(abundleScriptName, scriptName));

	}




    protected IEnumerator LoadScript(string assetBundleName, string assetName)
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
            string tempStr = bundle.m_AssetBundle.LoadAsset<TextAsset>(assetName).text;
            luaenv.DoString(tempStr);
        }


        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (bundle == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");

    }


	// Update is called once per frame
	void Update () {
		
	}
}
