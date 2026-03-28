using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : PlayerState
{
    [Header("Settings")]
    [SerializeField]private float chargeTime = 2f;
    [SerializeField]private Transform magicPosition;
    [SerializeField]private GameObject magicBallPrefab;
    [SerializeField]private float shootingSpeed =15f;
    private bool isHolding = false;
    private bool isCharged = false;
    private float holdTimer = 0f;

    private GameObject _currentMagicBall;


    protected override void Awake() {
        base.Awake();
        attack = InputSystem.actions.FindAction("Attack");
    }

    protected override void InitState() {
        base.InitState();;
    }

    protected override void GetInput() {
        base.GetInput();
    }

    public override void ExecuteState() {
            
            if (!isHolding) return;

            holdTimer += Time.deltaTime;

            if (_currentMagicBall == null) {
                _currentMagicBall = Instantiate(magicBallPrefab, magicPosition.transform.position, magicPosition.rotation);
            }

            _currentMagicBall.transform.position = magicPosition.position;

            float chargeProgress = Mathf.Clamp01(holdTimer / chargeTime);
            //Debug.Log($"holding time:{chargeProgress * 100f:0}%");
            
            if (holdTimer >= chargeTime && !isCharged) {
                isCharged=true;
                Debug.Log("Complete Charge");
            }
        }

    public override void SetAnimation() {
        base.SetAnimation();
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
        holdTimer = 0f;
        isCharged = false;
        Debug.Log("Attack");
        
    }

    private void OnAttackReleased(InputAction.CallbackContext context) {        
        if (isCharged) {
            SmallAttack();
        }

        isHolding = false;
        holdTimer = 0f;
        isCharged = false;
        Debug.Log("Cancel");
        Destroy(_currentMagicBall);
        
    }

    private void SmallAttack() {
        holdTimer += Time.deltaTime;

        if (_currentMagicBall == null) {
            _currentMagicBall = Instantiate(magicBallPrefab, magicPosition.transform.position, magicPosition.rotation);
        }

        _currentMagicBall.transform.position = magicPosition.position;


        float chargeProgress = Mathf.Clamp01(holdTimer / chargeTime);

        if (_currentMagicBall == null) return;

        GameObject ball = _currentMagicBall;
        _currentMagicBall = null;

        // mouse direction
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        Vector2 direction = (mouseWorldPos - ball.transform.position).normalized;

        Rigidbody2D theRB = ball.GetComponent<Rigidbody2D>();
        if (theRB != null) {
            theRB.linearVelocity = direction * shootingSpeed;
        }

        Destroy(ball, 3f);
    }

    private void Shoot() {
        GameObject ball = Instantiate(magicBallPrefab, magicPosition.position, magicPosition.rotation);

        // ✅ 計算滑鼠方向
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        Vector2 direction = (mouseWorldPos - ball.transform.position).normalized;

        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.linearVelocity = direction * shootingSpeed;
        }

        Destroy(ball, 3f);
    }

}
