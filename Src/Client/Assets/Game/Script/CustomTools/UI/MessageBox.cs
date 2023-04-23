using UnityEngine;
using CustomTools;

internal class MessageBox
{
    private static Object cacheObject = null;

    public static UIMessageBox Show(string message, string title = "", MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        if (cacheObject == null)
        {
            cacheObject = Resloader.Load<Object>("UI/Prefabs/UIMessageBox");
        }
        GameObject go = GameObject.Instantiate(cacheObject) as GameObject;
        UIMessageBox msgbox = go.GetComponent<UIMessageBox>();
        msgbox.Init(title, message, type, btnOK, btnCancel);
        return msgbox;
    }
}

public enum MessageBoxType
{
    Information = 1,

    Confirm = 2,

    Error = 3
}