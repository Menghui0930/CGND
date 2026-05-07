using UnityEngine;

public class BossRoomCamera : MonoBehaviour
{
    public float normalSize = 6f;      // normal camera size
    public float bossSize = 8f;        // zoomed out size for boss room
    public float zoomSpeed = 2f;       // how fast it zooms

    private Camera cam;
    private bool playerInRoom = false;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (playerInRoom)
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, bossSize, zoomSpeed * Time.deltaTime);
        else
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, normalSize, zoomSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRoom = true;
            Debug.Log("Entered boss room!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerInRoom = false;
            Debug.Log("Left boss room");
        }
    }
}