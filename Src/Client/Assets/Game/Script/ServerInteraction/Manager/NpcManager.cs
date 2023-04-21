using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data;
using CustomTools;

namespace Manager
{
    public class NpcManager : Singleton<NpcManager>
    {
        public Func<NpcDefine, bool> NpcActionHandler; // Npc动作事件

        private Dictionary<NpcFunction, Func<NpcDefine, bool>> eventMap = new Dictionary<NpcFunction, Func<NpcDefine, bool>>(); // 类型 动作事件字典

        public void RegisterNpcEvent(NpcFunction function, Func<NpcDefine, bool> action)
        {
            if (!eventMap.ContainsKey(function)) eventMap[function] = action;
            else eventMap[function] += action;
        }  // 注册事件

        public NpcDefine GetNpcDefine(int NpcID)
        {
            DataManager.Instance.Npcs.TryGetValue(NpcID, out NpcDefine npcDefine);
            return npcDefine;
        } // 获取NPC定义

        // 交互 一层 判断Npc定义是否存在
        public bool Interactive(int npcId)
        {
            if (DataManager.Instance.Npcs.ContainsKey(npcId))
            {
                var npc = DataManager.Instance.Npcs[npcId];
                return Interactive(npc);
            }
            return false;
        }

        // 交互 二层 判断Npc类型是否存在

        public bool Interactive(NpcDefine npc)
        {
            if (DoTaskInteractive(npc)) return true;
            else if (npc.Type == NpcType.Functional) return DoFunctionInteractive(npc);
            return false;
        }

        // 任务交互
        private bool DoTaskInteractive(NpcDefine npc)
        {
            NpcQuestStatus status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None) return false;
            return QuestManager.Instance.OpenNpcQuest(npc.ID);
        }

        // 功能交互
        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if (npc.Type != NpcType.Functional) return false;
            if (!eventMap.ContainsKey(npc.Function)) return false;
            return eventMap[npc.Function](npc);
        }
    }
}