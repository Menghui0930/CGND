using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    [Header("Health")]
    [SerializeField] private Image[] HealthImage;
    [SerializeField] private RectTransform HealthPanel;
    [SerializeField] private float HealthPanelOffset = 10f;
    [SerializeField] private Sprite FullHealth;
    [SerializeField] private Sprite WhiteHealth;
    [SerializeField] private Sprite EmptyHealth;

    [Header("MP")]
    [SerializeField] private Image Poison;
    [SerializeField] private Image[] MP;
    [SerializeField] private Sprite GrassPoison;
    [SerializeField] private Sprite WaterPoison;
    [SerializeField] private Sprite WindPoison;
    [SerializeField] private Sprite FullMP;
    [SerializeField] private Sprite EmptyMP;
    [SerializeField] private Sprite WhiteMP;

    public Transform currentPlayer;
    private Coroutine _mpWaveCoroutine;

    private Vector2[] _mpOriginalPos;

    void Start() {
        _mpOriginalPos = new Vector2[MP.Length];
        for (int i = 0; i < MP.Length; i++) {
            _mpOriginalPos[i] = MP[i].GetComponent<RectTransform>().anchoredPosition;
        }
    }

    // Update is called once per frame
    void Update() {
        UpdatePoison();
    }

    private void OnPlayerLifes(int currentLifes) {
        StartCoroutine(HurtEffect(currentLifes));
    }

    private IEnumerator HurtEffect(int currentLifes) {

        if (currentLifes < HealthImage.Length) {
            HealthImage[currentLifes].sprite = WhiteHealth;
        }

        Vector2 originalPos = HealthPanel.anchoredPosition;
        Vector2 upPos = originalPos + new Vector2(0, HealthPanelOffset);

        float duration = 0.08f;
        float elapsed = 0f;

        // Move UP
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            HealthPanel.anchoredPosition = Vector2.Lerp(originalPos, upPos, elapsed / duration);
            yield return null;
        }

        // Move Down
        elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            HealthPanel.anchoredPosition = Vector2.Lerp(upPos, originalPos, elapsed / duration);
            yield return null;
        }

        // Move Back
        HealthPanel.anchoredPosition = originalPos; 

        for (int i = 0; i < HealthImage.Length; i++) {
            HealthImage[i].sprite = i < currentLifes ? FullHealth : EmptyHealth;
        }
    }

    private void UpdatePoison() {
        if (currentPlayer != null) {
            PlayerElementSwitch element = currentPlayer.transform.GetComponent<PlayerElementSwitch>();
            Poison.sprite = element.current_element switch {
                PlayerElementSwitch.Element.Grass => GrassPoison,
                PlayerElementSwitch.Element.Water => WaterPoison,
                PlayerElementSwitch.Element.Wind => WindPoison,
                _ => GrassPoison
            };
        }
    }

    private void OnPlayerMP(int currentMP, string status) {
        if (status == "Increase") {
            if (_mpWaveCoroutine != null) {
                StopCoroutine(_mpWaveCoroutine);
                ResetMPPosition(); // ✅ 強制還原所有位置
            }
            _mpWaveCoroutine = StartCoroutine(MPWaveEffect(currentMP));
        } else {
            for (int i = 0; i < MP.Length; i++) {
                MP[i].sprite = i < currentMP ? FullMP : EmptyMP;
            }
        }
    }

    private void ResetMPPosition() {
        foreach (Image mp in MP) {
            RectTransform rect = mp.GetComponent<RectTransform>();
            // anchoredPosition 還原成原始位置
            rect.anchoredPosition = _mpOriginalPos[System.Array.IndexOf(MP, mp)];
        }
    }

    private IEnumerator MPWaveEffect(int currentMP) {
        float waveDelay = 0.08f;   // 每粒之間的延遲
        float moveDuration = 0.12f; // 每粒上移的時間
        float moveUpAmount = 8f;    // 上移幾像素

        RectTransform[] mpRects = new RectTransform[MP.Length];
        Vector2[] originalPos = new Vector2[MP.Length];

        // ✅ 記錄所有原始位置
        for (int i = 0; i < MP.Length; i++) {
            mpRects[i] = MP[i].GetComponent<RectTransform>();
            originalPos[i] = mpRects[i].anchoredPosition;
        }

        // ✅ 先把所有該更新的 MP 變成 WhiteMP
        for (int i = 0; i < currentMP; i++) {
            MP[i].sprite = WhiteMP;
        }

        // ✅ 波浪效果：每粒依序上移，上一粒同時下移
        for (int i = 0; i < currentMP; i++) {
            int index = i; // 避免 closure 問題
            StartCoroutine(MoveUp(mpRects[index], originalPos[index], moveUpAmount, moveDuration));

            // 上一粒開始下移
            if (index > 0) {
                int prevIndex = index - 1;
                StartCoroutine(MoveDown(mpRects[prevIndex], originalPos[prevIndex], moveUpAmount, moveDuration));
                MP[prevIndex].sprite = FullMP; // ✅ 下移時變成 FullMP
            }

            yield return new WaitForSeconds(waveDelay);
        }

        // ✅ 最後一粒下移回原位
        yield return new WaitForSeconds(moveDuration);
        int lastIndex = currentMP - 1;
        if (lastIndex >= 0) {
            StartCoroutine(MoveDown(mpRects[lastIndex], originalPos[lastIndex], moveUpAmount, moveDuration));
            MP[lastIndex].sprite = FullMP;
        }

        // ✅ 更新剩餘的 Empty
        yield return new WaitForSeconds(moveDuration);
        for (int i = currentMP; i < MP.Length; i++) {
            MP[i].sprite = EmptyMP;
        }
    }

    private IEnumerator MoveUp(RectTransform rect, Vector2 origin, float amount, float duration) {
        Vector2 target = origin + new Vector2(0, amount);
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(origin, target, elapsed / duration);
            yield return null;
        }
        rect.anchoredPosition = target;
    }

    private IEnumerator MoveDown(RectTransform rect, Vector2 origin, float amount, float duration) {
        Vector2 start = origin + new Vector2(0, amount);
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(start, origin, elapsed / duration);
            yield return null;
        }
        rect.anchoredPosition = origin;
    }


    private void SetPlayer(PlayerMotor player) {
        currentPlayer= player.transform;
    }

    private void OnEnable() {
        LevelManager.OnPlayerSpawn += SetPlayer;
        Health.OnLifesChanged += OnPlayerLifes;
        MagicPoint.OnMPChanged += OnPlayerMP;
        //Health.OnMPChanged += OnPlayerMP;
    }

    private void OnDisable() {
        Health.OnLifesChanged -= OnPlayerLifes;
        LevelManager.OnPlayerSpawn -= SetPlayer;
        MagicPoint.OnMPChanged -= OnPlayerMP;
        //Health.OnMPChanged -= OnPlayerMP;
    }
}
