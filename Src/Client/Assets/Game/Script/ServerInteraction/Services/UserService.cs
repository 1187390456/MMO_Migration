using Common;
using Manager;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 用户服务层
/// </summary>
namespace Services
{
    internal class UserService : Singleton<UserService>, IDisposable
    {
        // 服务器响应事件委托

        public UnityAction<Result, string> Res_Login;
        public UnityAction<Result, string> Res_Register;
        public UnityAction<Result, string> Res_CreateCharacter;

        // 是否断开游戏
        private bool isQuitGame = false;

        public void Init()
        {
        }

        // 订阅 连接和断开连接处理 分发收到的消息处理
        public UserService()
        {
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(Recv_Login);
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(Recv_Register);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(Recv_CreateCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(Recv_GameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(Recv_GameLeave);
        }

        // 解除 连接和断开连接处理 分发收到的消息处理
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(Recv_Login);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(Recv_Register);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(Recv_CreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(Recv_GameEnter);
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(Recv_GameLeave);
        }

        #region 发送层

        // 登录
        public void Send_Login(string user, string psw)
        {
            Debug.LogFormat("Send_Login::user :{0} psw:{1}", user, psw);
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.userLogin = new UserLoginRequest { User = user, Passward = psw };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 注册
        public void Send_Register(string user, string psw)
        {
            Debug.LogFormat("Send_Register::user :{0} psw:{1}", user, psw);
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.userRegister = new UserRegisterRequest { User = user, Passward = psw };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 创建角色
        public void Send_CreateCharacter(string name, CharacterClass cls)
        {
            Debug.LogFormat("Send_CreateCharacter::name :{0} clase:{1}", name, cls);
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.createChar = new UserCreateCharacterRequest { Name = name, Class = cls };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 进入游戏
        public void Send_GameEnter(int characterIndex)
        {
            Debug.LogFormat("Send_GameEnter::characterIndex :{0}", characterIndex);
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.gameEnter = new UserGameEnterRequest { characterIdx = characterIndex };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 离开游戏
        public void Send_GameLeave(bool isQuitGame = false)
        {
            this.isQuitGame = isQuitGame;
            Debug.LogFormat("Send_GameLeave");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.gameLeave = new UserGameLeaveRequest();
            NetService.Instance.CheckConnentAndSend(msg);
        }

        #endregion 发送层

        #region 接收层

        private void Recv_Login(object sender, UserLoginResponse res)
        {
            Debug.LogFormat("Recv_Login:{0} [{1}]", res.Result, res.Errormsg);
            if (res.Result == Result.Success) User.Instance.SetupUserInfo(res.Userinfo);
            Res_Login?.Invoke(res.Result, res.Errormsg);
        }

        private void Recv_Register(object sender, UserRegisterResponse res)
        {
            Debug.LogFormat("Recv_Register:{0} [{1}]", res.Result, res.Errormsg);
            Res_Register?.Invoke(res.Result, res.Errormsg);
        }

        private void Recv_CreateCharacter(object sender, UserCreateCharacterResponse res)
        {
            Debug.LogFormat("Recv_CreateCharacter: {0} [{1}]", res.Result, res.Errormsg);
            if (res.Result == Result.Success) User.Instance.SetupUserCharacter(res.Characters);
            Res_CreateCharacter?.Invoke(res.Result, res.Errormsg);
        }

        private void Recv_GameEnter(object sender, UserGameEnterResponse res)
        {
            Debug.LogFormat("Recv_GameEnter: {0} [{1}]", res.Result, res.Errormsg);
            if (res.Result == Result.Success)
            {
                if (res.Character != null)
                {
                    User.Instance.CurrentCharacter = res.Character;
                    // TODO 初始化其他管理系统
                    ItemManager.Instance.Init(res.Character.Items);
                    BagManager.Instance.Init(res.Character.Bag);
                    EquipManager.Instance.Init(res.Character.Equips);
                    QuestManager.Instance.Init(res.Character.Quests);
                    FriendManager.Instance.Init(res.Character.Friends);
                    GuildManager.Instance.Init(res.Character.Guild);
                }
            };
        }

        private void Recv_GameLeave(object sender, UserGameLeaveResponse res)
        {
            MapService.Instance.CurrentMapId = 0;
            User.Instance.CurrentCharacter = null;
            Debug.LogFormat("Recv_GameLeave: {0} [{1}]", res.Result, res.Errormsg);
            if (isQuitGame)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
               Application.Quit();
#endif
            }
        }

        #endregion 接收层
    }
}