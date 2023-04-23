using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public class Entity
    {
        public int entityId => EntityData.Id;    // 实体 id

        public Vector3Int position; // 坐标
        public Vector3Int direction; // 方位
        public int speed; // 速度

        private NEntity entityData;

        public NEntity EntityData
        {
            get
            {
                UpdateEntityData();
                return entityData;
            }
            set
            {
                entityData = value;
                SetEntityData(value);
            }
        } // 传输实体数据

        // 将传输数据转成本地数据
        public void SetEntityData(NEntity entity)
        {
            position = position.FromNVector3(entity.Position);
            direction = direction.FromNVector3(entity.Direction);
            speed = entity.Speed;
        }

        // 将本地数据转成传输数据
        public void UpdateEntityData()
        {
            entityData.Position.FromVector3Int(position);
            entityData.Direction.FromVector3Int(direction);
            entityData.Speed = speed;
        }

        // 更新位置
        public virtual void OnUpdate(float delta)
        {
            if (speed != 0)
            {
                Vector3 dir = direction;
                position += Vector3Int.RoundToInt(delta * speed * dir / 100f);
            }
        }

        public Entity(NEntity entity)
        {
            entityData = entity;
            SetEntityData(entity);
        }
    }
}