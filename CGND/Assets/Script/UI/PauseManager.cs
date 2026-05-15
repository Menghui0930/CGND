using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour {
    public static PauseManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject pauseCanvas;

    [Header("Restart SP Per Scene (by Build Index)")]
    [SerializeField] private int[] restartSPByBuildIndex = { 0, 0, 0, 0, 0, 2, 4 };

    private bool _isPaused = false;
    private InputAction _pauseAction;

    private void Awake() {
        Instance = this;
        _pauseAction = InputSystem.actions.FindAction("ESC");
    }

    private void Start() {
        pauseCanvas.SetActive(false);
    }

    private void Update() {
        if (_pauseAction.WasPressedThisFrame()) {
            // SkillTree 正在开着 → 不处理
            if (SpawnPoint.ActiveSkillTree != null) return;

            // 本帧刚关掉 SkillTree（不管谁先跑 Update，帧号相同就跳过）
            if (SpawnPoint.SkillTreeClosedFrame == Time.frameCount) return;

            TogglePause();
        }
    }

    public void TogglePause() {
        _isPaused = !_isPaused;
        SetPause(_isPaused);
    }

    public void SetPause(bool pause) {
        _isPaused = pause;
        Time.timeScale = pause ? 0f : 1f;

        if (pauseCanvas != null)
            pauseCanvas.SetActive(pause);

        var motor = LevelManager.Instance?.CurrentPlayer
                    ?.GetComponentInChildren<PlayerMotor>();
        if (motor != null) {
            if (pause) motor.DisableControl();
            else motor.EnableControl();
        }
    }

    public void ResumeButton() => SetPause(false);

    public void RestartButton() {
        Time.timeScale = 1f;

        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        int sp = (buildIndex < restartSPByBuildIndex.Length)
                 ? restartSPByBuildIndex[buildIndex]
                 : 0;

        LevelEntryContext.SetFromSelect(bonusSP: sp);
        SceneManager.LoadScene(buildIndex);
    }

    public void MainMenuButton() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}