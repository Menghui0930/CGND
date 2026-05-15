using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPoint : MonoBehaviour {
    public InputAction _OpenSkillTree;
    public InputAction _CloseSkillTree;

    [SerializeField] private GameObject _skillTree;
    [SerializeField] private GameObject _TalkBubble;
    [SerializeField] private bool _LongBubble;

    private bool _playerInRange = false;
    private bool toggle = true;

    public static SpawnPoint ActiveSkillTree = null;

    /// <summary>
    /// 记录 SkillTree 被 ESC 关掉时的帧号。
    /// PauseManager 用 Time.frameCount 对比，同帧就跳过 Pause。
    /// 不依赖 LateUpdate，不受执行顺序影响。
    /// </summary>
    public static int SkillTreeClosedFrame = -1;

    private PlayerMotor _playerMotor;

    private void Awake() {
        _OpenSkillTree = InputSystem.actions.FindAction("Interact");
        _CloseSkillTree = InputSystem.actions.FindAction("ESC");
    }

    private void Start() {
        _skillTree.SetActive(false);
        _TalkBubble.SetActive(false);
    }

    private void Update() {
        if (_playerInRange && _OpenSkillTree.WasPressedThisFrame() && toggle) {
            toggle = false;
            _skillTree.SetActive(true);
            ActiveSkillTree = this;
            _playerMotor?.DisableControl();
        }

        if (_playerInRange && _CloseSkillTree.WasPressedThisFrame() && !toggle) {
            toggle = true;
            _skillTree.SetActive(false);
            ActiveSkillTree = null;
            SkillTreeClosedFrame = Time.frameCount; // ← 记录关闭时的帧号
            _playerMotor?.EnableControl();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            LevelManager.Instance.SetSpawnPoint(transform.position);
            _playerInRange = true;
            _playerMotor = other.GetComponent<PlayerMotor>();

            if (_TalkBubble != null) {
                _TalkBubble.SetActive(true);
                if (!_LongBubble)
                    _TalkBubble.GetComponent<Animator>().Play("Talk_checkpoint");
                else
                    _TalkBubble.GetComponent<Animator>().Play("Long_Speech_checkpoint");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            _playerInRange = false;
            if (_TalkBubble != null)
                _TalkBubble.SetActive(false);
        }
    }
}