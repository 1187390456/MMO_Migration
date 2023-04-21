using CustomTools;
using Nirvana;
using Services;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : UIBase
{
    private UINameTable table;

    private InputField username;
    private InputField password;

    private GameObject loginView;
    private GameObject loginRoot;
    private GameObject default_login;
    private GameObject selectServer;
    private GameObject createRole;

    private Text txtServer;

    private void Awake()
    {
        table = GetComponent<UINameTable>();

        loginRoot = table.Find("LoginRoot");

        #region LoginView

        loginView = table.Find("LoginView");
        username = table.Find("UserName").GetComponent<InputField>();
        password = table.Find("Passwrod").GetComponent<InputField>();
        table.Find("BtnLogin").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!CommonTools.TestString(username.text, "请输入账号")) return;
            if (!CommonTools.TestString(password.text, "请输入密码")) return;
            UserService.Instance.Send_Login(username.text, password.text);
        });

        #endregion LoginView

        #region Default_login

        default_login = table.Find("Default_login");
        txtServer = table.Find("TxtServer").GetComponent<Text>();

        table.Find("BtnReturn2").GetComponent<Button>().onClick.AddListener(() =>
        {
            //TODO 服务器退出登录  返回步骤0
        });
        table.Find("BtnStart").GetComponent<Button>().onClick.AddListener(() =>
        {
            SwitchStep(2);
        });
        table.Find("BtnSelect").GetComponent<Button>().onClick.AddListener(() =>
         {
             // 打开选择服务器面板
             CommonTools.SetGameObjeActive(transform, false);
             selectServer.SetActive(true);
         });
        table.Find("BtnDefaultService").GetComponent<Button>().onClick.AddListener(() =>
        {
            // 打开选择服务器面板
            CommonTools.SetGameObjeActive(transform, false);
            selectServer.SetActive(true);
        });

        #endregion Default_login

        #region SelectServer

        selectServer = table.Find("SelectServer");

        table.Find("SelectBackBtn").GetComponent<Button>().onClick.AddListener(() => SwitchStep(1));
        table.Find("SelectConfirmBtn").GetComponent<Button>().onClick.AddListener(() => SwitchStep(2));

        #endregion SelectServer

        #region createRole

        createRole = table.Find("CreateRole");

        #endregion createRole
    }

    private void Start()
    {
        UserService.Instance.Res_Login += Res_Login;

        SwitchStep(0);
    }

    // 登录服务器返回
    private void Res_Login(Result res, string msg)
    {
        if (res == Result.Success) SwitchStep(1);
        else TipsConfig.Instance.ShowSystemTips(msg, 2);
    }

    // 步骤控制
    private void SwitchStep(int step)
    {
        switch (step)
        {
            case 0:   // 初始登录场景
                CommonTools.SetGameObjeActive(transform, false);
                loginRoot.SetActive(true);
                loginView.SetActive(true);
                break;

            case 1:   // 登录成功跳转
                CommonTools.SetGameObjeActive(transform, false);
                loginRoot.SetActive(true);
                txtServer.text = "临时服务器";
                default_login.SetActive(true);
                break;

            case 2:   // 创建角色
                CommonTools.SetGameObjeActive(transform, false);
                createRole.SetActive(true);
                break;

            default:
                break;
        }
    }
}