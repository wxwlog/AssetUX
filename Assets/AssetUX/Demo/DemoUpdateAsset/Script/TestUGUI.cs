using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUGUI : MonoBehaviour {

    
    public Text showText = null;
    public GameObject canvas;
    public GameObject canvas_instance;

    Text[] GUI_Texts;
	// Use this for initialization
	void Start () {
		
        //实例化Canvas
        canvas = (GameObject)Resources.Load("Profabs/Canvas"); //预制物体放Resources目录下

        if (canvas != null)
        {
            //参数一：是预设 参数二：实例化预设的坐标  参数三：实例化预设的旋转角度;
            canvas_instance = (GameObject)Instantiate(canvas, transform.position, transform.rotation);

            //获取UGUI子元素 Button
            
            Button[] buttons = canvas_instance.transform.GetComponentsInChildren<Button>(true);
            foreach (Button bu in buttons)      //绑定按钮事件;
            {
                if (bu.name == "Button")
                {
                    bu.onClick.AddListener(ShowClickText);
                }
                if (bu.name == "Return Button")
                {
                    bu.onClick.AddListener(ReturnUpScene); 
                }
            }

            GUI_Texts = canvas_instance.transform.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < GUI_Texts.Length;i++ )
            {
                Text te = GUI_Texts[i];
                if (te.name == "Show Text")
                {
                    showText = GUI_Texts[i];
                        //showText = canvas_instance.Find("Show Text");
                    Debug.Log("找到Show Text");
                }


            }
            
        }
        else
        {
            Debug.Log("资源物体为" + canvas);
        }
      

	}


    public void ShowClickText()
    {

        if (showText == null)
        {
            Debug.Log("showText是null");
            return;
        }

        if (showText.text == "")
        {
            showText.text = "Hello TestUGUI";
        }
        else
        {
            showText.text = "";
        }
        Debug.Log(showText.text);
    }

    public void ReturnUpScene()
    {
        Application.LoadLevel("DemoLoadAsset_02");
    }

	// Update is called once per frame
	void Update () {
		
	}
}
