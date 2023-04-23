using Common.Data;
using CustomTools;
using Manager;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTools
{
    [MenuItem("Map Tools/Export Teleporters")]
    public static void ExportTeleporters()
    {
        DataManager.Instance.Load();
        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;

        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }
        List<TeleporterObject> teleporterObjects = new List<TeleporterObject>();
        foreach (var map in DataManager.Instance.Maps.Values)
        {
            string sceneFile = "Assets/Levels/" + map.Resource + ".unity";
            Debug.Log(sceneFile);
            if (!File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene not existed!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);
            TeleporterObject[] teleporters = Object.FindObjectsOfType<TeleporterObject>();
            foreach (var teleporter in teleporters)
            {
                if (!DataManager.Instance.Teleporters.ContainsKey(teleporter.ID))
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图：{0} 中配置的Teleporter:[{1}] 不存在", map.Resource, teleporter.ID), "确定");
                    return;
                }
                TeleporterDefine def = DataManager.Instance.Teleporters[teleporter.ID];
                if (def.MapID != map.ID)
                {
                    EditorUtility.DisplayDialog("错误", string.Format("地图：{0} 中配置的Teleporter:[{1}] MapID:{2} 错误", map.Resource, teleporter.ID, def.MapID), "确定");
                    return;
                }
                def.Position = GameObjectTool.WorldToLogicN(teleporter.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(teleporter.transform.forward);
            }
        }
        DataManager.Instance.SaveTeleporters();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", "传送点导出完成", "确定");
    }

    [MenuItem("Map Tools/Export Spawns")]
    public static void ExportSpawns()
    {
        DataManager.Instance.Load();
        Scene current = EditorSceneManager.GetActiveScene();
        string currentScene = current.name;

        if (current.isDirty)
        {
            EditorUtility.DisplayDialog("提示", "请先保存当前场景", "确定");
            return;
        }
        if (DataManager.Instance.SpawnPoints == null)
        {
            DataManager.Instance.SpawnPoints = new Dictionary<int, Dictionary<int, SpawnPointDefine>>();
        }
        foreach (var map in DataManager.Instance.Maps.Values)
        {
            string sceneFile = "Assets/Levels/" + map.Resource + ".unity";
            Debug.Log(sceneFile);
            if (!File.Exists(sceneFile))
            {
                Debug.LogWarningFormat("Scene not existed!", sceneFile);
                continue;
            }
            EditorSceneManager.OpenScene(sceneFile, OpenSceneMode.Single);
            SpawnObject[] spawns = Object.FindObjectsOfType<SpawnObject>();

            if (!DataManager.Instance.SpawnPoints.ContainsKey(map.ID)) // 刷怪点 未初始化地图
            {
                DataManager.Instance.SpawnPoints[map.ID] = new Dictionary<int, SpawnPointDefine>();
            }
            foreach (var sp in spawns)
            {
                if (!DataManager.Instance.SpawnPoints[map.ID].ContainsKey(sp.ID)) // 地图不包括该刷怪点
                {
                    DataManager.Instance.SpawnPoints[map.ID][sp.ID] = new SpawnPointDefine();
                }
                SpawnPointDefine def = DataManager.Instance.SpawnPoints[map.ID][sp.ID];
                def.ID = sp.ID;
                def.MapID = map.ID;
                def.Position = GameObjectTool.WorldToLogicN(sp.transform.position);
                def.Direction = GameObjectTool.WorldToLogicN(sp.transform.forward);
            }
        }
        DataManager.Instance.SaveSpawnPoints();
        EditorSceneManager.OpenScene("Assets/Levels/" + currentScene + ".unity");
        EditorUtility.DisplayDialog("提示", "刷怪点导出完成", "确定");
    }
}