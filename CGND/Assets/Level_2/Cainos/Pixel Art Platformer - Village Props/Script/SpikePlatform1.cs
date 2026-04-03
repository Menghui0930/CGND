using NUnit.Framework;
using UnityEngine;

public class SpikePlatform1 : MonoBehaviour
{
    public GameObject spikes;         
    public float waitTime = 3f;      

    private float timer = 0f;
    private bool playerOnPlatform = false;

    //flashing effect after player stand on the platform for 3s
    public float flashSpeed = 5f;
    private SpriteRenderer sr;
    private bool flashing = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }


    void Update()
    {

        if (flashing)
        {
            float flash = Mathf.PingPong(Time.time * flashSpeed, 1f);
            sr.color = Color.Lerp(Color.white, Color.red, flash);
        }
        else
        {
            sr.color = Color.white;
        }

        if (playerOnPlatform)
        {
            timer += Time.deltaTime;

            if (timer >= waitTime)
            {
                spikes.SetActive(true);   // show spikes
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerOnPlatform = true;
        }
    }

    public void StartFlash() { flashing = true; }
    public void StopFlash() { flashing = false; }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerOnPlatform = false;
            timer = 0f;                  
            spikes.SetActive(false);      
        }
    }
}

