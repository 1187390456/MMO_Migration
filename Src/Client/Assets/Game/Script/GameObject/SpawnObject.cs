using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public int ID; // 刷怪点id
    private Mesh mesh = null;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
    }

#if UNITY_EDITOR

    // 编辑模式下绘制渲染线框
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position + .5f * transform.localScale.y * Vector3.up;
        Gizmos.color = Color.red;
        if (mesh != null) Gizmos.DrawWireMesh(mesh, pos, transform.rotation, transform.localScale);
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1.0f, EventType.Repaint);
        UnityEditor.Handles.Label(pos, "SpawnPoint:" + ID);
    }

#endif
}