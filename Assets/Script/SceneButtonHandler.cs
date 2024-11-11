using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneButtonHandler : MonoBehaviour
{
    public Button startButton; // 在 Inspector 面板中拖入按钮对象

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(LoadNextScene);
        }
        else
        {
            Debug.LogError("Start Button is not assigned in the Inspector!");
        }
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
