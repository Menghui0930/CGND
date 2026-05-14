using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    public static Action<int> OnLifesChanged;
    public static Action<int> OnMaxLifeChanged;
    public static Action<PlayerMotor> OnDeath;
    public InputAction Damage;
    public InputAction Cheat;

    [Header("Settings")]
    [SerializeField] private SpriteRenderer[] allSR;
    [SerializeField] private int lifes = 3;
    [SerializeField] private float invincibilityDuration = 1.0f; 
    [SerializeField] private float blinkInterval = 0.2f;

    private int _maxLifes;
    private int _currentLifes;
    //private string status;

    private bool invincible = false;
    private float invincibilityTimer = 0f;

    // shield
    private bool _isImmune = false;
    private bool _cheatMode = false;   // ← 新增

    private void Awake() {
        Damage = InputSystem.actions.FindAction("Damage");
        Cheat = InputSystem.actions.FindAction("Cheat");
        //theSR = GetComponent<SpriteRenderer>();
        allSR = GetComponentsInChildren<SpriteRenderer>();
        _maxLifes = lifes;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetLife();
    }

    // Update is called once per frame
    void Update()
    {
        if (Damage.WasPressedThisFrame()) {
            Debug.Log("Damage");
            LoseLife();
        }

        // 按 O 切换作弊模式
        if (Cheat.WasPressedThisFrame()) {
            _cheatMode = !_cheatMode;
            _isImmune = _cheatMode;
            Debug.Log(_cheatMode ? "Cheat ON：无敌模式" : "Cheat OFF：恢复正常");
        }

    }

    public void LoseLife() {
        if (invincible) return;
        if (_isImmune) return;

        _currentLifes -= 1;
        if (_currentLifes <= 0) {
            _currentLifes = 0;
            //Death
            UpdateLifesUI();
            Camera2D.instance.stopFollow = false;
            Camera2D.instance.verticalFollow = true;
            OnDeath?.Invoke(gameObject.GetComponent<PlayerMotor>());
            return;
        }
        //status = "Hurt";
        UpdateLifesUI();

        invincible = true;
        invincibilityTimer = invincibilityDuration;
        StartCoroutine(Invincibility());
    }

    public void AddLife() {
        _currentLifes += 1;
        if(_currentLifes > _maxLifes) {
            _currentLifes = _maxLifes;
        }
        //status = "Heal";
        UpdateLifesUI();
    }

    public void ResetLife() {
        _currentLifes = _maxLifes;
        invincible = false;
        StopAllCoroutines();
        foreach (SpriteRenderer sr in allSR) {
            sr.color = new Color(1, 1, 1, 1);
        }

        UpdateLifesUI();
        //status = "Heal";
    }

    private void UpdateLifesUI() {
        // UIManager
        OnLifesChanged?.Invoke(_currentLifes);
    }

    private IEnumerator Invincibility() {
        invincible = true;
        StartCoroutine(BlinkEffect());
        yield return new WaitForSeconds(invincibilityDuration);
        invincible = false;
        //theSR.color = new Color(1, 1, 1, 1);
    }

    private IEnumerator BlinkEffect() {
        while (invincible) {
            for (float t = 0; t < blinkInterval; t += Time.deltaTime) {
                float alpha = Mathf.Lerp(1f, 0.3f, t / blinkInterval);
                foreach(SpriteRenderer sr in allSR) {
                    sr.color = new Color(1, 1, 1, alpha);
                }
                yield return null;
            }
        }

        foreach (SpriteRenderer sr in allSR) {
            sr.color = new Color(1, 1, 1, 1);
        }
    }

    // Shield
    public void SetImmune(bool immune) {
        // 作弊模式中不让盾覆盖状态，退出作弊后盾才能正常关闭
        _isImmune = immune || _cheatMode;
    }

    public void AddMaxLife(int amount) {
        _maxLifes += amount;
        _currentLifes += amount;
        OnMaxLifeChanged?.Invoke(_maxLifes); 
        UpdateLifesUI();
    }
}
