using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    [SerializeField] private GameObject p1;
    [SerializeField] private GameObject p2;

    private InputAction p1_point;
    private InputAction p2_point;

    private PlayerMotor currentPlayer;

    private void Awake() {
        p1_point = InputSystem.actions.FindAction("T1");
        p2_point = InputSystem.actions.FindAction("T2");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (p1_point.WasPressedThisFrame()) {
            currentPlayer.transform.position = p1.transform.position;
        }

        if (p2_point.WasPressedThisFrame()) {
            currentPlayer.transform.position = p2.transform.position;
        }
    }

    private void OnEnable() {
        LevelManager.OnPlayerSpawn += LevelManager_OnPlayerSpawn;
    }

    private void OnDisable() {
        LevelManager.OnPlayerSpawn -= LevelManager_OnPlayerSpawn;
    }

    private void LevelManager_OnPlayerSpawn(PlayerMotor obj) {
        currentPlayer = obj;
    }
}
