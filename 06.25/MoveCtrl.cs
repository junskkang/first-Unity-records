using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCtrl : MonoBehaviour
{
    public enum MoveType
    {
        WAY_POINT,
        LOOK_AT,
        DAYDREAM
    }

    //이동방식
    public MoveType moveType = MoveType.WAY_POINT;

    //이동속도
    public float speed = 1.0f;
    //회전 시 회전 속도를 조절할 계수
    public float damping = 3.0f;

    private Transform tr;
    private Transform camTr;
    private CharacterController cc;
    //웨이포인트를 저장할 배열
    private Transform[] points;
    //다음에 이동해야 할 위치 인덱스 변수
    private int nextIdx = 1;

    public static bool isStopped = false;


    void Start()
    {        
        tr = GetComponent<Transform>();
        camTr = Camera.main.GetComponent<Transform>();
        cc = GetComponent<CharacterController>();


        //WayPointGroup 게임오브젝트 차일드로 있는 모든 Point의 Transform컴포넌트를 추출
        points = GameObject.Find("WaypointGroup").GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStopped) return;

        switch (moveType)
        {
            case MoveType.WAY_POINT:
                MoveWayPoint();
                break;

            case MoveType.LOOK_AT:
                MoveLookAt();
                break;
            case MoveType.DAYDREAM:
                break;
        }
    }

    void MoveWayPoint()
    {
        //현재 위치에서 다음 웨이포인트를 바라보는 벡터를 계산
        Vector3 direction = points[nextIdx].position - tr.position;
        //산출된 벡터의 회전 각도를 쿼터니언 타입으로 산출
        Quaternion rot = Quaternion.LookRotation(direction);
        //현재 각도에서 회전해야 할 각도까지 부드럽게 회전 처리
        tr.rotation = Quaternion.Slerp(tr.rotation, rot, Time.deltaTime * damping);

        //전진 방향으로 이동 처리
        tr.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    void MoveLookAt()
    {
        //메인 카메라가 바라보는 방향
        Vector3 dir = camTr.TransformDirection(Vector3.forward);    //월드를 > 로컬 좌표로
        Vector3 dir2 = camTr.forward;
        //dir방향으로 초당 speed만큼씩 이동
        cc.SimpleMove(dir * speed);
        //rigidbody로 이동 시키기 : AddForce() , velocity
        //Transform로 이동 시키기 : Translate(), position
        //CharacterController로 이동 시키기 : SimpleMove()
    }

    private void OnTriggerEnter(Collider other)
    {
        //웨이포인트에 충돌 여부 판단
        if (other.CompareTag("WAY_POINT"))
        {
            //맨 마지막 웨이포인트에 도달했을 때 처음 인덱스로 변경
            nextIdx = (++nextIdx >= points.Length) ? 1 : nextIdx;
        }
    }
}
