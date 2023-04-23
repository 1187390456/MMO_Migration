using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem
{
    public Text guildId;
    public Text guildName;
    public Text members;
    public Text leaderName;

    public Image backGround;

    private NGuildInfo info;

    public NGuildInfo Info
    {
        get => info;
        set
        {
            info = value;
            if (info != null) UpdateGuildInfo(info);
        }
    }

    private void UpdateGuildInfo(NGuildInfo info)
    {
        if (guildId != null) guildId.text = info.Id.ToString();
        if (guildName != null) guildName.text = info.GuildName;
        if (members != null) members.text = info.memberCount.ToString();
        if (leaderName != null) leaderName.text = info.leaderName;
    }

    public override void OnSelectedHandler(bool seleted) => backGround.enabled = seleted;
}