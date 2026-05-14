using Unity.VisualScripting;
using UnityEngine;

public class BossHealth : EnemyHealth {
    private Animator _anim;
    public BoxCollider2D _blockPlayer;
    public BoxCollider2D _blockPlayerBehind;
    private bool isStart;

    [Header("Camera Offset Override")]
    [SerializeField] private float horizontalOffset = -3f;
    [SerializeField] private float verticalOffset = 0f;
    [SerializeField] private float transitionSpeed = 2f;  // 过渡速度
    [SerializeField] private float MinY = 0.85f;  // 过渡速度
    [SerializeField] private bool isStopfollowing = false;

    [SerializeField] private CrystalGet crystalGet;

    protected override void Start() {
        base.Start();
        _anim = GetComponent<Animator>();
    }

    public override void TakeDamage(int damage) {
        if (transform.gameObject.GetComponent<BossScript>().isStart == !true) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if(currentHealth <= 0) {
            Die();
            return;   
        }

        _anim.SetTrigger("Hurt");
    }


    protected override void Die() {
        _anim.ResetTrigger("Hurt");
        _anim.SetTrigger("Death");
        Camera2D.instance.stopFollow = false;
        Camera2D.instance.horizontalFollow = true;
        Camera2D.instance.SetOffsets(horizontalOffset, verticalOffset, transitionSpeed, MinY, isStopfollowing);
        _blockPlayer.gameObject.SetActive(false);
        _blockPlayerBehind.gameObject.SetActive(false);
        GetComponent<BossScript>().enabled = false;
        //GetComponent<BossAudio>().PlayLevelBGM();

        if (crystalGet != null)
            crystalGet.PlayAnimationCrystal();   // ← 触发水晶动画
        crystalGet._bossIsDeath = true;

        Destroy(gameObject, 2f);
    }

    public void ResetHealth() {
        currentHealth = maxHealth;
        //_blockPlayer.gameObject.SetActive(false);
    }
}