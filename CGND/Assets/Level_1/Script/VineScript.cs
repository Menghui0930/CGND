using UnityEngine;

public class VineScript : MonoBehaviour
{
    public Sprite shortVine;    
    public Sprite longVine;   
    public bool isGrown = false;

    private SpriteRenderer sr;
    private BoxCollider2D col;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
        sr.sprite = shortVine;
    }

    public void Grow()
    {
        if (!isGrown)
        {
            isGrown = true;
            sr.sprite = longVine;
            col.size = new Vector2(col.size.x, col.size.y * 3f);
            col.offset = new Vector2(col.offset.x, col.offset.y - col.size.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("WaterBall"))
        {
            Grow();
        }
    }
}
