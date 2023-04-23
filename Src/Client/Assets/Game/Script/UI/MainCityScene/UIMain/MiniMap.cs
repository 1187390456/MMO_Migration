using Manager;
using Models;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : UIBase
{
    private Collider miniMapBoundingBox;
    private Transform playerTransform;

    private Text mapName;
    private Image miniMap;
    private Image arrow;

    private void Awake()
    {
        mapName = transform.Find("Name").GetComponent<Text>();
        miniMap = transform.Find("Mask/Map").GetComponent<Image>();
        arrow = transform.Find("Mask/Arrow").GetComponent<Image>();
    }

    private void Start()
    {
        MiniMapManager.Instance.MiniMap = this;
        UpdateMap();
    }

    private void Update()
    {
        if (!CheckEnvironment()) return;

        // 真实地图宽高
        float realWidth = miniMapBoundingBox.bounds.size.x;
        float realHeight = miniMapBoundingBox.bounds.size.z;

        // 玩家相对真实地图的位置  x 玩家世界坐标x-地图左下角世界坐标x     y 玩家世界坐标z - 地图世界坐标z
        float relaX = playerTransform.position.x - miniMapBoundingBox.bounds.min.x;
        float relaY = playerTransform.position.z - miniMapBoundingBox.bounds.min.z;

        // 计算玩家中心点 将玩家中心点 赋值地图中心点
        float privotX = relaX / realWidth;
        float privotY = relaY / realHeight;
        miniMap.rectTransform.pivot = new Vector2(privotX, privotY);
        miniMap.rectTransform.localPosition = Vector2.zero;
        arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
    }

    // 检测玩家和地图边界盒环境
    private bool CheckEnvironment()
    {
        if (playerTransform == null) playerTransform = MiniMapManager.Instance.PlayerTransform;
        if (miniMapBoundingBox == null) miniMapBoundingBox = MiniMapManager.Instance.MiniMapBoundingBox;
        if (playerTransform == null)
        {
            Debug.LogWarning("NO playerTransform !");
            return false;
        }
        if (miniMapBoundingBox == null)
        {
            Debug.LogError("NO miniMapBoundingBox !");
            return false;
        }
        return true;
    }

    // 更新地图信息
    public void UpdateMap()
    {
        var info = User.Instance.CurrentMapData;
        mapName.text = info.Name;
        miniMap.overrideSprite = MiniMapManager.Instance.LoadCurrentMiniMap();
        miniMap.SetNativeSize();
        miniMap.transform.localPosition = Vector3.zero;
        playerTransform = null;
        miniMapBoundingBox = null;
    }
}