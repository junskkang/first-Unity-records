using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby_Mgr : MonoBehaviour
{
    public Button Store_Btn;
    public Button MyRoom_Btn;
    public Button Exit_Btn;
    public Button Start_Btn;

    // 멤버변수 3개 생성

    void Start()
    {
        if (Store_Btn != null)         //일종의 방어코드
            // 해당 변수가 연결이 되어있을 때만 동작하도록 하는 코드
            // if (변수명 == true)     <<< 이건 작동하지 않음
            // int AAA = 0;
            // float BBB = 0.0f;
            // bool CCC = false;

            // 유니티에서 제공해주지 않는 객체 데이터형은 초기값이 "null"
            // ~~~~~~ != null    // "어떠한 변수의 값이 0이 아닐 때 = 뭔가 연결이 되었을 때"

            Store_Btn.onClick.AddListener(StoreBtnClick);

        if (MyRoom_Btn != null)
            MyRoom_Btn.onClick.AddListener(MyRoomBtnClick);

        if (Exit_Btn != null)
            Exit_Btn.onClick.AddListener(ExitBtnClick);

        if (Start_Btn != null)
            Start_Btn.onClick.AddListener(StartBtnClick);

    }


    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    private void StoreBtnClick()
    {
        //Debug.Log("상점으로 가기 버튼 클릭");
        SceneManager.LoadScene("ShopScene");
    }

    private void MyRoomBtnClick()
    {
        //Debug.Log("꾸미기 방으로 가기 버튼 클릭");
        SceneManager.LoadScene("MyRoomScene");
    }

    private void ExitBtnClick()
    {
        //Debug.Log("타이틀 씬으로 가기 버튼 클릭");
        SceneManager.LoadScene("TitleScene");
    }

    
    private void StartBtnClick()
    {
        SceneManager.LoadScene("EvenOddGame");
    }
}
