using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AssetUX
{
    public class Settings : ScriptableObject
    {
        private const string SettingsAssetName = "AssetGenerSettings";
        //private const string SettingsAssetPath = "Assets/AssetUpdater/Resources";//原语句;
        private const string SettingsAssetPath = "Assets/AssetUX/Component/AssetGenerated/AssetBundleManager/Resources";
        private const string SettingsAssetExtension = ".asset";

        
        private static Settings instance;
        
        public static Settings Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    instance = Resources.Load(SettingsAssetName) as Settings;
                    if (ReferenceEquals(instance, null))
                    {
                        // If not found, autocreate the asset object.
                        instance = CreateInstance<Settings>();
#if UNITY_EDITOR
                        UnityEditor.AssetDatabase.CreateAsset(instance,
                            Path.Combine(SettingsAssetPath, SettingsAssetName) + SettingsAssetExtension);
#endif
                    }
                }
                return instance;
            }
        }
        [SerializeField] 
        private string _versionNumber = "1.0.0.0904rele";      //版本号;

        [SerializeField]
        private string  _nextVersionNumber = "1.0.0.0905rele";    //下次更新版本号;
        
        [SerializeField] 
        private string _remoteUrl = "http://localhost/";

        [SerializeField] 
        private string _versionFileName = "version_file.bytes";

        [SerializeField]
        private string _relativePath = "1.0.0.0904rele";

        [SerializeField] 
        private string _projectName = "TestGame";  //项目名 Edit wxwlog 2017.9.4

        [SerializeField] 
        private BuildPlatform _currentPlatform = BuildPlatform.OSX;
        
        [SerializeField] 
        private List<string> _loaclBundles = new List<string>();

        public static string ProjectName
        {
            get { return Instance._projectName; }
            set { Instance._projectName = value; }
        }

        public static string NextVersionNumber
        {
            get { return Instance._nextVersionNumber; }
            set { Instance._nextVersionNumber = value; }
        }

        public static string VersionNumber
        {
            get { return Instance._versionNumber; }
            set { Instance._versionNumber = value; }
        }

        public static string RemoteUrl
        {
            get { return Instance._remoteUrl; }
            set { Instance._remoteUrl = value; }
        }

        public static string VersionFileName
        {
            get { return Instance._versionFileName; }
            set { Instance._versionFileName = value; }
        }

        public static string RelativePath
        {
            get { return Instance._relativePath; }
            set { Instance._relativePath = value; }
        }

        public static List<string> LoaclBundles
        {
            get { return Instance._loaclBundles; }
        }

        public static BuildPlatform Platform
        {
            get { return Instance._currentPlatform; }
            set { Instance._currentPlatform = value; }
        }

        public static void SetBundleToLocal(string bundleName)
        {
            if (!Instance._loaclBundles.Contains(bundleName))
            {
                Instance._loaclBundles.Add(bundleName);
            }
        }

        public static void SetBundleToRemote(string bundleName)
        {
            if (Instance._loaclBundles.Contains(bundleName))
            {
                Instance._loaclBundles.Remove(bundleName);
            }
        }
    }
}