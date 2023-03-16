using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildInfo : MonoBehaviour
{
    public Text guildName;
    public Text guildId;
    public Text leaderName;
    public Text notice;
    public Text memberCount;

    private NGuildInfo info;

    public NGuildInfo Info
    {
        get => info;
        set
        {
            info = value;
            if (info != null) UpdataInfo(value);
        }
    }

    private void UpdataInfo(NGuildInfo info)
    {
        if (guildName != null) guildName.text = $"工会名称: {info.GuildName}";
        if (guildId != null) guildId.text = $"工会id: {info.Id}";
        if (leaderName != null) leaderName.text = $"会长: {info.leaderName}";
        if (notice != null) notice.text = info.Notice;
        if (memberCount != null) memberCount.text = $"成员数量: {info.memberCount}/50";
    }
}