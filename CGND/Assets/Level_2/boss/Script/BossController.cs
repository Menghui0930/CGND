using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform firePoint;

    [Header("Movement")]
    public float flySpeed = 3f;
    public float flyRadius = 4f;
    public Transform roomCenter;

    [Header("Bullet")]
    public float bulletSpeed = 7f;
    public float minPatternDuration = 3f;
    public float maxPatternDuration = 5f;
    public float patternCooldown = 1f;

    [Header("Room")]
    public BossRoom bossRoom;

    public float maxHP = 500f;
    private float currentHP;
    private bool bossActive = false;
    private Vector2 flyTarget;
    private Rigidbody2D rb;
    private Animator anim;
    private bool patternRunning = false;

    private List<System.Func<IEnumerator>> patterns;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
        rb.gravityScale = 0f;

        patterns = new List<System.Func<IEnumerator>>
        {
            PatternSpiral,
            PatternRadialBurst,
            PatternAimedStream,
            PatternCross,
            PatternRingExpand
        };

        
    }

    void Update()
    {
        if (!bossActive) return;

        // fly randomly around room
        transform.position = Vector2.MoveTowards( transform.position, flyTarget, flySpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, flyTarget) < 0.2f)
            PickNewFlyTarget();

    }

    void PickNewFlyTarget()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        flyTarget = (Vector2)roomCenter.position + randomDir * Random.Range(2f, flyRadius);
    }

    public void ActivateBoss()
    {
        bossActive = true;
        PickNewFlyTarget();
        StartCoroutine(BossRoutine());
        Debug.Log("Boss activated!");
    }

    IEnumerator BossRoutine()
    {
        Debug.Log("BossRoutine started!");
        yield return new WaitForSeconds(1.5f);

        while (bossActive)
        {
            var pattern = patterns[Random.Range(0, patterns.Count)];
            float duration = Random.Range(minPatternDuration, maxPatternDuration);

            Debug.Log("Running pattern for " + duration + "s");
            patternRunning = true;
            Coroutine activePattern = StartCoroutine(pattern());
            yield return new WaitForSeconds(duration);

            patternRunning = false;
            StopCoroutine(activePattern);

            yield return new WaitForSeconds(0.1f);
            yield return new WaitForSeconds(patternCooldown);
        }
    }

    // ── BULLET PATTERNS ──────────────────────────────

    IEnumerator PatternSpiral()
    {
        float angle = 0f;
        while (patternRunning)
        {
            FireBulletAtAngle(angle);
            angle += 15f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator PatternRadialBurst()
    {
        while (patternRunning)
        {
            int bulletsPerBurst = 16;
            for (int i = 0; i < bulletsPerBurst; i++)
                FireBulletAtAngle(i * (360f / bulletsPerBurst));
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator PatternAimedStream()
    {
        while (patternRunning)
        {
            if (player != null)
            {
                Vector2 dir = (player.position - firePoint.position).normalized;
                float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                FireBulletAtAngle(baseAngle + Random.Range(-10f, 10f));
            }
            yield return new WaitForSeconds(0.10f);
        }
    }

    IEnumerator PatternCross()
    {
        float rotation = 0f;
        while (patternRunning)
        {
            float[] angles = { 0f, 90f, 180f, 270f, 45f, 135f, 225f, 315f };
            foreach (float a in angles)
                FireBulletAtAngle(a + rotation);
            rotation += 10f;
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator PatternRingExpand()
    {
        float offset = 0f;
        while (patternRunning)
        {
            int n = 12;
            for (int i = 0; i < n; i++)
            {
                float a = i * (360f / n) + offset;
                FireBulletAtAngle(a, bulletSpeed * 0.6f);
                FireBulletAtAngle(a + (180f / n), bulletSpeed * 1.4f);
            }
            offset += 15f;
            yield return new WaitForSeconds(0.4f);
        }
    }

    void FireBulletAtAngle(float angleDeg, float? overrideSpeed = null)
    {
        Debug.Log("Firing bullet at angle: " + angleDeg);
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        float spd = overrideSpeed ?? bulletSpeed;
        GameObject b = BulletPool.Instance.Get(firePoint.position, Quaternion.identity);
        b.GetComponent<Bullet>().Init(dir, spd);
    }

    public void TakeDamage(float amount)
    {
        if (!bossActive) return;
        currentHP -= amount;
        Debug.Log("[Boss] HP: " + currentHP);
        if (currentHP <= 0) Die();
    }

    void Die()
    {
        bossActive = false;
        anim.SetTrigger("Die");
        if (bossRoom != null) bossRoom.UnlockRoom();
        Debug.Log("[Boss] Defeated!");
    }
}