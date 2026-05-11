using UnityEngine;

public class DebugSpawnPoint : MonoBehaviour
{
    [SerializeField] public Transform bossRoomSpawn;

    void Awake()
    {
#if UNITY_EDITOR
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            player.transform.position = bossRoomSpawn.position;
#endif
    }
}