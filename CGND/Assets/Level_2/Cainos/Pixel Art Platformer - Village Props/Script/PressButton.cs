using UnityEngine;

public class PressButton : MonoBehaviour
{
    public DoorController door;
    public Color pressedColor = Color.blue;

    private bool isPressed = false;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "WaterBall" && !isPressed)
        {
            isPressed = true;
            sr.color = pressedColor;
            door.ButtonPressed();              
            Debug.Log("Button pressed!");
        }
    }
}
