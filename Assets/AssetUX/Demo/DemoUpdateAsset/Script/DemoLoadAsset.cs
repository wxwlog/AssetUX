using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using zcode.AssetBundlePacker;
using UnityEngine.SceneManagement;

public class DemoLoadAsset : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //设定资源加载模式为仅加载AssetBundle资源
        ResourcesManager.LoadPattern = new AssetBundleLoadPattern();

        //设定场景加载模式为仅加载AssetBundle资源
        SceneResourcesManager.LoadPattern = new AssetBundleLoadPattern();

	}

    //------------------------------------------------------------------------
    //-------------------------1、加载文本---------------------------------------
    
    public void LoadTextFile()
    {
        //const string TEXT_FILE = "Assets/Resources/Version_1/Text/Text.txt"; //原语句
        string TEXT_FILE = Application.persistentDataPath + "/Windows/" + "hello_text"; 
        string text_content_ = null;

        Debug.Log(TEXT_FILE);

        AssetBundleManager.Instance.Launch(); //启动 Edit wxwlog 2017.8.24
        Debug.Log("AssetBundleManager 是否准备好：" + AssetBundleManager.Instance.IsReady);

        TextAsset text_asset = AssetBundleManager.Instance.LoadAsset<TextAsset>("hello_text"); // ResourcesManager.Load<TextAsset>(TEXT_FILE);

        //AssetBundle ab =  AssetBundle.LoadFromFile(TEXT_FILE);
        //TextAsset text_asset = ab.LoadAsset<TextAsset>(ab.name);
        if (text_asset != null)
        {
            text_content_ = text_asset.text;

            if (!string.IsNullOrEmpty(text_content_))
            {
                GUI.Label(new Rect(0f, 100f, Screen.width, 60f), text_content_);
            }
        }
        else
        {
            Debug.Log("test_asset 为空"); //Edit wxw 2017.8.24
        }
    }
    //------------------------------------------------------------------------

    //------------------------------------------------------------------------
    //------------------------------2、加载纹理-------------------------------
    const string TEXTURE_FILE = "Assets/Resources/Version_1/Texture/Tex_1.png";
    Texture2D texture_ = null;
    /// <summary>
    /// 
    /// </summary>
    void LoadTexture()
    {
        texture_ = ResourcesManager.Load<Texture2D>(TEXTURE_FILE);
    }
    //------------------------------------------------------------------------


    //---------------------------------------------------------------------------------------------------------
    //---------------------------------3、加载模型---------------------------------------
    const string MODEL_FILE = "Assets/AssetBundlePacker-Examples/Cache/Resources/Version_1/Models/Cube.prefab";
    GameObject model_;
    void LoadModel()
    {
        GameObject prefab = ResourcesManager.Load<GameObject>(MODEL_FILE);
        if (prefab != null)
        {
            model_ = GameObject.Instantiate(prefab);
            model_.transform.position = Vector3.zero;
        }
    }
    //------------------------------------------------------------------------

    //------------------------------------------------------------------------
    //----------------------------4、加载场景---------------------------------
    const string SCENE_FILE = "SimpleScene";//原语句
    //const string SCENE_FILE = "Assets/AssetBundlePacker-Examples/Cache/Scenes/SimpleScene.unity";//测试
    string original_scene;
   
    void LoadScene()
    {
        original_scene = SceneManager.GetActiveScene().name;

        Debug.Log("original_scene = " + original_scene);

        SceneResourcesManager.LoadSceneAsync(SCENE_FILE);
    }
    //------------------------------------------------------------------------

	// Update is called once per frame
	void Update () {
		
	}
}
