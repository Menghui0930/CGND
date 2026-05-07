using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerBasicAttack : PlayerState {
    [Header("Settings")]
    [SerializeField] private float shootingSpeed = 15f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private Transform magicPosition;
    [SerializeField] private GameObject grassBallPrefab;
    [SerializeField] private GameObject waterBallPrefab;
    [SerializeField] private GameObject windBallPrefab;
    private Transform currentMagicPosition1;

    [Header("Skill Double Shot")]
    [SerializeField] private Transform magicPosition2;   
    [SerializeField] private Transform newmagicPosition1;
    [SerializeField] private bool _isDoubleShot = false;

    private bool isHolding = false;
    private float fireTimer = 0f;

    private PlayerElementSwitch _playerElementSwitch;
    private GameObject elementBallPrefab;

    protected override void Awake() {
        base.Awake();
        attack = InputSystem.actions.FindAction("Attack");
    }

    protected override void InitState() {
        base.InitState();
        _playerElementSwitch = GetComponent<PlayerElementSwitch>();
        currentMagicPosition1 = magicPosition;
    }

    public override void ExecuteState() {
        if(_playerController.isClimbing) return;
        if (!isHolding || _playerController.isChargeAttack) return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate) {
            fireTimer = 0f;
            Shoot();
        }
    }

    private void OnEnable() {
        attack.started += OnAttackStarted;
        attack.canceled += OnAttackReleased;
    }

    private void OnDisable() {
        attack.started -= OnAttackStarted;
        attack.canceled -= OnAttackReleased;
    }

    private void OnAttackStarted(InputAction.CallbackContext context) {
        isHolding = true;
        fireTimer = fireRate;
    }

    private void OnAttackReleased(InputAction.CallbackContext context) {
        isHolding = false;
    }

    private void Shoot() {
        elementBallPrefab = _playerElementSwitch.current_element switch {
            PlayerElementSwitch.Element.Grass => grassBallPrefab,
            PlayerElementSwitch.Element.Water => waterBallPrefab,
            PlayerElementSwitch.Element.Wind => windBallPrefab,
            _ => grassBallPrefab
        };

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        Vector2 direction = (mouseWorldPos - currentMagicPosition1.position).normalized;

        SpawnBall(currentMagicPosition1, direction);
        if (_isDoubleShot)
            SpawnBall(magicPosition2, direction);       
    }

    private void SpawnBall(Transform spawnPoint, Vector2 direction) {
        GameObject ball = Instantiate(elementBallPrefab, spawnPoint.position, spawnPoint.rotation);

        if (!_playerController.facingRight)
            ball.transform.localScale = new Vector3(
                ball.transform.localScale.x * -1,
                ball.transform.localScale.y,
                ball.transform.localScale.z);

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * shootingSpeed;

        Destroy(ball, 3f);
    }

    // Skill Tree
    public void UnlockDoubleShot() { 
        _isDoubleShot = true;
        currentMagicPosition1 = newmagicPosition1;

    }
    public void LockDoubleShot() { 
        _isDoubleShot = false;
        currentMagicPosition1 = magicPosition;
    }
}

