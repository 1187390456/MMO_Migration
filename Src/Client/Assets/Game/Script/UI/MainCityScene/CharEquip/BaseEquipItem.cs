using Common.Data;
using Manager;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseEquipItem : UIBase
{
    public Item item;
    public bool isEquip;
    public UICharEquip owner;

    public Action<bool> leftActiveHandler;

    private bool selected;

    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            leftActiveHandler?.Invoke(selected);
        }
    }

    public virtual void SetEquipItem(int itemid, Item item, UICharEquip owner, bool isEquip)
    {
        this.isEquip = isEquip;
        this.owner = owner;
        this.item = item;
    }

    // 装备
    public void Equip()
    {
        var oldEquip = EquipManager.Instance.GetEquipInfo(item.EquipDefine.Slot);
        if (oldEquip != null) MessageBox.Show($"要替换装备{item.Define.Name}吗?", "确认", MessageBoxType.Confirm).OnYes = () => owner.Equip(item);
        else
        {
            var action = MessageBox.Show($"要装备{item.Define.Name}吗?", "确认", MessageBoxType.Confirm);
            action.OnYes = () => owner.Equip(item);
            action.OnNo = () => Selected = true;
        }
    }

    // 脱下
    public void UnEquip() => MessageBox.Show($"要取下装备{item.Define.Name}吗?", "确认", MessageBoxType.Confirm).OnYes = () => owner.UnEquip(item);
}