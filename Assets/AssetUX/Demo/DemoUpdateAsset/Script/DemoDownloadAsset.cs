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

        do
        {
            yield return mainUpdater.LoadAllVersionFiles();
            

            if (mainUpdater.State == 1) //有新更新;
            {
                Debug.Log("开始从服务器是下载资源");
                yield return mainUpdater.UpdateFromRemoteAsset();
            }    

            yield return null;

            if (mainUpdater.State == 2) //发生错误时退出循环;
                break;

            if (mainUpdater.State == 0) //版本相同时退出循环;
                break;

        } while (true); 

        if (mainUpdater.State == 2)
        {
            Debug.Log("读取版本文件发生错误");
        }
            

        if (mainUpdater.State == 0) //版本相同;不需要更新;
        {
            Application.LoadLevel("DemoLoadAsset_02");
        }
        
    }	
	
	// Update is called once per frame
	void Update () {
		
	}
}
