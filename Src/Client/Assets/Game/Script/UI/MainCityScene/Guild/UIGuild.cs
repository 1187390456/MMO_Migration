using Manager;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UIGuild : UIBase
{
    [HideInInspector] public UIGuildMemberItem selectedItem;

    public UIGuildInfo uiInfo;
    public GameObject itemPrefab;
    public ListView listMain;

    public GameObject panelLeader;
    public GameObject panelAdmin;

    private void Start()
    {
        listMain.OnItemSelected += OnGuildItemSelected;
        GuildService.Instance.OnGuildUpdate += UpdateGuildList;
        UpdateGuildList();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        GuildService.Instance.OnGuildUpdate -= UpdateGuildList;
    }

    // 成员选中
    private void OnGuildItemSelected(ListView.ListViewItem selectedItem)
    {
        this.selectedItem = selectedItem as UIGuildMemberItem;
    }

    // 更新工会信息
    private void UpdateGuildList()
    {
        if (GuildManager.Instance.guildInfo == null)
        {
            // 被踢了或者离开了
            Close(UIResult.Yes);
            return;
        }
        uiInfo.Info = GuildManager.Instance.guildInfo; // 更新左侧
        ClearList();
        InitItems();

        panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.None);
        panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.President);
    }

    // 清空列表
    private void ClearList() => listMain.RemoveAll();

    // 初始化列表
    private void InitItems()
    {
        foreach (var guild in uiInfo.Info.Members)
        {
            GameObject go = Instantiate(itemPrefab, listMain.transform);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.Info = guild;
            listMain.Add(ui);
        }
    }

    #region 操作

    // 离开工会
    public void OnClickLeave()
    {
        var guildInfo = GuildManager.Instance.guildInfo;
        MessageBox.Show($"确定要离开 {guildInfo.GuildName} 工会吗?", "提示", MessageBoxType.Confirm).OnYes = () =>
        GuildService.Instance.SendGuildLeaveRequest();
    }

    // 私聊
    public void OnClickChat()
    {
    }

    // 打开申请列表
    public void OnClickApplylist() => UIManager.Instance.Show<UIGuildApplyList>();

    // 踢人
    public void OnClickKickout()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要踢出的成员!");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.President)
        {
            MessageBox.Show("你不可以踢会长哦!");
            return;
        }
        MessageBox.Show($"确定要把{selectedItem.Info.Info.Name}踢出工会吗?", "踢出工会", MessageBoxType.Confirm).OnYes = () =>
        GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, selectedItem.Info.Info.Id);
    }

    // 提升职位
    public void OnClickPromote()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要晋升职位的成员!");
            return;
        }
        if (selectedItem.Info.Title != GuildTitle.None)
        {
            MessageBox.Show("对方已经有职位了哦!");
            return;
        }
        MessageBox.Show($"确定要提升{selectedItem.Info.Info.Name}的职位吗?", "提升职位", MessageBoxType.Confirm).OnYes = () =>
        GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, selectedItem.Info.Info.Id);
    }

    // 罢免职位
    public void OnClickDepose()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要罢免职位的成员!");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.None)
        {
            MessageBox.Show("对方没有职位可以罢免哦!");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.President)
        {
            MessageBox.Show("会长不可以被罢免哦!");
            return;
        }
        MessageBox.Show($"确定要罢免{selectedItem.Info.Info.Name}的职位吗?", "罢免职位", MessageBoxType.Confirm).OnYes = () =>
        GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, selectedItem.Info.Info.Id);
    }

    // 会长转让
    public void OnClickTransfer()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要转让的成员!");
            return;
        }
        if (selectedItem.Info.Title == GuildTitle.President)
        {
            MessageBox.Show("你不可以选择自己!");
            return;
        }

        MessageBox.Show($"确定要把会长转让给{selectedItem.Info.Info.Name}吗?", "会长转让", MessageBoxType.Confirm).OnYes = () =>
        GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, selectedItem.Info.Info.Id);
    }

    // 修改工会宣言
    public void OnClickEditorNotice()
    {
        //TODO
    }

    #endregion 操作
}