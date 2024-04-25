using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    //�̵� ���� ����
    Vector3 m_DirVec = Vector3.zero;    //���ư� ���� ����
    Vector3 m_StartPos = new Vector3(0, 0, 1);  //���� ��ġ ���� ����

    Vector3 m_MoveStep = Vector3.zero; //�������� �� �̵� ���� ���� ����
    float m_MoveSpeed = 35.0f;   //�Ѿ� �̵��ӵ�

    //��Ÿ� ���� ����
    float m_ShootRange = 30.0f; //��Ÿ�
    void Start()
    {
        
    }

    
    void Update()
    {
        //���ư��� �� �������� �̵���Ű��
        m_MoveStep = m_DirVec * (Time.deltaTime * m_MoveSpeed);
        m_MoveStep.y = 0.0f;

        transform.Translate(m_MoveStep, Space.World);

        //�Ѿ� ����
        Vector3 a_Pos = Camera.main.WorldToViewportPoint(transform.position);
        if (a_Pos.x < -0.1f || 1.1f < a_Pos.x ||a_Pos.y < -0.1f || 1.1f < a_Pos.y)
            Destroy(gameObject);
        else
        {//�Ѿ��� ��Ÿ� ����
            float a_Length = Vector3.Distance(transform.position, m_StartPos);
            // == float a_Length = (transform.position - m_StartPos).magnitude;
            if (m_ShootRange < a_Length)
                Destroy(gameObject);
        }

        ////����! �ݴ�� ȭ�� �ٱ����� ȭ�� �������� �����Ǵ� ����ü 
        //Vector3 a_SPos = Vector3.zero;
        //a_SPos.x = 1.1f;
        //a_SPos.y = Random.Range(0.0f, 1.0f);
        //Vector3 a_CacPos = Camera.main.ViewportToWorldPoint(a_SPos);

    }

    public void BulletSpawn(Vector3 a_OwnPos, Vector3 a_DirVec, float a_ShootRange = 30.0f)
    {
        //���ư��� �� ���� ���� ���ϱ�
        a_DirVec.y = 0.0f;
        m_DirVec = a_DirVec;
        m_DirVec.Normalize();

        //ĳ������ ��ġ ���ؿ��� ���ư��Բ� �ϱ�
        m_StartPos = a_OwnPos + (m_DirVec * 2.5f);  //�Ѿ� ���ư��� ��ġ�� ĳ���ͺ��� ��¦ ��������
        m_StartPos.y = transform.position.y;

        transform.position = new Vector3 (m_StartPos.x, transform.position.y, m_StartPos.z);

        //�Ѿ��� ���ư��� �������� �ٶ󺸰� ȸ�� �����ֱ�
        //transform.rotation = Quaternion.LookRotation(m_DirVec);
        transform.forward = m_DirVec;

        //��Ÿ� ���� 
        //ĳ���Ϳ��� �Լ��� ȣ���ϸ� �Ű������� �Ѿ�� a_ShootRange�� ���� 
        m_ShootRange = a_ShootRange;
    }
}