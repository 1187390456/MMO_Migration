using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomTools;

public class UIQuestItem : ListView.ListViewItem
{
    public Text title;
    public Sprite normalSprite;
    public Sprite activeSprite;

    public Quest quest;
    private Image normalImage;

    private void Awake()
    {
        normalImage = GetComponent<Image>();
    }

    // 选中事件回调
    public override void OnSelectedHandler(bool seleted) => normalImage.overrideSprite = seleted ? activeSprite : normalSprite;

    // 设置任务Item信息
    public void SetQuestInfo(Quest quest)
    {
        this.quest = quest;
        title.text = string.Format("[{0}] {1}", EnumUtil.GetEnumDescription(quest.Define.Type), quest.Define.Name);
    }
}