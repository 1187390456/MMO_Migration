using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomTools;
using Manager;
using Models;
using Network;
using SkillBridge.Message;
using UnityEngine;

namespace Services
{
    public class ItemService : Singleton<ItemService>, IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(Recv_ItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(Recv_ItemEquip);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(Recv_ItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(Recv_ItemEquip);
        }

        public void Init()
        { }

        #region 发送层

        // 发送购买商品
        public void Send_ItemBuy(int shopId, int shopItemId)
        {
            Debug.LogFormat("SendBuyItem :shopId:{0} shopItemId:{1}", shopId, shopItemId);
            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.itemBuy = new ItemBuyRequest { shopId = shopId, shopItemId = shopItemId };
            NetService.Instance.CheckConnentAndSend(msg);
        }

        private Item pendingEquip = null; // 当前发送装备道具
        private bool isEquip = false; //  穿戴动作

        // 发送装备动作
        public bool Send_ItemEquip(Item equip, bool isEquip)
        {
            if (pendingEquip != null) return false;
            Debug.LogFormat("Send_ItemEquip :Item:{0} action:{1}", equip, isEquip ? "穿" : "脱");

            pendingEquip = equip;
            this.isEquip = isEquip;

            NetMessage msg = new NetMessage { Request = new NetMessageRequest() };
            msg.Request.itemEquip = new ItemEquipRequest { Slot = (int)equip.EquipDefine.Slot, itemId = equip.Id, isEquip = isEquip };
            NetService.Instance.CheckConnentAndSend(msg);

            return true;
        }

        #endregion 发送层

        #region 接收层

        // 接收购买商品返回
        private void Recv_ItemBuy(object sender, ItemBuyResponse res)
        {
            Debug.LogFormat("Recv_BuyItem :{0}", res.Result);
            MessageBox.Show(res.Errormsg, "购买提示");
        }

        // 接收道具装备

        private void Recv_ItemEquip(object sender, ItemEquipResponse res)
        {
            Debug.LogFormat("Recv_ItemEquip :{0}", res.Result);
            if (res.Result == Result.Success)
            {
                if (pendingEquip != null)
                {
                    if (isEquip) EquipManager.Instance.Recv_EquipItem(pendingEquip);
                    else EquipManager.Instance.Recv_UnEquipItem(pendingEquip.EquipDefine.Slot);
                    pendingEquip = null;
                }
            }
            else MessageBox.Show(res.Errormsg, "穿戴提示");
        }

        #endregion 接收层
    }
}