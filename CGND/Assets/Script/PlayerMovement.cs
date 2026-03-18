using UnityEngine;

public class PlayerMovement : PlayerState {
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    private float _movement;

    private float _horizontalMovement;

    protected override void InitState() {
        base.InitState();
    }
    protected override void GetInput() {
        _horizontalMovement = _horizontalInput;
    }

    public override void ExecuteState() {
        MovePlayer();
    }

    private void MovePlayer() {
        if (Mathf.Abs(_horizontalMovement) > 0.1f) {
            _movement = _horizontalMovement;
        } else {
            _movement = 0f;
        }

        float moveSpeed = _movement * speed;
        _playerController.SetHorizontalForce(moveSpeed);
    }

    public override void SetAnimation() {

    }
}
