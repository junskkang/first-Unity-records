using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCtrl : MonoBehaviour
{
    private Transform tr;
    public float rotSpeed = 1000.0f;

    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //float angle = -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * rotSpeed;
        //tr.Rotate(angle, 0, 0);          

        //메인 카메라에서 마우스 커서의 위치로 캐스팅되는 레이캐스트생성
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("TERRAIN"))) 
        {
            //포신에서 히트를 향하는 벡터
            Vector3 cacVec = hit.point - transform.position;
            //해당 벡터의 회전값을 구함
            Quaternion rotate = Quaternion.LookRotation(cacVec.normalized); 
            //x축으로만 회전을 줄것이기에 나머지는 고정시키기 
            rotate.eulerAngles = new Vector3(rotate.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            //보간함수를 통해 스무스하게 회전되도록함
            transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * 10.0f);
            //y,z축으로는 회전이 없도록 다시 한 번 초기값으로 고정시켜버림
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0.0f, 0.0f);
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
                rotate.eulerAngles = new Vector3(rotate.eulerAngles.x,
                    transform.eulerAngles.y, transform.eulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * 10.0f);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0.0f, 0.0f);
            }
        }

        //포신 각도 제한
        Vector3 angle = transform.localEulerAngles;
        if (angle.x < 180.0f)
        {
            if (5.0f < angle.x)
                angle.x = 5.0f;
        }
        else
        {
            if (angle.x < 330.0f)
                angle.x = 330.0f;
        }
        transform.localEulerAngles = angle;

    }
}
