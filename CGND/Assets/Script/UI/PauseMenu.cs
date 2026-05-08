using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    private bool isPaused = false;

    void Update()
    {
        Debug.Log("Update running");

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("esc pressed");
            if (isPaused)
                Resume();
            else 
                Pause();
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Setting()
    {
        
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}