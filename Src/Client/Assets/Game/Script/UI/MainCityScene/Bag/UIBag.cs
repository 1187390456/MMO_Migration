using Manager;
using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIBase
{
    private Toggle[] navBarToggles = new Toggle[2];
    private GameObject[] bagViews = new GameObject[2]; // 背包数量
    private List<Image> slots; // 背包插槽

    private GameObject bagItem; // 背包item项

    private Text gold; // 金币

    private void Awake()
    {
        navBarToggles[0] = transform.Find("Bg/Navbar/OpenBag1").GetComponent<Toggle>();
        navBarToggles[1] = transform.Find("Bg/Navbar/OpenBag2").GetComponent<Toggle>();

        bagViews[0] = transform.Find("Bg/Bag1").gameObject;
        bagViews[1] = transform.Find("Bg/Bag2").gameObject;
        bagViews[1].SetActive(false);

        gold = transform.Find("Bg/Gold/Count").GetComponent<Text>();

        bagItem = Resources.Load<GameObject>("UI/Prefabs/UIBagItem");
    }

    private void Start()
    {
        AddListen();
        InitSlots();
    }

    private void AddListen()
    {
        for (int i = 0; i < navBarToggles.Length; i++)
        {
            var temp = i;
            navBarToggles[i].onValueChanged.AddListener((isOn) => bagViews[temp].SetActive(isOn)); // 切换背包
        }
    }

    // 初始化插槽
    private void InitSlots()
    {
        if (slots == null)
        {
            slots = new List<Image>();
            for (int i = 0; i < bagViews.Length; i++) slots.AddRange(bagViews[i].transform.GetChild(0).GetChild(0).GetComponentsInChildren<Image>(true));
        }
        StartCoroutine(InitBags());
    }

    // 初始化背包
    private IEnumerator InitBags()
    {
        // 遍历解锁的格子
        for (int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            // 格子存在 实例化一个物品 并赋值
            if (item.ItemId > 0)
            {
                var define = ItemManager.Instance.Items[item.ItemId].Define;
                if (define.Type == ItemType.Normal || define.Type == ItemType.Material || define.Type == ItemType.Task) // 类型判断
                {
                    GameObject go = Instantiate(bagItem, slots[i].transform);
                    go.GetComponent<UIBagItem>().SetBagItem(define.Icon, item.Count.ToString());
                }
                else i--; // 不是指定类型
            }
        }

        // 遍历没有解锁的格子 设置成灰色

        for (int i = BagManager.Instance.Items.Length; i < slots.Count; i++) slots[i].color = Color.gray;

        SetGold();

        yield return null;
    }

    public void SetGold() => gold.text = User.Instance.CurrentCharacter.Gold.ToString();

    public void OnReset()
    {
        BagManager.Instance.Reset();
        // TODO 重新渲染背包
    }
}