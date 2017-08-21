using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetUX
{
    [Serializable]
    public class BundleInfo
    {
        public string Name;

        public int Size;

        public string Md5Code;

        public string[] Dependencies;

    }
    [Serializable]
    public class VersionInfo
    {
        public long VersionNum = 100000000000;

        public string RelativePath = "";

        public bool IsVersionCompelete = false;

        public List<BundleInfo> Bundles = new List<BundleInfo>();

        public Dictionary<string, string> BundlePath = new Dictionary<string, string>();

        public void UpdateBundle(BundleInfo newBundle)
        {
            var isContain = false;
            foreach (var bundle in Bundles)
            {
                if (bundle.Name == newBundle.Name)
                {
                    Bundles[Bundles.IndexOf(bundle)] = newBundle;
                    isContain = true;
                    break;
                }
            }
            if (!isContain)
            {
                Bundles.Add(newBundle);
            }
        }

        public void UpdateVersion(VersionInfo newVersion)
        {
            VersionNum = newVersion.VersionNum;
            RelativePath = newVersion.RelativePath;
            BundlePath = newVersion.BundlePath;
        }
    }
}