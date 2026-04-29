using UnityEngine;

public class EnemyTestDash : MonoBehaviour {
    [Header("Detection")]
    public float detectionRange = 10f;
    public Transform player;

    [Header("Dash")]
    public float dashSpeed = 8f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1.5f;

    [Header("Bounce")]
    public float bounceForce = 5f;

    private float dashTimer = 0f;
    private float cooldownTimer = 0f;

    private bool isDashing = false;
    private bool hasDashed = false;

    private float dashDirection = 1f;

    private Rigidbody2D rb;
    //private Animator anim;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();

        rb.freezeRotation = true;
    }

    void Update() {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerInRange = distanceToPlayer <= detectionRange;

        if (playerInRange) {
            cooldownTimer += Time.deltaTime;

            if (!isDashing && cooldownTimer >= dashCooldown) {
                StartDash();
            }
        } else if (!isDashing && hasDashed) {
            rb.linearVelocity = Vector2.zero;
        } else {
            isDashing = false;
            hasDashed = false;
            cooldownTimer = 0f;

            rb.linearVelocity = Vector2.zero;

            
            /*if (anim != null)
                anim.SetBool("isDashing", false);*/
        }

        if (isDashing) {
            dashTimer += Time.deltaTime;

            rb.linearVelocity = new Vector2(
                dashDirection * dashSpeed,
                rb.linearVelocity.y
            );

            if (dashTimer >= dashDuration) {
                EndDash();
            }
        }
    }

    void StartDash() {
        isDashing = true;
        dashTimer = 0f;

        dashDirection = player.position.x > transform.position.x ? 1f : -1f;

        /*
        if (anim != null)
            anim.SetBool("isDashing", true);
        */

        Debug.Log("enemy is dashing");
    }

    void EndDash() {
        isDashing = false;
        hasDashed = true;

        dashTimer = 0f;
        cooldownTimer = 0f;

        rb.linearVelocity = Vector2.zero;

        /*
        if (anim != null)
            anim.SetBool("isDashing", false);
        */
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts) {
            if (contact.normal.y < -0.5f) {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

                if (playerRb != null) {
                    playerRb.linearVelocity = new Vector2(
                        playerRb.linearVelocity.x,
                        bounceForce
                    );
                }

                Debug.Log("Bounced from top");
                return;
            }
        }
    }

    private void SetPlayer(PlayerMotor newPlayer) {
        player = newPlayer.transform;
    }

    private void OnEnable() {
        LevelManager.OnPlayerSpawn += SetPlayer;
    }

    private void OnDisable() {
        LevelManager.OnPlayerSpawn -= SetPlayer;
    }
}