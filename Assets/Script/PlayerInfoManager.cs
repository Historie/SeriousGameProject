using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerInfoManager : MonoBehaviour
{
    public TMP_InputField nameInputField; // ��������������ֶ�
    public TMP_InputField budgetInputField; // ����Ԥ����ֶ�
    public Button nextButton;

    void Start()
    {
        // ������ʱΪ NEXT ��ť��ӵ���¼�
        nextButton.onClick.AddListener(SavePlayerInfo);
    }

    private void SavePlayerInfo()
    {
        // ����ҵ��������浽 PlayerData
        PlayerData.Instance.PlayerName = nameInputField.text;

        // ���Խ�Ԥ��ת��Ϊ�����������浽 PlayerData
        if (float.TryParse(budgetInputField.text, out float budget))
        {
            PlayerData.Instance.Budget = budget;
        }
        else
        {
            Debug.LogError("��������Ч��Ԥ�����֣�");
        }

        // �л�����һ������
        SceneManager.LoadScene("Purchase Records");
    }
}
