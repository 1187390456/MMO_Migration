using Common.Data;
using Manager;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterObject : MonoBehaviour
{
    public int ID; // 传输门id

    private Mesh mesh = null;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }

#if UNITY_EDITOR

    // 编辑模式下绘制渲染线框
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (mesh != null)
        {
            Gizmos.DrawWireMesh(mesh, transform.position + .5f * transform.localScale.y * Vector3.up, transform.rotation, transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1.0f, EventType.Repaint);
    }

#endif

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerInputController player) && player.isActiveAndEnabled)
        {
            TeleporterDefine td = DataManager.Instance.Teleporters[ID];
            if (td == null) throw new Exception(string.Format("TeleporterDefine not existed ! TeleporterObject: Character [{0}] Enter Teleporter[{1}]", player.character.Info.Name, ID));
            Debug.LogFormat("TeleporterObject: Character [{0}] Enter Teleporter[{1}]", player.character.Info.Name, ID);
            if (td.LinkTo > 0)
            {
                if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo)) MapService.Instance.Send_MapTeleport(ID);
                else Debug.LogErrorFormat("LinkToID Error ! Teleporter: ID:[{0}] LinkID:[{1}]", td.ID, td.LinkTo);
            }
        }
    }
}