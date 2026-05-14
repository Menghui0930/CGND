/// <summary>
/// 静态上下文，记录玩家是「从选关界面」还是「正常通关」进入关卡。
/// 静态变量在场景切换后仍然保留，不需要 DontDestroyOnLoad。
/// </summary>
public static class LevelEntryContext {
    /// <summary>true = 从选关界面跳入，false = 正常通关流程</summary>
    public static bool IsFromLevelSelect { get; private set; } = false;

    /// <summary>从选关界面进入时要给的初始 SP</summary>
    public static int BonusSP { get; private set; } = 0;

    /// <summary>在 LevelSelectUI 里调用：标记来源并设好 SP</summary>
    public static void SetFromSelect(int bonusSP) {
        IsFromLevelSelect = true;
        BonusSP = bonusSP;
    }

    /// <summary>在 SkillTreeManager.Start() 读取后调用，重置标记，避免影响下一次判断</summary>
    public static void Consume() {
        IsFromLevelSelect = false;
        BonusSP = 0;
    }
}
