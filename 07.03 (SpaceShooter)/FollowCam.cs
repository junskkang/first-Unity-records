using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;      //추적할 타깃 게임오브젝트의 Transform 변수
    FireCtrl fireCtrl = null;       //추적할 타깃이 갖고 있는 fireCtrl스크립트 참조 변수 
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

    //카메라 위치 계산용 변수
    float rotV = 0.0f;          //마우스 상하 조작값 계산용 변수
    float defaultRotV = 25.2f;  //높이 기준의 회전각도, 절대강좌 방식으로 했을 때 카메라의 초기회전값이 x = 25.2임
    float marginRotV = 22.3f;   //총구와의 마진 각도 : firePos와 카메라의 오프셋 값
    float minLimitV = -17.9f;   //위 아래 각도 제한
    float maxLimitV = 52.9f;    //위 아래 각도 제한
    float maxDist = 4.0f;       //마우스 줌 아웃 최대거리 제한 
    float minDist = 0.1f;       //마우스 줌 인 최대 거리 제한
    float zoomSpeed = 0.7f;      //마우스 휠 조작에 대한 줌 인 줌 아웃 스피드 값

    Quaternion buffRot;         //카메라 회전 계산용 변수
    Vector3 buffPos;            //카메라 회전에 대한 위치 좌표 계산용 변수
    Vector3 basicPos = Vector3.zero; //위치 계산용 변수
    float saveDist;

    //총 조준 방향 계산용 변수
    public static Vector3 riffleDir = Vector3.zero;     //총 조준 방향
    Quaternion cacRFRot;
    Vector3 cacRFPos = Vector3.forward;

    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;
        saveDist = dist;

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

        //카메라 위치 계산
        rotV = defaultRotV;
    }

    void Update()
    {
        //사망 상태 시 조작 불가능
        if (GameManager.GameState == GameState.GameEnd) return;

        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        if (GameManager.IsPointerOverUIObject() == false)
        {
            //예전 방식
            ////--- 카메라 위 아래 바라보는 각도 조절을 위한 높낮이 변경 코드
            //height -= (rotSpeed * Time.deltaTime * Input.GetAxis("Mouse Y"));  

            //if (height < 0.1f)
            //    height = 0.1f;

            //if (5.7f < height)
            //    height = 5.7f;
            ////--- 카메라 위 아래 바라보는 각도 조절을 위한 높낮이 변경 코드


            //구면좌표계를 이용한 신방식 24.07.03
            float addRotSpeed = 235.0f;     //튜닝값
            rotSpeed = addRotSpeed;
            rotV -= (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse Y"));
            if(rotV < minLimitV)
                rotV = minLimitV;
            if(maxLimitV <rotV)
                rotV = maxLimitV;
        }

        //카메라 줌인 줌아웃
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && dist < maxDist)
        {
            dist += zoomSpeed;
            saveDist = this.dist;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && minDist < dist)
        {
            dist -= zoomSpeed;
            saveDist = this.dist;
        }       

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

        //카메라 위치 잡아주는 절대강좌 소스 (직각좌표계 이용)
        //카메라의 위치를 추적대상의 dist 변수만큼 뒤쪽으로 배치하고
        //height 변수만큼 위로 올림
        //Lerp함수 한번에 값을 변경하는게 아니라 점차점차 도달하도록 하는 함수
        //카메라 위치가 급격하게 변했을때 한번에 띵~ 하고변하는게 아니라 이동하는 효과를 볼 수 있음
        //transform.position = Vector3.Lerp(transform.position,
        //                                    playerTr.position
        //                                    - (playerTr.forward * dist)
        //                                    + (Vector3.up * height),
        //                                    Time.deltaTime * dampTrace);

        //구면좌표계를 직각좌표계로 환산하여 카메라 위치를 잡아주는 소스 24.07.03

        buffRot = Quaternion.Euler(rotV, targetTr.eulerAngles.y, 0.0f);
        //첫번째 매개변수에 x축에다가 회전값을 넣어 위아래로 움직이도록 함
        //두번째 매개변수에는 y축의 회전값인데 카메라의 타겟(플레이어)이 바라보고 있는 회전 각도를 가져옴
        basicPos = new Vector3(0.0f, 0.0f, -dist);
        //basicPos는 dist거리 만큼 뒤를 향하는 벡터, 일정한 거리를 유지하도록 하는 벡터 막대기 역할
        buffPos = m_PlayerVec + (buffRot * basicPos);
        //Vector3에다가 Quaternion을 곱하면 Quaternion의 회전각도가 적용된 Vector3가 나오게 됨.
        //고로 buffRot 방향을 향하는 basicPos의 z축(-dist)거리만큼
        //떨어진 Vector3가 buffPos가 되는 것
        transform.position = Vector3.Lerp(transform.position, buffPos, Time.deltaTime * dampTrace);
        
        //카메라가 타깃 게임오브젝트를 바라보게 설정
        transform.LookAt(m_PlayerVec);

        //총구의 위아래 방향 계산
        if (fireCtrl == null)
            fireCtrl = targetTr.GetComponent<FireCtrl>();

        Vector3 cPos = Vector3.zero;
        if (rotV < 6.0f) 
        {
            cPos = fireCtrl.firePos.localPosition;
            cPos.y = 1.53f;
            fireCtrl.firePos.localPosition = cPos;
        }
        else
        {
            cPos = fireCtrl.firePos.localPosition;
            cPos.y = 1.41f;
            fireCtrl.firePos.localPosition = cPos;
        }


        cacRFRot = Quaternion.Euler(Camera.main.transform.eulerAngles.x - marginRotV,
                                    targetTr.eulerAngles.y, 0.0f);
        riffleDir = cacRFRot * cacRFPos;
        //카메라가 바라보는 방향을 cacRFPos(forward전방벡터)에다가 곱해주어 
        //카메라가 바라보는 방향으로 riffleDir를 구해줌
        //초기의 카메라와 firePos의 각도를 유지한 채 회전시키기 위해 margin각도를 빼줌



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
            transform.position = hit.point;

            //float collDist = (transform.position - m_PlayerVec).magnitude;
            ////Debug.Log(hit.collider.name);
            ////hit.collider.gameObject.GetComponent<MeshRenderer>().material = materials;
            //if (collDist > 0.85f)
            //    this.dist = collDist - 0.3f;            
        }
        else if (!isBorder)
        {
            this.dist = Mathf.Lerp(this.dist, saveDist, Time.deltaTime * zoomSpeed * 2);
        }
    }
}
