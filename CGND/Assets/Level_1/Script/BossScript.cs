using UnityEngine;

public class BossScript : MonoBehaviour
{
    public Transform player;
    public Animator anim;
    public float walkSpeed = 2f;
    public float walkRange = 5f;
    public float biteRange = 2f;
    public float biteCooldown = 2f;
    public float biteDamageDelay = 0.3f;

    private float biteTimer = 0f;
    private bool isAttacking = false;

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Debug.Log("Distance: " + distance);
        Debug.Log("isAttacking: " + isAttacking);
        Debug.Log("Walk condition: " + (distance <= walkRange && distance > biteRange && !isAttacking));
        Debug.Log("Boss Position: " + transform.position);
        biteTimer -= Time.deltaTime;

        // 玩家在检测范围内
        if (distance <= walkRange)
        {
            FacePlayer();

            // 走向玩家
            if (distance > biteRange && !isAttacking)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    player.position,
                    walkSpeed * Time.deltaTime);
                anim.SetFloat("Walk", 1f);
            }
            else
            {
                anim.SetFloat("Walk", 0f);
            }

            // 咬攻击
            if (distance <= biteRange && biteTimer <= 0 && !isAttacking)
            {
                StartCoroutine(BiteAttack());
            }
        }
        else
        {
            anim.SetFloat("Walk", 0f);
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