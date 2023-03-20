using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class AssetManager : Singleton<AssetManager>
{
    [LuaCallCSharp]
    public void LoadSceneCallBack(string name, float process)
    {
        Debug.Log(name);
        Debug.Log(process);
    }

    public void InstanAndSetLayer(GameObject prefab, int layer)
    {
        var go = GameObject.Instantiate(prefab);
        go.transform.SetSiblingIndex(layer);
    }
}