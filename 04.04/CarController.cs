using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class CarController : MonoBehaviour
{
    float speed = 0;
    Vector2 startPos;

    //bool IsMove = false;  //자동차의 상태 구분

    GameDirector m_GameD = null;  //게임디렉터 스크립트를 찾아오기 위한 변수

    void Start()
    {
        Application.targetFrameRate = 60;      //프레임 고정
        QualitySettings.vSyncCount = 0;        //모니터 주사율도 영향받지 않도록 고정
        //모니터 주사율이 다른 컴퓨터일 경우 캐릭터 조작시 빠르게 움직일 수 있다.

        m_GameD = GameObject.FindObjectOfType<GameDirector>();

        ////게임디렉터 찾는 정석 방법
        //GameObject a_Obj = GameObject.Find("GameDirector"); //하이어라키에 있는 오브젝트 불러옴
        //if(a_Obj != null )
        //    m_GameD = a_Obj.GetComponent<GameDirector>(); 
        ////오브젝트에 붙어있는 게임디렉터 컴포넌트(스크립트)를 불러옴
    }

    void Update()
    {
        ////마우스 클릭시 속도 부여
        //if (Input.GetMouseButtonDown(0))       //마우스 클릭시
        //{
        //    this.speed = 0.2f;                 //속도 부여
        //}
        if (m_GameD.Player.Length <= m_GameD.PlayerNum)
            return;

        //스와이프 한 만큼 속도 부여
        //if (IsMove == false) //자동차가 멈춰있을 때만 조작 가능하도록
        if(GameDirector.s_State == GameState.Ready)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //마우스를 클릭한 좌표
                this.startPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                //마우스를 뗀 좌표
                Vector2 endPos = Input.mousePosition;
                if (endPos.x < startPos.x)
                    return;
                else
                {
                    float swipeLength = endPos.x - startPos.x;
                    //스와이프한 길이만큼 속도 부여
                    this.speed = swipeLength / 500.0f;   //500은 저자가 정한 상수값

                    //효과음 재생
                    GetComponent<AudioSource>().Play();

                    m_GameD.RecordTimer = 2.0f;
                        
                    GameDirector.s_State = GameState.Move;
                }
            }
        }
        //else if (IsMove == true)  //자동차가 움직이는 상태
        else if (GameDirector.s_State == GameState.Move)
        {
            transform.Translate(this.speed, 0, 0); //속도만큼 x축으로 이동
                                                   //transform.position += new Vector3(this.speed, 0, 0);  위에와 같은 의미
            this.speed *= 0.98f;                   //감속

            if (this.speed <= 0.005f)  //이쯤이면 정지 판정
            {
                this.speed = 0;

                m_GameD.RecordTimer -= Time.deltaTime;  //2초 후 기록 저장
                if (m_GameD.RecordTimer <= 0)
                    m_GameD.RecordTimer = 0;

                if (m_GameD != null && m_GameD.RecordTimer == 0) 
                {
                    m_GameD.Record();  //이번 플레이어의 기록 저장
                    transform.position = new Vector3(-7, -3.7f, 0);  //자동차 위치 초기화
                    //IsMove = false;  //자동차 상태 변경
                    GameDirector.s_State = GameState.Ready;
                }
            }
        }
    }
}
