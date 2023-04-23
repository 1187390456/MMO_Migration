using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    public class Monster : CharacterBase
    {
        public Monster(int mapId, int tid, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, tid, level, pos, dir)
        {
            Define = DataManager.Instance.Characters[tid];
            Info = new NCharacterInfo
            {
                Type = CharacterType.Monster,
                Id = EntityId,
                Name = Define.Name,
                Entity = EntityData,
                EntityId = EntityData.Id,
                ConfigId = tid, // 配置id 即表格的id
                Level = level,
                Class = Define.Class,
                mapId = mapId,
            };
        }
    }
}