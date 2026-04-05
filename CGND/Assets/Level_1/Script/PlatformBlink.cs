using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformBlink : MonoBehaviour
{
    [Header("Settings")]
    public float visibleTime = 5f;    
    public float invisibleTime = 2f;

    private TilemapRenderer tr;      
    private TilemapCollider2D col;
    private float timer;
    private bool isVisible = true;

    void Start()
    {
        tr = GetComponent<TilemapRenderer>();
        col = GetComponent<TilemapCollider2D>();
        timer = visibleTime;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            if (isVisible)
            {
                isVisible = false;
                tr.enabled = false;
                col.enabled = false;
                timer = invisibleTime;
            }
            else
            {
                isVisible = true;
                tr.enabled = true;
                col.enabled = true;
                timer = visibleTime;
            }
        }
    }
}
