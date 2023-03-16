using Common;
using Manager;
using Models;
using Network;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class FriendService : Singleton<FriendService>, IDisposable
{
    public UnityAction OnFriendUpdate; // 好友列表更新

    public void Init()
    { }

    public FriendService()
    {
        MessageDistributer.Instance.Subscribe<FriendAddRequest>(Recv_FriendAddRequest); // 别人添加发过来的请求

        MessageDistributer.Instance.Subscribe<FriendAddResponse>(Recv_FriendAddResponse);
        MessageDistributer.Instance.Subscribe<FriendListResponse>(Recv_FriendList);
        MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(Recv_FriendRemove);
    }

    public void Dispose()
    {
        MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(Recv_FriendAddRequest);
        MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(Recv_FriendAddResponse);
        MessageDistributer.Instance.Unsubscribe<FriendListResponse>(Recv_FriendList);
        MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(Recv_FriendRemove);
    }

    #region 发送层

    //  发送添加好友请求
    public void SendFriendAddRequest(int friendId, string friendName)
    {
        Debug.LogFormat($"SendFriendAddRequest: friendId:{friendId} friendName:{friendName}");
        NCharacterInfo self = User.Instance.CurrentCharacter;
        NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
        msg.Request.friendAddReq = new FriendAddRequest
        {
            FromId = self.Id,
            FromName = self.Name,
            ToId = friendId,
            ToName = friendName
        };
        NetService.Instance.CheckConnentAndSend(msg);
    }

    //  发送添加好友响应  同意或拒绝该玩家
    public void SendFriendAddResponse(bool accept, FriendAddRequest request)
    {
        Debug.LogFormat($"SendFriendAddResponse: fromId:{request.FromId} fromName:{request.FromName}");
        NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
        msg.Request.friendAddRes = new FriendAddResponse
        {
            Request = request,
            Result = accept ? Result.Success : Result.Failed,
            Errormsg = accept ? $"{request.ToName}同意了你的请求" : $"{request.ToName}拒绝了你的请求"
        };
        NetService.Instance.CheckConnentAndSend(msg);
    }

    // 发送删除好友请求
    public void SendFriendRemoveRequest(int selfId, int friendId)
    {
        Debug.LogFormat($"SendFriendRemoveRequest: selfId:{selfId} friendId:{friendId}");
        NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
        msg.Request.friendRemove = new FriendRemoveRequest
        {
            Id = selfId,
            friendId = friendId
        };
        NetService.Instance.CheckConnentAndSend(msg);
    }

    #endregion 发送层

    #region 接收层

    //  接收添加好友请求
    private void Recv_FriendAddRequest(object sender, FriendAddRequest res)
    {
        Debug.LogFormat($"Recv_FriendAddRequest: FromName:{res.FromName}");
        UIMessageBox confirm = MessageBox.Show($"玩家{res.FromName}请求添加你为好友!", "好友请求", MessageBoxType.Confirm, "接受", "拒绝");
        confirm.OnYes = () => SendFriendAddResponse(true, res);
        confirm.OnNo = () => SendFriendAddResponse(false, res);
    }

    //  接收添加好友响应
    private void Recv_FriendAddResponse(object sender, FriendAddResponse res)
    {
        Debug.LogFormat($"Recv_FriendAddResponse: Errormsg:{res.Errormsg}");
        if (res.Result == Result.Success) MessageBox.Show(res.Errormsg, "添加好友成功");
        else MessageBox.Show(res.Errormsg, "添加好友失败");
    }

    //  接收移除好友
    private void Recv_FriendRemove(object sender, FriendRemoveResponse res)
    {
        Debug.LogFormat($"Recv_FriendRemove: Errormsg:{res.Errormsg}");
        if (res.Result == Result.Success) MessageBox.Show(res.Errormsg, "删除好友");
        else MessageBox.Show(res.Errormsg, "删除好友", MessageBoxType.Error);
    }

    //  接收获取好友列表 好友列表更新
    private void Recv_FriendList(object sender, FriendListResponse res)
    {
        Debug.LogFormat($"Recv_FriendAddResponse: FromName:{res.Errormsg}");
        FriendManager.Instance.allFriends = res.Friends;
        OnFriendUpdate?.Invoke();
    }

    #endregion 接收层
}