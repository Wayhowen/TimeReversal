using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLevelButtonController : MonoBehaviour
{
    public void backToMainMenu()
    {
        SceneManager.LoadScene("main_menu");
    }
}
