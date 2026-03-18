using UnityEngine;

public class PlayerState : MonoBehaviour {
    protected PlayerController _playerController;
    protected float _horizontalInput;
    protected float _verticalInput;

    protected virtual void Start() {
        InitState();
    }

    protected virtual void InitState() {
        _playerController = GetComponent<PlayerController>();
    }

    public virtual void ExecuteState() {

    }

    // Gets the normal Input   
    public virtual void LocalInput() {

        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        GetInput();
    }

    // Override to support other Inputs
    protected virtual void GetInput() {

    }

    public virtual void SetAnimation() {

    }
}
