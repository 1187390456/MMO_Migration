using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomTools;
using UnityEngine.UI;
using Common.Data;
using Models;
using Manager;
using System;

public class UIShop : UIBase
{
    private GameObject[] pages;
    private int pageCount;
    private int currendPage;

    public int CurrentPage
    {
        get => currendPage;
        set
        {
            currendPage = value < 0 ? pageCount - 1 : value % pageCount;
            pageText.text = $"{CurrentPage + 1}/{pageCount}";
            SetPage(currendPage);
        }
    }

    private Button preBtn;
    private Button nextBtn;
    private Text pageText;

    private Text title;
    private Text gold;
    private ShopDefine shop;

    private Button buyBtn;

    public GameObject shopItem;
    public UIShopItem selectItem;

    private void Awake()
    {
        preBtn = transform.Find("Bg/Pagination/Left").GetComponent<Button>();
        nextBtn = transform.Find("Bg/Pagination/Right").GetComponent<Button>();
        pageText = transform.Find("Bg/Pagination/Main/Text").GetComponent<Text>();

        title = transform.Find("Title/Name").GetComponent<Text>();
        gold = transform.Find("Bg/Gold/Count").GetComponent<Text>();

        buyBtn = transform.Find("Bg/Buy").GetComponent<Button>();
        shopItem = Resources.Load<GameObject>("UI/Prefabs/UIShopItem");
    }

    private void Start()
    {
        StartCoroutine(InitItems());
        AddListen();
    }

    private void AddListen()
    {
        preBtn.onClick.AddListener(() => CurrentPage -= 1);

        nextBtn.onClick.AddListener(() => CurrentPage += 1);

        buyBtn.onClick.AddListener(() =>
        {
            if (selectItem == null)
            {
                MessageBox.Show("请选择要购买的道具", "购买提示");
                return;
            }
            ShopManager.Instance.BuyItem(shop.ID, selectItem.shopItemId);
        });
    }

    // 初始化商品Item
    private IEnumerator InitItems()
    {
        int temp = 0;
        int index = 0;
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if (kv.Value.Status > 0)
            {
                index = temp == 10 ? index + 1 : index;
                if (temp == 10) temp = 0;
                GameObject go = Instantiate(shopItem, pages[index].transform);
                go.GetComponent<UIShopItem>().SetShopItem(kv.Key, kv.Value, this);
                temp++;
            }
        }
        yield return null;

        CurrentPage = 0;
    }

    // 获取商品页数
    public int GetPageCount()
    {
        double count = 0;
        foreach (var item in DataManager.Instance.ShopItems[shop.ID].Values)
        {
            if (item.Status > 0) count++;
        }
        return (int)Math.Ceiling(count / 10);
    }

    // 设置商品页面
    private void SetPage(int index) => CommonTools.SetSoleActive(pages, index);

    // 创建商店页面
    private void CreatePage()
    {
        pages = new GameObject[pageCount];
        for (int i = 0; i < pageCount; i++)
        {
            var go = Instantiate(Resources.Load<GameObject>("UI/Prefabs/UIShopContent"), transform.Find("Bg"));
            pages[i] = go;
        }
    }

    // 设置商店信息
    public void SetShop(ShopDefine shopDefine)
    {
        shop = shopDefine;
        title.text = shopDefine.Name;
        gold.text = User.Instance.CurrentCharacter.Gold.ToString();
        pageCount = GetPageCount();

        CreatePage();
    }

    // 设置选择的商品
    public void SelectShopItem(UIShopItem selectItem)
    {
        if (this.selectItem != null) this.selectItem.Selected = false; // 去除上次的选中
        this.selectItem = selectItem;
    }
}