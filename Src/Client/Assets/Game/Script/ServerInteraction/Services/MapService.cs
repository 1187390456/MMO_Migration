using Common.Data;
using CustomTools;
using Entities;
using Manager;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Text;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// 地图服务层
    /// </summary>
    internal class MapService : Singleton<MapService>, IDisposable
    {
        // 当前地图id
        public int CurrentMapId = 0;

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(Recv_MapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(Recv_MapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(Recv_MapEntitySync);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(Recv_MapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(Recv_MapCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(Recv_MapEntitySync);
        }

        public void Init()
        { }

        #region 发送层

        // 地图实体同步
        public void Send_MapEntitySync(EntityEvent entityEvent, NEntity entity, int param)
        {
            //  Debug.LogFormat("SendMapEntitySync :ID:{0} POS:{1} DIR:{2} SPD:{3} ", entity.Id, entity.Position.String(), entity.Direction.String(), entity.Speed);
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.mapEntitySync = new MapEntitySyncRequest { entitySync = new NEntitySync { Id = entity.Id, Event = entityEvent, Entity = entity, Param = param } };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        // 地图传送
        public void Send_MapTeleport(int ID)
        {
            Debug.LogFormat("SendMapTeleport :TeleportID:{0}", ID);
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.mapTeleport = new MapTeleportRequest { teleporterId = ID };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        #endregion 发送层

        #region 接收层

        // 接收地图角色进入
        private void Recv_MapCharacterEnter(object sender, MapCharacterEnterResponse res)
        {
            Debug.LogFormat("Recv_MapCharacterEnter:Map:{0} Count:{1}", res.mapId, res.Characters.Count);

            // 遍历地图角色 刷新角色信息
            foreach (var cha in res.Characters)
            {
                if (User.Instance.CurrentCharacter == null || (cha.Type == CharacterType.Player && User.Instance.CurrentCharacter.Id == cha.Id)) User.Instance.CurrentCharacter = cha;
                CharacterManager.Instance.Refresh_Character(cha);
            }

            // 当前地图id不一样 切换地图
            if (CurrentMapId != res.mapId)
            {
                EnterMap(res.mapId);
                CurrentMapId = res.mapId;
            }
        }

        // 接收地图角色离开
        private void Recv_MapCharacterLeave(object sender, MapCharacterLeaveResponse res)
        {
            Debug.LogFormat("Recv_MapCharacterLeave:characterID:{0}", res.entityId);
            if (User.Instance.CurrentCharacter != null && res.entityId != User.Instance.CurrentCharacter.EntityId) CharacterManager.Instance.Remove_Character(res.entityId);
            else CharacterManager.Instance.Clear();
        }

        // 接收角色同步
        private void Recv_MapEntitySync(object sender, MapEntitySyncResponse res)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Recv_MapEntitySync: Entity{0}", res.entitySyncs.Count);

            foreach (var entity in res.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);
                sb.AppendFormat("[{0}]ect:{1} entity:{2}", entity.Id, entity.Event, entity.Entity.String());
                sb.AppendLine();
            }
            Debug.Log(sb.ToString());
        }

        #endregion 接收层

        // 进入地图
        public void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
            }
            else Debug.LogErrorFormat("EnterMap: Map {0} not existed", mapId);
        }
    }
}