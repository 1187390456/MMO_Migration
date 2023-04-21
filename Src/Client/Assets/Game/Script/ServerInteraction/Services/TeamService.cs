using CustomTools;
using Manager;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public class TeamService : Singleton<TeamService>, IDisposable
    {
        public void Init()
        { }

        public TeamService()
        {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(Recv_TeamInviteRequest);

            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(Recv_TeamInviteResponse);
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(Recv_TeamInfoResponse);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(Recv_TeamLeaveResponse);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(Recv_TeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(Recv_TeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(Recv_TeamInfoResponse);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(Recv_TeamLeaveResponse);
        }

        #region 邀请组队

        // 发送组队邀请
        public void SendTeamInviteRequest(int friendId, string friendName)
        {
            Debug.LogFormat($"SendTeamInviteRequest: friendId:{friendId} friendName:{friendName}");
            NCharacterInfo self = User.Instance.CurrentCharacter;
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.teamInviteReq = new TeamInviteRequest
            {
                FromId = self.Id,
                FromName = self.Name,
                ToId = friendId,
                ToName = friendName
            };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 收到组队邀请

        private void Recv_TeamInviteRequest(object sender, TeamInviteRequest res)
        {
            Debug.LogFormat($"Recv_TeamInviteRequest: FromName:{res.FromName}");
            UIMessageBox confirm = MessageBox.Show($"玩家{res.FromName}邀请你加入队伍!", "组队邀请", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () => SendTeamInviteResponse(true, res);
            confirm.OnNo = () => SendTeamInviteResponse(false, res);
        }

        // 发送组队邀请响应结果
        public void SendTeamInviteResponse(bool accept, TeamInviteRequest request)
        {
            Debug.LogFormat($"SendTeamInviteResponse: fromId:{request.FromId} fromName:{request.FromName}");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.teamInviteRes = new TeamInviteResponse
            {
                Request = request,
                Result = accept ? Result.Success : Result.Failed,
                Errormsg = accept ? $"组队成功" : $"{request.ToName}拒绝了你的组队请求"
            };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 接收组队邀请结果
        private void Recv_TeamInviteResponse(object sender, TeamInviteResponse res)
        {
            Debug.LogFormat($"Recv_TeamInviteResponse: Errormsg:{res.Errormsg}");
            if (res.Result == Result.Success) MessageBox.Show(res.Errormsg, "邀请组队");
            else MessageBox.Show(res.Errormsg, "邀请组队");
        }

        #endregion 邀请组队

        #region 离开队伍

        // 发送离开组队
        public void SendTeamLeaveRequest()
        {
            Debug.LogFormat($"SendTeamLeaveRequest");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.teamLeave = new TeamLeaveRequest
            {
                TeamId = User.Instance.CurrentTeam.Id,
                characterId = User.Instance.CurrentCharacter.Id
            };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 接收离开组队
        private void Recv_TeamLeaveResponse(object sender, TeamLeaveResponse res)
        {
            if (res.Result == Result.Success)
            {
                TeamManager.Instance.UpdateTeamInfo(null);
                MessageBox.Show("退出成功", "退出组队");
            }
            else MessageBox.Show("退出失败", "退出组队", MessageBoxType.Error);
        }

        #endregion 离开队伍

        #region 接收队伍信息

        // 接收队伍信息
        private void Recv_TeamInfoResponse(object sender, TeamInfoResponse res)
        {
            Debug.LogFormat($"Recv_TeamInfoResponse: FromName:{res.Errormsg}");
            TeamManager.Instance.UpdateTeamInfo(res.Team);
        }

        #endregion 接收队伍信息
    }
}