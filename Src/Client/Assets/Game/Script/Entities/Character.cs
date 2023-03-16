using Common.Data;
using Manager;
using Models;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// 角色实体
    /// </summary>
    public class Character : Entity
    {
        public NCharacterInfo Info; // 传输角色信息

        public CharacterDefine Define { get; set; } // 角色定义 数据表

        public int Id => Info.Id; // 角色id

        public string Name // 角色名称
        {
            get
            {
                if (Info.Type == CharacterType.Player) return Info.Name;
                else return Define.Name;
            }
        }

        public bool IsPlayer => Info.Type == CharacterType.Player; // 是否是玩家

        public bool IsCurrentPlayer => IsPlayer && Info.Id == User.Instance.CurrentCharacter.Id; // 是否是当前玩家

        // 前进
        public void MoveForward()
        {
            //  Debug.LogFormat("MoveForward");
            speed = Define.Speed;
        }

        //后退
        public void MoveBack()
        {
            //  Debug.LogFormat("MoveBack");
            speed = -Define.Speed;
        }

        // 停止
        public void Stop()
        {
            //  Debug.LogFormat("Stop");
            speed = 0;
        }

        // 设置方向
        public void SetDirection(Vector3Int dir)
        {
            // Debug.LogFormat("SetDirection:{0}", dir);
            direction = dir;
        }

        // 设置位置
        public void SetPosition(Vector3Int dir)
        {
            //   Debug.LogFormat("SetPosition:{0}", dir);
            position = dir;
        }

        public Character(NCharacterInfo info) : base(info.Entity)
        {
            Info = info;
            Define = DataManager.Instance.Characters[info.ConfigId];
        }
    }
}