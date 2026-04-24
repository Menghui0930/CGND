using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
   

    public void StartGame()
    {
        SceneManager.LoadScene("Level_Tutorial");

    }

    public void Setting()
    {

    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
