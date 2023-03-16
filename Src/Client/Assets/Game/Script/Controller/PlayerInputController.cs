using CustomTools;
using Entities;
using Manager;
using Models;
using Services;
using SkillBridge.Message;
using UnityEngine;

/// <summary>
/// 角色控制器
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    public Rigidbody rb;

    private CharacterState state;

    public Character character;

    public float rotateSpeed = 2.0f;

    public float turnAngle = 10;

    public int speed;

    public EntityController entityController;

    public bool onAir = false;

    private void Start()
    {
        state = CharacterState.Idle;
        if (character == null)
        {
            DataManager.Instance.Load();
            NCharacterInfo cinfo = new NCharacterInfo();
            cinfo.Id = 1;
            cinfo.Name = "Test";
            cinfo.ConfigId = 1;
            cinfo.Entity = new NEntity();
            cinfo.Entity.Position = new NVector3();
            cinfo.Entity.Direction = new NVector3();
            cinfo.Entity.Direction.X = 0;
            cinfo.Entity.Direction.Y = 100;
            cinfo.Entity.Direction.Z = 0;
            character = new Character(cinfo);

            if (entityController != null) entityController.entity = this.character;
        }
    }

    private void FixedUpdate()
    {
        if (character == null) return;
        //  if (InputManager.Instance != null && InputManager.Instance.IsInputMode) return;

        // 上下移动
        float v = Input.GetAxis("Vertical");
        if (v > 0.01)
        {
            if (state != CharacterState.Move)
            {
                state = CharacterState.Move;
                character.MoveForward();
                SendEntityEvent(EntityEvent.MoveFwd);
            }
            rb.velocity = rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (character.speed + 9.81f) / 100f;
        }
        else if (v < -0.01)
        {
            if (state != CharacterState.Move)
            {
                state = CharacterState.Move;
                character.MoveBack();
                SendEntityEvent(EntityEvent.MoveBack);
            }
            rb.velocity = rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (character.speed + 9.81f) / 100f;
        }
        else
        {
            if (state != CharacterState.Idle)
            {
                state = CharacterState.Idle;
                rb.velocity = Vector3.zero;
                character.Stop();
                SendEntityEvent(EntityEvent.Idle);
            }
        }

        // 跳跃
        if (Input.GetButtonDown("Jump")) SendEntityEvent(EntityEvent.Jump);

        // 左右旋转
        float h = Input.GetAxis("Horizontal");
        if (h < -0.1 || h > 0.1)
        {
            transform.Rotate(0, h * rotateSpeed, 0);
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, transform.forward);

            if (rot.eulerAngles.y > turnAngle && rot.eulerAngles.y < (360 - turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(transform.forward));
                rb.transform.forward = transform.forward;
                SendEntityEvent(EntityEvent.None);
            }
        }
        //Debug.LogFormat("velocity {0}", this.rb.velocity.magnitude);
    }

    private Vector3 lastPos;
    //  private float lastSync = 0;

    // 位置同步
    private void LateUpdate()
    {
        if (character == null) return;

        Vector3 offset = rb.transform.position - lastPos;
        speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        lastPos = rb.transform.position;

        //Debug.LogFormat("LateUpdate velocity {0} : {1}", this.rb.velocity.magnitude, this.speed);

        Vector3Int goLogicPos = GameObjectTool.WorldToLogic(rb.transform.position);
        float logicOffset = (goLogicPos - character.position).magnitude; // 实际位置与网络传输位置

        if (logicOffset > 100)
        {
            character.SetPosition(GameObjectTool.WorldToLogic(rb.transform.position)); // 强制设置位置
            SendEntityEvent(EntityEvent.None);
        }
        transform.position = rb.transform.position;
    }

    // 发送实体同步事件
    public void SendEntityEvent(EntityEvent entityEvent, int param = 0)
    {
        if (User.Instance.CurrentCharacter == null) return;
        if (entityController != null) entityController.OnEntityEvent(entityEvent, param);
        MapService.Instance.Send_MapEntitySync(entityEvent, character.EntityData, param);
    }
}