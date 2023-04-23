using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// AssetBundle编辑
/// </summary>
public class AssetBundleEditor
{
    #region 自动做标记

    //思路
    //1.找到资源保存的文件夹
    //2.遍历里面的每个场景文件夹
    //3.遍历场景文件夹里的所有文件系统
    //4.如果访问的是文件夹：再继续访问里面的所有文件系统，直到找到 文件 （递归）
    //5.找到文件 就要修改他的 assetbundle labels
    //6.用 AssetImporter 类 修改名称和后缀
    //7.保存对应的 文件夹名 和 具体路径

    [MenuItem("AssetBundle/Set AssetBundle Labels")]
    public static void SetAssetBundleLabels()
    {
        //移除所有没有使用的标记
        AssetDatabase.RemoveUnusedAssetBundleNames();

        //1.找到资源保存的文件夹
        string assetDirectory = Application.dataPath + "/AssetBundles";
        //Debug.Log(assetDirectory);

        DirectoryInfo directoryInfo = new DirectoryInfo(assetDirectory);
        DirectoryInfo[] sceneDirectories = directoryInfo.GetDirectories();
        //2.遍历里面的每个场景文件夹
        foreach (DirectoryInfo tmpDirectoryInfo in sceneDirectories)
        {
            string sceneDirectory = assetDirectory + "/" + tmpDirectoryInfo.Name;
            DirectoryInfo sceneDirectoryInfo = new DirectoryInfo(sceneDirectory);
            //错误检测
            if (sceneDirectoryInfo == null)
            {
                Debug.LogError(sceneDirectory + " 不存在!");
                return;
            }
            else
            {
                Dictionary<string, string> namePahtDict = new Dictionary<string, string>();

                //3.遍历场景文件夹里的所有文件系统
                //sceneDirectory
                //C:\Users\张晋枭\Documents\ABLesson\Assets\AssetBundles\Res\Scene1

                //C:/Users/张晋枭/Documents/ABLesson/Assets/AssetBundles/Res/Scene1
                int index = sceneDirectory.LastIndexOf("/");
                string sceneName = sceneDirectory.Substring(index + 1);
                onSceneFileSystemInfo(sceneDirectoryInfo, sceneName, namePahtDict);

                onWriteConfig(sceneName, namePahtDict);
            }
        }

        AssetDatabase.Refresh();

        Debug.Log("设置成功");
    }

