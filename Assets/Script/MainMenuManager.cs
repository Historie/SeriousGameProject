using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Player Info"); // ÇÐ»»µ½ Player Info ³¡¾°
    }
}
