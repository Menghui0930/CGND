using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlowerBuff : MonoBehaviour
{
    private InputAction _interact;

    public enum FlowerType {Grass, Water, Wind}

    [Header("Settings")]
    [SerializeField] private FlowerType flowerType;
    [SerializeField] private float windSpeedBonus = 2f;
    [SerializeField] private float windDuration = 5f;

    private bool _playerInRange = false;
    private PlayerMovement _playerMovement;
    private Health _health;
    private MagicPoint _magicPoint;

    private void Awake() {
        _interact = InputSystem.actions.FindAction("Interact");
    }


    private void Update() {
        if (_playerInRange && _interact.WasPressedThisFrame())
            ApplyBuff();
    }

    private void ApplyBuff() {
        switch (flowerType) {
            case FlowerType.Grass:
                _health.AddLife();                              // 回复1命
                break;
            case FlowerType.Water:
                _magicPoint.IncreaseMP();                      // 回复1MP
                break;
            case FlowerType.Wind:
                StartCoroutine(WindBuff());                    // 短暂加速
                break;
        }
        Destroy(gameObject);                                   // 拾取后消失
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            _playerInRange = true;
            _health = other.GetComponentInParent<Health>();
            _magicPoint = MagicPoint.Instance;
            _playerMovement = other.GetComponentInParent<PlayerMotor>()
                                  .GetComponent<PlayerMovement>();   // ← 从 PlayerMotor 上找
        }
    }

    private IEnumerator WindBuff() {
        _playerMovement._speedBonus = windSpeedBonus;
        yield return new WaitForSeconds(windDuration);
        _playerMovement._speedBonus = 0f;
        
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player"))
            _playerInRange = false;
    }

}
