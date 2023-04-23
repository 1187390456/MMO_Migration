using Common;
using Manager;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Services
{
    public class QuestService : Singleton<QuestService>, IDisposable
    {
        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(Recv_QuestSubmit);
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(Recv_QuestAccept);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(Recv_QuestSubmit);
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(Recv_QuestAccept);
        }

        #region 发送层

        // 发送任务接受
        public bool Send_QuestAccept(Quest quest)
        {
            Debug.Log($"Send_QuestAccept: questId:{quest.Define.ID}");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.questAccept = new QuestAcceptRequest { QuestId = quest.Define.ID };
            NetService.Instance.CheckConnentAndSend(msg);
            return true;
        }

        // 发送任务提交
        public bool Send_QuestSubmit(Quest quest)
        {
            Debug.Log($"Send_QuestSubmit: questId:{quest.Define.ID}");
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.questSubmit = new QuestSubmitRequest { QuestId = quest.Define.ID };
            NetService.Instance.CheckConnentAndSend(msg);
            return true;
        }

        #endregion 发送层

        #region 接收层

        // 接收任务接受返回
        private void Recv_QuestAccept(object client, QuestAcceptResponse res)
        {
            Debug.Log($"Recv_QuestAccept: questId:{res.Quest.QuestId} res:{res.Result} msg:{res.Errormsg}");
            if (res.Result == Result.Success) QuestManager.Instance.Recv_QuestAccept(res.Quest);
            else MessageBox.Show("任务接受失败", "错误", MessageBoxType.Error);
        }

        // 接收任务提交返回
        private void Recv_QuestSubmit(object client, QuestSubmitResponse res)
        {
            Debug.Log($"Recv_QuestSubmit: questId:{res.Quest.QuestId} res:{res.Result} msg:{res.Errormsg}");
            if (res.Result == Result.Success) QuestManager.Instance.Recv_QuestSubmit(res.Quest);
            else MessageBox.Show("任务完成失败", "错误", MessageBoxType.Error);
        }

        #endregion 接收层

        public void Init()
        {
        }
    }
}