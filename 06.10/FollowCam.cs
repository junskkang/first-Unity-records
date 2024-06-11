using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;      //추적할 타깃 오브젝트의 Transform변수
    public float dist = 10.0f;      //카메라와의 일정 거리
    public float height = 3.0f;     //카메라의 높이 설정
    public float dampTrace = 20.0f; //부덜운 추적을 위한 변수

    Vector3 playerVec = Vector3.zero;

    Vector3 startPos = Vector3.zero;
    Vector3 dragPos = Vector3.zero;

    Vector3 toStartPos = Vector3.zero;
    Quaternion rotation;
    float rotateSpeed = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;
    }

    //Update 함수 호출 이후 한번씩 호출되는 함수인 LateUpdate 사용
    //추적할 타깃의 이동이 종료된 이후에 카메라가 추적하기 위해 LateUpdate를 사용
    // Update is called once per frame
    void LateUpdate()
    {


        playerVec = targetTr.position;
        playerVec.y += 1.2f;

        //카메라의 위치를 추적대상의 dist 변수만큼 뒤쪽으로 배치하고
        //height 변수만큼 위로 올림
        transform.position = Vector3.Lerp(transform.position
                                        , targetTr.position 
                                        - (targetTr.forward * dist)
                                        + (Vector3.up * height)
                                        , Time.deltaTime * dampTrace);

        //카메라가 타깃 게임오브젝트를 바라보게 설정
        transform.position = playerVec;

        if (Input.GetMouseButtonDown(1))
        {
            ClickMouse();
        }
        else if (Input.GetMouseButton(1))
        {
            DragMouse();
        }
    }

    public void ClickMouse()
    {
        startPos = Input.mousePosition;
    }

    public void DragMouse()
    {
        dragPos = Input.mousePosition;

        toStartPos = dragPos - startPos;

        //
        //
        //toStartPos.Normalize();

        if (startPos.y > dragPos.y) //시점 위쪽 회전
            this.transform.rotation = new Quaternion(this.transform.rotation.x + rotateSpeed * Time.deltaTime,
                this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
        if (startPos.y < dragPos.y) //시점 아래쪽 회전
            this.transform.rotation = new Quaternion(this.transform.rotation.x - rotateSpeed * Time.deltaTime,
                this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);

        //rotation = Quaternion.LookRotation(Vector3.right, toStartPos);
        //transform.rotation = rotation;
    }
}
