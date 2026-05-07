using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour {
    public SkillData skillData;
    public bool isUnlocked = false;         // 已花 SP 解锁
    public bool isActive = false;           // 当前激活（分支技能用）

    private Image _buttonImage;
    private Button _button;


    private void Awake() {
        _buttonImage = GetComponent<Image>();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    private void Start() {
        UpdateVisual();
    }

    private void OnClick() {
        SkillTreeManager.instance.SelectSkill(this);
    }

    // 解锁并激活
    public void Unlock() {
        isUnlocked = true;
        isActive = true;
        UpdateVisual();
    }

    // 切换激活状态（已解锁的分支技能用）
    public void SetActive(bool active) {
        isActive = active;
        UpdateVisual();
    }

    public void UpdateVisual() {
        // 改成 isActive 决定亮暗，而不是 isUnlocked
        if (isActive && skillData.upgradedIcon != null)
            _buttonImage.sprite = skillData.upgradedIcon;
        else
            _buttonImage.sprite = skillData.icon;

        _button.interactable = SkillTreeManager.instance.ArePrerequisitesMet(skillData);
    }
}