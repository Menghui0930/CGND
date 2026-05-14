using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D

public enum ShieldElement { Blue, Green, Yellow }

public class ShieldController : MonoBehaviour {
    private SpriteRenderer theSR;

    [Header("Shield Sprites")]
    public Sprite blueSprite;
    public Sprite greenSprite;
    public Sprite yellowSprite;

    [Header("Shield Effect Prefabs")]
    public GameObject blueEffectPrefab;
    public GameObject greenEffectPrefab;
    public GameObject yellowEffectPrefab;

    [Tooltip("Effect 会跟随的 Boss Transform（不填则跟随自身）")]
    public Transform bossTransform;

    [Header("Shield Light2D")]
    [Tooltip("Shield 本体上的 Light2D")]
    public Light2D shieldLight;

    [Header("Shield Background")]
    [Tooltip("Shield 的背景 GameObject（自身 SpriteRenderer 换色 + children Light2D 换色）")]
    public GameObject shieldBackground;

    // ── 颜色表 ──────────────────────────────────────────────────────────────

    // Shield 本体 Light2D
    private static readonly Color LightBlue = HexColor("0053FF");
    private static readonly Color LightGreen = HexColor("FFE903");
    private static readonly Color LightYellow = HexColor("00D706");

    // Background SpriteRenderer 自身颜色
    private static readonly Color BgSprBlue = HexColor("007EDD");
    private static readonly Color BgSprGreen = HexColor("CBD300");
    private static readonly Color BgSprYellow = HexColor("3AA700");

    // Background children Light2D 颜色
    private static readonly Color BgLightBlue = HexColor("00A4FF");
    private static readonly Color BgLightGreen = HexColor("F4FF09");
    private static readonly Color BgLightYellow = HexColor("3AA700");

    // ── 内部状态 ────────────────────────────────────────────────────────────

    public ShieldElement currentElement { get; private set; }

    [SerializeField] private int hitCount = 0;
    private int maxHit = 50;
    private float switchTimer = 0f;
    private float switchInterval = 5f;

    private GameObject _blueEffect;
    private GameObject _greenEffect;
    private GameObject _yellowEffect;

    private SpriteRenderer _bgSR;    // shieldBackground 自身的 SpriteRenderer
    private Light2D _bgLight; // shieldBackground children 的 Light2D

    // ── Unity ───────────────────────────────────────────────────────────────

    private void Start() {
        theSR = GetComponent<SpriteRenderer>();

        if (shieldBackground != null) {
            _bgSR = shieldBackground.GetComponent<SpriteRenderer>();
            _bgLight = shieldBackground.GetComponentInChildren<Light2D>();
        }

        SpawnEffects();

        // 默认全部隐藏
        if (shieldLight != null) shieldLight.gameObject.SetActive(false);
        if (shieldBackground != null) shieldBackground.SetActive(false);
    }

    private void Update() {
        switchTimer += Time.deltaTime;
        if (switchTimer >= switchInterval) {
            switchTimer = 0f;
            SwitchElement();
        }

        FollowBoss();
    }

    // ── 公开接口 ────────────────────────────────────────────────────────────

    public void shieldActivate() {
        hitCount = 0;
        switchTimer = 0f;

        if (shieldLight != null) shieldLight.gameObject.SetActive(true);
        if (shieldBackground != null) shieldBackground.SetActive(true);

        SwitchElement();
        gameObject.SetActive(true);
    }

    public void TakeHit(ShieldElement attackElement) {
        if (attackElement == currentElement) {
            Debug.Log("无效攻击！");
            return;
        }

        hitCount++;
        Debug.Log($"盾受击 {hitCount}/{maxHit}");

        if (hitCount >= maxHit)
            BreakShield();
    }

    // ── 盾破 ────────────────────────────────────────────────────────────────

