using UnityEngine;
using UnityEngine.UIElements;

public class VineRoot : MonoBehaviour
{
    public static VineRoot instance;
    private BoxCollider2D box;

    private void Awake() {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start() {
         box = GetComponentInParent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExtendColliderDown(float amount) {
        // 当前 size
        Vector2 size = box.size;

        // 增加高度
        size.y += amount;
        box.size = size;

        // ⚠️ 关键：offset 要往下移一半
        Vector2 offset = box.offset;
        offset.y -= amount / 2f;
        box.offset = offset;
    }
}
