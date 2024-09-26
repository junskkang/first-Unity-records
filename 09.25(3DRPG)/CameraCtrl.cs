using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    GameObject playerHero = null;
    Vector3 m_TargetPos = Vector3.zero;

    //ī�޶� ��ġ ���� ������
    float m_RotH = 0.0f;    //���콺 �¿� ���۰� ���� ����
    float m_RotV = 0.0f;    //���콺 ���� ���۰� ���� ����
    float hSpeed = 5.0f;    //���콺 �¿� ȸ���� ���� ���ǵ� ������
    float vSpeed = 2.4f;    //���콺 ���� ȸ���� ���� ���ǵ� ������
    float vMinLimit = -7.0f;    //�� �Ʒ� ���� ����
    float vMaxLimit = 80.0f;    //�� �Ʒ� ���� ����
    float zoomSpeed = 0.5f;     //���콺 �� ���ۿ� ���� �� �ӵ� ��
    float minDist = 2.0f;       //���콺 ���� �ּ� �Ÿ���
    float maxDist = 20.0f;      //���콺 �� �ƿ� �ִ� �Ÿ���

    //���� ����
    float m_CurDistance = 17.0f;
    Quaternion m_BuffRot = Quaternion.identity;
    Vector3 m_BasicPos = Vector3.zero;
    Vector3 m_BuffPos = Vector3.zero;

    //���ΰ��� �������� �� ������� ����ǥ�� ������ �ʱⰪ
    //���ϴ� ��ġ�� ã�� ����� ���信�� ī�޶� ��ġ�� ���� ������
    //Ctrl+Shift+F�� ������ ī�޶� �ش� ���信 �°Բ� ��ǥ�� �ٲ�
    //���� ������ Rotation X���� Position Z���� ���� �ϸ� ����
    float m_DefaultRotH = 0.0f;     //���� ȸ������
    float m_DefaultRotV = 25.0f;    //���� ȸ������
    float m_DefaultDist = 5.2f;     //���ΰ��������� ī�޶������ �Ÿ�

    public void InitCamera(GameObject a_Player)     //����ٴ� ĳ���� �޾ƿ��� �Լ�
    {
        playerHero = a_Player;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerHero == null)
            return;

        m_TargetPos = playerHero.transform.position;
        m_TargetPos.y += 1.4f;  //ĳ������ �������κ��� ��¦ �� ������ �ٶ󺸰Բ�

        //ī�޶� ��ġ ��� ���� (����ǥ�踦 ������ǥ��� ȯ���ϴ� �κ�)
        m_RotH = m_DefaultRotH;
        m_RotV = m_DefaultRotV;
        m_CurDistance = m_DefaultDist;

        m_BuffRot = Quaternion.Euler(m_RotV, m_RotH, 0.0f);
        //ĳ���͸� �������� �������� ���� ���� �۾�
        m_BasicPos.x = 0.0f;    
        m_BasicPos.y = 0.0f;
        m_BasicPos.z = -m_CurDistance;

        m_BuffPos = m_TargetPos + (m_BuffRot * m_BasicPos); //Quaternion�� Vector3�� ���ϸ� ������ �ݿ��� Vector3�� ��
        transform.position = m_BuffPos; //ī�޶��� ������ǥ�� ������ ��ġ
        transform.LookAt(m_TargetPos);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (playerHero == null) return;//����ٴ� ĳ���Ͱ� �����ؾ���

        m_TargetPos = playerHero.transform.position;
        m_TargetPos.y += 1.4f;  //ĳ������ �������κ��� ��¦ �� ������ �ٶ󺸰Բ�

        if (Input.GetMouseButton(1))
        {
            //���콺�� �¿�� �������� ���� ��
            m_RotH += Input.GetAxis("Mouse X") * hSpeed;
            //���콺�� ���Ϸ� �������� ���� ��
            m_RotV -= Input.GetAxis("Mouse Y") * vSpeed;

            m_RotV = ClampAngle(m_RotV, vMinLimit, vMaxLimit);      //-7~80�� ������ ���� �����Բ�
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && m_CurDistance < maxDist)
        {
            m_CurDistance += zoomSpeed;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && m_CurDistance > minDist)
        {
            m_CurDistance -= zoomSpeed;
        }

        //����� ���� �����ų ���
        m_BuffRot = Quaternion.Euler(m_RotV, m_RotH, 0.0f);
        m_BasicPos.x = 0.0f;
        m_BasicPos.y = 0.0f;
        m_BasicPos.z = -m_CurDistance;

        m_BuffPos = m_TargetPos + (m_BuffRot * m_BasicPos);
        transform.position = m_BuffPos;
        transform.LookAt(m_TargetPos);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360.0f)
            angle += 360.0f;
        if (angle > 360.0f)
            angle -= 360.0f;

        return Mathf.Clamp(angle, min, max);
    }
}
