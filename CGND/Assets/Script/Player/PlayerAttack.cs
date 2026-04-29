using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : PlayerState
{
    public static PlayerAttack Instance2;

    [Header("Settings")]
    [SerializeField]private float chargeTime = 2f;
    [SerializeField]private Transform magicPosition;
    [SerializeField]private Transform TornadomagicPosition;
    [SerializeField]private float shootingSpeed =15f;
    public bool isHolding = false;
    private bool isCharged = false;
    private float holdTimer = 0f;

    [SerializeField] private GameObject grassBallPrefab;
    [SerializeField] private GameObject waterBallPrefab;
    [SerializeField] private GameObject windBallPrefab;
    [SerializeField] private GameObject windTornadoPrefab;

    private GameObject magicBallPrefab;
    private GameObject _currentMagicBall;

    private PlayerElementSwitch _playerElementSwitch;

    public bool isUpgradeWind;


    protected override void Awake() {
        base.Awake();
        Instance2 = this;

        ChargeAttack = InputSystem.actions.FindAction("ChargeAttack");

    }

    protected override void InitState() {
        base.InitState();;
        _playerElementSwitch = GetComponent<PlayerElementSwitch>();
    }

    protected override void GetInput() {
        base.GetInput();
    }

    public override void ExecuteState() {
        if(_playerController.isClimbing) return;

        SwitchElement();
            
        if (!isHolding) return;

        holdTimer += Time.deltaTime;

        Transform currentPosition = (isUpgradeWind && _playerElementSwitch.current_element == PlayerElementSwitch.Element.Wind)
        ? TornadomagicPosition
        : magicPosition;

        if (_currentMagicBall == null) {
            _currentMagicBall = Instantiate(magicBallPrefab, currentPosition.position, currentPosition.rotation);
        }

        if (isHolding && isUpgradeWind && _playerElementSwitch.current_element == PlayerElementSwitch.Element.Wind) {
            PlayerController.instance.SetHorizontalForce(0);
        }

        _currentMagicBall.transform.position = currentPosition.position;

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
        ChargeAttack.started += OnAttackStarted;
        ChargeAttack.canceled += OnAttackReleased;
    }

    private void OnDisable() {
        ChargeAttack.started -= OnAttackStarted;
        ChargeAttack.canceled -= OnAttackReleased;
    }

    private void OnAttackStarted(InputAction.CallbackContext context) {     
        _playerController.isChargeAttack = true;
        isHolding = true;
        holdTimer = 0f;
        isCharged = false;
        Debug.Log("Attack");
        
    }

    private void OnAttackReleased(InputAction.CallbackContext context) {        
        if (isCharged) {
            if (isUpgradeWind && _playerElementSwitch.current_element == PlayerElementSwitch.Element.Wind) {
                ShootTornado();
            } else {
                Shoot();
            }
        } else {
            Destroy(_currentMagicBall);
        }

        _playerController.isChargeAttack=false;
        isHolding = false;
        holdTimer = 0f;
        isCharged = false;
        Debug.Log("Cancel");
        //Destroy(_currentMagicBall);
        
    }

    private void SwitchElement() {
        magicBallPrefab = _playerElementSwitch.current_element switch {
            PlayerElementSwitch.Element.Grass => grassBallPrefab,
            PlayerElementSwitch.Element.Water => waterBallPrefab,
            PlayerElementSwitch.Element.Wind => windBallPrefab,
            _ => grassBallPrefab
        };

        if (isUpgradeWind) {
            if (magicBallPrefab == windBallPrefab) {
                magicBallPrefab = windTornadoPrefab;
            }
        }



    }
    /*
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
    */

    private void Shoot() {
        if (_currentMagicBall == null) return;
        MagicPoint.Instance.DecreaseMP();

        GameObject ball = _currentMagicBall;
        _currentMagicBall = null; 

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        Vector2 direction = (mouseWorldPos - ball.transform.position).normalized;
        float dirX = direction.x >= 0 ? ball.transform.localScale.x : -ball.transform.localScale.x;
        ball.transform.localScale = new Vector3(dirX, ball.transform.localScale.y, transform.localScale.z);


        Animator ballAnim = ball.GetComponent<Animator>();
        ballAnim.SetTrigger("Shoot");
        Rigidbody2D theRB = ball.GetComponent<Rigidbody2D>();
        if (theRB != null) {
            theRB.linearVelocity = direction * shootingSpeed;
        }

        Destroy(ball, 3f);
    }

    private void ShootTornado() {
        if (_currentMagicBall == null) return;
        MagicPoint.Instance.DecreaseMP();

        GameObject ball = _currentMagicBall;
        ball.GetComponent<BoxCollider2D>().enabled = true;
        _currentMagicBall = null;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        Vector2 direction = Vector2.right;
        if (!_playerController.facingRight) {
            direction = Vector2.left;
        }

        Animator ballAnim = ball.GetComponent<Animator>();
        //ballAnim.SetTrigger("Shoot");
        Rigidbody2D theRB = ball.GetComponent<Rigidbody2D>();
        if (theRB != null) {
            theRB.linearVelocity = direction * shootingSpeed;
        }

        Destroy(ball, 3f);
    }

    public bool CheckHoldingTornado() {
        bool isHoldingTornado = isHolding && isUpgradeWind && _playerElementSwitch.current_element == PlayerElementSwitch.Element.Wind;
        return isHoldingTornado;
    }

}
