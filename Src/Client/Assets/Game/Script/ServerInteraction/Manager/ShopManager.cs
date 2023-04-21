using Common.Data;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomTools;
using UnityEngine;
using Services;

namespace Manager
{
    public class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        {
            NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OpenInvokeShop);
        }

        // 打开商店
        private bool OpenInvokeShop(NpcDefine npc)
        {
            Debug.LogFormat("TestManager.OnNpcInvokeShop: Npc: [{0}:{1}] Type:{2} Func:{3} ", npc.ID, npc.Name, npc.Type, npc.Function);

            // 获取存在的商店 传入商店id
            if (DataManager.Instance.Shops.TryGetValue(npc.Param, out ShopDefine shop))
            {
                UIShop uishop = UIManager.Instance.Show<UIShop>();
                if (uishop != null) uishop.SetShop(shop);
                return true;
            }
            else return false;
        }

        // 购买商品
        public bool BuyItem(int shopId, int shopItemId)
        {
            ItemService.Instance.Send_ItemBuy(shopId, shopItemId);
            return true;
        }
    }
}