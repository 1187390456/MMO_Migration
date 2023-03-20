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
        public GameObject BackGround;
        public Transform BackGroundURL;

        public UnityEngine.UI.Slider progressBar;
        public Text progressTips;
        public Text progressNumber;

        public GameObject assetCheck;

        [HideInInspector] public bool isDone { get; set; }

        // 资源部分

        private bool allAssetDone = false;
        private string downLoadPath;
        private bool needUpdate = false;

        // 事件回调

        private Func<int, int, bool, IEnumerator> downLoadCallBack; // 单个资源下载回调 或 资源变化回调(未更新跳过)
        private Action downLoadAssetDone; // 资源更新完毕
        private LuaFunction lpCallBack;

        private void Awake()
        {
            StartCoroutine(InitLoginAsset()); // 加载登录相关的资源

            downLoadPath = PathUtil.GetAssetBundleOutPath();

            downLoadCallBack = AssetDownAnm;
            downLoadAssetDone = AssetDownLoadDone;

            /*            StartCoroutine(LoadBackGroundURL()); // 加载背景切换动画
                        StartCoroutine(FreshTips()); // 提示文字动画
                        StartCoroutine(AssetCheckAnm()); // 资源检测动画*/
        }

        private IEnumerator Start()
        {
            yield return StartCoroutine(InitLog());    // 初始化日志

            yield return new WaitUntil(() => allAssetDone);

            yield return StartCoroutine(InitSceneAndDate());  // 初始化场景和配置文件

            yield return StartCoroutine(InitServerAndManager());     // 初始化服务

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
            yield return null;
        }

        #region 登录资源加载

        private GameObject GameTips;
        private GameObject Loading;
        private GameObject DefaultBg;

        private IEnumerator InitLoginAsset()
        {
            yield return null;
            /*  AssetBundleManager.Instance.LoadAssetBundle("UIs", "UILogin", lpCallBack);

              yield return new WaitUntil(() => AssetBundleManager.Instance.IsFinsh("UIs", "UILogin"));

              var assets = AssetBundleManager.Instance.LoadAllAssets("UIs", "UILogin");

              var UILayer = GameObject.Find("UILayer");

              foreach (var asset in assets)
              {
                  if (asset.GetType() == typeof(GameObject) && asset.name == "DefaultBg") DefaultBg = Instantiate(asset as GameObject, UILayer.transform);
                  if (asset.GetType() == typeof(GameObject) && asset.name == "Loading") Loading = Instantiate(asset as GameObject, UILayer.transform);
                  if (asset.GetType() == typeof(GameObject) && asset.name == "GameTips") GameTips = Instantiate(asset as GameObject, UILayer.transform);
              }

              GameTips.transform.SetAsLastSibling();
              GameTips.SetActive(false);
              Loading.SetActive(false);
              DefaultBg.SetActive(false);

              allAssetDone = true;

              yield return new WaitForEndOfFrame();*/
        }

        #endregion 登录资源加载

        #region 初始化相关

        // 日志
        private IEnumerator InitLog()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
            UnityLogger.Init();
            Common.Log.Init("Unity");
            Common.Log.Info("LoadingManager start");
            yield return null;
        }

        // 场景和配置文件

        private IEnumerator InitSceneAndDate()
        {
            GameTips.SetActive(true);
            Loading.SetActive(false);
            // UILogin.SetActive(false);
            yield return new WaitForSeconds(2f);
            Loading.SetActive(true);
            yield return new WaitForSeconds(1f);
            GameTips.SetActive(false);
            yield return DataManager.Instance.LoadData();
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

        #endregion 初始化相关

        #region 资源 检测更新下载 md5相关

        // 资源更新检测
        private IEnumerator CheckAssetUpdate()
        {
            // 请求md5效验文件
            string url = "http://hotfix.itxcm.cn/Windows/";
            string fileUrl = url + "files.txt";
            UnityWebRequest www = UnityWebRequest.Get(fileUrl);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) Debug.Log(www.error);

            if (!Directory.Exists(downLoadPath))
            {
                // 本地不存在效验文件 需要更新
                needUpdate = true;
                yield break;
            }

            // 读取文件效验内容 文件名-md5
            string filesText = www.downloadHandler.text;
            string[] lines = filesText.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue; // 空行
                string[] kv = lines[i].Split('|'); // 分割
                string fileName = kv[0];
                string localFile = (downLoadPath + "/" + fileName).Trim();

                if (!File.Exists(localFile))  // 本地不存在该文件 需要更新
                {
                    needUpdate = true;
                    yield break;
                }
                else
                {
                    string md5 = kv[1].Trim();
                    string localMd5 = GetFileMd5(localFile).Trim();

                    if (md5 != localMd5)  // 本地存在文件 但是更新了
                    {
                        needUpdate = true;
                        yield break;
                    }
                }
            }
            needUpdate = false; // 所有文件名和md5都对上了
            yield break;
        }

        // 资源下载
        private IEnumerator StartDownLoadAssets()
        {
            // 获取远程Md5文件
            string url = "http://hotfix.itxcm.cn/Windows/";
            string fileUrl = url + "files.txt";
            UnityWebRequest www = UnityWebRequest.Get(fileUrl);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success) Debug.Log(www.error);
            // 判断本地是否有这个文件 并拷贝
            if (!Directory.Exists(downLoadPath)) Directory.CreateDirectory(downLoadPath);
            // 下载写入本地
            File.WriteAllBytes(downLoadPath + "/files.txt", www.downloadHandler.data);
            // 读取文件内容
            string filesText = www.downloadHandler.text;
            string[] lines = filesText.Split('\n');

            Debug.Log($"一共{lines.Length - 1 }个资源"); // 资源个数

            int total = lines.Length - 1;
            int cur = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) continue; // 空行
                string[] kv = lines[i].Split('|'); // 分割
                string fileName = kv[0];
                string localFile = (downLoadPath + "/" + fileName).Trim();

                cur++;

                if (!File.Exists(localFile)) // 本地不存在这个文件 进行下载
                {
                    string dir = Path.GetDirectoryName(localFile);
                    Directory.CreateDirectory(dir);

                    StartCoroutine(downLoadCallBack?.Invoke(total, cur, false));
                    yield return StartCoroutine(DownFileAndSave(url + fileName, localFile)); // 开始网络下载
                }
                else // 有文件 比对md5 效验是否有更新
                {
                    string md5 = kv[1].Trim();
                    string localMd5 = GetFileMd5(localFile).Trim();

                    if (md5 != localMd5)   // 更新了 删除本地文件 下载新的
                    {
                        File.Delete(localFile);

                        StartCoroutine(downLoadCallBack?.Invoke(total, cur, false));
                        yield return StartCoroutine(DownFileAndSave(url + fileName, localFile)); // 开始网络下载
                    }
                    else StartCoroutine(downLoadCallBack?.Invoke(total, cur, true));
                }
            }

            yield return new WaitUntil(() => total == cur);
            downLoadAssetDone?.Invoke();
        }

        //  下载文件并保存在本地
        private IEnumerator DownFileAndSave(string url, string savePath)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError) Debug.Log(www.error);
            File.WriteAllBytes(savePath, www.downloadHandler.data);
            yield return new WaitUntil(() => File.Exists(savePath));
            Debug.Log("文件" + savePath + "下载完成!");
        }

        // 获取文件md5
        private static string GetFileMd5(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] result = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));
            }
            return sb.ToString();
        }

        #endregion 资源 检测更新下载 md5相关

        #region 场景文字图片 动画相关

        // 检测资源 文字动画
        private IEnumerator AssetCheckAnm()
        {
            int dot = 1;
            Text txt = assetCheck.GetComponent<Text>();
            while (assetCheck.gameObject.activeSelf)
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
            while (progressBar.gameObject.activeSelf)
            {
                progressTips.text = TipsConfig.loadingTip[Random.Range(1, 12)];
                yield return new WaitForSeconds(2.0f);
            }
        }

        // 加载背景切换 图片动画
        private IEnumerator LoadBackGroundURL()
        {
            var bgurl = BackGroundURL.GetComponent<LoadRawImageURL>();

            while (progressBar.value < 40.0f)
            {
                yield return new WaitForSeconds(5.0f);
                if (isDone) yield break;
                BackGroundURL.gameObject.SetActive(true);
                bgurl.URL = "http://" + $"www.itxcm.cn/sceneloading_bg/sceneloading_bg_{Random.Range(1, 12)}.jpg";
            }
        }

        #endregion 场景文字图片 动画相关

        // 单个资源下载完毕 或跳过
        private IEnumerator AssetDownAnm(int total, int cur, bool isPass)
        {
            yield return new WaitForEndOfFrame();
            string msg = isPass ? $"该资源不更新{cur}/{total}" : $"正在下载资源{cur}/{total}";
            Debug.Log(msg);
            progressNumber.text = $"{cur}/{total}"; ;
            progressNumber.transform.parent.GetComponent<Text>().text = $"正在下载资源";
        }

        // 全部资源下载完毕
        private void AssetDownLoadDone()
        {
            Debug.Log("更新完毕");

            progressNumber.text = "";
            progressNumber.transform.parent.GetComponent<Text>().text = "更新完毕";
        }
    }
}