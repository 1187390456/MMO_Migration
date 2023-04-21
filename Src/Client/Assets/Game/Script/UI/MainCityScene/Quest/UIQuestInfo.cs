using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomTools;

public class UIQuestInfo : MonoBehaviour
{
    [Header("标题")] public Text title;
    [Header("任务描述")] public Text description;
    [Header("任务目标")] public GameObject[] targets;
    [Header("任务奖励 - 道具")] public Image[] itemReward;
    [Header("任务奖励 - 金币")] public Text goldReward;
    [Header("任务奖励 - 经验")] public Text expReward;

    [Header("布局Content")] public Transform content;

    public void SetQuestInfo(Quest quest)
    {
        title.text = string.Format("[{0}] {1}", EnumUtil.GetEnumDescription(quest.Define.Type), quest.Define.Name);

        if (quest.Info == null) description.text = quest.Define.Dialog; // 任务可接 显示任务对话描述
        else if (quest.Info.Status == QuestStatus.Complated) description.text = quest.Define.DialogFinish; // 任务进行中 且完成 显示完成对话描述

        goldReward.text = quest.Define.RewardGold.ToString();
        expReward.text = quest.Define.RewardExp.ToString();

        StartCoroutine(CommonTools.ForceUpdataContent(content));
    }
}