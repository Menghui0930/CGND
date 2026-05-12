using UnityEngine;
using System.Collections;
using System.Data.Common;

public class BossScript : MonoBehaviour {
    [Header("References")]
    private PlayerMotor player;
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
    public bool isStart = false;

    //private float bossScale;

    void Start() {
        anim = GetComponent<Animator>();
        anim.Play("BossBeforeStart");
        //bossScale = Mathf.Abs(transform.localScale.x);
        //StartCoroutine(FindPlayer());
    }

    /*
    private IEnumerator FindPlayer() {
        yield return new WaitForSeconds(0.5f);

        if (LevelManager.Instance != null) {
            GameObject playerObj = LevelManager.Instance.CurrentPlayer;
            if (playerObj != null) {
                player = playerObj.transform;
                Debug.Log("Boss found player!");
            } else {
                Debug.Log("Player not found!");
            }
        }
    }

    */

    void Update() {
        if(!isStart) return;
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        biteTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;

        //FacePlayer();

        // 咬攻击（近距离）
        if (distance <= biteRange && biteTimer <= 0 && !isAttacking) {
            StartCoroutine(BiteAttack());
        }
        
        // 射击攻击（远距离）
        else if (distance > biteRange &&
                 distance <= shootRange &&
                 shootTimer <= 0 && !isAttacking) {
            StartCoroutine(ShootAttack());
        }
        
    }

    private IEnumerator BiteAttack() {
        isAttacking = true;
        biteTimer = biteCooldown;
        anim.SetBool("isAttack", true);
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(biteDamageDelay);
        
        isAttacking = false;
        anim.SetBool("isAttack", false);
    }

    private IEnumerator ShootAttack() {
        isAttacking = true;
        shootTimer = shootCooldown;
        anim.SetBool("isShoot", true);
        anim.SetTrigger("Shoot");

        yield return new WaitForSeconds(1.5f);
        isAttacking = false;
        anim.SetBool("isShoot",false);
    }

    // Check in Animation Event
    private void CheckAndBite() {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <= biteRange) {
            Debug.Log("Bite Player");
            Health playerHealth = player.GetComponentInParent<Health>();
            playerHealth?.LoseLife();
        }
    }

    private void CheckAndShoot() {
        if (bulletPrefab != null && shootPoint != null) {
            GameObject bullet = Instantiate(bulletPrefab,shootPoint.position,Quaternion.identity);

            Vector2 direction = (player.transform.position - shootPoint.position).normalized;
            bullet.GetComponent<BulletScript>().direction = direction;
        }
    }

    /*
    void FacePlayer() {
        if (player.position.x < transform.position.x) {
            transform.localScale = new Vector3(
                bossScale, bossScale, 1);
        } else {
            transform.localScale = new Vector3(
                -bossScale, bossScale, 1);
        }
    }
    */

    private void OnEnable() {
        LevelManager.OnPlayerSpawn += OnPlayerSpawn;
        Health.OnDeath += BossReset;
    }

    private void OnDisable() {
        LevelManager.OnPlayerSpawn -= OnPlayerSpawn;
        Health.OnDeath -= BossReset;
    }

    private void OnPlayerSpawn(PlayerMotor playerMotor) {
        player = playerMotor;
        //Debug.Log("Boss updated player reference!");
    }

    public void BossActivate() {
        StartCoroutine(OpeningSequence());
    }

    private IEnumerator OpeningSequence() {
        // 播放开场动画
        LevelManager.Instance.HealthUIFadeOut();
        anim.Play("BossOpenning");

        // 等动画播完
        yield return null;
        float clipLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(clipLength);

        isStart = true;
        player.EnableControl();

        LevelManager.Instance.HealthUIFadeIn();
        yield return new WaitForSeconds(1f);
        Camera2D.instance.stopFollow = true;
        Camera2D.instance.horizontalFollow = false;
    }

    public void BossReset(PlayerMotor playerMotor) {
        Camera2D.instance.stopFollow = false;
        Camera2D.instance.horizontalFollow = true;
        anim.Play("BossBeforeStart");
        transform.GetComponent<BossHealth>().ResetHealth();
    }
}