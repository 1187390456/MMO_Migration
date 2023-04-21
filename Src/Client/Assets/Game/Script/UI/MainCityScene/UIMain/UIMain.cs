using CustomTools;
using Manager;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{
    private Button bagBtn;
    private Button equipBtn;
    private Button questBtn;
    private Button friendBtn;
    private Button guildBtn;

    public UITeam uiTeam;

    protected override void OnAwake()
    {
        bagBtn = transform.Find("Bag").GetComponent<Button>();
        equipBtn = transform.Find("Equip").GetComponent<Button>();
        questBtn = transform.Find("Quest").GetComponent<Button>();
        friendBtn = transform.Find("Friend").GetComponent<Button>();
        guildBtn = transform.Find("Guild").GetComponent<Button>();
    }

    private void Start()
    {
        bagBtn.onClick.AddListener(() => UIManager.Instance.Show<UIBag>());
        equipBtn.onClick.AddListener(() => UIManager.Instance.Show<UICharEquip>());
        questBtn.onClick.AddListener(() => UIManager.Instance.Show<UIQuest>());
        friendBtn.onClick.AddListener(() => UIManager.Instance.Show<UIFriends>());
        guildBtn.onClick.AddListener(() => GuildManager.Instance.ShowGuild());
    }

    // 控制队伍UI显示
    public void ShowTeamUI(bool isShow) => uiTeam.ShowTeamUI(isShow);
}