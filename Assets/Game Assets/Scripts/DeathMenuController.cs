using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuController : MonoBehaviour
{


    public void ResumeGame(string level)
    {
        SceneManager.UnloadSceneAsync("death_menu");
    }

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

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("main_menu");
        Time.timeScale = 1.0f;
    }
}
