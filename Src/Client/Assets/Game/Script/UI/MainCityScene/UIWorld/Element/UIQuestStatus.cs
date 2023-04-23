using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using CustomTools;

public class UIQuestStatus : UIBase
{
    public void SetQuestStatus(NpcQuestStatus npcStatus) => CommonTools.SetSoleActive(transform, (int)npcStatus);
}