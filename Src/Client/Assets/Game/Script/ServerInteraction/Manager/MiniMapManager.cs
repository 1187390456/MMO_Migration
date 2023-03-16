using CustomTools;
using Models;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// 小地图管理器
    /// </summary>
    public class MiniMapManager : Singleton<MiniMapManager>
    {
        private MiniMap miniMap;

        private Collider miniMapBoundingBox;

        public Sprite LoadCurrentMiniMap() => Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);   // 加载当前小地图

        public Transform PlayerTransform // 玩家
        {
            get
            {
                if (User.Instance.CurrentCharacterObject == null) return null;
                return User.Instance.CurrentCharacterObject.transform;
            }
        }

        public Collider MiniMapBoundingBox => miniMapBoundingBox;  // 地图边界

        public MiniMap MiniMap // 小地图
        {
            get => miniMap;
            set
            {
                miniMap = value;
                Debug.LogWarningFormat("MinimapManager.Instance.Minimap[{0}] Set", miniMap.GetInstanceID());
            }
        }

        public void UpdataMiniMap(Collider miniMapBoundingBox)
        {
            this.miniMapBoundingBox = miniMapBoundingBox;
            if (miniMap != null) miniMap.UpdateMap();
        }
    }
}