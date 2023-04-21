using CustomTools;
using Entities;
using Manager;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager>
{
    #region 角色头顶名称

    public GameObject namePrefab; // 角色头顶名称
    private Dictionary<Transform, GameObject> elementsName = new Dictionary<Transform, GameObject>(); // 存储角色头顶名称

    // 添加角色跟随
    public void AddCharacterNameBar(Transform owner, Character character)
    {
        GameObject go = Instantiate(namePrefab, transform);
        go.name = "Namebar" + character.entityId;
        go.GetComponent<UIWorldElement>().owner = owner;
        go.GetComponent<UINameBar>().character = character;
        go.SetActive(true);
        elementsName[owner] = go;
    }

    // 移除角色跟随
    public void RemoveCharacterNameBar(Transform owner)
    {
        if (elementsName.ContainsKey(owner))
        {
            Destroy(elementsName[owner]);
            elementsName.Remove(owner);
        }
    }

    #endregion 角色头顶名称

    public GameObject npcStatusPrefab; // npc头顶状态
    private Dictionary<Transform, GameObject> elementsStatus = new Dictionary<Transform, GameObject>(); // 存储npc头顶状态

    // 添加npc任务状态
    public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
    {
        if (elementsStatus.ContainsKey(owner)) elementsStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        else
        {
            GameObject go = Instantiate(npcStatusPrefab, transform);
            go.name = "NpcQuestStatus_" + owner.name;
            go.GetComponent<UIWorldElement>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            go.SetActive(true);
            elementsStatus[owner] = go;
        }
    }

    // 移除npc任务状态
    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (elementsStatus.TryGetValue(owner, out GameObject go))
        {
            Destroy(go);
            elementsStatus.Remove(owner);
        }
    }
}