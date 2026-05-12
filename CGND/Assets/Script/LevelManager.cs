using System;
using System.Collections;
using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;   
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public static event Action<PlayerMotor> OnPlayerSpawn;

    // Level_1 Only
    public static event Action OnGameStart;

    public InputAction Revive;

    [Header("Settings")]
    [SerializeField] private Transform levelStartPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float waitToRespawn;

    [Header("Opening Cutscene")]
    [SerializeField] private PlayableDirector openingTimeline; 
    [SerializeField] private GameObject dummyCharacter;         
    [SerializeField] private Dialogue openingDialogue;
    [SerializeField] private Animator healthUIAnimator;
    [SerializeField] private GameObject crystalParent;      // ← Parent（有 Animator）
    [SerializeField] private GameObject[] crystals;         // ← 两颗水晶子物件
    [SerializeField] private float crystalFlySpeed = 5f;   // ← 飞行速度
    [SerializeField] private SkillTreeManager SkillTreeManager;   // ← 飞行速度

    [Header("level1_Only")]
    [SerializeField] private GameObject SkillTreeTutorialPanel;
    [SerializeField] private GameObject MapName;

    

    private Vector3 spawnPoint;

    private GameObject player;
    private PlayerMotor currentPlayer;

    public GameObject CurrentPlayer => player;

    private void Awake() {
        Instance = this;
        Revive = InputSystem.actions.FindAction("Revive");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = levelStartPoint.position;

        // Level1_only
        if (SkillTreeTutorialPanel != null) {
            SkillTreeTutorialPanel.SetActive(false);
        }

        if (openingTimeline != null) {
            // 有 Timeline → 不立刻生成玩家，等 Timeline 结束
            openingTimeline.stopped += OnTimelineFinished;
            openingTimeline.Play();
            if (MapName != null) {
                MapName.GetComponent<Animator>().Play("MapName_IN");
            }
        } else {
            // 没有 Timeline → 直接生成玩家
            SpawnPlayer(playerPrefab);
        }
    }

    // Timeline 结束时自动调用
    private void OnTimelineFinished(PlayableDirector director) {
        openingTimeline.stopped -= OnTimelineFinished;
        DialogueManager.instance.StartDialogue(openingDialogue);
        StartCoroutine(WaitForDialogueToEnd());
    }

    private IEnumerator WaitForDialogueToEnd() {
        // 等 Dialogue 结束
        yield return new WaitUntil(() => !DialogueManager.instance.isDialogueActive);

        // ── 新加：播水晶飞起动画 ──
        if (crystalParent != null)
            yield return StartCoroutine(CrystalSequence());

        // 记录假角色的位置，生成真玩家在同样位置
        spawnPoint = dummyCharacter.transform.position;
        dummyCharacter.SetActive(false);
        SpawnPlayer(playerPrefab);

        // 相机平滑移动（不 snap）
        if (currentPlayer != null)
            Camera2D.instance.SetTargetSmooth(currentPlayer);

        // 触发生命值 UI 动画
        HealthUIFadeIn();



        // Level01_only
        if (SkillTreeTutorialPanel != null) {
            SkillTreeTutorialPanel.SetActive(true);
        } else {
            // Level 3：没有 Tutorial，直接开放控制
            currentPlayer.EnableControl();
        }

        OnGameStart?.Invoke();
    }

    private IEnumerator CrystalSequence() {
        // Step 1：播 Parent 的飞起动画
        Animator crystalAnim = crystalParent.GetComponent<Animator>();
        crystalAnim.Play("GetCrystal");

        yield return null;
        float clipLength = crystalAnim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(clipLength);

        // Step 2：把两颗水晶从 Parent 脱离，各自飞向玩家
        foreach (GameObject crystal in crystals) {
            Vector3 worldPos = crystal.transform.position;  // ← 先记录世界坐标
            crystal.transform.SetParent(null);
            crystal.transform.position = worldPos;           // ← 脱离后还原
        }

        // Step 2：把两颗水晶从 Parent 脱离，各自飞向玩家
        foreach (GameObject crystal in crystals)
            crystal.transform.SetParent(null);  // 脱离 Parent，独立移动

        crystalAnim.enabled = false;   // ← 停止 Animator 继续影响

        foreach (GameObject crystal in crystals) {
            Vector3 worldPos = crystal.transform.position;
            crystal.transform.SetParent(null);
            crystal.transform.position = worldPos;
        }

        Transform playerTransform = dummyCharacter.transform;

        // 两颗同时飞
        bool[] arrived = new bool[crystals.Length];
        while (!System.Array.TrueForAll(arrived, a => a)) {
            for (int i = 0; i < crystals.Length; i++) {
                if (arrived[i]) continue;
                crystals[i].transform.position = Vector3.MoveTowards(
                    crystals[i].transform.position,
                    playerTransform.position,
                    crystalFlySpeed * Time.deltaTime
                );
                if (Vector3.Distance(crystals[i].transform.position, playerTransform.position) < 0.2f)
                    arrived[i] = true;
            }
            yield return null;
        }

        // Step 3：全部到达后消失
        foreach (GameObject crystal in crystals)
            Destroy(crystal);
        Destroy(crystalParent);

        SkillTreeManager.GetCrystal(2);
        SkillTreeManager.UpdateSkillPointsUI();

        yield return new WaitForSeconds(1.5f);
    }












    // Update is called once per frame
    void Update()
    {
        if (Revive.WasPressedThisFrame()) {
            RevivePlayer(false);
        }
    }

    private void SpawnPlayer(GameObject sPlayer) {
        if (sPlayer != null) {
            player = Instantiate(sPlayer, spawnPoint, Quaternion.identity);
            currentPlayer = player.GetComponentInChildren<PlayerMotor>();
            player.GetComponent<Health>().ResetLife();

            // Call Event
            OnPlayerSpawn?.Invoke(currentPlayer);
        }
    }

    public void SetSpawnPoint(Vector3 newSpawnPoint) {
        Debug.Log("Change SpawnPoint");
        spawnPoint = newSpawnPoint;
    }

    private void PlayerDeath(PlayerMotor playerMotor) {
        if (player != null) {
            //_currentPlayer = player;
            player.gameObject.SetActive(false);
            StartCoroutine(RespawnCo(false));
        } else {
            Debug.Log("PlayerDeath no player");
        }
    }

    private void RevivePlayer(bool MinusHealth) {
        if (player != null) {
            player.gameObject.SetActive(true);
            currentPlayer.SpawnPlayer(spawnPoint);

            if (!MinusHealth) {
                player.GetComponent<Health>().ResetLife();
            }
            //player.GetComponent<Health>().Revive();
        }
    }

    public IEnumerator RespawnCo(bool MinusHealth) {
        Debug.Log("Respawn");
        yield return new WaitForSeconds(1f);
        WipeController.instance.FadeOut();
        yield return new WaitForSeconds(1f);
        WipeController.instance.FadeIn();
        RevivePlayer(MinusHealth);
    }

    public void OnFinish() {
        StartCoroutine(FinishCo());
    }

    private IEnumerator FinishCo() {
        // Disable Player
        currentPlayer.enabled = false;

        // Character Move RIght
        PlayerController pc = player.GetComponent<PlayerController>();
        pc.SetHorizontalForce(5f);

        // Wait
        yield return new WaitForSeconds(2f);

        // UI Wipe
        WipeController.instance.FadeOut();
        yield return new WaitForSeconds(2f);

        // Next Scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void HealthUIFadeIn() {
        if (healthUIAnimator != null)
            healthUIAnimator.SetTrigger("IN");
    }

    public void HealthUIFadeOut() {
        if (healthUIAnimator != null)
            healthUIAnimator.SetTrigger("OUT");
    }


    private void OnEnable() {
        Health.OnDeath += PlayerDeath;
    }

    private void OnDisable() {
        Health.OnDeath -= PlayerDeath;
    }
}
