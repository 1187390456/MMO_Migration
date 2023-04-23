using Common.Data;
using Manager;
using Models;
using System.Collections;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public int NpcID;
    private Animator anim;
    private NpcDefine npc;

    private bool inInteractive; // 是否正在交互

    private SkinnedMeshRenderer render; // 渲染器
    private Color orignColor; // 初始材质颜色

    private NpcQuestStatus questStatus; // 当前npc存在的任务状态

    private void Awake()
    {
        anim = GetComponent<Animator>();
        npc = NpcManager.Instance.GetNpcDefine(NpcID);
        render = GetComponentInChildren<SkinnedMeshRenderer>();
        orignColor = render.sharedMaterial.color;
        StartCoroutine(Actions());
        RefreshQuestStatus();
        QuestManager.Instance.OnQuestStatusChanged += OnQuestStatusChanged;
    }

    private void OnMouseDown() => Interactive();

    private void OnMouseEnter() => HighLight(true);

    private void OnMouseOver() => HighLight(true);

    private void OnMouseExit() => HighLight(false);

    private void OnDestroy()
    {
        QuestManager.Instance.OnQuestStatusChanged -= OnQuestStatusChanged;
        if (UIWorldElementManager.Instance != null) UIWorldElementManager.Instance.RemoveNpcQuestStatus(transform);
    }

    // 任务状态改变
    private void OnQuestStatusChanged(Quest quest) => RefreshQuestStatus();

    // 刷新任务
    private void RefreshQuestStatus()
    {
        questStatus = QuestManager.Instance.GetQuestStatusByNpc(NpcID);
        UIWorldElementManager.Instance.AddNpcQuestStatus(transform, questStatus);
    }

    // Npc随机动作
    private IEnumerator Actions()
    {
        while (true)
        {
            if (inInteractive) yield return new WaitForSeconds(2f);
            else yield return new WaitForSeconds(Random.Range(5.0f, 10.0f));
            Relax();
        }
    }

    // 放松动作
    private void Relax() => anim.SetTrigger("Relax");

    // 高亮人物
    private void HighLight(bool isHighLight)
    {
        if (isHighLight)
        {
            if (render.sharedMaterial.color != Color.white) render.sharedMaterial.color = Color.white;
        }
        else
        {
            if (render.sharedMaterial.color != orignColor) render.sharedMaterial.color = orignColor;
        }
    }

    // 交互一层 判断是否正在交互
    private void Interactive()
    {
        if (!inInteractive)
        {
            inInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }

    // 开始交互
    private IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();
        if (NpcManager.Instance.Interactive(NpcID)) anim.SetTrigger("Talk");
        yield return new WaitForSeconds(3f);
        inInteractive = false;
    }

    // 转向玩家
    private IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - transform.position).normalized; // 获取与目标的方向矢量
        while (Mathf.Abs(Vector3.Angle(transform.forward, faceTo)) > 5)
        {
            transform.forward = Vector3.Lerp(gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }
}