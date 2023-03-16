using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using CustomTools;
using Common;

public class UIGuildMemberItem : ListView.ListViewItem
{
    public Text nickName;
    public Text @class;
    public Text level;
    public Text title;
    public Text joinTime;
    public Text status;

    public Image backGround;
    public Sprite normalBg;
    public Sprite selectedBg;

    private NGuildMemberInfo info;

    public NGuildMemberInfo Info
    {
        get => info;
        set
        {
            info = value;
            if (info != null) UpdateGuildMemberInfo(info);
        }
    }

    private void UpdateGuildMemberInfo(NGuildMemberInfo info)
    {
        if (nickName != null) nickName.text = info.Info.Name;
        if (@class != null) @class.text = info.Info.Class.ToString();
        if (level != null) level.text = info.Info.Level.ToString();
        if (title != null) title.text = info.Title.ToString();
        if (joinTime != null) joinTime.text = TimeUtil.GetTime(info.joinTime).ToShortDateString();
        if (status != null) status.text = info.Status == 1 ? "在线" : TimeUtil.GetTime(Info.lastTime).ToShortDateString();
    }

    public override void OnSelectedHandler(bool seleted) => backGround.overrideSprite = seleted ? selectedBg : normalBg;
}