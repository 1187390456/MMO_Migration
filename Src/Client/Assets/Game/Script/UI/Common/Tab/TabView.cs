using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TabView : MonoBehaviour
{
    public TabButton[] tabButtons; // tab按钮
    public GameObject[] tabpages; // 控制显示的页面游戏对象
    public Action<int> OnTabSelected; // 选中事件

    private int curInex = -1; // 当前选择索引

    private IEnumerator Start()
    {
        for (int i = 0; i < tabButtons.Length; i++) tabButtons[i].SetTabView(this, i);
        yield return new WaitForEndOfFrame();
        SelectTab(0);
    }

    // 选中Tab
    public void SelectTab(int nextIndex)
    {
        if (curInex != nextIndex)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(i == nextIndex);   // 设置按钮选中样式
                if (i < tabpages.Length) tabpages[i].SetActive(i == nextIndex); // 设置游戏对象显示
            }
            OnTabSelected?.Invoke(nextIndex);
        }
    }
}