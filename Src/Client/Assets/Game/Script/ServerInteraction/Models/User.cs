using Common.Data;
using CustomTools;
using SkillBridge.Message;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模型层 请求的数据存在本地访问 实体
/// </summary>
namespace Models
{
    // 本地存储的角色信息
    internal class User : Singleton<User>
    {
        private NUserInfo currentUserInfo;

        public NUserInfo CurrentUserInfo => currentUserInfo; // 当前用户信息

        public NCharacterInfo CurrentCharacter { get; set; }//  当前角色信息

        public NTeamInfo CurrentTeam { get; set; } // 当前队伍信息

        public MapDefine CurrentMapData { get; set; }    // 当前地图信息

        public GameObject CurrentCharacterObject { get; set; }  // 当前角色游戏对象

        public void SetupUserInfo(NUserInfo userInfo) => currentUserInfo = userInfo;   // 存储用户信息

        public void SetupUserCharacter(List<NCharacterInfo> nCharacterInfos)   // 存储角色信息
        {
            currentUserInfo.Player.Characters.Clear();
            currentUserInfo.Player.Characters.AddRange(nCharacterInfos);
        }

        //  添加金币
        public void AddGold(int count) => CurrentCharacter.Gold += count;
    }
}