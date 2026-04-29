using UnityEngine;

public class DoorController : MonoBehaviour
{
    public int totalButtons = 4;      
    private int pressedCount = 0;
    private Animator animator;
    private bool isOpen;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    public void ButtonPressed()
    {
        pressedCount++;
        Debug.Log("Buttons pressed: " + pressedCount + "/" + totalButtons);

        if (pressedCount >= totalButtons)
        {
            if (!isOpen) {
                isOpen = true;  
                OpenDoor();
            }
        }
    }

    private void OpenDoor()
    {
        Debug.Log("All buttons pressed, door will open");
        //gameObject.SetActive(false);
        animator.Play("Door_Open");
    }

    private void DestroySelf() {
        Destroy(gameObject);
    }
}