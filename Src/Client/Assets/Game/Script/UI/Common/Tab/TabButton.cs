using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour
{
    public Sprite activeImage;
    public Sprite normalImage;

    private Image bg;

    private TabView tabView;
    private int tabIndex = 0;

    private void Start()
    {
        bg = GetComponent<Image>();
        GetComponent<Button>().onClick.AddListener(() => tabView.SelectTab(tabIndex));
    }

    // 设置选择状态
    public void Select(bool select) => bg.overrideSprite = select ? activeImage : normalImage;

    // 设置绑定TabView信息
    public void SetTabView(TabView tabView, int tabIndex)
    {
        this.tabView = tabView;
        this.tabIndex = tabIndex;
    }
}