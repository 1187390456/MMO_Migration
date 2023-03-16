using CustomTools;
using Entities;
using Manager;
using SkillBridge.Message;
using UnityEngine;

/// <summary>
/// 实体控制器
/// </summary>
public class EntityController : MonoBehaviour, IEntityNotify
{
    public Entity entity;

    // 动画刚体

    public Animator anim;
    public Rigidbody rb;
    private AnimatorStateInfo currentBaseState;

    // 当前方位

    public Vector3 position;
    public Vector3 direction;
    private Quaternion rotation;

    // 上一次方位

    public Vector3 lastPosition;
    private Quaternion lastRotation;

    // 参数

    public float speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;

    public bool isPlayer = false;

    private void Start()
    {
        // 实体存在订阅实体通知接口 更新初始坐标
        if (entity != null)
        {
            EntityManager.Instance.RegisterEntityChangeNotify(entity.entityId, this);
            UpdateTransform();
        }
        // 不是玩家 关闭重力
        if (!isPlayer) rb.useGravity = false;
    }

    private void OnDestroy()
    {
        if (entity != null) Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4} ", name, entity.entityId, entity.position, entity.direction, entity.speed);

        if (UIWorldElementManager.Instance != null) UIWorldElementManager.Instance.RemoveCharacterNameBar(transform);
    }

    private void FixedUpdate()
    {
        if (entity == null) return;

        entity.OnUpdate(Time.fixedDeltaTime);

        if (!isPlayer) UpdateTransform();
    }

    // 更新坐标
    private void UpdateTransform()
    {
        position = GameObjectTool.LogicToWorld(entity.position);
        direction = GameObjectTool.LogicToWorld(entity.direction);

        rb.MovePosition(position);
        transform.forward = direction;
        lastPosition = position;
        lastRotation = rotation;
    }

    // 监听实体变化
    public void OnEntityChanged(Entity entity) => Debug.LogFormat("OnEntityChanged :ID:{0} POS:{1} DIR:{2} SPD:{3} ", entity.entityId, entity.position, entity.direction, entity.speed);

    // 监听实体移除
    public void OnEntityRemoved()
    {
        if (UIWorldElementManager.Instance != null) UIWorldElementManager.Instance.RemoveCharacterNameBar(transform);
        Destroy(gameObject);
    }

    // 监听实体事件
    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;

            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;

            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;

            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
        }
    }
}