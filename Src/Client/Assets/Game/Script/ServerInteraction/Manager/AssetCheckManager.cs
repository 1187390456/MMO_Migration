using Common;
using CustomTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using XLua;

public class AssetCheckManager : MonoSingleton<AssetCheckManager>
{
    private string downLoadPath = PathUtil.GetAssetBundleOutPath(); // 本地路径
    private string assetUrl = "http://hotfix.itxcm.cn/Windows/"; // 资源下载地址
    private string fileUrl = "http://hotfix.itxcm.cn/Windows/files.txt"; // 文件md5效验地址

    public bool needUpdate = false; // 是否需要更新
    public Func<int, int, bool, IEnumerator> downLoadCallBack; // 单个资源下载回调 或 资源变化回调(未更新跳过)
    public Action downLoadAssetDone; // 资源更新完毕
    public Action loadAssetDone; // 加载资源完毕

    // 资源更新检测
    public IEnumerator CheckAssetUpdate()
    {
        // 请求md5效验文件
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
    public IEnumerator StartDownLoadAssets()
    {
        // 获取远程Md5文件
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
                yield return StartCoroutine(DownFileAndSave(assetUrl + fileName, localFile)); // 开始网络下载
            }
            else // 有文件 比对md5 效验是否有更新
            {
                string md5 = kv[1].Trim();
                string localMd5 = GetFileMd5(localFile).Trim();

                if (md5 != localMd5)   // 更新了 删除本地文件 下载新的
                {
                    File.Delete(localFile);

                    StartCoroutine(downLoadCallBack?.Invoke(total, cur, false));
                    yield return StartCoroutine(DownFileAndSave(assetUrl + fileName, localFile)); // 开始网络下载
                }
                else StartCoroutine(downLoadCallBack?.Invoke(total, cur, true));
            }
        }

        yield return new WaitUntil(() => total == cur);
        downLoadAssetDone?.Invoke();
    }

    // 资源加载
    public IEnumerator StartLoadAsset()
    {
        // 开始资源加载
        //   AssetBundleManager.Instance.LoadAssetBundle("Common", "TTF", null);
        AssetBundleManager.Instance.LoadAssetBundle("UIs", "UILogin", null);

        yield return new WaitUntil(() => AssetFinshConditon());

        loadAssetDone?.Invoke();
    }

    // 资源加载完成条件
    private bool AssetFinshConditon()
    {
        return AssetBundleManager.Instance.IsFinsh("UIs", "UILogin");
        //  AssetBundleManager.Instance.IsFinsh("Common", "TTF") &&

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

    /*    [LuaCallCSharp]
        public void LoadSceneCallBack(string name, float process)
        {
            Debug.Log(name);
            Debug.Log(process);
        }

        public void InstanAndSetLayer(GameObject prefab, int layer)
        {
            var go = GameObject.Instantiate(prefab);
            go.transform.SetSiblingIndex(layer);
        }*/
}