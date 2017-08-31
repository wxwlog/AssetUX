using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetUX;

public class DemoDownloadAsset : MonoBehaviour
{

    public MainUpdater mainUpdater;

    // Use this for initialization
    IEnumerator Start()
    {
        yield return mainUpdater.LoadAllVersionFiles();
        yield return mainUpdater.UpdateFromRemoteAsset();
        Application.LoadLevel("DemoLoadAsset_02");
    }	
	
	// Update is called once per frame
	void Update () {
		
	}
}
