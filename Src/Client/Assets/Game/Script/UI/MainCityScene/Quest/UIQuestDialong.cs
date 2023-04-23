using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SkillBridge.Message;

public class UIQuestDialong : UIBase
{
    [Header("当前任务信息")] public UIQuestInfo questInfo;

    [Header("拒绝按钮")] public GameObject refuseBtn;
    [Header("接受按钮")] public GameObject recvBtn;
    [Header("完成按钮")] public GameObject doneBtn;

    public Quest quest;

    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        UpdataQuest();

        if (quest.Info == null) // 可接任务
        {
            recvBtn.SetActive(true);
            refuseBtn.SetActive(true);
            doneBtn.SetActive(false);
        }
        else // 进行中任务
        {
            if (quest.Info.Status == QuestStatus.Complated) // 已完成
            {
                refuseBtn.SetActive(false);
                recvBtn.SetActive(false);
                doneBtn.SetActive(true);
            }
            else //未完成
            {
                refuseBtn.SetActive(true);
                recvBtn.SetActive(false);
                doneBtn.SetActive(false);
            }
        }
    }

    // 更新任务信息
    private void UpdataQuest()
    {
        if (quest != null && questInfo != null) questInfo.SetQuestInfo(quest);
    }
}