using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UIGuildApplyListItem : ListView.ListViewItem
{
    public Text nickName;
    public Text @class;
    public Text level;

    private NGuildApplyInfo info;

    public NGuildApplyInfo Info
    {
        get => info;
        set
        {
            info = value;
            if (info != null) UpdateGuildInfo(info);
        }
    }

    private void UpdateGuildInfo(NGuildApplyInfo info)
    {
        if (nickName != null) nickName.text = info.Name;
        if (nickName != null) nickName.text = info.Class.ToString();
        if (level != null) level.text = info.Level.ToString();
    }

    // 接收
    public void OnAccept()
    {
        MessageBox.Show($"要通过{info.Name}的工会申请吗?", "审批申请", MessageBoxType.Confirm, "同意加入", "取消").OnYes = () =>
        GuildService.Instance.SendGuildJoinApply(true, info);
    }

    // 拒绝
    public void OnDecline()
    {
        MessageBox.Show($"要拒绝{info.Name}的工会申请吗?", "审批申请", MessageBoxType.Confirm, "拒绝加入", "取消").OnYes = () =>
        GuildService.Instance.SendGuildJoinApply(false, info);
    }
}