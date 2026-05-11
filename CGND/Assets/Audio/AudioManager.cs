using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource audioSource;

    [Header("BGM")]
    public AudioClip mainMenuBGM;
    public AudioClip level1BGM;
    public AudioClip level2BGM;
    public AudioClip level3BGM;
    public AudioClip boss1BGM;
    public AudioClip boss2BGM;
    public AudioClip boss3BGM;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
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
            case "Main Menu":
            case "LevelSelect":
                PlayBGM(mainMenuBGM);
                break;
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
}