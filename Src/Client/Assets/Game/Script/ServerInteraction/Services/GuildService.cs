using Common;
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
using UnityEngine.Events;
using UnityEngine.Networking.Types;

namespace Services
{
    public class GuildService : Singleton<GuildService>, IDisposable
    {
        public UnityAction OnGuildUpdate; // 工会更新返回
        public UnityAction<Result> OnGuildJoinRes; // 加入工会响应
        public UnityAction<bool> OnGuildCreateResult; // 工会创建返回
        public UnityAction<List<NGuildInfo>> OnGuildListResult; // 工会列表返回

        public void Init()
        { }

        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(Recv_GuildCreateResponse);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(Recv_GuildListResponse);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(Recv_GuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(Recv_GuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(Recv_GuildResponse);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(Recv_GuildLeaveResponse);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(Recv_GuildAdminResponse);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(Recv_GuildCreateResponse);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(Recv_GuildListResponse);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(Recv_GuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(Recv_GuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(Recv_GuildResponse);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(Recv_GuildLeaveResponse);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(Recv_GuildAdminResponse);
        }

        #region 创建工会

        // 发起工会创建
        public void SendGuildCreateRequest(string guildName, string notice)
        {
            Debug.LogFormat($"SendGuildCreateRequest: characterId:{User.Instance.CurrentCharacter.Id} guildName:{guildName}");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.guildCreate = new GuildCreateRequest
            {
                GuildName = guildName,
                GuildNotice = notice
            };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 工会创建响应
        private void Recv_GuildCreateResponse(object sender, GuildCreateResponse res)
        {
            Debug.LogFormat($"Recv_GuildCreateResponse: Result:{res.Result}");
            OnGuildCreateResult?.Invoke(res.Result == Result.Success); // 创建通知事件
            if (res.Result == Result.Success)
            {
                GuildManager.Instance.Init(res.guildInfo);
                MessageBox.Show($"{res.guildInfo.GuildName}工会创建成功", "工会");
            }
            else MessageBox.Show($"{res.guildInfo.GuildName}工会创建失败", "工会");
        }

        #endregion 创建工会

        #region 加入工会

        // 发起加入工会
        public void SendGuildJoinRequest(int guildId)
        {
            Debug.LogFormat($"SendGuildJoinRequest: characterId:{User.Instance.CurrentCharacter.Id} guildId:{guildId}");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.guildJoinReq = new GuildJoinRequest();
            msg.Request.guildJoinReq.Apply = new NGuildApplyInfo { GuildId = guildId };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 收到加入工会的请求
        private void Recv_GuildJoinRequest(object client, GuildJoinRequest ret)
        {
            Debug.LogFormat($"Recv_GuildJoinRequest: characterId:{User.Instance.CurrentCharacter.Id} guildId:{ret.Apply.GuildId}");
            var confirm = MessageBox.Show($"{ret.Apply.Name} 申请加入工会", "工会申请", MessageBoxType.Confirm, "同意", "拒绝");
            confirm.OnYes = () => SendGuildJoinResponse(true, ret);
            confirm.OnNo = () => SendGuildJoinResponse(false, ret);
        }

        // 发送加入工会请求的响应 (管理审批)
        public void SendGuildJoinResponse(bool accept, GuildJoinRequest ret)
        {
            Debug.LogFormat($"SendGuildJoinResponse: accept:{accept}");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.guildJoinRes = new GuildJoinResponse();
            msg.Request.guildJoinRes.Result = Result.Success;
            msg.Request.guildJoinRes.Apply = ret.Apply;
            msg.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 申请者接收到加入工会的响应
        private void Recv_GuildJoinResponse(object sender, GuildJoinResponse res)
        {
            Debug.LogFormat($"Recv_GuildJoinResponse: Result:{res.Result}");
            MessageBox.Show(res.Errormsg, "工会"); // 这里服务端写返回
            OnGuildJoinRes?.Invoke(res.Result);
        }

        #endregion 加入工会

        #region 离开工会

        // 发起离开工会
        public void SendGuildLeaveRequest()
        {
            Debug.LogFormat($"SendGuildLeaveRequest: characterId:{User.Instance.CurrentCharacter.Id}");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.guildLeave = new GuildLeaveRequest();
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 离开工会响应
        private void Recv_GuildLeaveResponse(object sender, GuildLeaveResponse res)
        {
            Debug.LogFormat($"Recv_GuildLeaveResponse: Result:{res.Result}");
            if (res.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("离开工会成功", "工会");
                OnGuildUpdate?.Invoke(); // 这里掉下刷新页面
            }
            else MessageBox.Show("离开工会失败", "工会", MessageBoxType.Error);
        }

        #endregion 离开工会

        #region 获取工会列表

        // 发起获取工会列表
        public void SendGuildListRequest()
        {
            Debug.LogFormat($"SendGuildListRequest: characterId:{User.Instance.CurrentCharacter.Id}");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.guildList = new GuildListRequest();
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 接收工会列表信息
        private void Recv_GuildListResponse(object client, GuildListResponse res)
        {
            Debug.LogFormat($"Recv_GuildListResponse: characterId:{User.Instance.CurrentCharacter.Id}");
            OnGuildListResult?.Invoke(res.Guilds);
        }

        #endregion 获取工会列表

        #region 工会审批

        // 发送加入工会请求的响应 (管理审批)
        public void SendGuildJoinApply(bool accept, NGuildApplyInfo info)
        {
            Debug.LogFormat($"SendGuildJoinApply: accept:{accept}");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.guildJoinRes = new GuildJoinResponse();
            msg.Request.guildJoinRes.Result = Result.Success;
            msg.Request.guildJoinRes.Apply = info;
            msg.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetService.Instance.CheckConnentAndSend(msg);
        }

        #endregion 工会审批

        #region 工会管理操作

        // 发送工会操作
        public void SendAdminCommand(GuildAdminCommand command, int characterId)
        {
            Debug.LogFormat($"SendAdminCommand: characterId:{User.Instance.CurrentCharacter.Id} GuildAdminCommand:{command} catchCharacterID:{characterId}");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.guildAdmin = new GuildAdminRequest
            {
                Command = command,
                Target = characterId
            };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 工会操作响应
        private void Recv_GuildAdminResponse(object sender, GuildAdminResponse res)
        {
            Debug.LogFormat($"Recv_GuildAdminResponse: Command{res.Command} Result:{res.Result}");
            MessageBox.Show(res.Errormsg, "提示");
        }

        #endregion 工会管理操作

        #region 额外批处理

        // 接收工会信息
        private void Recv_GuildResponse(object sender, GuildResponse res)
        {
            Debug.LogFormat($"Recv_GuildResponse: Result:{res.Result} guildInfo:{res.guildInfo}");
            GuildManager.Instance.Init(res.guildInfo);
            OnGuildUpdate?.Invoke();
        }

        #endregion 额外批处理
    }
}