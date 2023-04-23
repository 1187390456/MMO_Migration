using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomTools;
using Models;
using Services;
using SkillBridge.Message;
using UnityEngine;

namespace Manager
{
    // 管理自己的道具
    public class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>(); // 格子id 对应物品

        // 初始化
        public void Init(List<NItemInfo> items)
        {
            Items.Clear(); // 清空之前的

            // 遍历传过来的物品 添加到缓存
            foreach (var info in items)
            {
                Item item = new Item(info);
                Items.Add(item.Id, item);

                Debug.LogFormat("ItemManager:Init[{0}]", item);
            }
            // 注册道具变化通知
            StatusService.Instance.RegisterStatusNofity(StatusType.Item, Recv_ItemNofify);
        }

        // 添加道具
        private void AddItem(int itemId, int count)
        {
            if (Items.TryGetValue(itemId, out Item item)) item.Count += count;
            else Items.Add(itemId, new Item(itemId, count)); // 没有该道具 添加该道具
            BagManager.Instance.AddItem(itemId, count); // 添加到背包
        }

        // 移除道具
        private void RemoveItem(int itemId, int count)
        {
            if (Items.TryGetValue(itemId, out Item item))
            {
                if (item.Count < count) throw new Exception("RemoveItem : No enough count");
                item.Count -= count;
                BagManager.Instance.RemoveItem(itemId, count); // 从背包移除
            }
            else throw new Exception($"RemoveItem : No this Item :{itemId}");
        }

        // 接收道具变化通知
        private bool Recv_ItemNofify(NStatus status)
        {
            if (status.Action == StatusAction.Add) AddItem(status.Id, status.Value);
            else if (status.Action == StatusAction.Delete) RemoveItem(status.Id, status.Value);
            return true;
        }
    }
}