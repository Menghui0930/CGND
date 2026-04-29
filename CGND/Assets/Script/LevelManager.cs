using System;
using System.Collections;
using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public static event Action<PlayerMotor> OnPlayerSpawn;

    public InputAction Revive;

    [Header("Settings")]
    [SerializeField] private Transform levelStartPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float waitToRespawn;
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
        SpawnPlayer(playerPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (Revive.WasPressedThisFrame()) {
            RevivePlayer();
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
            StartCoroutine(RespawnCo());
        } else {
            Debug.Log("PlayerDeath no player");
        }
    }

    private void RevivePlayer() {
        if (player != null) {
            player.gameObject.SetActive(true);
            currentPlayer.SpawnPlayer(spawnPoint);
            player.GetComponent<Health>().ResetLife();
            //player.GetComponent<Health>().Revive();
        }
    }

    private IEnumerator RespawnCo() {
        yield return new WaitForSeconds(1f);
        WipeController.instance.FadeOut();
        yield return new WaitForSeconds(1f);
        WipeController.instance.FadeIn();
        RevivePlayer();
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


    private void OnEnable() {
        Health.OnDeath += PlayerDeath;
    }

    private void OnDisable() {
        Health.OnDeath -= PlayerDeath;
    }
}
