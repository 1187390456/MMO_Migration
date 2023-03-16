using Manager;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UITeamItem : ListView.ListViewItem
{
    public Text nickName;
    public Image classIcon;
    public Image leaderIcon;
    public Image backGround;

    public override void OnSelectedHandler(bool seleted) => backGround.enabled = seleted;

    public int idx;
    public NCharacterInfo Info;

    private void Start() => backGround.enabled = false;

    public void SetMemberInfo(int idx, NCharacterInfo item, bool isLeader)
    {
        this.idx = idx;
        Info = item;
        if (nickName != null) nickName.text = Info.Level.ToString().PadRight(4) + Info.Name;
        //  if (classIcon != null) classIcon.overrideSprite = SpriteManager.Instance.classIcons[(int)Info.Class - 1];
        if (leaderIcon != null) leaderIcon.gameObject.SetActive(isLeader);
    }
}