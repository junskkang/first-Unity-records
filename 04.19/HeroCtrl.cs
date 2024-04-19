using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroCtrl : MonoBehaviour
{
    //키보드 이동 관련 변수
    float h, v;                   //키보드 입력값을 받기 위한 변수
    float m_MoveSpeed = 10.0f;    //초당 이동속도

    Vector3 m_DirVec;             //이동하려는 방향 벡터 변수


    //좌표 계산용 변수들
    Vector3 m_CurPos;
    Vector3 m_CacEndVec;

    //총알 발사 관련 변수
    float m_AttSpeed = 0.1f;    //공격속도
    float m_CacAttTick = 0.0f;  //기관총 발사 주기 만들기
    float m_ShootRange = 30.0f; //사거리

    //캐릭터 화면 벗어나기 금지용 변수
    float maxX = 0.98f;
    float minX = 0.02f;
    float maxY = 0.93f;
    float minY = 0.08f;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        KeyBoardUpdate();

        //총알 발사 코드
        if ( 0.0f < m_CacAttTick)       //총알 장전
            m_CacAttTick -= Time.deltaTime;

        if (Input.GetMouseButton(1) == true)    //마우스 우클릭 총알 발사
        {
            if (m_CacAttTick <= 0.0f)
            {
                //마우스의 좌표를 월드좌표계로 받아서 그 벡터를 매개변수로 넣음
                ShootFire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                m_CacAttTick = m_AttSpeed;
            }
        }
    }

    void KeyBoardUpdate()   //키보드 이동처리
    {
        h = Input.GetAxisRaw("Horizontal"); // -1 ~ 1
        v = Input.GetAxisRaw("Vertical");   // -1 ~ 1

        if (h != 0.0f || v != 0.0f)  //이동 키보드를 조작하고 있으면
        {
            m_DirVec = (Vector3.right * h) + (Vector3.forward * v);
            if(1.0f < m_DirVec.magnitude)
                m_DirVec.Normalize();   //방향단위벡터

            //화면 바깥으로 나가지 못하도록 제어
            Vector3 a_Pos = Camera.main.WorldToViewportPoint(transform.position);
            if (a_Pos.x < minX)
            {
                a_Pos.x = minX + 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else if (a_Pos.x > maxX)
            {
                a_Pos.x = maxX - 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else if (a_Pos.y < minY)
            {
                a_Pos.y = minY + 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            if (a_Pos.y > maxY)
            {
                a_Pos.y = maxY - 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else
                transform.Translate(m_DirVec * m_MoveSpeed * Time.deltaTime);
        }
    }

    public void ShootFire(Vector3 a_Pos)    //목표지점을 매개변수로 받음
    {//클릭 이벤트가 발생했을 때 함수 호출
        GameObject a_Obj = Instantiate(GameMgr.m_BulletPrefab);

        m_CacEndVec = a_Pos - transform.position;   //목표지점 - 현재캐릭터지점
        m_CacEndVec.y = 0.0f;

        BulletCtrl a_BulletSc = a_Obj.GetComponent<BulletCtrl>();
        a_BulletSc.BulletSpawn(transform.position, m_CacEndVec.normalized, m_ShootRange);
    }
}
