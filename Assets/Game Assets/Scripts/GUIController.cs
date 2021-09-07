using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIController : MonoBehaviour
{
    void Update()
    {
        if (PlayerHitsEscape())
        {
            ChangePauseState();
        }
    }

    // checks if player clicks escpae
    private bool PlayerHitsEscape()
    {
        return Input.GetKeyDown("escape");
    }

    // function that changes the pause state basing on the current game state
    private void ChangePauseState()
    {
            // case when user is in pause menu and wants to close it
            if (IsInPauseMenu())
            {
                Time.timeScale = 1.0f;
                SceneManager.UnloadSceneAsync("pause_menu");
            }

            else if (!IsInPauseMenu() && !IsInOtherMenu())
            {
                Time.timeScale = 0.0f;
                SceneManager.LoadScene("pause_menu", LoadSceneMode.Additive);
            }
    }

    // function that check if the game has any other menu open
    private bool IsInOtherMenu()
    {
        int countLoaded = SceneManager.sceneCount;
        for (int i = 0; i < countLoaded; i++)
        {
            Scene chosen_scene = SceneManager.GetSceneAt(i);
            if (chosen_scene.name.Contains("menu") && !chosen_scene.name.Contains("pause"))
            {
                return true;
            }
        }
        return false;
    }

    // function that checks whether a pause menu is currently open
    private bool IsInPauseMenu()
    {
        int countLoaded = SceneManager.sceneCount;
        for (int i = 0; i < countLoaded; i++)
        {
            Scene chosen_scene = SceneManager.GetSceneAt(i);
            if (chosen_scene.name.StartsWith("pause"))
            {
                return true;
            }
        }
        return false;
    }

    // function that loads a new menu in an additive mode while also stopping the game 
    public void LoadMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        Time.timeScale = 0.0f;
    }
}
