using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{


    public void Play()
    {
        SceneManager.LoadScene("level1");
        Time.timeScale = 1.0f;
    }

    public void SelectLevel()
    {
        SceneManager.LoadScene("select_level");
        Time.timeScale = 1.0f;
    }

    public void LaunchTutorial()
    {
        SceneManager.LoadScene("tutorial");
        Time.timeScale = 1.0f;
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit ();
        #endif
    }
}
