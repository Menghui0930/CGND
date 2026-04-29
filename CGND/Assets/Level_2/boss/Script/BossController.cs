using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossPhase { Idle, Phase1, Phase2, Enraged, Dead }

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

  
    private bool patternRunning = false;

    private List<System.Func<IEnumerator>> patterns;

    void Start()
    {
        currentHP = maxHP;
        currentPhase = BossPhase.Phase1;

        patterns = new List<System.Func<IEnumerator>>
        {
            PatternSpiral,
            PatternRadialBurst,
            PatternAimedStream,
            PatternCross,
            PatternRingExpand
        };

        StartCoroutine(BossRoutine());
    }

  

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
    }



    IEnumerator BossRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        while (currentPhase != BossPhase.Dead)
        {
            // choose random patternf
            var pattern = patterns[Random.Range(0, patterns.Count)];
            float duration = Random.Range(minPatternDuration, maxPatternDuration);

            Debug.Log($"[Boss] Starting pattern: {pattern.Method.Name} for {duration:F1}s");

            // run pattern
            patternRunning = true;
            StartCoroutine(pattern());           
            yield return new WaitForSeconds(duration); 
            patternRunning = false;             

           
            yield return new WaitForSeconds(0.1f);

          
            yield return new WaitForSeconds(patternCooldown);
        }

        Debug.Log("[Boss] Dead.");
    }

    // Some paatern
   

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

 

    public void TakeDamage(float amount)
    {
        if (currentPhase == BossPhase.Dead) return;
        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        CheckPhase();
    }
}