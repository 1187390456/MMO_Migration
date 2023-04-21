using CustomTools;
using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Manager
{
    /// <summary>
    /// 角色管理器
    /// </summary>
    public class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        // 角色列表 实体id
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        // 角色 进入离开 回调

        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<Character> OnCharacterLeave;

        // 清空角色
        public void Clear()
        {
            // 集合 Remove的时候 key一定要转成数组 不然会报修改未执行异常
            foreach (var entityId in Characters.Keys.ToArray()) Remove_Character(entityId);
            Characters.Clear();
        }

        // 添加角色 刷新信息
        public void Refresh_Character(NCharacterInfo cha)
        {
            Debug.LogFormat("AddCharacter:{0} Name:{1} Map:{2} Entity:{3}", cha.Id, cha.Name, cha.mapId, cha.Entity.String());

            Character character = new Character(cha);
            Characters[cha.EntityId] = character;
            EntityManager.Instance.AddEntity(character);

            OnCharacterEnter?.Invoke(character);
        }

        // 移除角色
        public void Remove_Character(int entityId)
        {
            if (Characters.ContainsKey(entityId))
            {
                EntityManager.Instance.RemoveEntity(Characters[entityId].Info.Entity);
                OnCharacterLeave?.Invoke(Characters[entityId]);
                Characters.Remove(entityId);
            }
        }

        // 获取角色
        public Character Get_Character(int entityId)
        {
            Characters.TryGetValue(entityId, out Character character);
            return character;
        }

        public void Dispose()
        { }
    }
}