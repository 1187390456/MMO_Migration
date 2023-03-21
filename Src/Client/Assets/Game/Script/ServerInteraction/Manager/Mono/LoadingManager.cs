using CustomTools;
using Nirvana;
using Services;
using System;
using System.Collections;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using XLua;
using Random = UnityEngine.Random;

namespace Manager
{
    public static class TipsConfig
    {
        public static string[] loadingTip =
        {
        "7级开启坐骑系统，骑上马，体验飞一般的感觉",
        "57级开启羽翼系统，开启飞翔之旅",
        "成为VIP2，可以免费双倍完成经验任务哦~",
        "首充1元，即送红色3星武器，绝版酷炫坐骑",
        "开服嘉年华，每天定个小目标，领走绝版形象哦",
        "80级就可以结婚啦，浪漫婚宴，可爱宝宝，趣味家园等你来体验~",
        "120级开启转职，开启高阶装备，高阶技能！",
        "BOSS地图凶险万分，小心他人攻击！当然你也可以选择打劫~",
        "记得去每日必做里看看自己还有什么没完成的",
        "经验本、双倍护送和经验跑环任务，是快速升级的有效途径",
        "挂机打怪之前记得使用经验药哦",
        "商店淘宝，折扣多多，还有几率刷出顶级仙女！",
        "挑战战魂塔，可解锁更多战魂，更高等级上限",
        "爬塔可获得大量境界令牌，还能解锁传世名剑！",
        "采集地图上的蛋可获得珍稀宠物，还有几率触发BOSS呢！",
        "市场上很多奇珍异宝，记得多逛逛哦",
        };
    }

    public class LoadingManager : MonoSingleton<LoadingManager>
    {
        // Prefabs

        private GameObject root;
        private GameObject GameTips;
        private GameObject Loading;

        // TableAssets

        private GameObject BackGround;
        private Transform BackGroundURL;
        private Text Version;
        private Text ProgressTips;
        private Text ProgressText;
        private Text AssetCheckText;
        private Slider ProgressBar;

        private void Awake()
        {
            InitPrefabs(); // 初始化默认加载预制件

            AssetCheckManager.Instance.startAssetCheck = StartAssetCheck;
            AssetCheckManager.Instance.assetCheckDone = AssetCheckDone;

            AssetCheckManager.Instance.startDownLoadAsset = StartDownLoadAsset;
            AssetCheckManager.Instance.downLoadAssetDone = AllAssetDownLoadDone;

            AssetCheckManager.Instance.startLoadAsset = StartLoadAsset;
            AssetCheckManager.Instance.loadAssetDone = AllAssetDone;

            AssetCheckManager.Instance.downLoadCallBack = SingleAssetDownDone;
        }

        private IEnumerator Start()
        {
            yield return StartCoroutine(InitScene());

            /*      yield return StartCoroutine(InitSceneAndDate());  // 初始化场景和配置文件

                  yield return StartCoroutine(CheckAssetUpdate()); // 资源检测

                  if (needUpdate)
                  {
                      assetCheck.SetActive(false);
                      progressBar.gameObject.SetActive(true);

                      // TODO 添加提示消息框
                      yield return StartCoroutine(StartDownLoadAssets()); // 资源下载
                  }
                  else
                  {
                      // TODO 加载场景
                      //  StartCoroutine(FreshTips());
                  }
      */
            /*         for (float i = 0; i < 100;)
                     {
                         i += Random.Range(0.1f, 1.5f);
                         progressBar.value = i;
                         progressNumber.text = $"【{Mathf.Floor(i)}%】";
                         //  yield return new WaitForSeconds(0.1f);
                         yield return new WaitForEndOfFrame();
                     }*/

            /*   UILoading.SetActive(false);
               UILogin.SetActive(true);

               isDone = true;
   */
        }

        // 初始化起始预制件
        private void InitPrefabs()
        {
            root = GoExtend.Instance.InstanAndSetLayer(AssetManager.Instance.Root, AssetManager.Instance.UILayer, LayerIndex.last, true, "UILogin");
            GameTips = GoExtend.Instance.InstanAndSetLayer(AssetManager.Instance.GameTips, root.transform, LayerIndex.first);
            Loading = GoExtend.Instance.InstanAndSetLayer(AssetManager.Instance.Loading, root.transform);

            UINameTable table = Loading.GetComponent<UINameTable>();
            BackGround = table.Find("BackGround");
            BackGroundURL = table.Find("BackGroundURL").transform;
            Version = table.Find("Version").GetComponent<Text>();
            ProgressTips = table.Find("ProgressTips").GetComponent<Text>();
            ProgressText = table.Find("ProgressText").GetComponent<Text>();
            ProgressBar = table.Find("ProgressBar").GetComponent<Slider>();
            AssetCheckText = table.Find("AssetCheckText").GetComponent<Text>();
        }

