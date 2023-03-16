using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInputBox : MonoBehaviour
{
    public Text title;
    public Text message;
    public Button buttonYes;
    public Button buttonNo;
    public InputField input;

    public Text buttonYesTitle;
    public Text buttonNoTitle;

    public delegate bool SubmitHandler(string inputText, out string tips);

    public event SubmitHandler OnSubmit;

    public Action OnCancle;

    public string emptyTips;

    public void Init(string title, string message, string btnOK = "", string btnCancel = "", string emptyTips = "")
    {
        if (!string.IsNullOrEmpty(title)) this.title.text = title;
        this.emptyTips = emptyTips;
        this.message.text = message;
        OnSubmit = null;

        if (!string.IsNullOrEmpty(btnOK)) buttonYesTitle.text = btnOK;
        if (!string.IsNullOrEmpty(btnCancel)) buttonNoTitle.text = btnCancel;

        buttonYes.onClick.AddListener(OnClickYes);
        buttonNo.onClick.AddListener(OnClickNo);
    }

    private void OnClickYes()
    {
        if (string.IsNullOrEmpty(input.text)) // 空提交验证
        {
            message.text = emptyTips;
            return;
        }
        if (OnSubmit != null)
        {
            if (!OnSubmit(input.text, out string tips)) // 提交效验 没通过返回错误提示
            {
                message.text = tips;
                return;
            }
        }
        Destroy(gameObject);
    }

    private void OnClickNo()
    {
        Destroy(gameObject);
        OnCancle?.Invoke();
    }
}