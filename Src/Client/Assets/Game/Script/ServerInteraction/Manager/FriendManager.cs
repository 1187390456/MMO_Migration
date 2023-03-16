using Common;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class FriendManager : Singleton<FriendManager>
    {
        public List<NFriendInfo> allFriends = new List<NFriendInfo>(); // 所有好友

        public void Init(List<NFriendInfo> friends) => allFriends = friends;
    }
}