    /// <summary>
    /// 记录配置文件
    /// </summary>
    private static void onWriteConfig(string sceneName, Dictionary<string, string> namePathDict)
    {
        string path = PathUtil.GetAssetBundleOutPath() + "/" + sceneName + "Record.txt";
        // Debug.Log(path);

        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
            //写二进制
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(namePathDict.Count);

                foreach (KeyValuePair<string, string> kv in namePathDict)
                    sw.WriteLine(kv.Key + " " + kv.Value);
            }
        }
    }

    /// <summary>
    /// 遍历场景文件夹里的所有文件系统
    /// </summary>
    private static void onSceneFileSystemInfo(FileSystemInfo fileSystemInfo, string sceneName, Dictionary<string, string> namePahtDict)
    {
        if (!fileSystemInfo.Exists)
        {
            Debug.LogError(fileSystemInfo.FullName + " 不存在!");
            return;
        }

        DirectoryInfo directoryInfo = fileSystemInfo as DirectoryInfo;
        FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
        foreach (var tmpFileSystemInfo in fileSystemInfos)
        {
            FileInfo fileInfo = tmpFileSystemInfo as FileInfo;
            if (fileInfo == null)
            {
                //代表强转失败，不是文件 就是文件夹
                //如果访问的是文件夹：再继续访问里面的所有文件系统，直到找到 文件 （递归）
                onSceneFileSystemInfo(tmpFileSystemInfo, sceneName, namePahtDict);
            }
            else
            {
                //就是文件
                //5.找到文件 就要修改他的 assetbundle labels
                setLabels(fileInfo, sceneName, namePahtDict);
            }
        }
    }

    /// <summary>
    /// 修改资源文件的 assetbundle labels
    /// </summary>
    private static void setLabels(FileInfo fileInfo, string sceneName, Dictionary<string, string> namePahtDict)
    {
        //对unity自身生成的meta文件 无视它
        if (fileInfo.Extension == ".meta")
            return;

        string bundleName = getBundleName(fileInfo, sceneName);
        //C:\Users\张晋枭\Documents\ABLesson\Assets\Res\Scene1\Buildings\Folder\Building4.prefab
        int index = fileInfo.FullName.IndexOf("Assets");
        //Assets\Res\Scene1\Buildings\Folder\Building4.prefab
        string assetPath = fileInfo.FullName.Substring(index);

        AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
        //用 AssetImporter 类 修改名称和后缀
        assetImporter.assetBundleName = bundleName.ToLower();
        if (fileInfo.Extension == ".unity")
            assetImporter.assetBundleVariant = "u3d";
        else
            assetImporter.assetBundleVariant = "assetbundle";

        string folderName = "";
        //添加到字典里
        if (bundleName.Contains("/"))
            folderName = bundleName.Split('/')[1];
        else
            folderName = bundleName;

        string bundlePath = assetImporter.assetBundleName + "." + assetImporter.assetBundleVariant;
        if (!namePahtDict.ContainsKey(folderName))
            namePahtDict.Add(folderName, bundlePath);
    }

    /// <summary>
    /// 获取包名
    /// </summary>
    private static string getBundleName(FileInfo fileInfo, string sceneName)
    {
        string windowsPath = fileInfo.FullName;//C:\Users\张晋枭\Documents\ABLesson\Assets\Res\Scene1\Buildings\Folder\Building4.prefab
        //转换成unity可识别的路径
        string unityPath = windowsPath.Replace(@"\", "/");

        //C:/Users/张晋枭/Documents/ABLesson/Assets/Res/Scene1 /Buildings/Folder/Building4.prefab
        //C: \Users\张晋枭\Documents\ABLesson\Assets\Res\Scene1\Scene1.unity
        int index = unityPath.IndexOf(sceneName) + sceneName.Length;

        string bundlePath = unityPath.Substring(index + 1);

        if (bundlePath.Contains("/"))
        {
            //Buildings/Folder/Folder/Folder/Folder/Folder/Building4.prefab
            string[] tmp = bundlePath.Split('/');
            return sceneName + "/" + tmp[0];
        }
        else
        {
            //Scene1.unity
            return sceneName;
        }
    }

    #endregion 自动做标记

    #region 打包

    [MenuItem("AssetBundle/Build AssetBundles")]
    private static void BuildAllAssetBundles()
    {
        string outPath = PathUtil.GetAssetBundleOutPath();

        BuildPipeline.BuildAssetBundles(outPath, 0, BuildTarget.StandaloneWindows64);

        // 对所有文件
    }

    #endregion 打包

    #region 一键删除

    [MenuItem("AssetBundle/Delete All")]
    private static void DeleteAssetBundle()
    {
        string outPath = PathUtil.GetAssetBundleOutPath();

        Directory.Delete(outPath, true);
        File.Delete(outPath + ".meta");

        AssetDatabase.Refresh();
    }

    #endregion 一键删除

    #region 创建md5文件

    [MenuItem("Tools/Create Files")]
    private static void CraeteFiles()
    {
        string outPath = PathUtil.GetAssetBundleOutPath();
        // 效验文件路径
        string filePath = outPath + "/files.txt";
        if (File.Exists(filePath)) File.Delete(filePath);

        // 遍历这个文件的下面所有文件
        List<string> fileList = new List<string>();
        GetFiles(new DirectoryInfo(outPath), ref fileList);

        FileStream fs = new FileStream(filePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);

        for (int i = 0; i < fileList.Count; i++)
        {
            string file = fileList[i];
            string ext = Path.GetExtension(file);
            if (ext.EndsWith(".meta")) continue;

            // 生成文件名和md5
            string md5 = GetFileMd5(file);
            string value = file.Replace(outPath + "/", string.Empty);

            // 写入文件
            sw.WriteLine(value + "|" + md5);
        }
        sw.Close();
        fs.Close();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 遍历文件夹下的所有文件
    /// </summary>
    private static void GetFiles(FileSystemInfo fileSystemInfo, ref List<string> fileList)
    {
        DirectoryInfo directoryInfo = fileSystemInfo as DirectoryInfo;
        // 获取所有文件系统
        FileSystemInfo[] files = directoryInfo.GetFileSystemInfos();

        foreach (FileSystemInfo file in files)
        {
            FileInfo fileInfo = file as FileInfo;
            if (fileInfo != null) fileList.Add(fileInfo.FullName.Replace("\\", "/")); // 是文件
            else GetFiles(file, ref fileList); // 是文件夹 继续递归直到是文件
        }
    }

    /// <summary>
    /// 获取文件md5
    /// </summary>
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

    #endregion 创建md5文件
}

// 移除操作
public class AutoRemoveAssetBundleLabel
{
    [MenuItem("AssetBundle/Remove AB Label")]
    public static void RemoveABLabel()
    {
        // 需要移除标记的根目录
        string strNeedRemoveLabelRoot = string.Empty;
        // 目录信息（场景目录信息数组，表示所有根目录下场景目录）
        DirectoryInfo[] directoryDIRArray = null;

        // 定义需要移除AB标签的资源的文件夹根目录
        strNeedRemoveLabelRoot = Application.dataPath + "/NoPackAssets";
        //Debug.Log("strNeedSetLabelRoot = "+strNeedSetLabelRoot);

        DirectoryInfo dirTempInfo = new DirectoryInfo(strNeedRemoveLabelRoot);
        directoryDIRArray = dirTempInfo.GetDirectories();

        // 遍历本场景目录下所有的目录或者文件
        foreach (DirectoryInfo currentDir in directoryDIRArray)
        {
            // 递归调用方法，找到文件，则使用 AssetImporter 类，标记“包名”与 “后缀名”
            JudgeDirOrFileByRecursive(currentDir);
        }

        // 清空无用的 AB 标记
        AssetDatabase.RemoveUnusedAssetBundleNames();
        // 刷新
        AssetDatabase.Refresh();

        // 提示信息，标记包名完成
        Debug.Log("AssetBundle 本次操作移除标记完成");
    }

    /// <summary>
    /// 递归判断判断是否是目录或文件
    /// 是文件，修改 Asset Bundle 标记
    /// 是目录，则继续递归
    /// </summary>
    /// <param name="fileSystemInfo">当前文件信息（文件信息与目录信息可以相互转换）</param>
    private static void JudgeDirOrFileByRecursive(FileSystemInfo fileSystemInfo)
    {
        // 调试信息
        //Debug.Log("currentDir.Name = " + fileSystemInfo.Name);
        //Debug.Log("sceneName = " + sceneName);

        // 参数检查
        if (fileSystemInfo.Exists == false)
        {
            Debug.LogError("文件或者目录名称：" + fileSystemInfo + " 不存在，请检查");
            return;
        }

        // 得到当前目录下一级的文件信息集合
        DirectoryInfo directoryInfoObj = fileSystemInfo as DirectoryInfo;           // 文件信息转为目录信息
        FileSystemInfo[] fileSystemInfoArray = directoryInfoObj.GetFileSystemInfos();

        foreach (FileSystemInfo fileInfo in fileSystemInfoArray)
        {
            FileInfo fileInfoObj = fileInfo as FileInfo;

            // 文件类型
            if (fileInfoObj != null)
            {
                // 修改此文件的 AssetBundle 标签
                RemoveFileABLabel(fileInfoObj);
            }
            // 目录类型
            else
            {
                // 如果是目录，则递归调用
                JudgeDirOrFileByRecursive(fileInfo);
            }
        }
    }

    /// <summary>
    /// 给文件移除 Asset Bundle 标记
    /// </summary>
    /// <param name="fileInfoObj">文件（文件信息）</param>
    private static void RemoveFileABLabel(FileInfo fileInfoObj)
    {
        // 调试信息
        //Debug.Log("fileInfoObj.Name = " + fileInfoObj.Name);
        //Debug.Log("scenesName = " + scenesName);

        // 参数定义
        // AssetBundle 包名称
        string strABName = string.Empty;
        // 文件路径（相对路径）
        string strAssetFilePath = string.Empty;

        // 参数检查（*.meta 文件不做处理）
        if (fileInfoObj.Extension == ".meta")
        {
            return;
        }

        // 得到 AB 包名称
        strABName = string.Empty;
        // 获取资源文件的相对路径
        int tmpIndex = fileInfoObj.FullName.IndexOf("Assets");
        strAssetFilePath = fileInfoObj.FullName.Substring(tmpIndex);        // 得到文件相对路径

        // 给资源文件移除 AB 名称
        AssetImporter tmpImportObj = AssetImporter.GetAtPath(strAssetFilePath);
        tmpImportObj.assetBundleName = strABName;
    }
}