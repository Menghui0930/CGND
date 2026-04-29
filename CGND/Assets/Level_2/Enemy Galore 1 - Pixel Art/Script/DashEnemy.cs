using UnityEngine;

public class DashEnemy : MonoBehaviour {
    [Header("Detection")]
    public float detectionRange = 10f;
    public Transform player;

    [Header("Dash")]
    public float dashSpeed = 8f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1.5f;

    [Header("Bounce")]
    public float bounceForce = 5f;

    [Tooltip("For sprite size tolerance, a range of 0.05 to 0.15 is recommended.）")]
    public float headTolerance = 0.1f;

    private enum State { Idle, Cooldown, Dashing }
    private State _state = State.Idle;

    private float _timer = 0f;
    private float _dashDirection = 1f;

    private Rigidbody2D _rb;
    private Animator _anim;
    public Collider2D _triggerCol; 

    void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _rb.freezeRotation = true;
    }

    void Update() {
        // check player
        bool playerInRange = Vector2.Distance(transform.position, player.position) <= detectionRange;

        if (!playerInRange) {
            if (_state != State.Idle) EnterIdle();
            return;
        }

        switch (_state) {
            case State.Idle:
                _state = State.Cooldown;
                _timer = 0f;
                break;

            case State.Cooldown:
                _timer += Time.deltaTime;
                if (_timer >= dashCooldown) StartDash();
                break;

            case State.Dashing:
                _timer += Time.deltaTime;
                _rb.linearVelocity = new Vector2(_dashDirection * dashSpeed, _rb.linearVelocity.y);
                if (_timer >= dashDuration) EndDash();
                break;
        }
    }


    void StartDash() {
        _state = State.Dashing;
        _timer = 0f;
        _dashDirection = player.position.x > transform.position.x ? 1f : -1f;
        _anim.SetBool("isDashing", true);
    }

    void EndDash() {
        _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
        _anim.SetBool("isDashing", false);
        _state = State.Cooldown;
        _timer = 0f;
    }

    void EnterIdle() {
        _state = State.Idle;
        _timer = 0f;
        _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
        _anim.SetBool("isDashing", false);
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player")) return;

        // A player's feet >=enemy's head (allowing for headTolerance error) for a player to be considered to have stomped on the enemy's head.
        float playerBottom = other.bounds.min.y;
        float enemyTop = _triggerCol != null
                             ? _triggerCol.bounds.max.y
                             : GetComponent<Collider2D>().bounds.max.y;

        if (playerBottom >= enemyTop - headTolerance) {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null)
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
        }
    }

    private void SetPlayer(PlayerMotor currentPlayer) {
        player = currentPlayer.transform;
    }

    private void OnEnable() {
        LevelManager.OnPlayerSpawn += SetPlayer;
    }

    private void OnDisable() {
        LevelManager.OnPlayerSpawn -= SetPlayer;
    }
}