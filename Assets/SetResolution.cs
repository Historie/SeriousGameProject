using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SetResolution : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Player Virutal Village")
        {
            // ������Ļ�ֱ���Ϊ 1920x1080��true ��ʾȫ��
            Screen.SetResolution(1920, 1080, true);
        }
        else
        {
            Screen.fullScreen = false;
            Screen.SetResolution(3840, 2160, true); }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
