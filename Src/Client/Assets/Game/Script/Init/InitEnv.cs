using CustomTools;
using Manager;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitEnv : MonoSingleton<InitEnv>
{
    private void Awake()
    {
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(InitLog());    // 初始化日志

        yield return StartCoroutine(AssetCheckManager.Instance.CheckAssetUpdate()); // 资源更新检测

        if (AssetCheckManager.Instance.needUpdate)
            yield return StartCoroutine(AssetCheckManager.Instance.StartDownLoadAssets()); // 资源下载

        yield return StartCoroutine(AssetCheckManager.Instance.StartLoadAsset()); // 开始资源加载

        yield return StartCoroutine(DataManager.Instance.LoadData()); //初始化配置表数据

        yield return StartCoroutine(InitServerAndManager());     // 初始化服务
    }

    // 日志
    private IEnumerator InitLog()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");
        yield return null;
    }

    // 服务层和管理器
    private IEnumerator InitServerAndManager()
    {
        NetService.Instance.Init();
        MapService.Instance.Init();
        UserService.Instance.Init();
        StatusService.Instance.Init();
        FriendService.Instance.Init();
        TeamService.Instance.Init();
        GuildService.Instance.Init();
        ShopManager.Instance.Init();
        yield return null;
    }
}