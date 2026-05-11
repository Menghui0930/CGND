using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerState {
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    private float _movement;

    // flower
    public float _speedBonus = 0f;

    protected override void Awake() {
        base.Awake();
    }

    protected override void InitState() {
        base.InitState();
    }
    protected override void GetInput() {
        _movement = Mathf.Abs(_horizontalInput) > 0.1f ? _horizontalInput : 0f;
    }

    public override void ExecuteState() {
        MovePlayer();
    }

    private void MovePlayer() {
        _playerController.SetHorizontalForce(_movement * (speed + _speedBonus));
    }

    public void AddSpeedBonus(float bonus) {
        Debug.Log("Add bonus");
        _speedBonus += bonus; }

    public override void SetAnimation() {
        _animator.SetBool("Idle", _horizontalInput == 0 && _playerController.isGrounded);
        if (!PlayerAttack.Instance2.CheckHoldingTornado()) {
            _animator.SetBool("Run", Mathf.Abs(_horizontalInput) > 0.1f && _playerController.isGrounded);
        }
    }
}
