using Common.Data;
using CustomTools;
using Manager;
using Models;
using Services;
using SkillBridge.Message;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CreatePanel : UIBase
{
    private Button backBtn;
    private Button startBtn;

    private Transform roloNameBox;
    private Text roloTypeDes;

    private Button[] roloTypeBtns = new Button[3];
    private Transform roleTypeTrans;

    private GameObject characterView;
    private CharacterDefine currentCharacter;
    private int currentActiveIndex;

    public int CurrenActiveIndex
    {
        get => currentActiveIndex;
        set
        {
            if (currentActiveIndex < 0 || currentActiveIndex > 2) throw new Exception("创建面板超出索引");

            currentActiveIndex = value;

            // 更新角色面板信息
            currentCharacter = DataManager.Instance.Characters[currentActiveIndex + 1];
            CommonTools.SetSoleActive(roloNameBox, currentActiveIndex);
            roloTypeDes.text = currentCharacter.Description;

            // 更新按钮
            for (int i = 0; i < roleTypeTrans.childCount; i++) CommonTools.SetActiveByName(roleTypeTrans.GetChild(i), "Normal", "Active", "Normal");
            CommonTools.SetActiveByName(roleTypeTrans.GetChild(currentActiveIndex), "Active", "Active", "Normal");

            // 更新左侧视图
            CommonTools.SetSoleActive(characterView.transform.Find("Root"), currentActiveIndex);
        }
    }

    private InputField userInputName;

    private void Awake()
    {
        backBtn = transform.Find("Bg/BackLogo").GetComponent<Button>();
        startBtn = transform.Find("InfoArea/BottomArea/Btn").GetComponent<Button>();

        roleTypeTrans = transform.Find("InfoArea/RoleType");

        roloNameBox = transform.Find("InfoArea/RoloNameBox");
        roloTypeDes = transform.Find("InfoArea/RoleDetailText").GetComponent<Text>();

        userInputName = transform.Find("InfoArea/BottomArea/Input").GetComponent<InputField>();

        characterView = GameObject.Find("CharacterView");

        Bind(UIEvent.SetCreatePanel_Active, UIEvent.SwitchCreatePanel_Event);
    }

    private void Start()
    {
        UserService.Instance.Res_CreateCharacter = Res_CreateCharacter;
        AddButtonAndListen();
        SwitchRole(1);
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SetCreatePanel_Active:
                SetActive((bool)message);
                break;

            case UIEvent.SwitchCreatePanel_Event:
                SetActive(true);
                SwitchRole(1);
                break;

            default:
                break;
        }
    }

    // 添加监听按钮
    private void AddButtonAndListen()
    {
        backBtn.onClick.AddListener(() =>
        {
            if (User.Instance.CurrentUserInfo.Player.Characters.Count == 0) MessageBox.Show("您还没有角色 , 先选择一个角色吧!", "提示");
            else SwitchPanel();
        });
        startBtn.onClick.AddListener(() =>
        {
            // 创建角色
            if (!CommonTools.TestString(userInputName.text, "名称不能为空哦!")) return;
            UserService.Instance.Send_CreateCharacter(userInputName.text, currentCharacter.Class);
        });

        for (int i = 0; i < roleTypeTrans.childCount; i++)
        {
            var index = i;
            var button = roleTypeTrans.GetChild(i).GetComponent<Button>();
            button.onClick.AddListener(() => SwitchRole(index));
            roloTypeBtns[i] = button;
        }
    }

    // 角色创建服务器返回
    private void Res_CreateCharacter(Result res, string msg)
    {
        if (res == Result.Success) SwitchPanel();
        else MessageBox.Show(msg, "错误", MessageBoxType.Error);
    }

    // 面板切换
    private void SwitchPanel()
    {
        // 跳转选择面板 重新渲染角色
        SetActive(false);
        Dispatch(AreaCode.UI, UIEvent.Render_CharacterInfo, null);
    }

    // 角色切换
    private void SwitchRole(int index) => CurrenActiveIndex = index;

    private void SetActive(bool value) => gameObject.SetActive(value);
}