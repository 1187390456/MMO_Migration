using CustomTools;
using Nirvana;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipsConfig : MonoSingleton<TipsConfig>
{
    private Transform systemTipsPool;

    private void Awake()
    {
        systemTipsPool = transform.Find("SystemTipsPool");
    }

    #region 系统提示

    private Queue<GameObject> sysTipsQueue = new Queue<GameObject>();
    private SystemTips preSysTips = null;
    [Header("系统提示消息")] public GameObject prefab;

    public void ShowSystemTips(string msg, int speed = 1)
    {
        if (preSysTips != null) preSysTips.SetSpeed(4);

        GameObject go;
        SystemTips st;

        if (sysTipsQueue.Count == 0)
        {
            //  队列没有
            go = Instantiate(prefab, systemTipsPool);
            st = go.GetComponent<SystemTips>();
            st.AddTip(msg, speed);
            st.ListenAt(sysTipsQueue);
        }
        else
        {
            // 队列有
            go = sysTipsQueue.Dequeue();
            st = go.GetComponent<SystemTips>();
            st.SetTip(msg);
            st.SetSpeed(speed);
            go.SetActive(true);
            st.ListenAt(sysTipsQueue);
        }

        preSysTips = st;
    }

    #endregion 系统提示
}