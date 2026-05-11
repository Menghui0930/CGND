using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//using UnityEngine.UIElements;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    private InputAction _continueDialog;

    [Header("Left character (Player)")]
    public Image[] playerDimOverlay;
    public TextMeshProUGUI playerNameText;
    public string playerName = "main";      // ← 填一次就好

    [Header("Right character (Rock)")]
    public Image rockDimOverlay;
    public TextMeshProUGUI rockNameText;
    public string rockName = "Rock";        // ← 填一次就好

    [Header("Dialogue Box")]
    public TextMeshProUGUI dialogueArea;

    [Header("Settings")]
    public float typingSpeed = 0.2f;
    public Animator animator;

    public static readonly Color dimColor = new Color32(130, 130, 130, 255);
    public static readonly Color clearColor = new Color32(0,0,0,0);

    // 角色颜色
    private static readonly Color playerColor = new Color32(0x6C, 0xEC, 0xAF, 255);  // #6CECAF
    private static readonly Color rockColor = new Color32(0xE2, 0xA2, 0x46, 255);  // #E2A246

    private Queue<DialogueLine> lines = new Queue<DialogueLine>();

    public bool isDialogueActive = false;

    private void Awake() {
        _continueDialog = InputSystem.actions.FindAction("ContinueDialog");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(instance == null)
            instance = this;
        animator = GetComponent<Animator>();
    }

    public void StartDialogue(Dialogue dialogue) {
        isDialogueActive=true;

        animator.Play("Dialog_IN");

        lines.Clear();
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines) {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine() {
        if (lines.Count == 0) {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        if (currentLine.side == CharacterSide.Left) {
            playerNameText.text = playerName;
            dialogueArea.color = playerColor;           // 自动
            foreach (Image overlay in playerDimOverlay)
                overlay.color = clearColor;
            rockDimOverlay.color = dimColor;
        } else {
            rockNameText.text = rockName;
            dialogueArea.color = rockColor;             // 自动
            rockDimOverlay.color = clearColor;
            foreach (Image overlay in playerDimOverlay)
                overlay.color = dimColor;
        }

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine.line));
    }

    IEnumerator TypeSentence(string sentence) {
        dialogueArea.text = "";
        foreach (char letter in sentence) {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue() {
        isDialogueActive = false;
        animator.Play("Dialog_OUT");
    }

    private void Update() {
        if (isDialogueActive && _continueDialog.WasPressedThisFrame()) {
            DisplayNextDialogueLine();
        }
    }
}
