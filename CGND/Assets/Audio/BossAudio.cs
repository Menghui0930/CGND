using UnityEngine;
using UnityEngine.SceneManagement;

public class BossAudio : MonoBehaviour
{
    public void PlayLevelBGM()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Level_1":
                AudioManager.Instance.PlayBGM(
                    AudioManager.Instance.level1BGM);
                break;
            case "Level_2":
                AudioManager.Instance.PlayBGM(
                    AudioManager.Instance.level2BGM);
                break;
            case "Level_3":
                AudioManager.Instance.PlayBGM(
                    AudioManager.Instance.level3BGM);
                break;
        }
    }
}