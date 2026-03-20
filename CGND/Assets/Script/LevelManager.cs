using Unity.Multiplayer.PlayMode;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform levelStartPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float waitToRespawn;
    private Vector3 spawnPoint;

    private GameObject player;
    private PlayerMotor currentPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = levelStartPoint.position;
        SpawnPlayer(playerPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnPlayer(GameObject sPlayer) {
        if (sPlayer != null) {
            player = Instantiate(sPlayer, spawnPoint, Quaternion.identity);
            currentPlayer = player.GetComponentInChildren<PlayerMotor>();
            currentPlayer.GetComponent<Health>().ResetLife();

            // Call Event
            //OnPlayerSpawn?.Invoke(_currentPlayer);
        }
    }
}
