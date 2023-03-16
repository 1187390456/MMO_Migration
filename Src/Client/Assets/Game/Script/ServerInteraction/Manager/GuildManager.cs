using Common;
using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class GuildManager : Singleton<GuildManager>
    {
        public NGuildInfo guildInfo; // 工会信息

        public NGuildMemberInfo myMemberInfo; // 自己在工会中的信息

        public bool HasGuild => guildInfo != null;

        public void Init(NGuildInfo guild)
        {
            guildInfo = guild;

            if (guildInfo == null)
            {
                myMemberInfo = null;
                return;
            }
            foreach (var member in guildInfo.Members)
            {
                if (User.Instance.CurrentCharacter.Id == member.characterId)
                {
                    myMemberInfo = member;
                    break;
                }
            }
        }

        // 显示工会
        public void ShowGuild()
        {
            if (HasGuild) UIManager.Instance.Show<UIGuild>(); // 有工会打开工会
            else
            {
                // 没工会 显示没工会弹窗
                var pop = UIManager.Instance.Show<UIGuildPopNoGuild>();
                pop.UIEventHandler = PopNoGuildEventHandler;
            }
        }

        // 无工会弹窗事件监听
        private void PopNoGuildEventHandler(UIBase root, UIBase.UIResult res)
        {
            if (res == UIBase.UIResult.Yes) UIManager.Instance.Show<UIGuildPopCreate>(); // 创建工会
            else if (res == UIBase.UIResult.No) UIManager.Instance.Show<UIGuildList>(); // 显示工会列表
        }
    }
}