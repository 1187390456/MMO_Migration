using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleAvatar : UIBase
{
    private Text roleName;
    private Text roleLevel;

    private void Awake()
    {
        roleName = transform.Find("Name").GetComponent<Text>();
        roleLevel = transform.Find("Leave/Lv").GetComponent<Text>();
    }

    private void Start() => UpdateUserInfo();

    private void UpdateUserInfo()
    {
        var info = Models.User.Instance.CurrentCharacter;
        roleName.text = string.Format("{0}[{1}]", info.Name, info.Id);
        roleLevel.text = info.Level.ToString();
    }
}