using CustomTools;
using Entities;
using SkillBridge.Message;
using System.Collections.Generic;

namespace Manager
{
    /// <summary>
    /// 实体通知接口
    /// </summary>
    public interface IEntityNotify
    {
        void OnEntityRemoved();

        void OnEntityChanged(Entity entity);

        void OnEntityEvent(EntityEvent @event, int param);
    }

    /// <summary>
    /// 实体管理器
    /// </summary>
    public class EntityManager : Singleton<EntityManager>
    {
        private Dictionary<int, Entity> entities = new Dictionary<int, Entity>(); // 所有实体字典

        private Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>(); // 所有通知接口字典

        public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify) => notifiers[entityId] = notify; // 注册 实体通知接口

        public void AddEntity(Entity entity) => entities[entity.entityId] = entity; // 添加实体

        public void RemoveEntity(NEntity entity) // 移除实体
        {
            if (entities.ContainsKey(entity.Id)) entities.Remove(entity.Id);
            if (notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].OnEntityRemoved(); // 执行通知实体离开
                notifiers.Remove(entity.Id);
            }
        }

        public void OnEntitySync(NEntitySync nEntity) //实体同步
        {
            entities.TryGetValue(nEntity.Id, out Entity entity);

            // 本地存在
            if (entity != null)
            {
                if (nEntity.Entity != null) entity.EntityData = nEntity.Entity;
                if (notifiers.ContainsKey(nEntity.Id))
                {
                    notifiers[entity.entityId].OnEntityChanged(entity);
                    notifiers[entity.entityId].OnEntityEvent(nEntity.Event, nEntity.Param);
                }
            }
        }
    }
}