    private void BreakShield() {
        HideAllEffects();

        if (shieldLight != null) shieldLight.gameObject.SetActive(false);
        if (shieldBackground != null) shieldBackground.SetActive(false);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Boss Reset 时调用：关掉盾、光、背景、所有 Effect，重置计数。
    /// </summary>
    public void ResetShield() {
        hitCount = 0;
        switchTimer = 0f;

        HideAllEffects();

        if (shieldLight != null) shieldLight.gameObject.SetActive(false);
        if (shieldBackground != null) shieldBackground.SetActive(false);

        gameObject.SetActive(false);
    }

    // ── 元素切换 ────────────────────────────────────────────────────────────

    private void SwitchElement() {
        ShieldElement next;
        do {
            next = (ShieldElement)UnityEngine.Random.Range(0, 3);
        } while (next == currentElement);

        currentElement = next;

        theSR.sprite = currentElement switch {
            ShieldElement.Blue => blueSprite,
            ShieldElement.Green => greenSprite,
            ShieldElement.Yellow => yellowSprite,
            _ => blueSprite
        };

        ApplyColors(currentElement);

        if (gameObject.activeSelf)
            ShowCurrentEffect();

        Debug.Log($"盾换成：{currentElement}");
    }

    private void ApplyColors(ShieldElement element) {
        (Color lightCol, Color bgSprCol, Color bgLightCol) = element switch {
            ShieldElement.Blue => (LightBlue, BgSprBlue, BgLightBlue),
            ShieldElement.Green => (LightGreen, BgSprGreen, BgLightGreen),
            ShieldElement.Yellow => (LightYellow, BgSprYellow, BgLightYellow),
            _ => (LightBlue, BgSprBlue, BgLightBlue)
        };

        if (shieldLight != null) shieldLight.color = lightCol;
        if (_bgSR != null) _bgSR.color = bgSprCol;
        if (_bgLight != null) _bgLight.color = bgLightCol;
    }

    // ── Effect 管理 ─────────────────────────────────────────────────────────

    private void SpawnEffects() {
        Transform follow = bossTransform != null ? bossTransform : transform;

        if (blueEffectPrefab != null) _blueEffect = Spawn(blueEffectPrefab, follow.position);
        if (greenEffectPrefab != null) _greenEffect = Spawn(greenEffectPrefab, follow.position);
        if (yellowEffectPrefab != null) _yellowEffect = Spawn(yellowEffectPrefab, follow.position);

        HideAllEffects();
    }

    private GameObject Spawn(GameObject prefab, Vector3 pos) {
        var go = Instantiate(prefab, pos, Quaternion.identity);
        go.SetActive(false);
        return go;
    }

    private void ShowCurrentEffect() {
        HideAllEffects();

        GameObject active = currentElement switch {
            ShieldElement.Blue => _blueEffect,
            ShieldElement.Green => _greenEffect,
            ShieldElement.Yellow => _yellowEffect,
            _ => null
        };

        if (active != null) active.SetActive(true);
    }

    private void HideAllEffects() {
        if (_blueEffect != null) _blueEffect.SetActive(false);
        if (_greenEffect != null) _greenEffect.SetActive(false);
        if (_yellowEffect != null) _yellowEffect.SetActive(false);
    }

    private void FollowBoss() {
        if (bossTransform == null) return;

        Vector3 pos = bossTransform.position;
        if (_blueEffect != null) _blueEffect.transform.position = pos;
        if (_greenEffect != null) _greenEffect.transform.position = pos;
        if (_yellowEffect != null) _yellowEffect.transform.position = pos;
    }

    // ── 工具 ────────────────────────────────────────────────────────────────

    private static Color HexColor(string hex) {
        ColorUtility.TryParseHtmlString("#" + hex, out Color c);
        return c;
    }

    private void OnDestroy() {
        if (_blueEffect != null) Destroy(_blueEffect);
        if (_greenEffect != null) Destroy(_greenEffect);
        if (_yellowEffect != null) Destroy(_yellowEffect);
    }
}