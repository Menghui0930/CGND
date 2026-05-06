using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour {
    public static SkillTreeManager instance;

    [Header("Skill Points")]
    public int skillPoints = 1;
    public TextMeshProUGUI skillPointsText;

    [Header("Info Panel")]
    public Image skillIcon;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillDescText;
    public Button upgradeButton;
    public TextMeshProUGUI upgradeButtonText;

    private SkillButton selectedSkill;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        UpdateSkillPointsUI();
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        // 默认隐藏 info panel
        skillIcon.gameObject.SetActive(false);
    }

    // 当玩家点击某个 Skill Button 时调用
    public void SelectSkill(SkillButton skillButton) {
        selectedSkill = skillButton;
        SkillData data = skillButton.skillData;

        skillIcon.gameObject.SetActive(true);
        skillIcon.sprite = skillButton.isPurchased ? data.upgradedIcon : data.icon;
        skillNameText.text = data.skillName;
        skillDescText.text = data.description;

        // 判断按钮状态
        if (skillButton.isPurchased) {
            upgradeButtonText.text = "Purchased";
            upgradeButton.interactable = false;
        } else if (skillPoints >= data.cost) {
            upgradeButtonText.text = $"Upgrade ({data.cost} SP)";
            upgradeButton.interactable = true;
        } else {
            upgradeButtonText.text = $"Not Enough SP ({data.cost} SP)";
            upgradeButton.interactable = false;
        }
    }

    private void OnUpgradeClicked() {
        if (selectedSkill == null || selectedSkill.isPurchased) return;

        int cost = selectedSkill.skillData.cost;
        if (skillPoints >= cost) {
            skillPoints -= cost;
            selectedSkill.Purchase();
            UpdateSkillPointsUI();
            SelectSkill(selectedSkill);   // 刷新 info panel
        }
    }

    private void UpdateSkillPointsUI() {
        skillPointsText.text = $"Skill Points: {skillPoints}";
    }
}