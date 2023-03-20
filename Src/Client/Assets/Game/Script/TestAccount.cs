using Manager;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAccount : UIBase
{
    private int user = 0;
    private int account = 0;

    private bool postSceneZeroDone = false;
    private bool postSceneOne = false;

    private void Awake()
    {
        UserService.Instance.Res_Login += Login_Res;
    }

    private IEnumerator Start()
    {
        DontDestroyOnLoad(this);
        // yield return new WaitUntil(() => LoadingManager.Instance.isDone);

        yield return null;
        SendNextCount();
    }

    private void Login_Res(Result res, string msg)
    {
        if (res != Result.Success) SendNextCount();
        else postSceneZeroDone = true;
    }

    private void Update()
    {
        if (!postSceneOne && postSceneZeroDone && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1)
        {
            postSceneOne = true;
            UserService.Instance.Send_GameEnter(0);
        }
    }

    private void SendNextCount()
    {
        user += 1;
        account += 1;
        UserService.Instance.Send_Login($"{user}", $"{account}");
    }
}