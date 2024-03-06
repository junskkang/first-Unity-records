using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title_Mgr : MonoBehaviour
{
    public Button StartBtn;
    // 클래스 소속의 멤버변수로 선언된 StartBtn
  

    void Start()
    {
        StartBtn.onClick.AddListener(StartClick);
        //AddListener 반응을 감지하겠다는 의미를 가진 함수
        //클릭이 들어오면 그 반응을 감지하여 StartClick이라는 함수를 실행하겠다는 의미
    }


    void Update()
    {
        
    }

    void StartClick()
    {
        //Debug.Log("버튼을 클릭 했어요.");
        SceneManager.LoadScene("LobbyScene");

    }
}
