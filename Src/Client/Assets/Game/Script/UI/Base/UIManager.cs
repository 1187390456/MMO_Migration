using System;
using System.Collections.Generic;
using UnityEngine;
using CustomTools;

public class UIManager : ManagerBase
{
    public static UIManager Instance = null;

    public class UIElement
    {
        public string Resources;
        public bool Cache;
        public GameObject Instance;
    }  // UI元素

    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>(); //  UI元素存储字典

    private void Awake()
    {
        Instance = this;
        AddCache();
    }

    // 初始添加缓存
    private void AddCache()
    {
        UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIElement/UIShop", Cache = false });
        UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIElement/UIBag", Cache = false });
        UIResources.Add(typeof(UICharEquip), new UIElement() { Resources = "UI/UIElement/UICharEquip", Cache = false });
        UIResources.Add(typeof(UIQuest), new UIElement() { Resources = "UI/UIElement/UIQuest", Cache = false });
        UIResources.Add(typeof(UIFriends), new UIElement() { Resources = "UI/UIElement/UIFriends", Cache = false });
        UIResources.Add(typeof(UIQuestDialong), new UIElement() { Resources = "UI/UIElement/UIQuestDialong", Cache = false });

        // 工会
        UIResources.Add(typeof(UIGuild), new UIElement() { Resources = "UI/UIElement/Guild/UIGuild", Cache = false });
        UIResources.Add(typeof(UIGuildPopNoGuild), new UIElement() { Resources = "UI/UIElement/Guild/UIGuildPopNoGuild", Cache = false });
        UIResources.Add(typeof(UIGuildPopCreate), new UIElement() { Resources = "UI/UIElement/Guild/UIGuildPopCreate", Cache = false });
        UIResources.Add(typeof(UIGuildList), new UIElement() { Resources = "UI/UIElement/Guild/UIGuildList", Cache = false });
        UIResources.Add(typeof(UIGuildApplyList), new UIElement() { Resources = "UI/UIElement/Guild/UIGuildApplyList", Cache = false });
    }

    // 显示UI
    public T Show<T>()
    {
        Type type = typeof(T);
        if (UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if (info.Instance != null) info.Instance.SetActive(true);
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);
                if (prefab == null) throw new Exception("Not this Resources !" + info.Resources);
                info.Instance = (GameObject)Instantiate(prefab, GameObject.FindGameObjectWithTag("Canvas").transform);
            }
#pragma warning disable UNT0014 // Invalid type for call to GetComponent
            return info.Instance.GetComponent<T>();
#pragma warning restore UNT0014 // Invalid type for call to GetComponent
        }
        return default;
    }

    // 关闭UI
    public void Close(Type type)
    {
        if (UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if (info.Cache) info.Instance.SetActive(false);
            else
            {
                Destroy(info.Instance);
                info.Instance = null;
            }
        }
    }
}