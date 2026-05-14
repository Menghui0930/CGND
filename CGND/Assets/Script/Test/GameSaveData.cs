using System;

[Serializable]
public class GameSaveData {
    public int saveSlotIndex;           // 存档槽编号（0, 1, 2 ...）
    public string saveDateTime;         // 存档时间（显示用）

    // ── 游戏进度 ──────────────────────────────────────────────
    public int highestLevelReached;     // 玩家最高到达的关卡（0 = Tutorial）
    public float totalPlayTime;         // 累计游玩时间（秒）

    // ── Skill Tree ────────────────────────────────────────────
    public int skillPoints;             // 剩余 SP
    public string[] unlockedSkills;     // 已解锁技能名称列表
    public string[] activeSkills;       // 已激活技能名称列表（分支技能用）
}
