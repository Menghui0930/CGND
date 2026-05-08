using UnityEngine;

public class BossRoom : MonoBehaviour
{
    [Header("References")]
    public BossController boss;

    [Header("Blockers")]
    public Collider2D entryBlocker;    // left entrance
    public Collider2D exitBlocker;     // right exit

    private bool bossDefeated = false;

    void Start()
    {
        entryBlocker.enabled = false;
        exitBlocker.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !bossDefeated)
        {
            exitBlocker.enabled = true;         // exit blocks immediately
            StartCoroutine(DelayEntryBlock());  // entry blocks after delay
            boss.ActivateBoss();
            Debug.Log("Both doors locked!");
        }
    }

    System.Collections.IEnumerator DelayEntryBlock()
    {
        yield return new WaitForSeconds(1f);   
        entryBlocker.enabled = true;            // now block entry
        Debug.Log("Entry blocked!");
    }

    public void UnlockRoom()
    {
        bossDefeated = true;
        entryBlocker.enabled = false;   // open entry
        exitBlocker.enabled = false;    // open exit
        Debug.Log("Boss defeated! Both doors unlocked!");
    }
}