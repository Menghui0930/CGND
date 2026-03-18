using UnityEngine;

public class PlayerJump : PlayerState {
    [Header("Settings")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private int maxJumps = 2;

    public int JumpLeft;

    protected override void InitState() {
        base.InitState();
        JumpLeft = maxJumps;
    }

    protected override void GetInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }
    }

    public override void ExecuteState() {
        base.ExecuteState();
    }

    private void Jump() {
        if (JumpLeft == 0) {
            return;
        }

        JumpLeft -= 1;
        float jumpForce = Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(Physics.gravity.y));
        _playerController.SetVerticalForce(jumpForce);
        _playerController.isJumping = true;
    }

    public override void SetAnimation() {

    }
}
