using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System;
using AssetUX;
using LitJson;
using System.Security.Cryptography;

namespace AssetBundles
{
	public class BuildScript
	{
		public static string overloadedDevelopmentServerURL = "";
	
		public static void BuildAssetBundles()
		{
			// Choose the output path according to the build target.
			string outputPath = Path.Combine(Utility.AssetBundlesOutputPath,  Utility.GetPlatformName());
			if (!Directory.Exists(outputPath) )
				Directory.CreateDirectory (outputPath);
	
			//@TODO: use append hash... (Make sure pipeline works correctly with it.)
            AssetBundleManifest mf = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            //测试
            Debug.Log(mf.name);

            GenerateVersionFile(mf, "", outputPath, Utility.GetPlatformName(), false);
            //m.Unload(true);
            //m = null;
		}
	
		public static void WriteServerURL()
		{
			string downloadURL;
			if (string.IsNullOrEmpty(overloadedDevelopmentServerURL) == false)
			{
				downloadURL = overloadedDevelopmentServerURL;
			}
			else
			{
				IPHostEntry host;
				string localIP = "";
				host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ip in host.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork)
					{
						localIP = ip.ToString();
						break;
					}
				}
				downloadURL = "http://"+localIP+":7888/";
			}
			
			string assetBundleManagerResourcesDirectory = "Assets/AssetBundleManager/Resources";
			string assetBundleUrlPath = Path.Combine (assetBundleManagerResourcesDirectory, "AssetBundleServerURL.bytes");
			Directory.CreateDirectory(assetBundleManagerResourcesDirectory);
			File.WriteAllText(assetBundleUrlPath, downloadURL);
			AssetDatabase.Refresh();
		}
	
		public static void BuildPlayer()
		{
			var outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
			if (outputPath.Length == 0)
				return;
	
			string[] levels = GetLevelsFromBuildSettings();
			if (levels.Length == 0)
			{
				Debug.Log("Nothing to build.");
				return;
			}
	
			string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
			if (targetName == null)
				return;
	
			// Build and copy AssetBundles.
			BuildScript.BuildAssetBundles();
			WriteServerURL();
	
			BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
			BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
		}
		
		public static void BuildStandalonePlayer()
		{
			var outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
			if (outputPath.Length == 0)
				return;
			
			string[] levels = GetLevelsFromBuildSettings();
			if (levels.Length == 0)
			{
				Debug.Log("Nothing to build.");
				return;
			}
			
			string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
			if (targetName == null)
				return;
			
			// Build and copy AssetBundles.
			BuildScript.BuildAssetBundles();
			BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Utility.AssetBundlesOutputPath) );
			AssetDatabase.Refresh();
			
			BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
			BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
		}
	
		public static string GetBuildTargetName(BuildTarget target)
		{
			switch(target)
			{
			case BuildTarget.Android :
				return "/test.apk";
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "/test.exe";
			case BuildTarget.StandaloneOSXIntel:
			case BuildTarget.StandaloneOSXIntel64:
			case BuildTarget.StandaloneOSXUniversal:
				return "/test.app";
			case BuildTarget.WebPlayer:
			case BuildTarget.WebPlayerStreamed:
			case BuildTarget.WebGL:
				return "";
				// Add more build targets for your own.
			default:
				Debug.Log("Target not implemented.");
				return null;
			}
		}
	
		static void CopyAssetBundlesTo(string outputPath)
		{
			// Clear streaming assets folder.
			FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
			Directory.CreateDirectory(outputPath);
	
			string outputFolder = Utility.GetPlatformName();
	
			// Setup the source folder for assetbundles.
			var source = Path.Combine(Path.Combine(System.Environment.CurrentDirectory, Utility.AssetBundlesOutputPath), outputFolder);
			if (!System.IO.Directory.Exists(source) )
				Debug.Log("No assetBundle output folder, try to build the assetBundles first.");
	
			// Setup the destination folder for assetbundles.
			var destination = System.IO.Path.Combine(outputPath, outputFolder);
			if (System.IO.Directory.Exists(destination) )
				FileUtil.DeleteFileOrDirectory(destination);
			
			FileUtil.CopyFileOrDirectory(source, destination);
		}
	
		static string[] GetLevelsFromBuildSettings()
		{
			List<string> levels = new List<string>();
			for(int i = 0 ; i < EditorBuildSettings.scenes.Length; ++i)
			{
				if (EditorBuildSettings.scenes[i].enabled)
					levels.Add(EditorBuildSettings.scenes[i].path);
			}
	
			return levels.ToArray();
		}



        //增加生成版本文件 Edit wxwlog 2017.8.21
        private static void GenerateVersionFile(AssetBundleManifest manifest, string relativePath, string outputPath, string manifestName,
            bool localLimit)
        {
            VersionInfo versionInfo = new VersionInfo();
            
            /*versionInfo.VersionNum = localLimit
                ? long.Parse(DateTime.Now.ToString("yyMMddHHmmss"))
                : long.Parse(DateTime.Now.ToString("yyMMddHHmmss")) + 1;*/

            versionInfo.VersionNum = Settings.VersionNumber;
            versionInfo.RelativePath = relativePath;

            // fill version file with normal assetbundle infomation
            foreach (var bundle in manifest.GetAllAssetBundles())
            {
                /*if (localLimit && !Settings.LoaclBundles.Contains(bundle)) 原语句
                {
                    continue;
                }*/

                var bytes = File.ReadAllBytes(Path.Combine(outputPath, bundle));
                FillBundleInfo(manifest, bundle, bytes, ref versionInfo);
            }

            // fill version file with assetbundle manifest infomation
            var manifestBytes = File.ReadAllBytes(Path.Combine(outputPath, manifestName));
            FillBundleInfo(manifest, manifestName, manifestBytes, ref versionInfo);

            // write version file with json
            string verJson = JsonMapper.ToJson(versionInfo);
            //File.WriteAllText(Path.Combine(outputPath, Settings.VersionFileName), verJson); //原语句;
            File.WriteAllText(Path.Combine(outputPath,"version_test"), verJson);
        }

        //add function  Edit wxwlog 2017.8.21
        private static void FillBundleInfo(AssetBundleManifest manifest, string name, byte[] bytes, ref VersionInfo versionInfo)
        {
            var bundleInfo = new BundleInfo();

            // write assetbundle normal infomation
            bundleInfo.Name = name;
            bundleInfo.Size = bytes.Length;
            MD5 md5 = new MD5CryptoServiceProvider();
            bundleInfo.Md5Code = Convert.ToBase64String(md5.ComputeHash(bytes));
            bundleInfo.Dependencies = manifest.GetAllDependencies(name);

            // relate assetbundle name with asset path
            var assetBundle = AssetBundle.LoadFromMemory(bytes);
            foreach (var content in assetBundle.GetAllAssetNames())
            {
                versionInfo.BundlePath.Add(content.ToLower(), name);
            }

            foreach (var content in assetBundle.GetAllScenePaths())
            {
                versionInfo.BundlePath.Add(content.ToLower(), name);
            }
            assetBundle.Unload(true);
            // add bundleInfo to versionInfo
            versionInfo.Bundles.Add(bundleInfo);
        }
	}

}