using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Player Info"); // �л��� Player Info ����
    }
}
