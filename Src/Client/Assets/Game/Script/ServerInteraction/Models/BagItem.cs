using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    // 结构体字节流相互转换声明特性 定义内存中的存储格式
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BagItem
    {
        // ushort 无符号16位 1个类型占两个字节 在内存中

        public ushort ItemId; // 物品id
        public ushort Count; // 物品数量
        public static BagItem zero = new BagItem { ItemId = 0, Count = 0 };

        public BagItem(int itemId, int count)
        {
            ItemId = (ushort)itemId;
            Count = (ushort)count;
        }

        // 操作符重载
        public static bool operator ==(BagItem lhs, BagItem rhs) => lhs.ItemId == rhs.ItemId && lhs.Count == rhs.Count;

        public static bool operator !=(BagItem lhs, BagItem rhs) => !(lhs == rhs);

        /// <summary>
        ///  调用方法判断是否相同
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (other is BagItem item)
            {
                return Equals(item);
            }
            return false;
        }

        public bool Equals(BagItem other) => this == other;

        public override int GetHashCode() => ItemId.GetHashCode() ^ (Count.GetHashCode() << 2);
    }
}