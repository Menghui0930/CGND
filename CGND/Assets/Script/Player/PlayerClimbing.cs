using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClimbing : PlayerState
{
    [SerializeField] private float climbSpeed = 4f;
    private InputAction _climbAction;
    private InputAction _jumpAction;
    //public bool isClimbing = false;
    private VineClimb _currentVine;

    private float _jumpStartY;

    protected override void Awake() {
        base.Awake();
        _climbAction = InputSystem.actions.FindAction("Move"); // W/S 用 Move 的 Y 軸
        _jumpAction = InputSystem.actions.FindAction("Jump");
    }

  
    public void StartClimb(VineClimb vine) {
        if (_playerController.isJumping) return;
        _playerController.isClimbing = true;
        _playerController.isJumping = false;       // ✅ 强制重置跳跃状态

        _playerController.theRB.gravityScale = 0f; // ✅ 马上关重力
        _currentVine = vine;

        

        _playerController.theRB.linearVelocity = Vector2.zero;

        Vector3 pos = transform.position;
        pos.x = vine.transform.position.x;
        transform.position = pos;
    }

    public void StopClimb() {
        _playerController.isClimbing = false;
        _currentVine = null;

        // ✅ 還原重力
        _playerController.theRB.gravityScale = 1f;
        _animator.SetBool("Climb", false);

        GetComponent<PlayerJump>().JumpLeft = 1;

        if (!_playerController.isGrounded && !_playerController.isClimbing) {
            Debug.Log("Jump");
            _animator.SetBool("Jump", true);
        }
    }

    public void Jump() {
        _jumpStartY = transform.position.y; // 记录起跳位置
        _playerController.isJumping = true;
        _animator.SetBool("Jump", true);

        StartCoroutine(ClimbJump());
    }

    private IEnumerator ClimbJump() {
        float jumpDistance = 2f;    // 往上跳多少距离
        float jumpSpeed = 8f;       // 移动速度

        float targetY = _playerController.theRB.position.y + jumpDistance;

        _playerController.isJumping = true;
        _animator.SetBool("Jump", true);

        while (_playerController.theRB.position.y < targetY) {
            Vector2 pos = _playerController.theRB.position;
            pos.y = Mathf.MoveTowards(pos.y, targetY, jumpSpeed * Time.deltaTime);
            _playerController.theRB.position = pos;
            yield return null;
        }

        _playerController.isJumping = false;
        _animator.SetBool("Jump", false);
    }

    public override void ExecuteState() {
        if (!_playerController.isClimbing) return;

        if (_jumpAction.WasPressedThisFrame()) {
            float horizontalInput = _climbAction.ReadValue<Vector2>().x;

            if (Mathf.Abs(horizontalInput) > 0.1f) {
                StopClimb();
                // ✅ 直接调Jump，不依赖JumpLeft和GetInput
                float jumpForce = Mathf.Sqrt(5f * Mathf.Abs(Physics2D.gravity.y));
                _playerController.theRB.linearVelocity = new Vector2(horizontalInput * 4f, jumpForce);
                _playerController.isJumping = true;
                _animator.SetBool("Climb", false);
                _animator.SetBool("Jump", true);
                return;
            } else {
                Jump();
                return;
            }
        }

        if (_playerController.isJumping) return;

        float targetX = _currentVine.transform.position.x;
        float smoothX = Mathf.Lerp(
            _playerController.theRB.position.x,
            targetX,
            10f * Time.deltaTime
        );
        _playerController.theRB.position = new Vector2(smoothX, _playerController.theRB.position.y);

        float verticalInput = _climbAction.ReadValue<Vector2>().y;
        _playerController.theRB.linearVelocity = new Vector2(0, verticalInput * climbSpeed);
    }

    protected override void GetInput() {
        // ✅ 攀爬時不執行原本的 Input
        if (_playerController.isClimbing) return;
        base.GetInput();
    }

    public override void SetAnimation() {
        if (!_playerController.isClimbing) {
            _animator.SetBool("Climb", false); // ✅ 不在爬就关掉
            return;
        }
        _animator.SetBool("Jump", false);
        _animator.SetBool("Climb", true);
    }

    public void CLoseCLimbing() {
        StopCoroutine(ClimbJump());
        _animator.SetBool("ClimbFall", true);
    }

}
