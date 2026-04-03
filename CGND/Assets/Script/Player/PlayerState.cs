using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerState : MonoBehaviour {
    protected InputActionAsset defaultInputs;
    protected InputAction playerMovement;
    protected InputAction jumping;
    protected InputAction dashing;
<<<<<<< HEAD
=======
    protected InputAction attack;
    protected InputAction ChargeAttack;
    protected InputAction switching;
>>>>>>> MengHui

    protected PlayerController _playerController;
    protected Animator _animator;

    // movement
    protected Vector2 moveDirection;
    protected float _horizontalInput;
    protected float _verticalInput;

    protected virtual void Awake() {
        playerMovement = InputSystem.actions.FindAction("Move");
    }

    protected virtual void Start() {
        InitState();
    }

    protected virtual void InitState() {
        _playerController = GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
    }

    public virtual void ExecuteState() {

    }

    // Gets the normal Input   
    public virtual void LocalInput() {
        moveDirection = playerMovement.ReadValue<Vector2>();

        _horizontalInput = moveDirection.x;
        _verticalInput = moveDirection.y;

        GetInput();
    }

    // Override to support other Inputs
    protected virtual void GetInput() {

    }

    public virtual void SetAnimation() {

    }
}
