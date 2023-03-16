using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UIGuildList : UIBase
{
    [HideInInspector] public UIGuildItem selectedItem;

    public UIGuildInfo uiInfo;
    public GameObject itemPrefab;
    public ListView listMain;

    private void Start()
    {
        listMain.OnItemSelected += OnGuildItemSelected;
        uiInfo.Info = null;

        GuildService.Instance.OnGuildListResult += UpdateGuildList;
        GuildService.Instance.OnGuildJoinRes += JoinRes;
        GuildService.Instance.SendGuildListRequest();
    }

   

    public override void OnDestroy()
    {
        base.OnDestroy();
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
    }

    // 申请加入
    public void OnClickJoin()
    {
        if (selectedItem == null) MessageBox.Show("请选择要加入的工会", "提示");
        else
        {
            MessageBox.Show($"确定要加入工会 {selectedItem.Info.GuildName} 吗?", "申请加入工会", MessageBoxType.Confirm).OnYes = () =>
            GuildService.Instance.SendGuildJoinRequest(selectedItem.Info.Id);
        }
    }
    // 加入工会返回
    private void JoinRes(Result res)
    {
        if (res == Result.Success) Close(UIResult.Yes);
    }

    // 工会列表item选中 赋值选择状态 左侧工会信息
    private void OnGuildItemSelected(ListView.ListViewItem selectedItem)
    {
        this.selectedItem = selectedItem as UIGuildItem;
        uiInfo.Info = this.selectedItem.Info;
    }

    // 更新工会列表
    private void UpdateGuildList(List<NGuildInfo> guilds)
    {
        ClearList();
        InitItems(guilds);
    }

    // 清空列表
    private void ClearList() => listMain.RemoveAll();

    // 初始化列表
    private void InitItems(List<NGuildInfo> guilds)
    {
        foreach (var guild in guilds)
        {
            GameObject go = Instantiate(itemPrefab, listMain.transform);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.Info = guild;
            listMain.Add(ui);
        }
    }
}