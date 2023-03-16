using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBagItem : UIBase
{
    private Image icon;
    private Text amount;

    private void Awake()
    {
        icon = GetComponent<Image>();
        amount = GetComponentInChildren<Text>();
    }

    public void SetBagItem(string iconPath, string count)
    {
        icon.overrideSprite = Resources.Load<Sprite>(iconPath);
        icon.SetNativeSize();
        amount.text = count;
    }
}