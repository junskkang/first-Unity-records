using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    Camera RefCam = null;
    public CinemachineVirtualCamera m_vCam; //����Ƽ ����

    //ī�޶� ���� �ܾƿ� 
    float maxDist = 20.0f;  //�� �ִ�
    float minDist = 5.0f;   //�� �ּڰ�
    float zoomSpeed = 1.0f; //���� �ӵ�
    float distance = 15.0f;

    
    
    void Start()
    {
        //���׸ӽ��� ������� �ʾ��� ��
        RefCam = GetComponent<Camera>();
        //if (RefCam != null)
        //    distance = RefCam.orthographicSize;

        //���׸ӽ��� ����Ͽ��� ��
        if(m_vCam != null)
            distance = m_vCam.m_Lens.OrthographicSize;
    }

    
    void LateUpdate()
    {
        //PC������ �۵��Ǵ� �����ܾƿ�
        //���콺 ���� �Ʒ��� ��� �� ���� Ŀ���Ƿ� �� �ƿ�
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && distance < maxDist)
        {
            distance += zoomSpeed;
            //���׸ӽ��� ������� ���� ��
            //RefCam.orthographicSize = distance; 
            
            //���׸ӽ��� ����� ��
            m_vCam.m_Lens.OrthographicSize = distance;
        }

        //���콺 ���� ���� �� �� ���� �۾����Ƿ� �� ��
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && distance > minDist)
        {
            distance -= zoomSpeed;
            //���׸ӽ��� ������� ���� ��
            //RefCam.orthographicSize = distance;

            //���׸ӽ��� ����� ��
            m_vCam.m_Lens.OrthographicSize = distance;
        }


    }
}
