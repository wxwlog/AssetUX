﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class TestXLua : MonoBehaviour {

	// Use this for initialization
	void Start () {
        LuaEnv luaenv = new LuaEnv();
        luaenv.DoString("CS.UnityEngine.Debug.Log('Xlua scene say hello world')");
        luaenv.Dispose();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}