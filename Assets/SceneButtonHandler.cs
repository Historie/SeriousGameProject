using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneButtonHandler : MonoBehaviour
{
    public Button startButton; // �� Inspector ��������밴ť����

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
