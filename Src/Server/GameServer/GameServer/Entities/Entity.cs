using GameServer.Core;
using SkillBridge.Message;

namespace GameServer.Entities
{
    public class Entity
    {
        private NEntity entityData;
        private Vector3Int position;
        private Vector3Int direction;
        private int speed;

        public int EntityId => EntityData.Id; // 实体id

        public NEntity EntityData
        {
            get => entityData;
            set
            {
                entityData = value;
                SetEntityData(value);
            }
        } // 实体传输数据

        public Vector3Int Position
        {
            get => position;
            set
            {
                position = value;
                entityData.Position = position;
            }
        } // 实体坐标

        public Vector3Int Direction
        {
            get => direction;
            set
            {
                direction = value;
                entityData.Direction = direction;
            }
        } // 实体方向

        public int Speed
        {
            get => speed;
            set
            {
                speed = value;
                entityData.Speed = speed;
            }
        } // 实体速度

        public Entity(Vector3Int pos, Vector3Int dir)
        {
            entityData = new NEntity { Position = pos, Direction = dir };
            SetEntityData(entityData);
        }

        public Entity(NEntity entity) => entityData = entity;

        // 设置实体数据
        public void SetEntityData(NEntity entity)
        {
            Position = entity.Position;
            Direction = entity.Direction;
            speed = entity.Speed;
        }
    }
}