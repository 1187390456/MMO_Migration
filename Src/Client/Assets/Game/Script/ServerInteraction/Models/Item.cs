using Common.Data;
using Manager;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    //  客户端来源网络 服务端来源DB
    public class Item
    {
        public int Id;
        public int Count;
        public ItemDefine Define;
        public EquipDefine EquipDefine;

        public Item(NItemInfo item) : this(item.Id, item.Count) // 重载构造
        { }

        public Item(int id, int count)
        {
            Id = id;
            Count = count;
            DataManager.Instance.Items.TryGetValue(Id, out Define);
            DataManager.Instance.Equips.TryGetValue(Id, out EquipDefine);
        }

        // 重载打印
        public override string ToString() => string.Format("ID:{0},Count:{1}", Id, Count);
    }
}