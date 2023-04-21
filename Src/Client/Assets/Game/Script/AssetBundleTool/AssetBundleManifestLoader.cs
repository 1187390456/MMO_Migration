using CustomTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AssetBundleTool
{
    /// <summary>
    /// 加载 manifest 文件
    /// </summary>
    public class AssetBundleManifestLoader : Singleton<AssetBundleManifestLoader>
    {
        private bool finish;
        private AssetBundleManifest manifest; // 加载的Manifest文件
        private string manifestPath; // 加载的Manifest文件路径
        private AssetBundle assetBundle; //  全局存在的assetbundle
        public bool Finish => finish; // 是否完成

        public AssetBundleManifestLoader()
        {
            this.manifestPath = PathUtil.GetWWWPath() + "/" + PathUtil.GetPlatformName();

            this.manifest = null;
            this.assetBundle = null;
            this.finish = false;
        }

        // 加载
        public IEnumerator Load()
        {
            //  Debug.LogFormat("开始加载Manifest文件 路径: {0}", manifestPath);
            WWW www = new WWW(manifestPath);
            yield return www;

            if (www.error != null) Debug.LogError("加载Manifest文件出错 : " + www.error);
            else
            {
                if (www.progress >= 1f)
                {
                    this.assetBundle = www.assetBundle;
                    this.manifest = this.assetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                    this.finish = true;

                    //   Debug.LogFormat("加载Manifest文件完毕 {0}-{1}-{2}", this.assetBundle, this.manifest, this.finish);
                }
            }
        }

        public string[] GetDependencies(string bundleName) => manifest.GetAllDependencies(bundleName);// 获取指定包名的所有的依赖关系

        public void UnLoad() => assetBundle.Unload(true); // 卸载 manifest
    }
}