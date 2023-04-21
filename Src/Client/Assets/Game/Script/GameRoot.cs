using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Networking;
using XLua;
using AssetBundleTool;
using CustomTools;

[LuaCallCSharp]
public class GameRoot : MonoSingleton<GameRoot>
{
    private void Awake()
    {
        // GameStart();
    }

    //  游戏初始化
    private IEnumerator InitGame()
    {
        /*     StartCoroutine(DownLoadRes());    // 下载资源进行对比

             yield return new WaitUntil(() => updateDone);*/

        yield return new WaitUntil(() => File.Exists(PathUtil.GetAssetBundleOutPath() + "/Lua/LGameInit.Lua"));
        // 游戏开始逻辑
        GameStart();

        yield return null;
    }

    public void TestFunc()
    {
        StartCoroutine(AssetBundleManager.Instance.StarLoadAsset("main", "UI", "Update", (value) =>
        {
            var prefabs = value as GameObject;
            var canvas = GameObject.Find("Canvas");
            var target = Instantiate(prefabs, canvas.transform);
            target.transform.Find("Content").GetComponent<Text>().text = "还没更新呢";
        }));
    }

    public void TestFunc1(string name, float process)
    {
        Debug.Log($"{name}-{process}");
        /*  StartCoroutine(AssetBundleManager.Instance.StarLoadAsset("main", "UI", "Update", (value) =>
          {
              var prefabs = value as GameObject;
              var canvas = GameObject.Find("Canvas");
              var target = Instantiate(prefabs, canvas.transform);
              target.transform.Find("Content").GetComponent<Text>().text = "更新完成";
          }));*/
    }

    // 游戏开始
    private void GameStart()
    {
        // 初始化lua
        StartCoroutine(LuaManager.Instance.DoString("/Lua/LGameInit.Lua", "require 'LGameInit'"));
        StartCoroutine(LuaManager.Instance.CallLuaFunction("/Lua/LGameInit.Lua", "LGameInit", "Init"));
    }
}