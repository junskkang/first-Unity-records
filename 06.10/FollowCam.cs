using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;      //추적할 타깃 게임오브젝트의 Transform 변수
    public float dist = 10.0f;      //카메라와의 일정 거리
    public float height = 3.0f;     //카메라의 높이 설정
    public float dampTrace = 20.0f; //부드러운 추적을 위한 변수

    Vector3 m_PlayerVec = Vector3.zero;
    float rotSpeed = 10.0f;

    public bool isBorder = false;
    public Material materials;
    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;
    }

    void Update()
    {
        //사망 상태 시 조작 불가능
        if (GameManager.GameState == GameState.GameEnd) return;

        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        if (GameManager.IsPointerOverUIObject() == false)
        {
            //--- 카메라 위 아래 바라보는 각도 조절을 위한 높낮이 변경 코드
            height -= (rotSpeed * Time.deltaTime * Input.GetAxis("Mouse Y"));  

            if (height < 0.1f)
                height = 0.1f;

            if (5.7f < height)
                height = 5.7f;
            //--- 카메라 위 아래 바라보는 각도 조절을 위한 높낮이 변경 코드
        }

        StopToWall();

    }//void Update()

    //Update 함수 호출 이후 한 번씩 호출되는 함수인 LateUpdate 사용
    //추적할 타깃의 이동이 종료된 이후에 카메라가 추적하기 위해 LateUpdate 사용
    // Update is called once per frame
    void LateUpdate()
    {
        switch (GameManager.playerCharacter)
        {
            case PlayerCharacter.Player1:
                FollowPlayer(GameManager.inst.player1.transform);
                break;
            case PlayerCharacter.Player2:
                FollowPlayer(GameManager.inst.player2.transform);
                break;
        }
    }

    void FollowPlayer(Transform playerTr)
    {
        m_PlayerVec = playerTr.position;
        m_PlayerVec.y += 1.2f;

        //카메라의 위치를 추적대상의 dist 변수만큼 뒤쪽으로 배치하고
        //height 변수만큼 위로 올림
        //Lerp함수 한번에 값을 변경하는게 아니라 점차점차 도달하도록 하는 함수
        //카메라 위치가 급격하게 변했을때 한번에 띵~ 하고변하는게 아니라 이동하는 효과를 볼 수 있음
        transform.position = Vector3.Lerp(transform.position,
                                            playerTr.position
                                            - (playerTr.forward * dist)
                                            + (Vector3.up * height),
                                            Time.deltaTime * dampTrace);

        //카메라가 타깃 게임오브젝트를 바라보게 설정
        transform.LookAt(m_PlayerVec);
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 3.2f, Color.green);  //시작 위치, 방향 * 길이, 색
        RaycastHit hit;
        
        isBorder = Physics.Raycast(transform.position, transform.forward, 
                  out hit, 3.2f, LayerMask.GetMask("Wall"));
        //Raycast(시작위치, 방향, 길이, 레이어마스크)
        //해당 레이캐스트가 wall이라는 레이어마스크에 닿으면 true값을 반환

        if (isBorder)
        {
            //Debug.Log(hit.collider.name);
            hit.collider.gameObject.GetComponent<MeshRenderer>().material = materials;
        }
        
    }
}
