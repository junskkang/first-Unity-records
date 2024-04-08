using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float m_PlayerSpeed = 10.0f;
    GameDirector m_GameD = null;

    bool m_IsRBtnDown = false;  //true일 때 버튼을 누르고 있다는 의미
    bool m_IsLBtnDown = false;  //true일 때 버튼을 누르고 있다는 의미
    void Start()
    {
        Application.targetFrameRate = 60;   //프레임 고정
        QualitySettings.vSyncCount = 0;     //모니터 주사율 영향 x

        m_GameD = GameObject.FindObjectOfType<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameDirector.m_State == GameState.GameIng)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || m_IsLBtnDown == true)  //왼쪽방향키를 눌렀을 때
            {
                transform.Translate(-m_PlayerSpeed * Time.deltaTime, 0, 0);
                //transform.position += new Vector3(-m_PlayerSpeed * Time.deltaTime, 0, 0)
                if (transform.position.x <= -8)
                {
                    transform.position = new Vector2(-8, -3.6f);
                }

            }

            if (Input.GetKey(KeyCode.RightArrow) || m_IsRBtnDown == true) //오른쪽방향키를 눌렀을 때
            {
                transform.Translate(m_PlayerSpeed * Time.deltaTime, 0, 0);
                //transform.position += new Vector3(m_PlayerSpeed * Time.deltaTime, 0, 0)
                if (transform.position.x >= 8)
                {
                    transform.position = new Vector2(8, -3.6f);
                }
            }

            //캐릭터가 배경 바깥으로 나가지 못하도록 막는 처리
            Vector3 a_vPos = transform.position;
            if (8.0f < a_vPos.x)
                a_vPos.x = 8.0f;
            if (a_vPos.x < -8.0f)
                a_vPos.x = -8.0f;
            transform.position = a_vPos;
        }
    }

    public void LButtonDown()
    {
        transform.Translate(-m_PlayerSpeed * Time.deltaTime, 0, 0);
        if (transform.position.x <= -8)
        {
            transform.position = new Vector2(-8, -3.6f);
        }
    }

    public void RButtonDown()
    {
        transform.Translate(m_PlayerSpeed * Time.deltaTime, 0, 0);
        if (transform.position.x >= 8)
        {
            transform.position = new Vector2(8, -3.6f);
        }
    }

    //Event Trigger 처리 함수
    public void OnRBtnDown()  //오른쪽 버튼을 누를 때
    {
        m_IsRBtnDown = true;
    }

    public void OnRBtnUp()    //오른쪽 버튼을 뗄 때
    {
        m_IsRBtnDown = false;
    }

    public void OnLBtnDown()  //왼쪽 버튼을 누를 때
    {
        m_IsLBtnDown = true;
    }

    public void OnLBtnUp()    //왼쪽 버튼을 뗄 때
    {
        m_IsLBtnDown = false;
    }
}

