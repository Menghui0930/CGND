using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour {
    public SkillData skillData;
    public bool isPurchased = false;

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

    public void Purchase() {
        isPurchased = true;
        UpdateVisual();
    }

    private void UpdateVisual() {
        if (isPurchased && skillData.upgradedIcon != null)
            _buttonImage.sprite = skillData.upgradedIcon;
        else
            _buttonImage.sprite = skillData.icon;
    }
}