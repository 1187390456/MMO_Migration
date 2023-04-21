using Common;
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
    internal class StatusService : Singleton<StatusService>, IDisposable
    {
        public delegate bool StatusNotifyHandler(NStatus status);

        public Dictionary<StatusType, StatusNotifyHandler> statusNotifyDic = new Dictionary<StatusType, StatusNotifyHandler>(); // 事件通知

        private HashSet<StatusNotifyHandler> statusNotifyHandlers = new HashSet<StatusNotifyHandler>(); // 事件监听哈希表 判断是否存在相同事件

        public StatusService()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(Recv_StatusNotify);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(Recv_StatusNotify);
        }

        //  注册状态 谁要变化谁注册
        public void RegisterStatusNofity(StatusType type, StatusNotifyHandler action)
        {
            if (statusNotifyHandlers.Contains(action)) return; // 存在该事件了 直接退出

            if (!statusNotifyDic.ContainsKey(type)) statusNotifyDic[type] = action;
            else statusNotifyDic[type] += action;

            statusNotifyHandlers.Add(action); // 添加到哈希表
        }

        // 接收状态通知
        private void Recv_StatusNotify(object sender, StatusNotify message)
        {
            foreach (var status in message.Status)
            {
                // 通知变化
                Debug.LogFormat("Recv_StatusNotify: Type[{0}] Action[{1}] Id[{2}] Value[{3}]", status.Type, status.Action, status.Id, status.Value);

                // 金币变化
                if (status.Type == StatusType.Money)
                {
                    if (status.Action == StatusAction.Add) User.Instance.AddGold(status.Value);
                    else if (status.Action == StatusAction.Delete) User.Instance.AddGold(-status.Value);
                }

                // 执行其他变化 道具等
                if (statusNotifyDic.TryGetValue(status.Type, out StatusNotifyHandler handler)) handler(status);
            }
        }

        public void Init()
        {
        }
    }
}