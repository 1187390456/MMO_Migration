public class MsgCenter : MonoBase
{
    public static MsgCenter Instance = null;

    private void Awake()
    {
        Instance = this;
        gameObject.AddComponent<UIManager>();
    }

    private void Start() => DontDestroyOnLoad(this);

    public void Dispatch(int areaCode, int eventCode, object message)
    {
        switch (areaCode)
        {
            case AreaCode.UI:
                UIManager.Instance.Execute(eventCode, message);
                break;

            default:
                break;
        }
    }
}