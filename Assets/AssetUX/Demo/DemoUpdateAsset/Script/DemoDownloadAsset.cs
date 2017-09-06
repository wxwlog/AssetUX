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
        

        if (mainUpdater.State == 2)
        {
            Debug.Log("读取版本文件发生错误");
        }
        else
        {
            if(mainUpdater.State == 0) //版本相同;不需要更新;
            {
                Application.LoadLevel("DemoLoadAsset_02");
            }
            else
            {
                yield return mainUpdater.UpdateFromRemoteAsset();
                Application.LoadLevel("DemoLoadAsset_02");
            }
            
        }
        
    }	
	
	// Update is called once per frame
	void Update () {
		
	}
}
