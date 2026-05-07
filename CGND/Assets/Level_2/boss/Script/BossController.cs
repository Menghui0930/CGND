using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossPhase { Idle, Phase1, Phase2, Enraged, Dead }
public enum BossState { Flying, Dashing, Charging }

public class BossController : MonoBehaviour
{
    [Header("Boss Stats")]
    public float maxHP = 1000f;
    private float currentHP;
    public BossPhase currentPhase = BossPhase.Idle;

    [Header("Bullet Settings")]
    public float bulletSpeed = 7f;
    public Transform firePoint;
    public Transform player;

    [Header("Pattern Timing")]
    public float minPatternDuration = 3f;
    public float maxPatternDuration = 5f;
    public float patternCooldown = 1f;

    [Header("Flying")]
    public float flySpeed = 3f;
    public float flyRadius = 4f;
    public Transform roomCenter;

    [Header("Dashing")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.4f;

    [Header("Charging")]
    public float chargeDuration = 3f;
    public float chargeStateDuration = 6f;
    public GameObject trackingBulletPrefab;   // separate tracking bullet prefab

    [Header("Defense")]
    public float normalDefense = 1f;
    public float chargeDefense = 0.2f;        // takes 20% damage while charging

    [Header("Room")]
    public BossRoom bossRoom;                 // drag BossRoom here

    // private
    private float currentDefense = 1f;
    private bool patternRunning = false;
    private bool bossActive = false;
    private BossState currentState = BossState.Flying;
    private Vector2 flyTarget;
    private Rigidbody2D rb;
    private Animator anim;

    private List<System.Func<IEnumerator>> patterns;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHP = maxHP;
        currentDefense = normalDefense;
        rb.gravityScale = 0f;

        patterns = new List<System.Func<IEnumerator>>
        {
            PatternSpiral,
            PatternRadialBurst,
            PatternAimedStream,
            PatternCross,
            PatternRingExpand
        };

        ActivateBoss();
    }

    void Update()
    {
        if (!bossActive) return;

        switch (currentState)
        {
            case BossState.Flying: UpdateFlying(); break;
        }
    }

    // ── ACTIVATE ────────────────────────────────────
    public void ActivateBoss()
    {
        bossActive = true;
        currentPhase = BossPhase.Phase1;
        currentState = BossState.Flying;
        PickNewFlyTarget();
        StartCoroutine(BossRoutine());
        Debug.Log("Boss activated!");
    }

    // ── FLYING ──────────────────────────────────────
    void UpdateFlying()
    {
        transform.position = Vector2.MoveTowards(
            transform.position, flyTarget,
            flySpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, flyTarget) < 0.2f)
            PickNewFlyTarget();
    }

    void PickNewFlyTarget()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        flyTarget = (Vector2)roomCenter.position
                  + randomDir * Random.Range(1f, flyRadius);
    }

    // ── BOSS ROUTINE ────────────────────────────────
    IEnumerator BossRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        while (currentPhase != BossPhase.Dead)
        {
            // pick random action: 0=bullet pattern, 1=dash, 2=charge
            int action = Random.Range(0, 3);

            if (action == 0)
            {
                // bullet pattern while flying
                var pattern = patterns[Random.Range(0, patterns.Count)];
                float duration = Random.Range(minPatternDuration, maxPatternDuration);
                patternRunning = true;
                StartCoroutine(pattern());
                yield return new WaitForSeconds(duration);
                patternRunning = false;
                yield return new WaitForSeconds(0.1f);
            }
            else if (action == 1)
            {
                // dash state
                yield return StartCoroutine(DashRoutine());
            }
            else
            {
                // charge state
                yield return StartCoroutine(ChargeRoutine());
            }

            yield return new WaitForSeconds(patternCooldown);
        }

        Debug.Log("[Boss] Dead.");
    }

    // ── DASH ROUTINE ────────────────────────────────
    IEnumerator DashRoutine()
    {
        currentState = BossState.Dashing;
        anim.SetTrigger("Dash");
        Debug.Log("[Boss] Dashing!");

        Vector2 dashDir = ((Vector2)player.position
                         - (Vector2)transform.position).normalized;

        float timer = 0f;
        while (timer < dashDuration)
        {
            rb.linearVelocity = dashDir * dashSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;

        // fly back opposite direction briefly
        flyTarget = (Vector2)transform.position + (-dashDir * 2f);
        currentState = BossState.Flying;
    }

    // ── CHARGE ROUTINE ──────────────────────────────
    IEnumerator ChargeRoutine()
    {
        currentState = BossState.Charging;
        anim.SetBool("isCharging", true);
        currentDefense = chargeDefense;     // defense up!
        Debug.Log("[Boss] Charging! Defense up!");

        yield return new WaitForSeconds(chargeDuration);

        // shoot tracking bullet after charge
        ShootTrackingBullet();
        Debug.Log("[Boss] Fired tracking bullet!");

        yield return new WaitForSeconds(chargeStateDuration - chargeDuration);

        // end charge
        currentDefense = normalDefense;
        anim.SetBool("isCharging", false);
        currentState = BossState.Flying;
    }

    void ShootTrackingBullet()
    {
        if (trackingBulletPrefab == null) return;
        GameObject bullet = Instantiate(trackingBulletPrefab,
                                        firePoint.position,
                                        Quaternion.identity);
        TrackingBullet tb = bullet.GetComponent<TrackingBullet>();
        if (tb != null) tb.target = player;
    }

    // ── PHASE CHECK ─────────────────────────────────
    void CheckPhase()
    {
        float pct = currentHP / maxHP;
        BossPhase next = pct > 0.5f ? BossPhase.Phase1
                       : pct > 0.25f ? BossPhase.Phase2
                       : pct > 0f ? BossPhase.Enraged
                       : BossPhase.Dead;

        if (next != currentPhase)
        {
            currentPhase = next;
            OnPhaseChanged();
        }
    }

    void OnPhaseChanged()
    {
        Debug.Log($"[Boss] Phase changed to: {currentPhase}");

        bulletSpeed = currentPhase switch
        {
            BossPhase.Phase1 => 7f,
            BossPhase.Phase2 => 9f,
            BossPhase.Enraged => 12f,
            _ => 7f
        };

        // speed up in later phases
        flySpeed = currentPhase switch
        {
            BossPhase.Phase1 => 3f,
            BossPhase.Phase2 => 4.5f,
            BossPhase.Enraged => 6f,
            _ => 3f
        };

        dashSpeed = currentPhase switch
        {
            BossPhase.Phase1 => 15f,
            BossPhase.Phase2 => 20f,
            BossPhase.Enraged => 25f,
            _ => 15f
        };

        if (currentPhase == BossPhase.Dead)
        {
            bossActive = false;
            anim.SetTrigger("Die");
            if (bossRoom != null) bossRoom.UnlockRoom();
        }
    }

    // ── DAMAGE ───────────────────────────────────────
    public void TakeDamage(float amount)
    {
        if (currentPhase == BossPhase.Dead) return;
        float actualDamage = amount * currentDefense;
        currentHP = Mathf.Clamp(currentHP - actualDamage, 0, maxHP);
        Debug.Log($"[Boss] Took {actualDamage} damage! HP: {currentHP}");
        CheckPhase();
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
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        float spd = overrideSpeed ?? bulletSpeed;
        GameObject b = BulletPool.Instance.Get(firePoint.position, Quaternion.identity);
        b.GetComponent<Bullet>().Init(dir, spd);
    }
}