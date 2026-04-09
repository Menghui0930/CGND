using UnityEngine;

public class MimicFlowerAI : MonoBehaviour
{
    private Animator anim;
    private bool isRevealed = false;

    public GameObject attackArea; 
    public float revealDistance = 2.5f;

    public int damageValue = 10;        
    public float attackRate = 1.0f;     
    private float nextAttackTime;

    private Transform player;

    void Awake()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (attackArea != null) attackArea.SetActive(false);
    }

    void Update()
    {
        if (isRevealed) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist < revealDistance)
        {
            Reveal();
        }
        else
        {
            CheckAndDamagePlayer();
        }

        void Reveal()
        {
            isRevealed = true;
            anim.SetTrigger("Reveal");


            Invoke("EnableAttack", 0.3f);
        }

        void EnableAttack()
        {
            if (attackArea != null) attackArea.SetActive(true);
        }
        void CheckAndDamagePlayer()
        {
            if (attackArea != null && attackArea.activeSelf && Time.time >= nextAttackTime)
            {

                Collider2D hitPlayer = Physics2D.OverlapBox(attackArea.transform.position, new Vector2(1.5f, 1.5f), 0, LayerMask.GetMask("Default"));

                if (hitPlayer != null && hitPlayer.CompareTag("Player"))
                {
                    Debug.Log("【伤害】花咬到了玩家！扣除 " + damageValue + " 点血");

                    nextAttackTime = Time.time + attackRate;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackArea != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackArea.transform.position, new Vector3(1.5f, 1.5f, 0));
        }
    }
}