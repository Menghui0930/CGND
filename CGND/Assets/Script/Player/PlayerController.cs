using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    public static PlayerController instance;

    [Header("Settings")]
    // Falling
    public Rigidbody2D theRB;
    [SerializeField] private float fallMultiplier = 1.5f;
    
    // Movement
    public Vector2 Force => _force;
    private Vector2 _force;

    [Header("Face Direction")]
    // Face Direction
    public bool facingRight;
    //private int internalFaceDirection = 1;
    //private int faceDirection;

    [Header("Jumping")]
    // Jumping
    public bool isJumping = false;
    public Transform groundChecker;
    public bool isGrounded = false;
    public LayerMask groundMask;

    [Header("Climbing")]
    public bool isClimbing = false;

    public void SetHorizontalForce(float xForce) => _force.x = xForce;
    public void SetVerticalForce(float yForce) => _force.y = yForce;

    //Attack
    public bool isChargeAttack = false;

    private void Awake() {
        instance = this;
    }

    void Start() {
        theRB = GetComponent<Rigidbody2D>();
    }

    void Update() {
        // Face Direction
        FaceDirection();
        RotateModel();

        // Movement
        theRB.linearVelocity = new Vector2(_force.x, theRB.linearVelocity.y);

        // Jumping
        isGrounded = Physics2D.OverlapCircle(groundChecker.position, 0.3f, groundMask);
        if (isJumping) {
            theRB.linearVelocityY = 0;
            theRB.AddForce(new Vector2(0, _force.y), ForceMode2D.Impulse);
            isGrounded = false;
            isJumping = false;
            _force.y = 0;
        }

        // Falling
        if (theRB.linearVelocity.y < 0) {
            theRB.linearVelocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        
    }

    private void FaceDirection() {
        /*
        faceDirection = internalFaceDirection;
        facingRight = faceDirection == 1;

        if (_force.x > 0.0001f) {
            faceDirection = 1;
            facingRight = true;
        } else if (_force.x < -0.0001f) {
            faceDirection = -1;
            facingRight = false;
        }
        internalFaceDirection = faceDirection;
        */

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        facingRight = mouseWorldPos.x > transform.position.x;
    }

    private void RotateModel() {
        transform.localScale = facingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmos() {
        if (groundChecker == null) return;

        // if ground will red color 
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundChecker.position, 0.3f); 
    }


}
