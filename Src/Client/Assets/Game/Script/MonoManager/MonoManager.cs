using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoManager : MonoBehaviour
{
    private void Awake()
    {
        gameObject.AddComponent<LuaManager>(); // 初始化Lua管理器
        gameObject.AddComponent<AssetBundleManager>(); // 初始化加载manifest文件
        gameObject.AddComponent<AssetCheckManager>(); // 资源检测类
        gameObject.AddComponent<LoadingManager>(); // 加载管理
        gameObject.AddComponent<SceneManager>();
    }
}