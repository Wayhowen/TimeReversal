using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    // function that resumes the paused game
    public void ResumeGame(string level)
    {
        SceneManager.UnloadSceneAsync("pause_menu");
        Time.timeScale = 1.0f;
    }

    // function that iterates over open scenes and and loads the one that has "level" in its name
    public void RestartLevel()
    {
        int countLoaded = SceneManager.sceneCount;
        for (int i = 0; i < countLoaded; i++)
        {
            Scene chosen_scene = SceneManager.GetSceneAt(i);
            if (chosen_scene.name.StartsWith("level"))
            {
                SceneManager.LoadScene(chosen_scene.name);
                Time.timeScale = 1.0f;
                break;
            }
        }
    }

    // function that loads the main menu scene 
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("main_menu");
    }
}
