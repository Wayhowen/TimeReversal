using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenuController : MonoBehaviour
{
    private string current_scene_name;

    private void Start()
    {
        current_scene_name = GetCurrentLevelSceneName();
        Time.timeScale = 0.0f;
    }

    public void NextLevel(string level)
    {
        int scene_no = int.Parse(current_scene_name.Substring(current_scene_name.Length - 1));
        if (scene_no == 3)
        {
            BackToMainMenu();
        }
        else
        {
            SceneManager.LoadScene(current_scene_name.Remove(current_scene_name.Length - 1, 1) + (scene_no + 1));
        }
        Time.timeScale = 1.0f;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(current_scene_name);
        Time.timeScale = 1.0f;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("main_menu");
        Time.timeScale = 1.0f;
    }

    private string GetCurrentLevelSceneName()
    {
        int countLoaded = SceneManager.sceneCount;
        for (int i = 0; i < countLoaded; i++)
        {
            Scene chosen_scene = SceneManager.GetSceneAt(i);
            if (chosen_scene.name.StartsWith("level"))
            {
                return chosen_scene.name;
            }
        }
        return null;
    }
}
