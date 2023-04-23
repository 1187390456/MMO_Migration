using Manager;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Services;

public class UIFriends : UIBase
{
    public GameObject itemPrefabs;
    public ListView listMain;
    public Transform itemRoot;

    private UIFriendItem selectedItem;

    private void Start()
    {
        FriendService.Instance.OnFriendUpdate = RefreshUI;
        listMain.OnItemSelected += OnFriendSelected;
        RefreshUI();
    }

    // 刷新UI
    private void RefreshUI()
    {
        ClearFriendList();
        InitFriendItems();
    }

    // 清空好友列表
    private void ClearFriendList() => listMain.RemoveAll();

    // 初始化好友列表
    private void InitFriendItems()
    {
        foreach (var item in FriendManager.Instance.allFriends)
        {
            GameObject go = Instantiate(itemPrefabs, itemRoot);
            UIFriendItem friendItem = go.GetComponent<UIFriendItem>();
            friendItem.SetFriendsInfo(item);
            listMain.Add(friendItem);
        }
    }

    // 添加点击
    public void OnClickFirendAdd() => InputBox.Show("输入要添加的好友名称或ID", "添加好友", "确定", "取消", "输入不能为空!").OnSubmit += OnFirendAddSubmit;

    // 私聊点击
    public void OnClickFirendChat() => MessageBox.Show("暂未开放");

    // 邀请组队点击
    public void OnClickFriendTeamInvite()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要邀请的好友!");
            return;
        }
        if (selectedItem.Info.Status == 0)
        {
            MessageBox.Show("请选择在线的好友!");
            return;
        }
        MessageBox.Show($"确定要邀请好友{selectedItem.Info.friendInfo.Name}加入队伍吗?", "邀请组队", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        TeamService.Instance.SendTeamInviteRequest(selectedItem.Info.friendInfo.Id, selectedItem.Info.friendInfo.Name);
    }

    // 删除点击
    public void OnClickFirendDelete()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要删除的好友!");
            return;
        }
        MessageBox.Show($"确定要删除好友{selectedItem.Info.friendInfo.Name}吗?", "删除好友", MessageBoxType.Confirm, "删除", "取消").OnYes = () =>
        FriendService.Instance.SendFriendRemoveRequest(selectedItem.Info.Id, selectedItem.Info.friendInfo.Id); // 好友表中的 标识id 和 friedid (后期再优化了)
    }

    // 好友选中
    public void OnFriendSelected(ListView.ListViewItem item) => selectedItem = item as UIFriendItem;

    // 添加提交
    public bool OnFirendAddSubmit(string input, out string tips)
    {
        tips = "";
        string friendName = "";
        if (!int.TryParse(input, out int friendId)) friendName = input; // 解析下看是否输入是id
        if (friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
        {
            tips = "不可以添加自己哦!";
            return false;
        }
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }
}