using Models;

public class CharSelectScene : UIBase
{
    private void Start() => CheckUserCharacter();

    // 检测角色数量
    private void CheckUserCharacter() => SetCharacterPanel(User.Instance.CurrentUserInfo.Player.Characters.Count != 0);

    // 设置面板
    private void SetCharacterPanel(bool hasCharacter)
    {
        Dispatch(AreaCode.UI, UIEvent.SetCreatePanel_Active, !hasCharacter);
        Dispatch(AreaCode.UI, UIEvent.SetSelectPanel_Active, hasCharacter);
    }
}