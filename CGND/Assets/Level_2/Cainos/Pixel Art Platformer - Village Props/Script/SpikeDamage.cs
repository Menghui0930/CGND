using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    public int damage = 0;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            Debug.Log("Player Hit");
            damage++;
        }
    }
}
