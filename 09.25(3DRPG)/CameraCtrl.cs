using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    GameObject playerHero = null;
    Vector3 m_TargetPos = Vector3.zero;

    //카메라 위치 계산용 변수들
    float m_RotH = 0.0f;    //마우스 좌우 조작값 계산용 변수
    float m_RotV = 0.0f;    //마우스 상하 조작값 계산용 변수
    float hSpeed = 5.0f;    //마우스 좌우 회전에 대한 스피드 설정값
    float vSpeed = 2.4f;    //마우스 상하 회전에 대한 스피드 설정값
    float vMinLimit = -7.0f;    //위 아래 각도 제한
    float vMaxLimit = 80.0f;    //위 아래 각도 제한
    float zoomSpeed = 0.5f;     //마우스 휠 조작에 대한 줌 속도 값
    float minDist = 2.0f;       //마우스 줌인 최소 거리값
    float maxDist = 20.0f;      //마우스 줌 아웃 최대 거리값

    //계산용 변수
    float m_CurDistance = 17.0f;
    Quaternion m_BuffRot = Quaternion.identity;
    Vector3 m_BasicPos = Vector3.zero;
    Vector3 m_BuffPos = Vector3.zero;

    //주인공을 기준으로 한 상대적인 구좌표계 기준의 초기값
    //원하는 위치를 찾는 방법은 씬뷰에서 카메라 위치를 잡은 다음에
    //Ctrl+Shift+F를 누르면 카메라가 해당 씬뷰에 맞게끔 좌표가 바뀜
    //나온 값에서 Rotation X값과 Position Z값을 참고 하면 좋음
    float m_DefaultRotH = 0.0f;     //수평 회전각도
    float m_DefaultRotV = 25.0f;    //수직 회전각도
    float m_DefaultDist = 5.2f;     //주인공에서부터 카메라까지의 거리

    public void InitCamera(GameObject a_Player)     //따라다닐 캐릭터 받아오는 함수
    {
        playerHero = a_Player;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerHero == null)
            return;

        m_TargetPos = playerHero.transform.position;
        m_TargetPos.y += 1.4f;  //캐릭터의 중점으로부터 살짝 더 위쪽을 바라보게끔

        //카메라 위치 계산 공식 (구좌표계를 직각좌표계로 환산하는 부분)
        m_RotH = m_DefaultRotH;
        m_RotV = m_DefaultRotV;
        m_CurDistance = m_DefaultDist;

        m_BuffRot = Quaternion.Euler(m_RotV, m_RotH, 0.0f);
        //캐릭터를 기준으로 뒤쪽으로 빼기 위한 작업
        m_BasicPos.x = 0.0f;    
        m_BasicPos.y = 0.0f;
        m_BasicPos.z = -m_CurDistance;

        m_BuffPos = m_TargetPos + (m_BuffRot * m_BasicPos); //Quaternion과 Vector3를 곱하면 각도가 반영된 Vector3가 됨
        transform.position = m_BuffPos; //카메라의 직각좌표계 기준의 위치
        transform.LookAt(m_TargetPos);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (playerHero == null) return;//따라다닐 캐릭터가 존재해야함

        m_TargetPos = playerHero.transform.position;
        m_TargetPos.y += 1.4f;  //캐릭터의 중점으로부터 살짝 더 위쪽을 바라보게끔

        if (Input.GetMouseButton(1))
        {
            //마우스를 좌우로 움직였을 때의 값
            m_RotH += Input.GetAxis("Mouse X") * hSpeed;
            //마우스를 상하로 움직였을 때의 값
            m_RotV -= Input.GetAxis("Mouse Y") * vSpeed;

            m_RotV = ClampAngle(m_RotV, vMinLimit, vMaxLimit);      //-7~80도 사이의 값이 나오게끔
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && m_CurDistance < maxDist)
        {
            m_CurDistance += zoomSpeed;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && m_CurDistance > minDist)
        {
            m_CurDistance -= zoomSpeed;
        }

        //변경된 값을 적용시킬 계산
        m_BuffRot = Quaternion.Euler(m_RotV, m_RotH, 0.0f);
        m_BasicPos.x = 0.0f;
        m_BasicPos.y = 0.0f;
        m_BasicPos.z = -m_CurDistance;

        m_BuffPos = m_TargetPos + (m_BuffRot * m_BasicPos);
        transform.position = m_BuffPos;
        transform.LookAt(m_TargetPos);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360.0f)
            angle += 360.0f;
        if (angle > 360.0f)
            angle -= 360.0f;

        return Mathf.Clamp(angle, min, max);
    }
}
