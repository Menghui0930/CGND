using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource audioSource;

    [Header("BGM")]
    public AudioClip mainMenuBGM;
    public AudioClip LevelTutorialBGM;
    public AudioClip level1BGM;
    public AudioClip level2BGM;
    public AudioClip level3BGM;
    public AudioClip boss1BGM;
    public AudioClip boss2BGM;
    public AudioClip boss3BGM;

    [Header("Volume UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button muteButton;
    [SerializeField] private GameObject muteIcon;

    private float _lastVolume = 0.75f;
    private bool _isMuted = false;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void Start() {
        float saved = PlayerPrefs.GetFloat("Volume", 0.75f);
        _lastVolume = saved;
        audioSource.volume = saved;

        if (volumeSlider != null) {
            volumeSlider.value = saved;
            volumeSlider.onValueChanged.AddListener(OnSliderChanged);
        }

        UpdateMuteButtonIcon();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                PlayBGM(mainMenuBGM); break;
            case "LevelSelect":
                PlayBGM(mainMenuBGM);
                break;
            case "Level_Tutorial":
                PlayBGM(LevelTutorialBGM); break;
            case "Level_1":
                PlayBGM(level1BGM);
                break;
            case "Level_2":
                PlayBGM(level2BGM);
                break;
            case "Level_3":
                PlayBGM(level3BGM);
                break;
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (audioSource.clip == clip) return;
        audioSource.clip = clip;
        audioSource.Play();
    }

    // Slider 拖动
    private void OnSliderChanged(float value) {
        _isMuted = false;
        _lastVolume = value;
        audioSource.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        UpdateMuteButtonIcon();
    }

    // Mute 按钮（绑在 Button OnClick）
    public void ToggleMute() {
        _isMuted = !_isMuted;
        audioSource.volume = _isMuted ? 0f : _lastVolume;
        // Slider 位置不动
        UpdateMuteButtonIcon();
    }

    private void UpdateMuteButtonIcon() {
        if (muteButton == null) return;
        if (_isMuted) {
            muteIcon.SetActive(true);
        } else {
            muteIcon.SetActive(false);
        }
    }
}