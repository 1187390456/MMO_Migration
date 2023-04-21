using Models;
using Services;
using UnityEngine;
using UnityEngine.UI;

public class UITeam : MonoBehaviour
{
    public Text teamTitle;
    public UITeamItem[] Member; // 当前成员Item
    public ListView list; // 当前列表

    private void Start()
    {
        foreach (var item in Member) list.Add(item);

        if (User.Instance.CurrentTeam == null) gameObject.SetActive(false);
    }

    private void OnEnable() => RenderUI();

    // 控制队伍显示
    public void ShowTeamUI(bool isShow)
    {
        gameObject.SetActive(isShow);
        if (isShow) RenderUI();
    }

    // 更新队伍信息
    private void RenderUI()
    {
        if (User.Instance.CurrentTeam == null) return;

        teamTitle.text = $"我的队伍({User.Instance.CurrentTeam.Members.Count}/5)";

        for (int i = 0; i < 5; i++)
        {
            if (i < User.Instance.CurrentTeam.Members.Count)
            {
                var info = User.Instance.CurrentTeam; // 缓存中的队伍信息
                Member[i].SetMemberInfo(i, info.Members[i], info.Members[i].Id == info.Leader);
                Member[i].gameObject.SetActive(true);
            }
            else Member[i].gameObject.SetActive(false);
        }
    }

    // 点击离开
    public void OnClickLeave()
    {
        MessageBox.Show("确定要离开队伍吗?", "退出队伍", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        TeamService.Instance.SendTeamLeaveRequest();
    }
}