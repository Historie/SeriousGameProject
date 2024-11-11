using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PurchaseDetailManager : MonoBehaviour
{
    public TMP_InputField[] purchaseInputs;
    public TMP_InputField[] amountInputs;
    public TMP_InputField totalAmountInput;
    public Button nextButton;

    void Start()
    {
        nextButton.onClick.AddListener(SavePurchaseDetails);

        foreach (TMP_InputField amountInput in amountInputs)
        {
            amountInput.onValueChanged.AddListener(delegate { UpdateTotalAmount(); });
        }
    }

    private void SavePurchaseDetails()
    {
        for (int i = 0; i < purchaseInputs.Length; i++)
        {
            string itemName = purchaseInputs[i].text;
            if (float.TryParse(amountInputs[i].text, out float amount))
            {
                Debug.Log("Item: " + itemName + ", Amount: " + amount);
                // ���Խ�������Ϣ���浽 PlayerData �������ط�
            }
            else
            {
                Debug.LogError("��������Ч�Ľ�");
            }
        }

        SceneManager.LoadScene("Player Virutal Village");
    }

    private void UpdateTotalAmount()
    {
        float total = 0;

        foreach (TMP_InputField amountInput in amountInputs)
        {
            if (float.TryParse(amountInput.text, out float amount))
            {
                total += amount;
            }
        }

        totalAmountInput.text = total.ToString("F2");
    }
}
