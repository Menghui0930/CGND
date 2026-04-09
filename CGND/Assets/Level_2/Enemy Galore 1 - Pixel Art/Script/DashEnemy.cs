using UnityEngine;

public class DashEnemy : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 10f;    
    public Transform player;             

    [Header("Dash")]
    public float dashSpeed = 8f;          
    public float dashDuration = 0.3f;    
    public float dashCooldown = 1.5f;

    [Header("Boundary")]
    public float leftLimit = -30f;    
    public float rightLimit = 30f;

    [Header("Bounce")]
    public float bounceForce = 5f;       

    private float dashTimer = 0f;
    private float cooldownTimer = 0f;
    private bool isDashing = false;
    private float dashDirection = 1f;
    private bool hasDashed = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerInRange = distanceToPlayer <= detectionRange;

       

        if (playerInRange)
        {
            cooldownTimer += Time.deltaTime;

            if (!isDashing && cooldownTimer >= dashCooldown)
            {
                StartDash();
            }
        }
        else if (!isDashing && hasDashed)
        {
            rb.linearVelocity = Vector2.zero; 
        }
        else
        {
            
            isDashing = false;
            hasDashed = false;
            cooldownTimer = 0f;
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isDashing", false);
        }

        if (isDashing)
        {
            dashTimer += Time.deltaTime;
            rb.linearVelocity = new Vector2(dashDirection * dashSpeed, rb.linearVelocity.y);
            Debug.Log("enemy is dashing");

            //bool hitLeftWall = transform.position.x <= leftLimit && dashDirection == -1f;
           // bool hitRightWall = transform.position.x >= rightLimit && dashDirection == 1f;

            //if (hitLeftWall || hitRightWall)
            //{
             //   isDashing = false;
             //   dashTimer = 0f;
            //    cooldownTimer = 0f;
            //    rb.linearVelocity = Vector2.zero;
            //    anim.SetBool("isDashing", false);
            //}







            if (dashTimer >= dashDuration)
            {
                isDashing = false;
                hasDashed = true;
                dashTimer = 0f;
                cooldownTimer = 0f;
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("isDashing", false);
            }
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = 0f;

       
        dashDirection = player.position.x > transform.position.x ? 1f : -1f;
        anim.SetBool("isDashing", true);
        Debug.Log("enemy is dashing");
    }


   

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                
                if (contact.normal.y < -0.5f)
                {
                    Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
                    Debug.Log("Bounced from top");
                    return;
                }
            }
            
        }



    }
}