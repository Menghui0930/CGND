using UnityEngine;

public class DoorController : MonoBehaviour
{
    public int totalButtons = 4;      
    private int pressedCount = 0;

    public void ButtonPressed()
    {
        pressedCount++;
        Debug.Log("Buttons pressed: " + pressedCount + "/" + totalButtons);

        if (pressedCount >= totalButtons)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        Debug.Log("All buttons pressed, door will open");
        gameObject.SetActive(false);       
    }
}