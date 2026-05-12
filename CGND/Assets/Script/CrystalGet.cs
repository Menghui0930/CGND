using System.Collections;
using UnityEngine;

public class CrystalGet : MonoBehaviour
{
    private Animator anim;
    private GameObject currentPlayer;

    [SerializeField] private GameObject crystalParent;   // Parent GameObject
    [SerializeField] private GameObject[] crystals;      // 两颗水晶子物件
    [SerializeField] private float flySpeed = 5f;

    private bool _playerInRange = false;
    private bool _isFlying = false;

    private void Start() {
        anim = GetComponent<Animator>();
    }
    public void PlayAnimationCrystal() {
        anim.Play("GetCrystal");
        anim.SetBool("IsCrystalStay",true);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player") && !_isFlying) {
            currentPlayer = collision.gameObject;
            _playerInRange = true;
            StartCoroutine(GetCrystal());
        }
    }

    private IEnumerator GetCrystal() {
        _isFlying = true;

        // 停止 Animator 避免继续影响子物件位置
        anim.enabled = false;

        // 把两颗水晶脱离 Parent，保留世界坐标
        foreach (GameObject crystal in crystals) {
            Vector3 worldPos = crystal.transform.position;
            crystal.transform.SetParent(null);
            crystal.transform.position = worldPos;
        }

        Transform playerTransform = currentPlayer.transform;

        // 两颗同时飞向玩家
        bool[] arrived = new bool[crystals.Length];
        while (!System.Array.TrueForAll(arrived, a => a)) {
            for (int i = 0; i < crystals.Length; i++) {
                if (arrived[i]) continue;
                crystals[i].transform.position = Vector3.MoveTowards(
                    crystals[i].transform.position,
                    playerTransform.position,
                    flySpeed * Time.deltaTime
                );
                if (Vector3.Distance(crystals[i].transform.position, playerTransform.position) < 0.2f)
                    arrived[i] = true;
            }
            yield return null;
        }

        // 全部到达后消失
        foreach (GameObject crystal in crystals)
            Destroy(crystal);
        Destroy(crystalParent);
        Destroy(gameObject);
    }
}
