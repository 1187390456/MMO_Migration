using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UILeftEquipItem : BaseEquipItem, IPointerClickHandler
{
    private Image normalImage;

    private Sprite normalSprite;
    private Sprite activeSprite;

    private Image icon;
    private Text equipName;
    private Text level;
    private Text equipType;

    private void Awake()
    {
        leftActiveHandler = HanderSelectChange;

        normalImage = GetComponent<Image>();
        normalSprite = Resources.Load<Sprite>("Image/NormalShopItem");
        activeSprite = Resources.Load<Sprite>("Image/ActiveShopItem");

        icon = transform.Find("Slot/Image").GetComponent<Image>();
        equipName = transform.Find("Name").GetComponent<Text>();
        level = transform.Find("Info/Lev").GetComponent<Text>();
        equipType = transform.Find("Class").GetComponent<Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Selected)
        {
            Equip();
            Selected = false;
        }
        else
        {
            Selected = true;
            owner.SetSelectItem(this);
        }
    }
    // 设置左边道具信息
    public override void SetEquipItem(int itemid, Item item, UICharEquip owner, bool isEquip)
    {
        base.SetEquipItem(itemid, item, owner, isEquip);

        equipName.text = item.Define.Name;
        level.text = item.Define.Level.ToString();
        equipType.text = item.EquipDefine.Slot.ToString();
        icon.overrideSprite = Resources.Load<Sprite>(item.Define.Icon);
    }
    // 选择改变
    private void HanderSelectChange(bool value) => normalImage.overrideSprite = value ? activeSprite : normalSprite;
}