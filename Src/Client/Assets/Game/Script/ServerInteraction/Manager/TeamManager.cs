using Common;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class TeamManager : Singleton<TeamManager>
    {
        public void Init()
        {
        }

        // 更新队伍信息
        public void UpdateTeamInfo(NTeamInfo teamInfo)
        {
            User.Instance.CurrentTeam = teamInfo;
            ShowTeamUI(teamInfo != null);
        }

        // 显示队伍UI
        public void ShowTeamUI(bool isShow)
        {
            if (UIMain.Instance != null) UIMain.Instance.ShowTeamUI(isShow);
        }
    }
}