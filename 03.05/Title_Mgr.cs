using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title_Mgr : MonoBehaviour
{
    public Button StartBtn;
    // Ŭ���� �Ҽ��� ��������� ����� StartBtn
  

    void Start()
    {
        StartBtn.onClick.AddListener(StartClick);
        //AddListener ������ �����ϰڴٴ� �ǹ̸� ���� �Լ�
        //Ŭ���� ������ �� ������ �����Ͽ� StartClick�̶�� �Լ��� �����ϰڴٴ� �ǹ�
    }


    void Update()
    {
        
    }

    void StartClick()
    {
        //Debug.Log("��ư�� Ŭ�� �߾��.");
        SceneManager.LoadScene("LobbyScene");

    }
}
