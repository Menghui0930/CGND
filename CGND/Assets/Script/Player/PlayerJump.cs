using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : PlayerState {
    [Header("Settings")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private int maxJumps = 2;

    public int JumpLeft;


    protected override void Awake() {
        base.Awake();
        jumping = InputSystem.actions.FindAction("Jump");
    }

    protected override void InitState() {
        base.InitState();
        JumpLeft = maxJumps;
    }

    protected override void GetInput() {
        if (jumping.WasPressedThisFrame()) {
            Jump();
        }
    }

    public override void ExecuteState() {
        if (_playerController.isGrounded && _playerController.Force.y == 0 && _playerController.theRB.linearVelocityY ==0f) {
            JumpLeft = maxJumps;
            _playerController.isJumping = false;
        }

        if (_playerController.isClimbing) {
            JumpLeft = 1;
        }
    }

    public void Jump() {
        if (JumpLeft == 0) {
            return;
        }

        if (_playerController.isGrounded || JumpLeft > 0) {

            JumpLeft -= 1;
            float jumpForce = Mathf.Sqrt(jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            _playerController.SetVerticalForce(jumpForce);
            _animator.SetBool("Jump",true);
            _playerController.isJumping = true;
        }
    }

    public override void SetAnimation() {
        if (_playerController.isClimbing) return;

        if (_playerController.theRB.linearVelocityY > 0) {
            _animator.SetFloat("FloatY", 1);
        } else if (_playerController.theRB.linearVelocityY <= 0) {
            _animator.SetFloat("FloatY", -1);
        }

        if (_playerController.isGrounded) {
            _animator.SetBool("Jump",false);
            _animator.SetBool("ClimbFall",false);
        }
    }
}
