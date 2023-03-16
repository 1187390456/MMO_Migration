using Common.Data;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem : UIBase, ISelectHandler
{
    private bool selected;

    public bool Selected
    {
        get => selected;
        set
        {
            selected = value;
            normalImage.overrideSprite = selected ? activeSprite : normalSprite;
        }
    }

    private Image normalImage;

    private Sprite normalSprite;
    private Sprite activeSprite;

    private Text title;
    private Text count;
    private Text price;
    private Image icon;

    // 所属商店 商店物品id 商店物品 物品定义

    public int shopItemId;
    private UIShop shop;
    private ShopItemDefine shopItem;
    private ItemDefine item;

    private void Awake()
    {
        normalImage = GetComponent<Image>();

        normalSprite = Resources.Load<Sprite>("Image/NormalShopItem");
        activeSprite = Resources.Load<Sprite>("Image/ActiveShopItem");

        title = transform.Find("Info/Title").GetComponent<Text>();
        count = transform.Find("Info/Count").GetComponent<Text>();
        price = transform.Find("Price/Gold").GetComponent<Text>();
        icon = transform.Find("Slot/Image").GetComponent<Image>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        Selected = true;
        shop.SelectShopItem(this);
    }

    // 设置商店物品Item
    public void SetShopItem(int id, ShopItemDefine shopItemDefine, UIShop owner)
    {
        shop = owner;
        shopItemId = id;
        shopItem = shopItemDefine;
        item = DataManager.Instance.Items[shopItem.ItemID];

        title.text = item.Name;
        count.text = shopItem.Count.ToString();
        price.text = shopItem.Price.ToString();
        icon.overrideSprite = Resources.Load<Sprite>(item.Icon);
    }
}