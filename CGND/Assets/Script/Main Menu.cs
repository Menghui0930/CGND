using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public Vector2 targetPos;
    
    public RectTransform rectDeco;
    public RectTransform rectMainMenu;
    private Vector2 startPosMainMenu;
    private Vector2 startPosDeco;

    private float time;
    private float duration = 1f;

    private bool isReversing = false;

    void Start() {
        rectDeco = rectDeco.GetComponent<RectTransform>();
        rectMainMenu = GetComponent<RectTransform>();
        startPosMainMenu = rectMainMenu.anchoredPosition;
        startPosDeco = rectDeco.anchoredPosition;
    }

    void Update() {

        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / duration); 
        t = t * t * (3f - 2f * t); // smoothstep

        if (!isReversing) {
            rectMainMenu.anchoredPosition = Vector2.Lerp(startPosMainMenu, targetPos, t);
            rectDeco.anchoredPosition = Vector2.Lerp(startPosDeco, targetPos, t);
        } else {
            rectMainMenu.anchoredPosition = Vector2.Lerp(targetPos, startPosMainMenu, t);
            rectDeco.anchoredPosition = Vector2.Lerp(targetPos, startPosDeco, t);
        }
    }


    public void StartGame()
    {
        isReversing = !isReversing;
        time = 0;
        //SceneManager.LoadScene("Level_Tutorial");

    }

    public void Setting()
    {

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
