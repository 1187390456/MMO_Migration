using Common.Data;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuest : UIBase
{
    [Header("标题")] public Text title;
    [Header("任务Item预制件")] public GameObject itemPrefab;
    [Header("任务信息面板")] public UIQuestInfo questInfo;

    [Header("TabView控制脚本")] public TabView tabs;
    [Header("主线任务列表父级")] public ListView mainListView;
    [Header("支线任务列表父级")] public ListView branchListView;

    private bool showAvailableList = false; // 是否显示可接任务

    private UIQuestItem SeletedItem; // 当前选择任务

    private void Start()
    {
        mainListView.OnItemSelected += OnQuestSelected;
        branchListView.OnItemSelected += OnQuestSelected;
        tabs.OnTabSelected += OnTabSelect;

        RefreshUI();
    }

    // Tab选中

    private void OnTabSelect(int index)
    {
        showAvailableList = index == 1;
        RefreshUI();
    }

    // 任务列表选中
    private void OnQuestSelected(ListView.ListViewItem selectItem)
    {
        if (SeletedItem != null) SeletedItem.Selected = false;
        SeletedItem = selectItem as UIQuestItem;
        SeletedItem.Selected = true;

        questInfo.SetQuestInfo((selectItem as UIQuestItem).quest); // 设置右侧面板信息
    }

    // 刷新UI
    private void RefreshUI()
    {
        // 清除当前任务列表
        mainListView.RemoveAll();
        branchListView.RemoveAll();

        // 重新生成任务列表
        foreach (var kv in QuestManager.Instance.allQuests)
        {
            if (showAvailableList) // 显示可接任务 info为空
            {
                if (kv.Value.Info != null) continue; // 不为空表示已经存在了 不是可接 则跳过
            }
            else if (kv.Value.Info == null) continue; // 显示进行中任务 如果为空 则跳过

            var isMainQuest = kv.Value.Define.Type == QuestType.Main;
            // 实例化任务 设置任务信息 添加到列表
            GameObject go = Instantiate(itemPrefab, isMainQuest ? mainListView.transform : branchListView.transform);
            UIQuestItem item = go.GetComponent<UIQuestItem>();
            item.SetQuestInfo(kv.Value);
            if (isMainQuest) mainListView.Add(item);
            else branchListView.Add(item);
        }
    }
}