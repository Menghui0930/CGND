using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : MonoBehaviour {
    public static SkillTreeManager instance;


    [Header("Skill Points")]
    public int skillPoints = 0;
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
        if (LevelEntryContext.IsFromLevelSelect) {
            // ── 从选关界面进来：清掉技能存档，给固定 SP ──────────────────
            SkillTreeSaveSystem.DeleteSave(allSkillButtons);
            skillPoints = LevelEntryContext.BonusSP;
            LevelEntryContext.Consume(); // 用完即清，避免影响下一次
        } else {
            // ── 正常通关进来：读存档继承技能，不额外给 SP ─────────────────
            // defaultSP = 0，如果没有存档（第一次进 Level_1）就给 0
            skillPoints = SkillTreeSaveSystem.Load(allSkillButtons, defaultSP: 0);
        }


        // 2. 刷新所有按钮视觉（让已解锁的按钮显示高亮状态）
        RefreshAllButtons();


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

        // ← 解锁后立刻存档，下一关进来就能读到
        SkillTreeSaveSystem.Save(allSkillButtons, skillPoints);
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

        // ← 切换分支后也存档
        SkillTreeSaveSystem.Save(allSkillButtons, skillPoints);
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

    public void GetCrystal(int num) {
        skillPoints += num;
        UpdateSkillPointsUI();
        // 切换分支后也保存
        SkillTreeSaveSystem.Save(allSkillButtons, skillPoints);
    }

    public void UpdateSkillPointsUI() {
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

        // 把所有 isActive == true 的技能效果重新应用给新生成的玩家
        foreach (SkillButton btn in allSkillButtons)
            if (btn.isActive)
                ApplySkillEffect(btn.skillData);
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