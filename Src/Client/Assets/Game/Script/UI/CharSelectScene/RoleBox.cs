using CustomTools;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class RoleBox : UIBase
{
    public NCharacterInfo nCharacterInfo = null;
    public Image activeImage;

    private Text level;
    private Text characerName;
    private Image avatar;
    private GameObject addBtnGobj;

    private Sprite defaultImage;

    private void Awake()
    {
        level = transform.Find("TopInfo/LeftInfo/Leave/Text").GetComponent<Text>();
        characerName = transform.Find("TopInfo/LeftInfo/Name").GetComponent<Text>();
        activeImage = transform.Find("TopInfo/AvatarOrBtn/Image").GetComponent<Image>();
        avatar = transform.Find("TopInfo/AvatarOrBtn/Avatar").GetComponent<Image>();
        addBtnGobj = transform.Find("TopInfo/AvatarOrBtn/Btn").gameObject;

        defaultImage = Resloader.Load<Sprite>("Image/DefaultAvatar");
    }

    // 设置角色
    public void SetRolo(NCharacterInfo nCharacterInfo)
    {
        this.nCharacterInfo = nCharacterInfo;

        level.text = "1级";
        characerName.text = nCharacterInfo.Name.ToString();
        avatar.overrideSprite = defaultImage;

        SetAvatartActive(true);
    }

    // 设置头像激活状态
    private void SetAvatartActive(bool value)
    {
        avatar.gameObject.SetActive(value);
        addBtnGobj.SetActive(!value);
    }
}