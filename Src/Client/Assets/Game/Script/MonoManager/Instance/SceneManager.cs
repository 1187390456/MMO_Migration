using CustomTools;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Manager
{
    /// <summary>
    /// 场景管理类
    /// </summary>
    public class SceneManager : MonoSingleton<SceneManager>
    {
        // 场景加载进度回调
        private UnityAction<float> onProgress = null;

        public void LoadScene(string name, LoadSceneMode mode = LoadSceneMode.Single) => StartCoroutine(LoadLevel(name, mode));

        private IEnumerator LoadLevel(string name, LoadSceneMode mode = LoadSceneMode.Single)
        {
            Debug.LogFormat("LoadLevel: {0}", name);
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name, mode);
            async.allowSceneActivation = true;
            async.completed += LevelLoadCompleted;
            while (!async.isDone)
            {
                onProgress?.Invoke(async.progress);
                yield return null;
            }
        }

        private void LevelLoadCompleted(AsyncOperation obj)
        {
            onProgress?.Invoke(1f);
            Debug.Log("LevelLoadCompleted:" + obj.progress);
        }

        //  可穿回调
        public void LoadSceneAsync(string name, Action<AsyncOperation> callback) => StartCoroutine(LoadLevel(name, callback));

        private IEnumerator LoadLevel(string name, Action<AsyncOperation> callback)
        {
            Debug.LogFormat("LoadLevel: {0}", name);
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
            async.allowSceneActivation = true;
            async.completed += callback;
            while (!async.isDone)
            {
                onProgress?.Invoke(async.progress);
                yield return null;
            }
        }
    }
}