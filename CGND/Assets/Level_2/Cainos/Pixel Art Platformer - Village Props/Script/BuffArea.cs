using UnityEngine;

public class BuffArea : MonoBehaviour
{
    public float bonusSpeed = 5f;
    public float buffDuration = 4f;

    private float timer = 0f;
    private bool buffActive = false;
    private PlayerDash pd;

    void Update()
    {
        if (buffActive)
        {
            timer += Time.deltaTime;

            if (timer >= buffDuration)
            {
                pd.dashingDis -= bonusSpeed;    
                buffActive = false;
                timer = 0f;
                Debug.Log("Buff End");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pd = collision.GetComponent<PlayerDash>();
            if (pd != null)
            {
                if (!buffActive) {
                    pd.dashingDis += bonusSpeed;
                    buffActive = true;
                    timer = 0f;
                    Debug.Log("Dash Speed Up");
                }
            }
        }
    }
}