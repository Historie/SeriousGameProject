using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneInitializer : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Player Name: " + PlayerData.Instance.PlayerName);
        Debug.Log("Budget: $" + PlayerData.Instance.Budget);
    }
}
