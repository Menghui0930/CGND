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

    [Header("All Skill Buttons")]
    public SkillButton[] allSkillButtons;   

    [Header("All Connections")]
    public SkillConnection[] allConnections;

    private SkillEffect[] skillEffects;

    private SkillButton selectedSkill;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        UpdateSkillPointsUI();
        upgradeButton.onClick.AddListener(OnUpgradeClicked);
        // 默认隐藏 info panel
        skillIcon.gameObject.SetActive(false);
        RefreshAllButtons();
    }

    // 当玩家点击某个 Skill Button 时调用
    public void SelectSkill(SkillButton skillButton) {
        selectedSkill = skillButton;
        SkillData data = skillButton.skillData;

        skillIcon.gameObject.SetActive(true);
        skillIcon.sprite = skillButton.isActive ? data.upgradedIcon : data.icon;
        skillNameText.text = data.skillName;
        skillDescText.text = data.description;

        bool prereqMet = ArePrerequisitesMet(data);
        bool groupHasAnyUnlocked = data.isBranchSkill && AnyInGroupUnlocked(data.branchGroupID);

        if (!prereqMet && !groupHasAnyUnlocked) {
            // 前置未满足，且组内也没解锁过
            upgradeButtonText.text = "Locked";
            upgradeButton.interactable = false;
        } else if (skillButton.isActive) {
            // 已激活
            upgradeButtonText.text = "Active";
            upgradeButton.interactable = false;
        } else if (groupHasAnyUnlocked) {
            // 组内已有人解锁过 → 免费切换，不管这颗有没有解锁
            upgradeButtonText.text = "Switch (Free)";
            upgradeButton.interactable = true;
        } else if (skillPoints >= data.cost) {
            upgradeButtonText.text = $"Unlock ({data.cost} SP)";
            upgradeButton.interactable = true;
        } else {
            upgradeButtonText.text = "Not Enough SP";
            upgradeButton.interactable = false;
        }
    }

    private void OnUpgradeClicked() {
        if (selectedSkill == null) return;

        SkillData data = selectedSkill.skillData;
        bool groupHasAnyUnlocked = data.isBranchSkill && AnyInGroupUnlocked(data.branchGroupID);

        if (groupHasAnyUnlocked && !selectedSkill.isActive) {
            SwitchBranch(selectedSkill);
        } else if (!selectedSkill.isUnlocked && skillPoints >= data.cost) {
            skillPoints -= data.cost;
            UpdateSkillPointsUI();

            if (data.isBranchSkill)
                DeactivateBranchGroup(data.branchGroupID);

            selectedSkill.Unlock();

            // ← 新增：触发对应的 SkillEffect
            ApplySkillEffect(data);

            RefreshAllButtons();
            SelectSkill(selectedSkill);
        }
    }

    private void SwitchBranch(SkillButton target) {
        if (!target.skillData.isBranchSkill) return;

        // 找到目前激活的那粒，移除它的效果
        foreach (SkillButton btn in allSkillButtons) {
            if (btn.skillData.isBranchSkill &&
                btn.skillData.branchGroupID == target.skillData.branchGroupID &&
                btn.isActive) {
                RemoveSkillEffect(btn.skillData);   // ← 移除旧效果
                break;
            }
        }

        DeactivateBranchGroup(target.skillData.branchGroupID);
        target.SetActive(true);
        ApplySkillEffect(target.skillData);         // ← 应用新效果

        RefreshAllButtons();
        SelectSkill(target);
    }

    // 把同组所有分支设为 inactive
    private void DeactivateBranchGroup(string groupID) {
        foreach (SkillButton btn in allSkillButtons) {
            if (btn.skillData.isBranchSkill && btn.skillData.branchGroupID == groupID)
                btn.SetActive(false);
        }
    }
    // ── 前置条件检查 ─────────────────────────────────
    public bool ArePrerequisitesMet(SkillData data) {
        foreach (SkillData prereq in data.prerequisites) {
            SkillButton btn = GetButtonByData(prereq);
            // 分支技能：只需要同组有一个已解锁即可
            if (prereq.isBranchSkill) {
                if (!AnyInGroupUnlocked(prereq.branchGroupID)) return false;
            } else {
                if (btn == null || !btn.isUnlocked) return false;
            }
        }
        return true;
    }

    // 检查某分支组是否有任何一个已解锁
    private bool AnyInGroupUnlocked(string groupID) {
        foreach (SkillButton btn in allSkillButtons) {
            if (btn.skillData.isBranchSkill &&
                btn.skillData.branchGroupID == groupID &&
                btn.isUnlocked) return true;
        }
        return false;
    }

    private SkillButton GetButtonByData(SkillData data) {
        foreach (SkillButton btn in allSkillButtons)
            if (btn.skillData == data) return btn;
        return null;
    }

    // ── 刷新所有按钮视觉 ─────────────────────────────
    private void RefreshAllButtons() {
        foreach (SkillButton btn in allSkillButtons)
            btn.UpdateVisual();

        foreach (SkillConnection conn in allConnections)
            conn.UpdateVisual();
    }

    private void UpdateSkillPointsUI() {
        skillPointsText.text = $"{skillPoints}";
    }

    private void OnEnable() {
        LevelManager.OnPlayerSpawn += OnPlayerSpawn;
    }

    private void OnDisable() {
        LevelManager.OnPlayerSpawn -= OnPlayerSpawn;
    }

    private void OnPlayerSpawn(PlayerMotor playerMotor) {
        skillEffects = playerMotor.GetComponentsInChildren<SkillEffect>();
        Debug.Log($"[SkillTree] 玩家出生，找到 {skillEffects.Length} 个 SkillEffect");
    }

    // Skill Tree
    private void ApplySkillEffect(SkillData data) {
        Debug.Log($"[SkillTree] 尝试触发 Effect，技能名：{data.skillName}，共有 {skillEffects.Length} 个 Effect");
        foreach (SkillEffect effect in skillEffects) {
            Debug.Log($"[SkillTree] 检查 Effect：{effect.GetType().Name}，对应 SkillData：{effect.skillData?.skillName}");
            if (effect.skillData == data) {
                Debug.Log($"[SkillTree] 匹配成功，调用 ApplyEffect");
                effect.ApplyEffect();
                return;
            }
        }
        Debug.LogWarning("[SkillTree] 没有找到匹配的 SkillEffect！");
    }

    private void RemoveSkillEffect(SkillData data) {
        foreach (SkillEffect effect in skillEffects) {
            if (effect.skillData == data) {
                effect.RemoveEffect();
                return;
            }
        }
    }


}