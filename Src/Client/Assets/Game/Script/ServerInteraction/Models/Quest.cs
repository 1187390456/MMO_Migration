using Common.Data;
using Manager;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomTools;

namespace Models
{
    public class Quest
    {
        public int QuestId => Define.ID;
        public QuestDefine Define; // 本地配置信息
        public NQuestInfo Info; // 网络配置信息

        public Quest(NQuestInfo info) // 通过网络配置信息构造 表示任务存在 可用
        {
            Info = info;
            Define = DataManager.Instance.Quests[info.QuestId];
        }

        public Quest(QuestDefine define) // 通过本地配置信息构造 表示任务可接 网络不存在 只存在本地
        {
            Define = define;
            Info = null;
        }

        // 获取任务定义名称
        public string GetTypeName() => EnumUtil.GetEnumDescription(Define.Type);
    }
}