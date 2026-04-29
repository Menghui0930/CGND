using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    public static Action<int> OnLifesChanged;
    public static Action<PlayerMotor> OnDeath;
    public InputAction Damage;

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

    private void Awake() {
        Damage = InputSystem.actions.FindAction("Damage");
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

    }

    public void LoseLife() {
        if (invincible) return;

        _currentLifes -= 1;
        if (_currentLifes <= 0) {
            _currentLifes = 0;
            //Death
            UpdateLifesUI();
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
}
