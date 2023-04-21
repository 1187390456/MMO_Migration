using Nirvana;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemTips : MonoBehaviour
{
    private Animator at;
    private RichTextGroup grounp;

    private void Awake()
    {
        at = GetComponent<Animator>();
        grounp = GetComponentInChildren<RichTextGroup>();
    }

    // 添加提示
    public void AddTip(string msg, int speed)
    {
        at.SetFloat("Speed", speed);
        grounp.AddText(msg);
    }

    // 修改提示
    public void SetTip(string msg)
    {
        var text = GetComponentInChildren<Text>();
        if (text != null) text.text = msg;
    }

    // 修改舒服
    public void SetSpeed(int speed) => at.SetFloat("Speed", speed);

    // 监听动画
    public void ListenAt(Queue<GameObject> queue)
    {
        at.WaitEvent("exit", (name, info) =>
        {
            gameObject.SetActive(false);
            queue.Enqueue(gameObject);
        });
    }
}