using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : UIBase
{
    public Character character;

    private Text avatarName;

    private void Awake() => avatarName = transform.Find("Name").GetComponent<Text>();

    private void Update()
    {
        if (character != null) UpdateInfo();
    }

    private void UpdateInfo()
    {
        string name = character.Name + "  " + "Lv." + character.Info.Level;
        if (name != avatarName.text) avatarName.text = name;
    }
}