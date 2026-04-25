using UnityEngine;
using UnityEngine.InputSystem;

public class characterClimb : MonoBehaviour
{
    private bool onVine = false;
    private Rigidbody2D rb;
    public float climbSpeed = 3f;

    private InputAction moveAction;
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        moveAction.Enable();
        jumpAction.Enable();
    }

    void Update()
    {
        if (onVine)
        {
            float verticalInput = moveAction.ReadValue<Vector2>().y;

            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(0, verticalInput * climbSpeed);

            if (jumpAction.WasPressedThisFrame())
            {
                LeaveVine();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Vine"))
        {
            VineScript vine = collision.gameObject.GetComponent<VineScript>();
            if (vine != null && vine.isGrown)
            {
                onVine = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Vine"))
        {
            LeaveVine();
        }
    }

    private void LeaveVine()
    {
        onVine = false;
        rb.gravityScale = 1f; 
    }
}