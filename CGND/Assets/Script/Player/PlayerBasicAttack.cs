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
    }

    public override void ExecuteState() {
        if (!isHolding) return;

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

        GameObject ball = Instantiate(elementBallPrefab, magicPosition.position, magicPosition.rotation);

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        Vector2 direction = (mouseWorldPos - ball.transform.position).normalized;

        Rigidbody2D theRB = ball.GetComponent<Rigidbody2D>();
        if (theRB != null) {
            theRB.linearVelocity = direction * shootingSpeed;
        }

        Destroy(ball, 3f);
    }
}

