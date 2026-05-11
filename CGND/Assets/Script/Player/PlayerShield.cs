using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShield : PlayerState {
    [Header("Settings")]
    [SerializeField] private float duration = 3f;
    [SerializeField] private float cooldown = 15f;

    [Header("References")]
    [SerializeField] private GameObject shieldObject;   // 拖入盾的 GameObject

    private InputAction _barrierAction;
    private bool _isUnlocked = false;
    private bool _isActive = false;
    private bool _isOnCooldown = false;
    private Health _health;

    // Skill Tree
    public void UnlockBarrier() { _isUnlocked = true; }
    public void LockBarrier() { _isUnlocked = false; }

    protected override void Awake() {
        base.Awake();
        _barrierAction = InputSystem.actions.FindAction("Shield");  // 你在 InputAction 里加一个 Barrier 键，建议绑 Q
    }

    protected override void InitState() {
        base.InitState();
        _health = GetComponentInParent<Health>();
        shieldObject.SetActive(false);
    }

    protected override void GetInput() {
        
        if (_barrierAction.WasPressedThisFrame() && _isUnlocked && !_isActive && !_isOnCooldown)
            StartCoroutine(ActivateBarrier());
       
    }

    public override void ExecuteState() { }

    private IEnumerator ActivateBarrier() {
        // 开启
        _isActive = true;
        _isOnCooldown = true;
        shieldObject.SetActive(true);
        _health.SetImmune(true);

        yield return new WaitForSeconds(duration);

        // 关闭
        _isActive = false;
        shieldObject.SetActive(false);
        _health.SetImmune(false);

        // 冷却
        yield return new WaitForSeconds(cooldown - duration);
        _isOnCooldown = false;
    }
}