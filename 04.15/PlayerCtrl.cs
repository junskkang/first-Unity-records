using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Moving
{
    IsMoving,
    Stop,
    GameOver

}

public class PlayerCtrl : MonoBehaviour
{
    public static Moving state;

    //이동을 위한 변수
    float h = 0.0f;
    float v = 0.0f;

    float moveSpeed = 20.0f;        //이동속도
    Vector3 moveDir = Vector3.zero; //이동방향

    //회전을 위한 변수
    float rotSpeed = 350.0f;
    Vector3 m_CacVec = Vector3.zero;


    void Start()
    {
        state = Moving.Stop;
    }


    void Update()
    {
        //이동 버튼 누를 시 상태 전환
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            state = Moving.IsMoving;
        else
            state = Moving.Stop;

        //카메라 회전
        if (Input.GetMouseButton(1) == true)    
        {
            m_CacVec = transform.eulerAngles; // 0~359도
            //Mouse X : 마우스를좌우로 움직이는 것 감지
            m_CacVec.y += (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse X"));
            m_CacVec.x -= (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse Y"));

            if (180.0f < m_CacVec.x && m_CacVec.x < 330.0f)
                m_CacVec.x = 330.0f;
            if (15.0f < m_CacVec.x && m_CacVec.x <= 180.0f)
                m_CacVec.x = 15.0f;

            transform.eulerAngles = m_CacVec;
        }
        //이동 구현 GetAxis는 float형이기 때문에 이동시에 가속이나 감속이 들어감. 
        //보다 스무스한 움직임
        h = Input.GetAxis("Horizontal");     // -1.0 ~ 1.0f; 
        v = Input.GetAxis("Vertical");       // -1.0 ~ 1.0f;

        //전후좌우 이동 방향 벡터 계산
        moveDir = (Vector3.forward * v) + (Vector3.right * h); //한쪽방향으로만 움직이면 방향벡터가 1 
        if (1.0f < moveDir.magnitude)   //대각선으로 움직일 때 방향벡터가 1이 되게끔 하는 코드
            moveDir.Normalize();

        //Translate(이동방향 * Time.deltaTime * 속도, 기준좌표계);
        transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);

        ////월드좌표계 기준
        //moveDir = (transform.forward * v) + (transform.right * h);
        //if (1.0f < moveDir.magnitude)
        //    moveDir.Normalize();
        //transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.World);

        float a_CacPosY = GameManager.Inst.m_RefMap.SampleHeight(transform.position);
        transform.position = new Vector3(transform.position.x, 5 + a_CacPosY, transform.position.z);

    }
}
