using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevelController : MonoBehaviour
{
    private string base_level_name = "level";

    public void selectLevel(int level_number)
    {
        SceneManager.LoadScene(base_level_name + level_number);
        Time.timeScale = 1.0f;
    }

    public void backToMainMenu()
    {
        SceneManager.LoadScene("main_menu");
    }
}
