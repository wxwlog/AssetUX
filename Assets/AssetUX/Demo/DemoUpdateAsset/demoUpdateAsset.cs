using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetUX;

public class demoUpdateAsset : MonoBehaviour {

    public MainUpdater mainUpdater;

    // Use this for initialization
    IEnumerator Start()
    {
        yield return mainUpdater.LoadAllVersionFiles();
        yield return mainUpdater.UpdateFromRemoteAsset();
    }	
	
	// Update is called once per frame
	void Update () {
		
	}
}
