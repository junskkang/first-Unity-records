using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyRoom_Mgr : MonoBehaviour
{
    public Button My_Room_Back_Btn;
    void Start()
    {
        My_Room_Back_Btn.onClick.AddListener(MyRoomBackBtn);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void MyRoomBackBtn()
    {
        //Debug.Log("�κ� ������ ���ư���");
        SceneManager.LoadScene("LobbyScene");
    }

}
