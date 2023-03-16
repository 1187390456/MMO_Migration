using Common;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    // 本地任务状态
    public enum NpcQuestStatus
    {
        None = 0,
        Complete,
        Available,
        Incomplete,
    }

    public class QuestManager : Singleton<QuestManager>
    {
        public List<NQuestInfo> questInfo = new List<NQuestInfo>(); // 角色当前身上已有任务
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();   // 所有任务
        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>(); // npc身上的任务
        public Action<Quest> OnQuestStatusChanged; // 任务状态改变事件

        public void Init(List<NQuestInfo> quests)
        {
            questInfo = quests;

            allQuests.Clear();
            npcQuests.Clear();

            InitQuests();
        }

        // 初始化任务
        private void InitQuests()
        {
            CheckServerQuests();
            CheckAvailableQuests();

            // 将所有人物添加到定义的NPC上
            foreach (var kv in allQuests)
            {
                AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        // 初始化服务端已有任务
        private void CheckServerQuests()
        {
            foreach (var info in questInfo)
            {
                Quest quest = new Quest(info);
                allQuests[quest.Info.QuestId] = quest;
            }
        }

        // 初始化本地可接任务
        private void CheckAvailableQuests()
        {
            // 初始化所有可用任务
            foreach (var kv in DataManager.Instance.Quests)
            {
                // 职业限制
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class) continue;
                // 等级限制
                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level) continue;
                // 任务存在
                if (allQuests.ContainsKey(kv.Key)) continue;

                if (kv.Value.PreQuest > 0) // 前置任务存在
                {
                    if (allQuests.TryGetValue(kv.Value.PreQuest, out Quest preQuest)) // 获取前置任务
                    {
                        if (preQuest.Info == null) continue; // 前置任务为空
                        if (preQuest.Info.Status != QuestStatus.Finished) continue; // 前置任务未完成
                    }
                    else continue; // 前置任务还没接
                }

                // 生成任务添加可接任务
                Quest quest = new Quest(kv.Value);
                allQuests[quest.Define.ID] = quest;
            }
        }

        // 添加npc任务
        private void AddNpcQuest(int npcId, Quest quest)
        {
            if (!npcQuests.ContainsKey(npcId)) npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>(); // 没有该任务 创建一个对应的任务字典

            // 获取添加npc任务
            AddNpcQuestStatus(npcId, NpcQuestStatus.Available);
            AddNpcQuestStatus(npcId, NpcQuestStatus.Complete);
            AddNpcQuestStatus(npcId, NpcQuestStatus.Incomplete);

            // 没有任务信息 添加npc可接受的任务列表
            if (quest.Info == null)
            {
                // 接受npc 添加可接受任务
                if (npcId == quest.Define.AcceptNPC)
                    AddNpcQuest(npcId, quest, NpcQuestStatus.Available);
            }
            else //有任务信息 添加完成和未完成的任务
            {
                // 提交npc  需要 多检测传过来的任务是指定类型
                if (npcId == quest.Define.SubmitNPC && quest.Info.Status == QuestStatus.Complated)
                    AddNpcQuest(npcId, quest, NpcQuestStatus.Complete);
                if (npcId == quest.Define.SubmitNPC && quest.Info.Status == QuestStatus.InProgress)
                    AddNpcQuest(npcId, quest, NpcQuestStatus.Incomplete);
            }
        }

        // 判断npc列表 并初始化
        private List<Quest> AddNpcQuestStatus(int npcId, NpcQuestStatus npcQuestStatus)
        {
            // 判断是否有该类型任务 有则直接返回 没有则创建一个空的返回
            if (!npcQuests[npcId].TryGetValue(npcQuestStatus, out List<Quest> quests))
            {
                quests = new List<Quest>();
                npcQuests[npcId][npcQuestStatus] = quests;
            }
            return quests;
        }

        // 添加npc列表
        private void AddNpcQuest(int npcId, Quest quest, NpcQuestStatus npcQuestStatus)
        {
            if (!npcQuests[npcId][npcQuestStatus].Contains(quest)) npcQuests[npcId][npcQuestStatus].Add(quest);
        }

        // 获取NPC任务状态
        public NpcQuestStatus GetQuestStatusByNpc(int npcId)
        {
            if (npcQuests.TryGetValue(npcId, out Dictionary<NpcQuestStatus, List<Quest>> quesDic))
            {
                if (quesDic[NpcQuestStatus.Complete].Count > 0) return NpcQuestStatus.Complete;
                if (quesDic[NpcQuestStatus.Available].Count > 0) return NpcQuestStatus.Available;
                if (quesDic[NpcQuestStatus.Incomplete].Count > 0) return NpcQuestStatus.Incomplete;
            }
            return NpcQuestStatus.None;
        }

        // 打开NPC当前任务
        public bool OpenNpcQuest(int npcId)
        {
            if (npcQuests.TryGetValue(npcId, out Dictionary<NpcQuestStatus, List<Quest>> quesDic))
            {
                if (quesDic[NpcQuestStatus.Complete].Count > 0) return ShowQuestDialog(quesDic[NpcQuestStatus.Complete].First());
                if (quesDic[NpcQuestStatus.Available].Count > 0) return ShowQuestDialog(quesDic[NpcQuestStatus.Available].First());
                if (quesDic[NpcQuestStatus.Incomplete].Count > 0) return ShowQuestDialog(quesDic[NpcQuestStatus.Incomplete].First());
            }
            return false;
        }

        // 显示任务对话框
        private bool ShowQuestDialog(Quest quest)
        {
            // 任务为空 或 任务完成了
            if (quest.Info == null || quest.Info.Status == QuestStatus.Complated)
            {
                UIQuestDialong dialong = UIManager.Instance.Show<UIQuestDialong>();
                dialong.SetQuest(quest);
                dialong.UIEventHandler += OnQuestDialongClose;
                return true;
            }
            // 任务不为空 或 任务状态完成
            if (quest.Info != null || quest.Info.Status == QuestStatus.Complated)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                    MessageBox.Show(quest.Define.DialogIncomplete);
            }
            return true;
        }

        // 任务对话框事件监听
        private void OnQuestDialongClose(UIBase sender, UIBase.UIResult res)
        {
            UIQuestDialong dialong = sender as UIQuestDialong;
            if (res == UIBase.UIResult.Yes) // 接或提交
            {
                if (dialong.quest.Info == null) QuestService.Instance.Send_QuestAccept(dialong.quest);// 接任务
                else if (dialong.quest.Info.Status == QuestStatus.Complated) QuestService.Instance.Send_QuestSubmit(dialong.quest); //  提交任务
            }
            else if (res == UIBase.UIResult.No) MessageBox.Show(dialong.quest.Define.DialogDeny); // 拒绝任务
        }

        // 刷新任务状态
        private Quest RefreshQuestStatus(NQuestInfo quest)
        {
            npcQuests.Clear(); // 清空之前的

            Quest res;

            // 同步服务器的任务信息
            if (allQuests.ContainsKey(quest.QuestId))
            {
                // 更新任务状态
                allQuests[quest.QuestId].Info = quest;
                res = allQuests[quest.QuestId];
            }
            else
            {
                // 添加任务
                res = new Quest(quest);
                allQuests[quest.QuestId] = res;
            }

            // 刷新可接任务并添加到npc中
            CheckAvailableQuests();
            foreach (var kv in allQuests)
            {
                AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }

            // 触发任务状态改变事件
            OnQuestStatusChanged?.Invoke(res);

            return res;
        }

        #region 服务端响应

        // 接收任务返回
        public void Recv_QuestAccept(NQuestInfo info)
        {
            var quest = RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        // 提交任务返回
        public void Recv_QuestSubmit(NQuestInfo info)
        {
            var quest = RefreshQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }

        #endregion 服务端响应
    }
}