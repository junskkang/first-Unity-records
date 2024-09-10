using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCtrl : MonoBehaviourPunCallbacks, IPunObservable
{
    private Transform tr;
    //ray가 지면에 맞는 위치를 저장할 변수
    private RaycastHit hit;

    //터렛의 회전 속도
    public float rotSpeed = 10.0f;

    //PhotonView 컴포넌트 변수
    private PhotonView pv = null;
    //원격 네트워크 탱크의 터렛 회전값을 저장할 변수
    private Quaternion currRot = Quaternion.identity;

    

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        tr = GetComponent<Transform>();

        //초기 회전값 설정
        currRot = tr.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //자신의 탱크일 때만 조정
        if (pv.IsMine)
        {        
            //메인 카메라에서 마우스 커서의 위치로 캐스팅되는 Ray를 생성
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //생성된 Ray를 Scene뷰에 녹색 광선으로 표현
            Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("TERRAIN")))
            {
                //Ray에 맞은 위치를 로컬좌표로 변환
                Vector3 relative = tr.InverseTransformPoint(hit.point);
                //역탄젠트 함수인 Atan2로 두 점 간의 각도를 계산
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                //rotSpeed 변수에 지정된 속도로 Y축 회전
                tr.Rotate(0, angle * Time.deltaTime * rotSpeed, 0);
            }
            else
            {
                //origin : 레이의 시작지점 
                //direction : 레이의 방향
                Vector3 orgVec = ray.origin + ray.direction * 2000.0f;
                //반대방향으로 쏘는 이유 : 구는 바깥에만 그리고 있어서 안에서 쏘면 히트가 되지 않음
                ray = new Ray(orgVec, -ray.direction);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("TurretPickObject")))
                {
                    Vector3 cacVec = hit.point - transform.position;
                    Quaternion rotate = Quaternion.LookRotation(cacVec.normalized);
                    rotate.eulerAngles = new Vector3(transform.eulerAngles.x,
                        rotate.eulerAngles.y, transform.eulerAngles.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * rotSpeed);
                    transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y, 0.0f);

                    ////Ray에 맞은 위치를 로컬좌표로 변환
                    //Vector3 relative = tr.InverseTransformPoint(hit.point);
                    ////역탄젠트 함수인 Atan2로 두 점 간의 각도를 계산
                    //float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                    ////rotSpeed 변수에 지정된 속도로 Y축 회전
                    //tr.Rotate(0, angle * Time.deltaTime * rotSpeed, 0);
                }
            }
        }
        else     //원격 네트워크 플레이어의 탱크일 경우
        {
            //현재 회전각도에서 수신받은 실시간 회전각도로 부드럽게 회전
            tr.localRotation = Quaternion.Slerp(tr.localRotation, currRot, Time.deltaTime * rotSpeed);
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)   //송신
        {
            stream.SendNext(tr.localRotation);
        }
        else   //수신
        {
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
