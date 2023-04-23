using CustomTools;
using Models;
using Services;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectPanel : UIBase
{
    private Transform roloTrans;

    private GameObject characterView;
    private GameObject[] activeGobjs = new GameObject[4]; // 激活图片对象集

    private Button backBtn;
    private Button enterBtn;

    private int currentSelectIndex;

    public int CurrentSelectIndex
    {
        get => currentSelectIndex;
        set
        {
            currentSelectIndex = value;

            // 设置当前角色信息
            var roleBoxScript = roloTrans.GetChild(currentSelectIndex).GetComponent<RoleBox>();
            CommonTools.SetSoleActive(activeGobjs, CurrentSelectIndex);
            transform.Find("CharacterView/Name").GetComponent<Text>().text = roleBoxScript.nCharacterInfo.Level + "级 - " + roleBoxScript.nCharacterInfo.Name;
            CommonTools.SetSoleActive(characterView.transform.Find("Root"), (int)roleBoxScript.nCharacterInfo.Class - 1);
        }
    }

    private void Awake()
    {
        roloTrans = transform.Find("RolePanel");
        characterView = GameObject.Find("CharacterView");
        backBtn = transform.Find("Bg/BackLogo").GetComponent<Button>();
        enterBtn = transform.Find("Btn").GetComponent<Button>();

        Bind(UIEvent.SetSelectPanel_Active, UIEvent.Render_CharacterInfo);
    }

    private void Start()
    {
        EnterAndBackBtnListen();
        StartCoroutine(RenderCharacterAndListen());
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.SetSelectPanel_Active:
                SetActive((bool)message);

                break;

            case UIEvent.Render_CharacterInfo:
                SetActive(true);
                StartCoroutine(RenderCharacterAndListen());

                break;

            default:
                break;
        }
    }

    // 进入退出按钮监听
    private void EnterAndBackBtnListen()
    {
        backBtn.onClick.AddListener(() =>
        {
            MessageBox.Show("您的真的要退出吗?", "提示", MessageBoxType.Confirm, "是的", "取消").OnYes = () => Application.Quit();
        });
        enterBtn.onClick.AddListener(() =>
        {
            if (currentSelectIndex < 0 || currentSelectIndex > 3) throw new Exception("角色索引异常");
            else MessageBox.Show("确定进入游戏吗?", "提示", MessageBoxType.Confirm, "是的", "取消").OnYes = () => UserService.Instance.Send_GameEnter(CurrentSelectIndex);
        });
    }

    // 重新渲染角色信息
    private IEnumerator RenderCharacterAndListen()
    {
        yield return StartCoroutine(RenderRoloInfo());
        yield return StartCoroutine(AddBtnListen());
        SwitchRole(0);
    }

    // 渲染角色信息
    private IEnumerator RenderRoloInfo()
    {
        var players = User.Instance.CurrentUserInfo.Player.Characters;
        if (players.Count == 0 || players.Count > 4) yield break;
        for (int i = 0; i < players.Count; i++) roloTrans.GetChild(i).GetComponent<RoleBox>().SetRolo(players[i]);
    }

    // 添加信息面板按钮监听
    private IEnumerator AddBtnListen()
    {
        for (int i = 0; i < roloTrans.childCount; i++)
        {
            var index = i;
            var button = roloTrans.GetChild(i).GetComponent<Button>();
            button.onClick.RemoveAllListeners(); // 移除之前的

            var roloBoxScript = roloTrans.GetChild(i).GetComponent<RoleBox>();
            activeGobjs[i] = roloBoxScript.activeImage.gameObject; // 添加激活图片

            // 判断是否有角色 有则切换角色 没则切换创建面板
            if (roloBoxScript.nCharacterInfo == null) button.onClick.AddListener(SwitchPanel);
            else button.onClick.AddListener(() => SwitchRole(index));
        }
        yield break;
    }

    // 切换面板
    private void SwitchPanel()
    {
        SetActive(false);
        Dispatch(AreaCode.UI, UIEvent.SwitchCreatePanel_Event, null);
    }

    // 切换角色
    private void SwitchRole(int index) => CurrentSelectIndex = index;

    private void SetActive(bool value) => gameObject.SetActive(value);
}