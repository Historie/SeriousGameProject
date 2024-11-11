using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerInfoManager : MonoBehaviour
{
    public TMP_InputField nameInputField; // 输入玩家姓名的字段
    public TMP_InputField budgetInputField; // 输入预算的字段
    public Button nextButton;

    void Start()
    {
        // 在运行时为 NEXT 按钮添加点击事件
        nextButton.onClick.AddListener(SavePlayerInfo);
    }

    private void SavePlayerInfo()
    {
        // 将玩家的姓名保存到 PlayerData
        PlayerData.Instance.PlayerName = nameInputField.text;

        // 尝试将预算转换为浮点数并保存到 PlayerData
        if (float.TryParse(budgetInputField.text, out float budget))
        {
            PlayerData.Instance.Budget = budget;
        }
        else
        {
            Debug.LogError("请输入有效的预算数字！");
        }

        // 切换到下一个场景
        SceneManager.LoadScene("Purchase Records");
    }
}
