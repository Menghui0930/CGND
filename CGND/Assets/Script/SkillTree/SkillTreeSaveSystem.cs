using UnityEngine;

/// <summary>
/// 负责把 Skill Tree 的解锁状态存进 PlayerPrefs，跨场景/关卡持久化。
/// </summary>
public static class SkillTreeSaveSystem {
    private const string PREFIX_UNLOCKED = "ST_U_"; // 已花 SP 解锁
    private const string PREFIX_ACTIVE = "ST_A_"; // 当前激活（分支技能用）
    private const string KEY_SP = "ST_SP"; // 剩余 SP

    // ── 保存 ────────────────────────────────────────────────────────────────
    public static void Save(SkillButton[] buttons, int skillPoints) {
        foreach (var btn in buttons) {
            string id = btn.skillData.skillName; // 用技能名当唯一 ID（名字不能重复）
            PlayerPrefs.SetInt(PREFIX_UNLOCKED + id, btn.isUnlocked ? 1 : 0);
            PlayerPrefs.SetInt(PREFIX_ACTIVE + id, btn.isActive ? 1 : 0);
        }
        PlayerPrefs.SetInt(KEY_SP, skillPoints);
        PlayerPrefs.Save();
    }

    // ── 读取 ────────────────────────────────────────────────────────────────
    /// <returns>读取到的 SP；如果从未保存过则返回 defaultSP</returns>
    public static int Load(SkillButton[] buttons, int defaultSP = 0) {
        if (!PlayerPrefs.HasKey(KEY_SP))
            return defaultSP;

        foreach (var btn in buttons) {
            string id = btn.skillData.skillName;
            btn.isUnlocked = PlayerPrefs.GetInt(PREFIX_UNLOCKED + id, 0) == 1;
            btn.isActive = PlayerPrefs.GetInt(PREFIX_ACTIVE + id, 0) == 1;
        }
        return PlayerPrefs.GetInt(KEY_SP, defaultSP);
    }

    // ── 清除存档（新游戏用）─────────────────────────────────────────────────
    public static void DeleteSave(SkillButton[] buttons) {
        foreach (var btn in buttons) {
            PlayerPrefs.DeleteKey(PREFIX_UNLOCKED + btn.skillData.skillName);
            PlayerPrefs.DeleteKey(PREFIX_ACTIVE + btn.skillData.skillName);
        }
        PlayerPrefs.DeleteKey(KEY_SP);
        PlayerPrefs.Save();
    }
}