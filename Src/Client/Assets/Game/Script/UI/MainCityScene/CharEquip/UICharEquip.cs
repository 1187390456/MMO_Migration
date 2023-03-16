using Manager;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CustomTools;
using Models;
using UnityEngine.UI;

public class UICharEquip : UIBase
{
    // 左侧

    private Transform leftEquipListContent;
    private GameObject leftEquipItem;

    // 中间

    private Transform mainEquipListContent;
    private GameObject mainEquipItem;
    private List<GameObject> mainEquipList = new List<GameObject>();

    private Text Glod;
    private BaseEquipItem selectItem;

    private void Awake()
    {
        leftEquipListContent = transform.Find("Bg/Center/Left/EquipList/Viewport/Content");
        leftEquipItem = Resources.Load<GameObject>("UI/Prefabs/Equips/UILeftEquipItem");
        mainEquipListContent = transform.Find("Bg/Center/Main/Equips");
        mainEquipItem = Resources.Load<GameObject>("UI/Prefabs/Equips/UIMainEquipItem");

        Glod = transform.Find("Bg/Gold/Count").GetComponent<Text>();
    }

    private void Start()
    {
        RenderUI();
        EquipManager.Instance.EquipChangedHandler += RenderUI;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        EquipManager.Instance.EquipChangedHandler -= RenderUI;
    }

    public void SetSelectItem(BaseEquipItem item)
    {
        if (selectItem != null) selectItem.Selected = false;
        selectItem = item;
    }

    // 重新刷新UI
    private void RenderUI()
    {
        RenderLeftEquipsList();
        RenderMainEquipsList();
        Glod.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    // 刷新左侧装备列表
    private void RenderLeftEquipsList()
    {
        // 清空之前的
        CommonTools.DestoryAllChild(leftEquipListContent);

        // 重新初始化 遍历道具中的装备
        foreach (var kv in ItemManager.Instance.Items)
        {
            if (kv.Value.Define.Type == ItemType.Equip) // 类型判断
            {
                if (EquipManager.Instance.Contains(kv.Key)) continue; // 已经准备了 不显示
                GameObject go = Instantiate(leftEquipItem, leftEquipListContent);
                go.GetComponent<UILeftEquipItem>().SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }

    // 刷新中间装备列表

    private void RenderMainEquipsList()
    {
        // 清空之前的
        if (mainEquipList.Count > 0) CommonTools.DestoryAllChild(mainEquipList);

        // 重新初始化
        for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i]; // 遍历当前装备数据

            if (item != null)
            {
                GameObject go = Instantiate(mainEquipItem, mainEquipListContent.GetChild(i));
                go.GetComponent<UIMainEquipItem>().SetEquipItem(item.Id, item, this, true);
                mainEquipList.Add(go);
            }
        }
    }

    // 装备
    public void Equip(Item item) => EquipManager.Instance.Send_EquipItem(item);

    // 脱下
    public void UnEquip(Item item) => EquipManager.Instance.Send_UnEquipItem(item);
}