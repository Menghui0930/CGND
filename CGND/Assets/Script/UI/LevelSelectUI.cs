using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour {
    [SerializeField] private GameObject levelSelectCanvas;

    public void OpenLevelSelect() => levelSelectCanvas.SetActive(true);
    public void CloseLevelSelect() => levelSelectCanvas.SetActive(false);

    // 从选关界面进入 → 清除技能存档，给对应 SP，标记来源
    public void GoToLevel1() {
        LevelEntryContext.SetFromSelect(bonusSP: 0);
        SceneManager.LoadScene("Level_1");
    }

    public void GoToLevel2() {
        LevelEntryContext.SetFromSelect(bonusSP: 2);
        SceneManager.LoadScene("Level_2");
    }

    public void GoToLevel3() {
        LevelEntryContext.SetFromSelect(bonusSP: 4);
        SceneManager.LoadScene("Level_3");
    }
}