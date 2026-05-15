using System;
using UnityEngine;

public class BossRoomCamera : MonoBehaviour
{
    public float normalSize = 6f;     
    public float bossSize = 8f;        // houw mch can zoom out 
    public float zoomSpeed = 2f;       

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

    internal static void SetActive(bool v)
    {
        throw new NotImplementedException();
    }
}