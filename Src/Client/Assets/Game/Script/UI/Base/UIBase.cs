using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBase
{
    #region 事件分发处理

    private List<int> list = new List<int>();

    protected void Bind(params int[] eventCodes)
    {
        list.AddRange(eventCodes);
        UIManager.Instance.Add(list.ToArray(), this);
    }

    protected void UnBind()
    {
        if (list == null) return;
        UIManager.Instance.Remove(list.ToArray(), this);
        list.Clear();
    }

    public void Dispatch(int areaCode, int eventCode, object message) => MsgCenter.Instance.Dispatch(areaCode, eventCode, message);

    public virtual void OnDestroy() => UnBind();

    #endregion 事件分发处理

    #region UI扩展

    public enum UIResult
    {
        None = 0,
        Yes,
        No,
    } // UI事件结果

    public Action<UIBase, UIResult> UIEventHandler; //  UI事件 当前类型 结果
    public virtual System.Type Type => GetType(); // 获取当前父级类型

    public void Close(UIResult result = UIResult.None)
    {
        UIManager.Instance.Close(Type);
        UIEventHandler?.Invoke(this, result);
        UIEventHandler = null;
    }  // 关闭

    public void OnCloseClick() => Close();

    public virtual void OnYesClick() => Close(UIResult.Yes);

    public virtual void OnNoClick() => Close(UIResult.No);

    public void OnMouseDown()
    {
        Debug.Log(name + "Clicked");
    }

    #endregion UI扩展
}