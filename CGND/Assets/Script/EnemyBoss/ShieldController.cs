using UnityEngine;

public enum ShieldElement { Blue, Green, Yellow }

public class ShieldController : MonoBehaviour
{
    private SpriteRenderer theSR;
    public Sprite blueSprite;
    public Sprite greenSprite;
    public Sprite yellowSprite;

    public ShieldElement currentElement { get; private set; }
    [SerializeField]private int hitCount = 0;
    private int maxHit = 50;
    private float switchTimer = 0f;
    private float switchInterval = 5f;

    private System.Action onShieldBroken;  // 盾破了通知 ShieldState

    private void Start() {
        theSR = GetComponent<SpriteRenderer>();    
    }

    void Update() {
        switchTimer += Time.deltaTime;
        if (switchTimer >= switchInterval) {
            switchTimer = 0f;
            SwitchElement();
        }
    }

    public void shieldActivate() {
        hitCount = 0;
        switchTimer = 0f;
        SwitchElement();
        gameObject.SetActive(true);
    }

    private void SwitchElement() {
        // 随机选一个和当前不同的元素
        ShieldElement next;
        do {
            next = (ShieldElement)Random.Range(0, 3);
        } while (next == currentElement);

        currentElement = next;

        theSR.sprite = currentElement switch {
            ShieldElement.Blue => blueSprite,
            ShieldElement.Green => greenSprite,
            ShieldElement.Yellow => yellowSprite,
            _ => blueSprite
        };

        Debug.Log($"盾换成：{currentElement}");
    }

    public void TakeHit(ShieldElement attackElement) {
        // 无效元素攻击，直接返回
        if (attackElement == currentElement) {
            Debug.Log("无效攻击！");
            return;
        }

        hitCount++;
        Debug.Log($"盾受击 {hitCount}/{maxHit}");

        if (hitCount >= maxHit) {
            onShieldBroken?.Invoke();
        }
    }
}