        // 初始化场景

        private IEnumerator InitScene()
        {
            StartCoroutine(LoadBackGroundURL()); // 加载背景切换动画
            StartCoroutine(FreshTips()); // 提示文字动画

            yield return new WaitForEndOfFrame();
            GameTips.SetActive(true);
            Loading.SetActive(false);
            yield return new WaitForSeconds(2f);
            Loading.SetActive(true);
            yield return new WaitForSeconds(1f);
            GameTips.SetActive(false);

            StartCoroutine(InitEnv.Instance.EnvStart()); // 环境加载
        }

        #region 资源回调

        // 开始检测资源
        private IEnumerator StartAssetCheck()
        {
            Debug.Log("开始检测资源");
            ProgressText.text = "";
            AssetCheckText.text = "开始检测资源";

            yield return new WaitForEndOfFrame();

            StartCoroutine(AssetCheckAnm()); // 资源检测动画
        }

        // 资源检测完毕
        private IEnumerator AssetCheckDone()
        {
            yield return new WaitForEndOfFrame();

            Debug.Log("资源检测完毕");
            ProgressText.text = "资源检测完毕";
            AssetCheckText.text = "";

            //TODO 弹窗 提示更新
        }

        // 开始下载资源
        private IEnumerator StartDownLoadAsset()
        {
            yield return new WaitForEndOfFrame();

            Debug.Log("开始下载资源");
            ProgressText.text = "开始下载资源";
        }

        // 单个资源下载完毕
        private IEnumerator SingleAssetDownDone(int total, int cur, bool isPass)
        {
            yield return new WaitForEndOfFrame();

            string msg = isPass ? $"该资源不更新{cur}/{total}" : $"正在下载资源{cur}/{total}";
            Debug.Log(msg);
            ProgressBar.maxValue = total;
            ProgressBar.value = cur;
            ProgressText.text = $"正在下载资源{cur}/{total}";
        }

        // 所有资源下载完毕
        private IEnumerator AllAssetDownLoadDone()
        {
            yield return new WaitForEndOfFrame();

            Debug.Log("更新完毕");
            ProgressText.text = "更新完毕";
        }

        // 开始资源加载
        private IEnumerator StartLoadAsset()
        {
            yield return new WaitForEndOfFrame();

            Debug.Log("开始加载资源");
            ProgressText.text = "开始加载资源";
        }

        // 所有资源加载完毕
        private IEnumerator AllAssetDone()
        {
            yield return new WaitForEndOfFrame();

            //TODO 测试用
            ProgressBar.maxValue = 1;
            ProgressBar.value = 1;

            Debug.Log("资源加载完毕");
            ProgressText.text = "资源加载完毕";

            var assets = AssetBundleManager.Instance.LoadAllAssets("UIs", "UILogin");
        }

        #endregion 资源回调

        #region 场景文字图片 动画相关

        // 检测资源 文字动画
        private IEnumerator AssetCheckAnm()
        {
            int dot = 1;
            while (AssetCheckText.text != "")
            {
                switch (dot)
                {
                    case 1:
                        AssetCheckText.text = "正在检测资源.";
                        break;

                    case 2:
                        AssetCheckText.text = "正在检测资源..";
                        break;

                    case 3:
                        AssetCheckText.text = "正在检测资源...";
                        break;
                }
                dot++;
                if (dot == 4) dot = 1;
                yield return new WaitForSeconds(0.4f);
            }
        }

        // 提示文字切换 文字动画
        private IEnumerator FreshTips()
        {
            while (ProgressBar.gameObject.activeSelf)
            {
                ProgressTips.text = TipsConfig.loadingTip[Random.Range(1, 12)];
                yield return new WaitForSeconds(2.0f);
            }
        }

        // 加载背景切换 图片动画
        private IEnumerator LoadBackGroundURL()
        {
            var bgurl = BackGroundURL.GetComponent<LoadRawImageURL>();

            while (ProgressBar.value < 40.0f)
            {
                yield return new WaitForSeconds(5.0f);
                //  if (isDone) yield break;
                BackGroundURL.gameObject.SetActive(true);
                bgurl.URL = "http://" + $"www.itxcm.cn/sceneloading_bg/sceneloading_bg_{Random.Range(1, 12)}.jpg";
            }
        }

        #endregion 场景文字图片 动画相关
    }
}