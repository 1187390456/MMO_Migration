using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Linq;

namespace GameServer.Services
{
    public class UserSerevice : Singleton<UserSerevice>
    {
        public UserSerevice()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(Recv_Login);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(Recv_CreateCharacter);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(Recv_GameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(Recv_GameLeave);
        }

        public void Init()
        {
        }

        // 注册请求处理
        [Obsolete("注册已与登录合并")]
        private void Recv_Register(NetConnection<NetSession> client, UserRegisterRequest request)
        {
            Log.InfoFormat("Recv_Register: User:{0} Pass:{1} ", request.User, request.Passward);

            client.Session.Response.userRegister = new UserRegisterResponse();
            UserRegisterResponse msg = client.Session.Response.userRegister;

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();

            if (user != null)
            {
                msg.Result = Result.Failed;
                msg.Errormsg = "用户已存在";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                msg.Result = Result.Success;
                msg.Errormsg = "None";
            }
            client.SendResponse();
        }

        // 登录请求处理 (为空注册 不为空直接登录)

        private void Recv_Login(NetConnection<NetSession> client, UserLoginRequest request)
        {
            Log.InfoFormat("Recv_Login: User:{0} Pass:{0} ", request.User, request.Passward);

            UserLoginResponse msg = new UserLoginResponse();
            client.Session.Response.userLogin = msg;

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();

            // 注册
            if (user == null)
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                user = new TUser() { Username = request.User, Password = request.Passward, Player = player };

                DBService.Instance.Entities.Users.Add(user);
                DBService.Instance.Entities.SaveChanges();
            }

            if (user.Password != request.Passward)
            {
                msg.Result = Result.Failed;
                msg.Errormsg = "密码错误!";
                client.SendResponse();
                return;
            }
            else if (user.Player.Characters.Count != 0 && SessionManager.Instance.GetSession(GetNCharacter(user.Player.Characters.ElementAt(0)).Id) != null)
            {
                // 这里临时加到判断是否该玩家已经在线上
                msg.Result = Result.Failed;
                msg.Errormsg = "用户已上线!";
                client.SendResponse();
                return;
            }

            client.Session.User = user; // 记录到缓存中

            msg.Result = Result.Success;
            msg.Errormsg = "none";
            msg.Userinfo = new NUserInfo { Id = (int)user.ID, Player = new NPlayerInfo { Id = user.Player.ID } };

            // 响应添加角色
            foreach (var item in user.Player.Characters) msg.Userinfo.Player.Characters.Add(GetNCharacter(item));

            client.SendResponse();
        }

        // 创建角色
        private void Recv_CreateCharacter(NetConnection<NetSession> client, UserCreateCharacterRequest request)
        {
            // 打印日志 从数据查询角色
            Log.InfoFormat("Recv_CreateCharacter: name:{0} clase:{0} ", request.Name, request.Class);
            TCharacter findCharacter = DBService.Instance.Entities.Characters.Where(c => c.Name == request.Name).FirstOrDefault();

            // 创建响应消息协议
            client.Session.Response.createChar = new UserCreateCharacterResponse();
            UserCreateCharacterResponse msg = client.Session.Response.createChar;

            if (findCharacter != null)
            {
                msg.Result = Result.Failed;
                msg.Errormsg = "名称已存在!";
            }
            else
            {
                //  创建数据库角色 添加数据库中 添加内存中 保存
                TCharacter character = new TCharacter
                {
                    Name = request.Name,
                    Class = (int)request.Class,
                    TID = (int)request.Class,
                    Level = 10,
                    MapID = 1,
                    MapPosX = 5000,
                    MapPosY = 4000,
                    MapPosZ = 820,
                    Gold = 100000,
                    Equips = new byte[28],
                    GuildId = 0,
                };
                // 初始化添加背包 角色背包一对一必须有 否则报错
                TCharacterBag bag = new TCharacterBag { Owner = character, Unlocked = 20, Items = new byte[0] };
                character.Bag = DBService.Instance.Entities.CharacterBags.Add(bag);
                character = DBService.Instance.Entities.Characters.Add(character);

                // 添加初始道具
                character.Items.Add(new TCharacterItem() { Owner = character, ItemID = 1, ItemCount = 20 });
                character.Items.Add(new TCharacterItem() { Owner = character, ItemID = 2, ItemCount = 20 });

                client.Session.User.Player.Characters.Add(character);
                DBService.Instance.Save();

                // 遍历内存中的角色 响应添加角色
                foreach (var item in client.Session.User.Player.Characters) msg.Characters.Add(GetNCharacter(item));

                msg.Result = Result.Success;
                msg.Errormsg = "None";
            }

            client.SendResponse();
        }

        // 进入游戏
        private void Recv_GameEnter(NetConnection<NetSession> client, UserGameEnterRequest request)
        {
            // 打印日志 从缓存User中拿数据  添加线上角色
            TCharacter dbCharacter = client.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("Recv_GameEnter: characterID:{0} Name:{1} Map:{2}", dbCharacter.ID, dbCharacter.Name, dbCharacter.MapID);

            // 添加到角色管理 记录连接对象
            Character character = CharacterManager.Instance.AddCharacter(dbCharacter);
            SessionManager.Instance.AddSession(character.Id, client);

            // 创建响应消息协议
            UserGameEnterResponse msg = new UserGameEnterResponse();

            // 赋值消息协议 并发送
            msg.Result = Result.Success;
            msg.Errormsg = "None";
            msg.Character = character.Info;

            // 存储内存的角色和批处理
            client.Session.Character = character;
            client.Session.PostResponser = character;

            // 角色上线处理
            if (character.Guild != null) character.Guild.timeStamp = TimeUtil.timeStamp; // 更新工会时间戳

            client.Session.Response.gameEnter = msg;
            client.SendResponse();

            // 地图初始化
            MapManager.Instance[character.Info.mapId].CharacterEnter(client, character);
        }

        // 退出游戏

        private void Recv_GameLeave(NetConnection<NetSession> client, UserGameLeaveRequest request)
        {
            // 打印日志 从内存中拿数据  移除线上角色
            Character character = client.Session.Character;
            Log.InfoFormat("Recv_GameLeave: characterID:{0} Name:{1} Map:{2}", character.Id, character.Name, character.Info.mapId);

            // 创建响应协议
            UserGameLeaveResponse msg = new UserGameLeaveResponse();

            // 处理角色离开
            CharacterLeave(character);

            // 赋值消息发送
            msg.Result = Result.Success;
            msg.Errormsg = "None";

            client.Session.Response.gameLeave = msg;
            client.SendResponse();
        }

        #region 私有方法

        // 根据 表信息 生成 传输信息
        private NCharacterInfo GetNCharacter(TCharacter character)
        {
            NCharacterInfo info = new NCharacterInfo();
            info.Id = character.ID;
            info.Name = character.Name;
            info.Type = CharacterType.Player;
            info.Class = (CharacterClass)character.Class;
            info.ConfigId = character.TID;
            return info;
        }

        // 角色离开处理
        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave： characterID:{0}:{1}", character.Id, character.Info.Name);

            // 角色离开处理
            if (character.Guild != null) character.Guild.timeStamp = TimeUtil.timeStamp; // 更新工会时间戳

            CharacterManager.Instance.RemoveCharacter(character.Id);
            character.Clear();
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);

            SessionManager.Instance.RemoveSession(character.Id);     // 删除连接
        }

        #endregion 私有方法
    }
}