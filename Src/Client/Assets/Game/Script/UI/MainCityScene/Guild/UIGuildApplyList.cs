using Manager;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UIGuildApplyList : UIBase
{
    public ListView listMain;
    public GameObject itemPrefab;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateList;
        GuildService.Instance.SendGuildListRequest(); // 发送一个消息 让批处理 过来同步更新
        UpdateList();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        GuildService.Instance.OnGuildUpdate -= UpdateList;
    }

    // 更新列表
    private void UpdateList()
    {
        if (GuildManager.Instance.guildInfo == null)
        {
            // 被踢了或者离开了
            Close(UIResult.Yes);
            return;
        }

        ClearList();
        InitItems();
    }

    // 清空列表
    private void ClearList() => listMain.RemoveAll();

    // 初始化列表
    private void InitItems()
    {
        foreach (var info in GuildManager.Instance.guildInfo.Applies)
        {
            GameObject go = Instantiate(itemPrefab, listMain.transform);
            UIGuildApplyListItem ui = go.GetComponent<UIGuildApplyListItem>();
            ui.Info = info;
            listMain.Add(ui);
        }
    }
}