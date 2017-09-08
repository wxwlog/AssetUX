using System.Collections;
using System.Collections.Generic;
using XLua;
using UnityEngine;
using AssetBundles;

namespace AssetUX
{
    public class LuaLoader : MonoBehaviour
    {
        private const float _gcInternal = 1f;
        private float _lastGCTime = 0;
        
        public LuaEnv LuaEnv { get; private set; }
        private Dictionary<string, TextAsset> LuaScripts { get; set; }

        public IEnumerator Initialize(string assetBundleName,string assetName)
        {
            LuaEnv = new LuaEnv();
            LuaScripts = new Dictionary<string, TextAsset>();
            
            /* //原语句加载assetbundle里Lua脚本资源
            var operation = _loader.GetLoadBundleOperation(luaScriptBundlePath);//加载资源;
            yield return operation;
            var assets = operation.GetAllAssets<TextAsset>();  //从操作中获取TestAsset
            */
             
            // Load asset from assetBundle.
            AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
            if (request == null)
                yield break;
            yield return StartCoroutine(request);//开启协程;

            string error;            
            Dictionary<string, TextAsset> result = new Dictionary<string, TextAsset>();

            LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(assetBundleName, out error);
            if (bundle != null)
            {
                //assets = bundle.m_AssetBundle.LoadAsset<TextAsset>(assetName);
                var assetPaths = bundle.m_AssetBundle.GetAllAssetNames();
                foreach (var path in assetPaths)
                {
                    TextAsset asset = bundle.m_AssetBundle.LoadAsset<TextAsset>(path);
                    var fileName =  System.IO.Path.GetFileName(path);
                    result.Add(fileName, asset);
                    Debug.Log(fileName);
                }
            }
            else
            {
                Debug.Log("读取脚本bundle失败");
                yield break;
            }

            LuaScripts = result;
            Debug.Log("lua assets count = " + LuaScripts.Count); //Edit wxw 2017.8.11


            LuaEnv.AddLoader(
                (ref string filepath) =>
                {
                    var filePath = filepath.ToLower();
                    Debug.AssertFormat(LuaScripts.ContainsKey(filePath), "lua script [{0}] not exist", filePath);
                    return LuaScripts[filePath].bytes;
                });

            
            //LuaFunction mainLuaFunction = LuaEnv.DoString(LuaScripts["main"].bytes)[0] as LuaFunction;//原来语句
            //LuaFunction mainLuaFunction = XLua.TemplateEngine.LuaTemplate.Compile(LuaEnv,LuaScripts["main"].text);
            //mainLuaFunction.Call(projectName);
        }

        public object[] DoString(string luaFileName, string chunkName)
        {
            var luaString = GetLuaScriptString(luaFileName);
            var result = LuaEnv.DoString(luaString, chunkName);
            return result;
        }
        
        private string GetLuaScriptString(string luaPath)
        {
            Debug.AssertFormat(LuaScripts.ContainsKey(luaPath.ToLower()), "Can't find lua file [{0}]", luaPath);
            return LuaScripts[luaPath.ToLower()].text;
        }
        
        private void Update()
        {
            if (LuaEnv != null && Time.time - _lastGCTime > _gcInternal)
            {
                LuaEnv.Tick();
                _lastGCTime = Time.time;
            }
        }
    }
}