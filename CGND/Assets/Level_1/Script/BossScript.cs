using UnityEngine;

public class BossScript : MonoBehaviour
{
    public Transform player;
    public Animator anim;

    public float walkSpeed = 2f;
    public float walkRange = 5f;
    public float moveSpeed = 2f;
    public float stopDistance = 2f;

    public float detectionRange = 5f;   
    public float biteRange = 2f;       
    public float biteCooldown = 2f;     
    public float biteDamageDelay = 0.3f; 

    private float biteTimer = 0f;
    private bool isAttacking = false;

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= walkRange && distance > biteRange && !isAttacking)
        {
            FacePlayer();
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                walkSpeed * Time.deltaTime);

            if (distance > stopDistance && !isAttacking)
            {
                // 移动
                Vector2 direction = (player.position - transform.position).normalized;
                transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

                anim.SetFloat("Speed", 1f);
            }
            else
            {
                anim.SetFloat("Speed", 0f);
            }

            if (player == null) return;

            biteTimer -= Time.deltaTime;

            if (distance <= detectionRange)
            {
                FacePlayer();

                if (distance <= biteRange && biteTimer <= 0 && !isAttacking)
                {
                    StartCoroutine(BiteAttack());
                }
            }
        }      
    }
    private System.Collections.IEnumerator BiteAttack()
    {
        isAttacking = true;
        biteTimer = biteCooldown;

        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(biteDamageDelay);

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= biteRange)
        {
            Health playerHealth = player.GetComponentInParent<Health>();
            if (playerHealth != null)
            {
                playerHealth.LoseLife();
            }
        }


        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    void FacePlayer()
    {
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),
                transform.localScale.y, 1);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x),
                transform.localScale.y, 1);
        }
    }
}
