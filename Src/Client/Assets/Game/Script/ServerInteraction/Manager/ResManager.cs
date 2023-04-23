using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResManager : Singleton<ResManager>
{
    // 实例化指定资源地址对象 并返回
    public T InstanGoAndReturn<T>(string path, Transform parent)
    {
        var go = Resources.Load<GameObject>(path);
        GameObject.Instantiate(go, parent);
        return go.GetComponent<T>();
    }

    public GameObject InstanGoAndReturn(string path, Transform parent)
    {
        var go = Resources.Load<GameObject>(path);
        GameObject.Instantiate(go, parent);
        return go;
    }
}