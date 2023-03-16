using CustomTools;
using Services;
using SkillBridge.Message;
using UnityEngine.UI;

public class UIRegister : UIBase
{
    private InputField username;
    private InputField password;
    private InputField passwordConfirm;

    private Button reg;
    private Button cancle;

    private void Awake()
    {
        username = transform.Find("Email/UserInput").GetComponent<InputField>();
        password = transform.Find("Password/PassInput").GetComponent<InputField>();
        passwordConfirm = transform.Find("PasswordConfirm/PassConfirmInput").GetComponent<InputField>();

        reg = transform.Find("Btns/Reg").GetComponent<Button>();
        cancle = transform.Find("Btns/Cancle").GetComponent<Button>();

        Bind(UIEvent.Set_RegPanel_Active);
    }

    private void Start() => UserService.Instance.Res_Register = Res_Register;

    private void OnEnable()
    {
        reg.onClick.AddListener(OnClickReg);
        cancle.onClick.AddListener(GoToLogin);
    }

    private void OnDisable()
    {
        reg.onClick.RemoveAllListeners();
        cancle.onClick.RemoveAllListeners();
    }

    public override void Execute(int eventCode, object message)
    {
        switch (eventCode)
        {
            case UIEvent.Set_RegPanel_Active:
                SetActive((bool)message);
                break;

            default:
                break;
        }
    }

    // 注册按钮触发
    private void OnClickReg()
    {
        if (!CommonTools.TestString(username.text, "请输入账号")) return;
        if (!CommonTools.TestString(password.text, "请输入密码")) return;
        if (!CommonTools.TestString(passwordConfirm.text, "请输入确认密码")) return;
        if (!CommonTools.CompareString(password.text, passwordConfirm.text, "两次输入的密码不一致")) return;
        UserService.Instance.Send_Register(username.text, password.text);
    }

    // 注册服务器返回
    private void Res_Register(Result res, string msg)
    {
        if (res == Result.Success) MessageBox.Show("注册成功,请登录", "提示", MessageBoxType.Information).OnYes = GoToLogin;
        else MessageBox.Show(msg, "错误", MessageBoxType.Error);
    }

    // 跳转登录
    private void GoToLogin()
    {
        SetActive(false);
        Dispatch(AreaCode.UI, UIEvent.Set_LoginPanel_Active, true);
    }

    private void SetActive(bool value) => gameObject.SetActive(value);
}