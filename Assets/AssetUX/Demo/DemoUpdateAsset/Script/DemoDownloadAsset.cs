using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AssetUX;

public class DemoDownloadAsset : MonoBehaviour
{

    public MainUpdater mainUpdater;
    public Slider ProgressSlider;
    public Text versionText;
    private UpdateOperation _updateOperation;
    // Use this for initialization
    IEnumerator Start()
    {
        RemoteLog.Instance.Start("192.168.10.15",2010);//远程日志输出;
        do
        {
            yield return mainUpdater.LoadAllVersionFiles();
            

            if (mainUpdater.State == 1) //有新更新;
            {
                Debug.Log("开始从服务器是下载资源，版本：" + mainUpdater.NextVersionNum);
                versionText.text = "更新版本" +mainUpdater.NextVersionNum;            
    
                _updateOperation = mainUpdater.UpdateFromRemoteAsset();
                yield return _updateOperation;

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
        if (_updateOperation != null)
        {
            ProgressSlider.value = _updateOperation.TotalProgress;
            Debug.Log("进度：" + _updateOperation.TotalProgress * 100 + "%");
        }
	}
}
