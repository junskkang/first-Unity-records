using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class TankMove : MonoBehaviourPunCallbacks, IPunObservable
{
    //탱크의 이동 및 회전 속도를 나타내는 변수
    public float moveSpeed = 20.0f;
    public float rotSpeed = 50.0f;

    //참조할 컴포넌트를 할당할 변수
    private Rigidbody rbody;
    private Transform tr;

    //키보드 입력값 변수
    private float h, v;

    //PhotonView 컴포넌트를 할당할 변수
    private PhotonView pv = null;
    //메인카메라가 추적할 CamPivot 게임오브젝트
    public Transform camPivot;

    //위치 정보를 송수신할 때 사용할 변수 선언 및 초깃값 설정
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    //탱크충돌 예외처리
    TankDamage tankDamage = null;
    Terrain terrain = null;
    Vector3 cacPos = Vector3.zero;

    void Start()
    {
        //컴포넌트 할당
        rbody = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();

        //Rigidbody의 무게중심을 낮게 설정
        rbody.centerOfMass = new Vector3(0.0f, -2.5f, 0.0f);

        //PhotonView가 자신의 탱크일 경우
        if (pv.IsMine)
        {
            //메인 카메라에 추가된 SmoothFollw 스크립트에 추적대상을 연결
            Camera.main.GetComponent<SmoothFollow>().target = camPivot;
        }
        else
        {
            //원격 네트워크 플레이어의 탱크는 물리력을 이용하지 않음
            rbody.isKinematic = true;
        }

        //원격 탱크의 위치 및 회전값을 처리할 변수의 초기값 설정
        currPos = transform.position;
        currRot = transform.rotation;

        //탱크 충돌 예외 처리를 위한 변수
        tankDamage = GetComponent<TankDamage>();
        terrain = FindObjectOfType<Terrain>();
    }

    // Update is called once per frame
    void Update()
    {
        //자신이 만든 네트워크 게임오브젝트가 아닌 경우 키보드 조작 불가능
        if (pv.IsMine) //  && !GameManager.inst.chatInput.isFocused
        {
            if (GameManager.isChat) return;

            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            //회전과 이동 처리
            tr.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);
            tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);

            UnitCollUpdate();
        }
        else //원격 플레이어에 보이는 나의 아바타일 때는...
        {
            if (10.0f < (transform.position - currPos).magnitude)
            {
                //중계 받은 좌표와 현재좌표의 거리차가 10m 이상이면 즉시 점프시켜 보정
                transform.position = currPos;
            }
            else
            {
                //원격 플레이어의 탱크를 수신받은 위치까지 부드럽게 이동시킴
                tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 10.0f);
            }
            tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 10.0f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보 송신
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else //원격 플레이어 PC에 보이는 IsMine의 아바타 오브젝트 위치 정보 수신
        {
            currPos = (Vector3) stream.ReceiveNext();
            currRot = (Quaternion) stream.ReceiveNext();
        }

    }

    void UnitCollUpdate()
    {
        if (tankDamage == null) return;

        if (tankDamage.currHp <= 0.0f) return;

        if (terrain == null) return;

        //지형 밑으로 떨어지지 않게 하기
        float curHeight = terrain.SampleHeight(transform.position);
        if (transform.position.y < curHeight - 1.0f)
        {
            transform.position = new Vector3(transform.position.x,
                                curHeight + 1.0f, transform.position.z);

            if (rbody != null)
            {
                rbody.isKinematic = false;
                rbody.velocity = new Vector3(0.0f, rbody.velocity.y, 0.0f);
                rbody.angularVelocity = Vector3.zero;
                rbody.isKinematic = true;
            }
        }

        //탱크가 지형 밖으로 벗어나지 못하게 막기
        cacPos = transform.position;
        if (245.0f < transform.position.x)
            cacPos.x = 245.0f;
        if (245.0f < transform.position.z)
            cacPos.z = 245.0f;
        if (transform.position.x < -245.0f)
            cacPos.x = -245.0f;
        if (transform.position.z < -245.0f)
            cacPos.z = -245.0f;

        transform.position = cacPos;

        //탱크가 뒤집어 지지 못하도록 바로 세우기
        if (transform.position.y < curHeight + 8.0f)
        if (Vector3.Dot(transform.up, Vector3.up) <= 0.0f)
        {
            //월드의 업벡터와 탱크의 업벡터를 내적하여 그 각도가 90도를 넘어가려할 때
            //탱크의 업벡터를 보정시켜버림
            transform.up = Vector3.up;
            rbody.isKinematic = false;
            rbody.angularVelocity = new Vector3(0, 0, 0);
            rbody.isKinematic = true;
            }

    }
}
