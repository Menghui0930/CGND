using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    //[Header("References")]
    //public BossScript bossScript;

    [Header("Boss Level")]
    public int bossLevel = 1;

    private bool triggered = false;

    //public void ResetTrigger()
    //{
    //    triggered = false;
    //    //bossScript.enabled = false;
    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !triggered)
        {
            triggered = true;

            switch (bossLevel)
            {
                case 1:
                    AudioManager.Instance.PlayBGM(
                        AudioManager.Instance.boss1BGM);
                    break;
                case 2:
                    AudioManager.Instance.PlayBGM(
                        AudioManager.Instance.boss2BGM);
                    break;
                case 3:
                    AudioManager.Instance.PlayBGM(
                        AudioManager.Instance.boss3BGM);
                    break;
            }

            //bossScript.enabled = true;
        }
    }
}