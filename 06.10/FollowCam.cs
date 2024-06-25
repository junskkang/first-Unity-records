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

    //벽투명화 쌤 풀이
    LayerMask wallMask = -1;
    List<WallCtrl> wallList = new List<WallCtrl>();
    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;

        ////Wall 리스트 만들기
        //wallMask = 1 << LayerMask.NameToLayer("SideWall");
        ////SideWall 레이어만 lay체크하기 위한 마스크 변수 생성

        //GameObject[] sideWalls = GameObject.FindGameObjectsWithTag("SideWall");
        //for (int i = 0; i < sideWalls.Length; i++)
        //{
        //    WallCtrl wallCtrl = sideWalls[i].GetComponent<WallCtrl>();
        //    wallCtrl.isColl = false;
        //    wallCtrl.WallAlphaOnOff(false); //불투명화로 시작
        //    wallList.Add(wallCtrl);
        //}
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

        //StopToWall();

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

        StopToWall();
    }

    void StopToWall()
    {
        float dist = (transform.position - m_PlayerVec).magnitude;

        Vector3 toCamera = transform.position - m_PlayerVec;
        toCamera.Normalize();

        Debug.DrawRay(m_PlayerVec, toCamera * dist, Color.green);  //시작 위치, 방향 * 길이, 색
        RaycastHit hit;
        

        //레이캐스트를 쏠 때 팁!
        //카메라에서 캐릭터를 향해 쏘는 것보다 캐릭터에서 카메라를 향해 쏘는 것이 감도가 좋음
        //왜? 벽의 두께가 존재하고 벽의 내부는 비어있음 고로 raycast가 hit되지 않아
        //카메라 위치가 벽의 내부에 존재할 때는 감지하지 못한다는 의미
        //카메라의 위치가 온전히 벽을 벗어나야 그때부터 hit가 되기 시작하기 때문에
        //벽의 두께가 얇다면 크게 차이는 없겠지만 벽이 두꺼워지면 두꺼워질수록
        //그 차이가 분명할 것. 따라서 캐릭터에서부터 카메라를 향해 쏘면
        //어떠한 오브젝트를 대상으로 한다해도 확실하게 처리할 수 있음

        isBorder = Physics.Raycast(m_PlayerVec, toCamera, 
                  out hit, dist, LayerMask.GetMask("Wall"));
        //Raycast(시작위치, 방향, 길이, 레이어마스크)
        //해당 레이캐스트가 wall이라는 레이어마스크에 닿으면 true값을 반환

        if (isBorder)
        {
            //Debug.Log(hit.collider.name);
            hit.collider.gameObject.GetComponent<MeshRenderer>().material = materials;
        }
        
    }
}
