using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {
    public static PauseManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject pauseCanvas;

    private bool _isPaused = false;
    private InputAction _pauseAction;

    // 每个场景对应的初始 SP（索引 = Build Index）
    // 例：Tutorial=0, Level_1=1, Level_2=2, Level_3=3
    // 根据你的 Build Settings 顺序调整这里的数值
    [Header("Restart SP Per Scene (by Build Index)")]
    [SerializeField] private int[] restartSPByBuildIndex = { 0,0,0,0, 0, 2, 4 };

    private void Awake() {
        Instance = this;
        _pauseAction = InputSystem.actions.FindAction("ESC");
    }

    private void Start() {
        pauseCanvas.SetActive(false);
    }

    private void Update() {
        if (_pauseAction.WasPressedThisFrame()) {
            // Skill tree 开着 → 不暂停，让 SpawnPoint 自己处理 Esc
            if (SpawnPoint.ActiveSkillTree != null) return;

            TogglePause();
        }
    }

    public void TogglePause() {
        _isPaused = !_isPaused;
        SetPause(_isPaused);
    }

    public void SetPause(bool pause) {
        _isPaused = pause;

        // 1. 冻结/恢复时间（物理、动画、boss 攻击全停）
        Time.timeScale = pause ? 0f : 1f;

        // 2. 显示/隐藏 Pause Canvas
        if (pauseCanvas != null)
            pauseCanvas.SetActive(pause);

        // 3. 停止/恢复玩家控制
        var motor = LevelManager.Instance?.CurrentPlayer
                    ?.GetComponentInChildren<PlayerMotor>();
        if (motor != null) {
            if (pause) motor.DisableControl();
            else motor.EnableControl();
        }
    }

    // ── 三个按钮 ──────────────────────────

    // 按钮 1：关闭 Pause
    public void ResumeButton() {
        SetPause(false);
    }

    public void RestartButton() {
        Time.timeScale = 1f;

        // 根据当前场景的 Build Index 取对应的初始 SP
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        int sp = (buildIndex < restartSPByBuildIndex.Length)
                 ? restartSPByBuildIndex[buildIndex]
                 : 0;

        // 标记为「选关/重置」进入，SkillTreeManager 会清除技能存档并套用 SP
        LevelEntryContext.SetFromSelect(bonusSP: sp);

        SceneManager.LoadScene(buildIndex);
    }

    // 按钮 3：回到 Main Menu
    public void MainMenuButton() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}