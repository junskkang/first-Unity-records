using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    Camera RefCam = null;
    public CinemachineVirtualCamera m_vCam; //유니티 연결

    //카메라 줌인 줌아웃 
    float maxDist = 20.0f;  //줌 최댓값
    float minDist = 5.0f;   //줌 최솟값
    float zoomSpeed = 1.0f; //줌의 속도
    float distance = 15.0f;

    
    
    void Start()
    {
        //씨네머신을 사용하지 않았을 때
        RefCam = GetComponent<Camera>();
        //if (RefCam != null)
        //    distance = RefCam.orthographicSize;

        //씨네머신을 사용하였을 때
        if(m_vCam != null)
            distance = m_vCam.m_Lens.OrthographicSize;
    }

    
    void LateUpdate()
    {
        //PC에서만 작동되는 줌인줌아웃
        //마우스 휠을 아래로 당길 때 값이 커지므로 줌 아웃
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && distance < maxDist)
        {
            distance += zoomSpeed;
            //씨네머신을 사용하지 않을 때
            //RefCam.orthographicSize = distance; 
            
            //씨네머신을 사용할 때
            m_vCam.m_Lens.OrthographicSize = distance;
        }

        //마우스 휠을 위로 밀 때 값이 작아지므로 줌 인
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && distance > minDist)
        {
            distance -= zoomSpeed;
            //씨네머신을 사용하지 않을 때
            //RefCam.orthographicSize = distance;

            //씨네머신을 사용할 때
            m_vCam.m_Lens.OrthographicSize = distance;
        }


    }
}
