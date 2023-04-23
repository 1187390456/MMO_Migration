using CustomTools;
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
    public class EquipManager : Singleton<EquipManager>
    {
        public Action EquipChangedHandler;
        public Item[] Equips = new Item[(int)EquipSlot.SlotMax]; // 装备区列表
        private byte[] Data;

        // 初始化
        public unsafe void Init(byte[] data)
        {
            Data = data;
            ParseEquipData(data);
        }

        #region 发送

        // 发送穿戴装备

        public void Send_EquipItem(Item equip) => ItemService.Instance.Send_ItemEquip(equip, true);

        // 发送脱下装备

        public void Send_UnEquipItem(Item equip) => ItemService.Instance.Send_ItemEquip(equip, false);

        #endregion 发送

        #region 接收

        // 接收穿戴装备返回
        internal void Recv_EquipItem(Item equip)
        {
            Item target = Equips[(int)equip.EquipDefine.Slot];
            if (target != null && target.Id == equip.Id) return; // 相同的装备
            Equips[(int)equip.EquipDefine.Slot] = ItemManager.Instance.Items[equip.Id]; // 从角色身上的道具中拿
            EquipChangedHandler?.Invoke();
        }

        // 接收脱下装备返回

        internal void Recv_UnEquipItem(EquipSlot slot)
        {
            if (Equips[(int)slot] != null)
            {
                Equips[(int)slot] = null;
                EquipChangedHandler?.Invoke();
            }
        }

        #endregion 接收

        // 获取指定格子的装备信息
        public Item GetEquipInfo(EquipSlot slot) => Equips[(int)slot];

        // 是否穿戴指定道具
        public bool Contains(int equipId)
        {
            for (int i = 0; i < Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].Id == equipId) return true;
            }
            return false;
        }

        // 解析数据库中的装备字节数据 并存储
        private unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = Data)
            {
                for (int i = 0; i < Equips.Length; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int)); // 拿到每个指针的对应的数据
                    if (itemId > 0) Equips[i] = ItemManager.Instance.Items[itemId];
                    else Equips[i] = null;
                }
            }
        }

        // 根据装备信息 返回一个字节数据
        private unsafe byte[] GetEquipData()
        {
            fixed (byte* pt = Data)
            {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemid = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null) *itemid = 0; // 装备不存在将该指针的值赋值为0
                    else *itemid = Equips[i].Id;
                }
            }
            return Data;
        }
    }
}