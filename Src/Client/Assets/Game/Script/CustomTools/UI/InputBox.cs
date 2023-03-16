using CustomTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBox
{
    private static Object cacheObject = null;

    public static UIInputBox Show(string message, string title = "", string btnOK = "", string btnCancel = "", string emotyTips = "")
    {
        if (cacheObject == null) cacheObject = Resloader.Load<Object>("UI/Prefabs/UIInputBox");
        GameObject go = GameObject.Instantiate(cacheObject) as GameObject;
        UIInputBox msgbox = go.GetComponent<UIInputBox>();
        msgbox.Init(title, message, btnOK, btnCancel, emotyTips);

        return msgbox;
    }
}