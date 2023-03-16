using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Models;

public class UIMainEquipItem : BaseEquipItem, IPointerClickHandler
{
    private Image icon;

    private void Awake()
    {
        icon = GetComponentInChildren<Image>();
    }
    public override void SetEquipItem(int itemid, Item item, UICharEquip owner, bool isEquip)
    {
        base.SetEquipItem(itemid, item, owner, isEquip);

        icon.overrideSprite = Resources.Load<Sprite>(item.Define.Icon);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Selected)
        {
            UnEquip();
            Selected = false;
        }
        else
        {
            Selected = true;
            owner.SetSelectItem(this);
        }
    }
}