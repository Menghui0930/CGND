using UnityEngine;

public class SpikeAppear : MonoBehaviour
{
    public float popSpeed = 5f;
    private bool show = false;
    public int damage = 0;
    private Collider2D col;

   

    void Start()
    {

        transform.localScale = Vector3.zero;
        col = GetComponent<Collider2D>();
        col.enabled = false;

       
    }

    void Update()
    {

        if (show)
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, popSpeed * Time.deltaTime);
        else
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, popSpeed * Time.deltaTime);

    }



    public void ShowSpikes()
    {
        show = true;
        col.enabled = true;
    }

    public void HideSpikes()
    {
        show = false;
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            
            Debug.Log("Player Hit");
            damage++;
        }
    }
}