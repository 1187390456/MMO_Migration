using CustomTools;
using Nirvana;
using Services;
using System;
using System.Collections;
using System.IO;
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
        private void Awake()
        {
            AssetCheckManager.Instance.loadAssetDone = AllAssetDone;
            AssetCheckManager.Instance.downLoadAssetDone = AllAssetDownLoadDone;
            AssetCheckManager.Instance.downLoadCallBack = SingleAssetDownDone;
        }

        private GameObject DefaultBg;
        private GameObject GameTips;
        private GameObject Loading;

        // TableAssets

        private GameObject BackGround;
        private Transform BackGroundURL;
        private Text Version;
        private Text ProgressTips;
        private Text ProgressText;
        private Text ProgressNumber;
        private GameObject AssetCheck;
        private Slider ProgressBar;

        // 所有资源加载完毕
        private void AllAssetDone()
        {
            var assets = AssetBundleManager.Instance.LoadAllAssets("UIs", "UILogin");

            var login = GameObject.Find("UILayer").transform.Find("Login");

            //TOOD 后期优化
            foreach (var asset in assets)
            {
                if (asset.GetType() == typeof(GameObject) && asset.name == "DefaultBg") DefaultBg = Instantiate(asset as GameObject, login);
                if (asset.GetType() == typeof(GameObject) && asset.name == "Loading") Loading = Instantiate(asset as GameObject, login);
                if (asset.GetType() == typeof(GameObject) && asset.name == "GameTips") GameTips = Instantiate(asset as GameObject, login);
            }

            UINameTable table = Loading.GetComponent<UINameTable>();
            BackGround = table.Find("BackGround");
            BackGroundURL = table.Find("BackGroundURL").transform;
            Version = table.Find("Version").GetComponent<Text>();
            ProgressTips = table.Find("ProgressTips").GetComponent<Text>();
            ProgressText = table.Find("ProgressText").GetComponent<Text>();
            ProgressNumber = table.Find("ProgressNumber").GetComponent<Text>();
            ProgressBar = table.Find("ProgressBar").GetComponent<Slider>();
            AssetCheck = table.Find("AssetCheck");

            GameTips.transform.SetAsLastSibling();
            GameTips.SetActive(false);
            Loading.SetActive(false);
            DefaultBg.SetActive(false);

            StartCoroutine(InitScene()); // 初始化场景
        }

        // 所有资源下载完毕

        private void AllAssetDownLoadDone()
        {
            Debug.Log("更新完毕");

            ProgressNumber.text = "";
            ProgressNumber.transform.parent.GetComponent<Text>().text = "更新完毕";
        }

        // 单个资源下载完毕
        private IEnumerator SingleAssetDownDone(int total, int cur, bool isPass)
        {
            yield return new WaitForEndOfFrame();
            string msg = isPass ? $"该资源不更新{cur}/{total}" : $"正在下载资源{cur}/{total}";
            Debug.Log(msg);
            ProgressNumber.text = $"{cur}/{total}"; ;
            ProgressNumber.transform.parent.GetComponent<Text>().text = $"正在下载资源";
        }

        // 初始化场景

        private IEnumerator InitScene()
        {
            StartCoroutine(LoadBackGroundURL()); // 加载背景切换动画
            StartCoroutine(FreshTips()); // 提示文字动画
            StartCoroutine(AssetCheckAnm()); // 资源检测动画

            yield return new WaitForEndOfFrame();

            GameTips.SetActive(true);
            Loading.SetActive(false);
            // UILogin.SetActive(false);
            yield return new WaitForSeconds(2f);
            Loading.SetActive(true);
            yield return new WaitForSeconds(1f);
            GameTips.SetActive(false);
        }

        private IEnumerator Start()
        {
            yield return null;

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

        #region 场景文字图片 动画相关

        // 检测资源 文字动画
        private IEnumerator AssetCheckAnm()
        {
            int dot = 1;
            Text txt = AssetCheck.GetComponent<Text>();
            while (AssetCheck.gameObject.activeSelf)
            {
                switch (dot)
                {
                    case 1:
                        txt.text = "正在检测资源.";
                        break;

                    case 2:
                        txt.text = "正在检测资源..";
                        break;

                    case 3:
                        txt.text = "正在检测资源...";
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