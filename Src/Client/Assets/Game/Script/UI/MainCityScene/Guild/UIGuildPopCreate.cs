using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

public class UIGuildPopCreate : UIBase
{
    public InputField guildNameInput;
    public InputField guildNoticeInput;

    private void Start() => GuildService.Instance.OnGuildCreateResult = OnCreateHandler;

    public override void OnDestroy()
    {
        base.OnDestroy();
        GuildService.Instance.OnGuildCreateResult = null;
    }

    // 返回结果
    private void OnCreateHandler(bool result)
    {
        if (result) Close(UIResult.Yes);
    }

    // 创建工会
    public override void OnYesClick()
    {
        if (string.IsNullOrEmpty(guildNameInput.text))
        {
            MessageBox.Show("公会名称不能为空!", "提示");
            return;
        }
        if (guildNameInput.text.Length < 4 || guildNameInput.text.Length > 10)
        {
            MessageBox.Show("公会名称为4-10个字符", "提示");
            return;
        }
        if (string.IsNullOrEmpty(guildNoticeInput.text))
        {
            MessageBox.Show("公会宗旨不能为空!", "提示");
            return;
        }
        if (guildNoticeInput.text.Length < 3 || guildNoticeInput.text.Length > 50)
        {
            MessageBox.Show("公会宗旨为3-50个字符", "提示");
            return;
        }
        GuildService.Instance.SendGuildCreateRequest(guildNameInput.text, guildNoticeInput.text);
    }
}