using UnityEngine;
using System.Collections;

public class BossScript : MonoBehaviour
{
    [Header("References")]
    private Transform player;
    private Animator anim;
    public GameObject bulletPrefab;
    public Transform shootPoint;

    [Header("Bite Attack")]
    public float biteRange = 5f;
    public float biteCooldown = 2f;
    public float biteDamageDelay = 0.3f;

    [Header("Shoot Attack")]
    public float shootRange = 8f;
    public float shootCooldown = 3f;

    private float biteTimer = 0f;
    private float shootTimer = 0f;
    private bool isAttacking = false;

    private float bossScale;

    void Start()
    {
        anim = GetComponentInChildren<Animator>(); 
        bossScale = Mathf.Abs(transform.localScale.x);
        StartCoroutine(FindPlayer());
    }

    private IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(0.5f);

        if (LevelManager.Instance != null)
        {
            GameObject playerObj = LevelManager.Instance.CurrentPlayer;
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Boss found player!");
            }
            else
            {
                Debug.Log("Player not found!");
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(
            transform.position, player.position);

        biteTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;

        FacePlayer();

        // 咬攻击（近距离）
        if (distance <= biteRange &&
            biteTimer <= 0 && !isAttacking)
        {
            StartCoroutine(BiteAttack());
        }
        // 射击攻击（远距离）
        else if (distance > biteRange &&
                 distance <= shootRange &&
                 shootTimer <= 0 && !isAttacking)
        {
            StartCoroutine(ShootAttack());
        }
    }

    private IEnumerator BiteAttack()
    {
        isAttacking = true;
        biteTimer = biteCooldown;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(biteDamageDelay);

        float distance = Vector2.Distance(
            transform.position, player.position);
        if (distance <= biteRange)
        {
            Health playerHealth = player
                .GetComponentInParent<Health>();
            if (playerHealth != null)
            {
                playerHealth.LoseLife();
            }
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    private IEnumerator ShootAttack()
    {
        isAttacking = true;
        shootTimer = shootCooldown;

        yield return new WaitForSeconds(0.5f);

        if (bulletPrefab != null && shootPoint != null)
        {
            GameObject bullet = Instantiate(
                bulletPrefab,
                shootPoint.position,
                Quaternion.identity);

            Vector2 direction = (player.position -
                shootPoint.position).normalized;
            bullet.GetComponent<BulletScript>().direction = direction;
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    void FacePlayer()
    {
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(
                bossScale, bossScale, 1);
        }
        else
        {
            transform.localScale = new Vector3(
                -bossScale, bossScale, 1);
        }
    }

    private void OnEnable()
    {
        LevelManager.OnPlayerSpawn += OnPlayerSpawn;
    }

    private void OnDisable()
    {
        LevelManager.OnPlayerSpawn -= OnPlayerSpawn;
    }

    private void OnPlayerSpawn(PlayerMotor playerMotor)
    {
        player = playerMotor.transform;
        Debug.Log("Boss updated player reference!");
    }
}