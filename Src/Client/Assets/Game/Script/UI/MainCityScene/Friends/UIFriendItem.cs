using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : ListView.ListViewItem
{
    public Text nickName;
    public Text @class;
    public Text level;
    public Text status;

    public Image backGround;
    public Sprite normalBg;
    public Sprite selectedBg;

    public NFriendInfo Info;

    public override void OnSelectedHandler(bool seleted) => backGround.overrideSprite = seleted ? selectedBg : normalBg;

    public void SetFriendsInfo(NFriendInfo item)
    {
        Info = item;
        if (nickName != null) nickName.text = Info.friendInfo.Name;
        if (@class != null) @class.text = Info.friendInfo.Class.ToString();
        if (level != null) level.text = Info.friendInfo.Level.ToString();
        if (status != null) status.text = Info.Status == 1 ? "在线" : "离线";
    }
}