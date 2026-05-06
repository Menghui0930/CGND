using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndingCutscene : MonoBehaviour
{
    [Header("Dialogue Lines")]
    public DialogueLine[] lines;

    [Header("UI")]
    public GameObject dialogueBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeSpeed = 1f;

    [Header("Scene")]
    public string nextScene = "";

    private int currentLine = 0;
    private float timer = 0f;
    private bool isFading = false;
    private bool cutsceneRunning = false;

    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        dialogueBox.SetActive(false);
        StartCutscene();
    }

    void Update()
    {
        if (!cutsceneRunning) return;

        if (isFading)
        {
            fadeImage.color = new Color(0, 0, 0,Mathf.MoveTowards(fadeImage.color.a, 1f, fadeSpeed * Time.deltaTime));

            if (fadeImage.color.a >= 1f)
            {
                isFading = false;
                if (nextScene != "")
                    SceneManager.LoadScene("Main Menu");
            }
            return;
        }

        timer += Time.deltaTime;
        if (timer >= lines[currentLine].displayTime)
        {
            NextLine();
        }
    }

    public void StartCutscene()
    {
        cutsceneRunning = true;
        currentLine = 0;
        dialogueBox.SetActive(true);
        ShowLine(currentLine);
    }

    void ShowLine(int index)
    {
        timer = 0f;
        nameText.text = lines[index].characterName;
        dialogueText.text = lines[index].dialogueText;

        if (lines[index].portrait != null)
        {
            portraitImage.color = new Color(1, 1, 1, 1);
            portraitImage.sprite = lines[index].portrait;
        }
        else
        {
            portraitImage.color = new Color(1, 1, 1, 0);  
        }
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine < lines.Length)
        {
            ShowLine(currentLine);
        }
        else
        {
            dialogueBox.SetActive(false);   //will fade black when scene end
            isFading = true;
            
        }
    }
}