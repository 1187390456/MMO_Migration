using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resloader
{
    public static T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }
}

// 本地资源
public class AssetManager : Singleton<AssetManager>
{
    public Transform UILayer => GameObject.Find("UILayer").transform;
    public GameObject Root => Resloader.Load<GameObject>("UIs/Root");

    #region login

    public GameObject GameTips => Resloader.Load<GameObject>("UIs/UILogin/Prefabs/GameTips");
    public GameObject Loading => Resloader.Load<GameObject>("UIs/UILogin/Prefabs/Loading");

    #endregion login
}