using CustomTools;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manager
{
    public class BagManager : Singleton<BagManager>
    {
        public int Unlocked; // 解锁格子数量
        public BagItem[] Items; // 背包格子存储的每个道具信息
        private NBagInfo info; // 背包信息

        /*
         不安全声明
        在不安全代码中，可以声明和操作指针、在指针和整型之间执行转换，以获取变量的地址等。
        在某种意义上，编写不安全代码非常类似于在 c # 程序中编写 C 代码。
         */

        public unsafe void Init(NBagInfo bagInfo)
        {
            info = bagInfo;
            Unlocked = info.Unlocked;
            Items = new BagItem[Unlocked]; // 初始化已解锁格子数

            // 背包物品不为空 且字节长度大于解锁格子数量 进行解析
            if (info.Items != null && info.Items.Length >= Unlocked) Analyze(info.Items);
            else
            {
                // 背包为空
                info.Items = new byte[sizeof(BagItem) * Unlocked];
                Reset();
            }
        }

        // 背包整理
        public void Reset()
        {
            int i = 0;

            // 遍历道具
            foreach (var kv in ItemManager.Instance.Items)
            {
                // 判断道具数量 堆叠限制
                int count = kv.Value.Count;
                int limit = kv.Value.Define.StackLimit;

                if (i >= Unlocked)
                {
                    // 背包满了
                    MessageBox.Show("背包已满,部分道具存储在邮件中,", "提示");
                    return;
                }

                if (count <= limit)
                {
                    Items[i].ItemId = (ushort)kv.Key;
                    Items[i].Count = (ushort)count;
                }
                else
                {
                    // 获取超过限制的数量 进行循环添加
                    while (count > limit)
                    {
                        // 背包满了
                        if (i >= Unlocked)
                        {
                            MessageBox.Show("背包已满,部分道具存储在邮件中,", "提示");
                            return;
                        }

                        // 超过限制了 该格子为限制的数量
                        Items[i].ItemId = (ushort)kv.Key;
                        Items[i].Count = (ushort)limit;
                        i++;
                        count -= limit;
                    }
                    // 剩余未超限制的再开一个格子
                    Items[i].ItemId = (ushort)kv.Key;
                    Items[i].Count = (ushort)count;
                }
                i++;
            }
        }

        // 添加道具
        public void AddItem(int itemId, int count)
        {
            ushort addCount = (ushort)count;
            bool isFull = false;
            bool isFind = false;

            for (int i = 0; i < Items.Length; i++) // 遍历背包中的道具
            {
                if (Items[i].ItemId == itemId) // 该道具已存在背包中
                {
                    ushort canAdd = (ushort)(DataManager.Instance.Items[itemId].StackLimit - Items[i].Count); // 堆叠限制 - 当前存在个数 = 可添加个数

                    // 能直接加
                    if (canAdd >= addCount)
                    {
                        Items[i].Count += addCount;
                        addCount = 0;
                        break;
                    }
                    // 不能直接加 加满到限制的数量
                    else
                    {
                        Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }

            // 还有多的找个新格子
            if (addCount > 0)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    if (Items[i].ItemId == 0)
                    {
                        int limit = DataManager.Instance.Items[itemId].StackLimit;

                        if (addCount <= limit)
                        {
                            Items[i].ItemId = (ushort)itemId;
                            Items[i].Count = addCount;
                        }
                        else
                        {
                            // 循环遍历
                            while (addCount > limit)
                            {
                                Items[i].ItemId = (ushort)itemId;
                                Items[i].Count = (ushort)limit;
                                i++;
                                addCount -= (ushort)limit;
                                if (i >= Unlocked)
                                {
                                    isFull = true;
                                    break;
                                }
                            }
                            if (!isFull)
                            {
                                Items[i].ItemId = (ushort)itemId;
                                Items[i].Count = addCount;
                            }
                        }
                        isFind = true;
                        break;
                    }
                }
                // 没找到 或者 满了
                if (!isFind || isFull) MessageBox.Show("背包已满!", "提示");
            }
        }

        // 移除道具
        public void RemoveItem(int itemId, int count)
        {
        }

        //  将字节数组 解析成 BagItem 结构体
        private unsafe void Analyze(byte[] data)
        {
            // fixed 语句禁止垃圾回收器重定位可移动的变量 执行过程中地址不发生改变
            fixed (byte* pt = data)
            {
                for (int i = 0; i < Unlocked; i++) // 遍历已解锁格子数
                {
                    // 通过内存字节流地址访问数据
                    // 值类型 结构体赋值 地址不会发生改变 值改变
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem)); // 开始指针 + 第几个格子的大小 标识第i个格子对应的值
                    Items[i] = *item; // 将每个背包格子 添加对应的值
                }
            }
        }

        // 将结构体BagItem 解析成 字节数组
        private unsafe NBagInfo GetBagInfo()
        {
            // 字节数组 相当于内存块
            // Analyze 方法 通过 内存 映射到 背包结构体数组
            // GetBagInfo 方法 通过 结构体数组 映射到 内存中
            fixed (byte* pt = info.Items)
            {
                for (int i = 0; i < Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
            }
            return info;
        }
    }
}