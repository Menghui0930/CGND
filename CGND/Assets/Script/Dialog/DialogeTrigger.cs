using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


public enum CharacterSide { Left, Right }

[System.Serializable]
public class DialogueLine {
    public CharacterSide side;       // 选 Left 或 Right 就够了
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue {
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}


public class DialogeTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDIalogue() {
        DialogueManager.instance.StartDialogue(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            TriggerDIalogue();
        }
    }
}
