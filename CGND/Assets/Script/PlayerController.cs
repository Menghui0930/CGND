using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController instance;

    [Header("Settings")]
    // Falling
    [SerializeField] private float fallMultiplier = 1.5f;

    public Rigidbody2D theRB;

    // Movement
    public Vector2 _movePosition = Vector2.zero;
    private Vector2 _force;

    // Face Direction
    public bool facingRight;
    private int _internalFaceDirection = 1;
    private int _faceDirection;

    // Jumping
    public bool isJumping = false;
    public Transform groundChecker;
    public bool isGrounded = false;
    public LayerMask groundMask;

    public void SetHorizontalForce(float xForce) => _force.x = xForce;
    public void SetVerticalForce(float yForce) => _force.y = yForce;

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

        //Jumping
        isGrounded = Physics2D.OverlapCircle(groundChecker.position, 0.5f, groundMask);
        if (isJumping) {
            theRB.AddForce(new Vector2(theRB.linearVelocityX, _force.y), ForceMode2D.Impulse);
            isJumping = false;
        }

        // Falling
        if (theRB.linearVelocity.y < 0) {
            theRB.linearVelocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

    }

    private void FaceDirection() {
        _faceDirection = _internalFaceDirection;
        facingRight = _faceDirection == 1;

        if (_force.x > 0.0001f) {
            _faceDirection = 1;
            facingRight = true;
        } else if (_force.x < -0.0001f) {
            _faceDirection = -1;
            facingRight = false;
        }
        _internalFaceDirection = _faceDirection;
    }

    private void RotateModel() {
        transform.localScale = facingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
    }


}
