using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroCtrl : MonoBehaviour
{
    //Ű���� �̵� ���� ����
    float h, v;                   //Ű���� �Է°��� �ޱ� ���� ����
    float m_MoveSpeed = 10.0f;    //�ʴ� �̵��ӵ�

    Vector3 m_DirVec;             //�̵��Ϸ��� ���� ���� ����


    //��ǥ ���� ������
    Vector3 m_CurPos;
    Vector3 m_CacEndVec;

    //�Ѿ� �߻� ���� ����
    float m_AttSpeed = 0.1f;    //���ݼӵ�
    float m_CacAttTick = 0.0f;  //����� �߻� �ֱ� �����
    float m_ShootRange = 30.0f; //��Ÿ�

    //ĳ���� ȭ�� ����� ������ ����
    float maxX = 0.98f;
    float minX = 0.02f;
    float maxY = 0.93f;
    float minY = 0.08f;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        KeyBoardUpdate();

        //�Ѿ� �߻� �ڵ�
        if ( 0.0f < m_CacAttTick)       //�Ѿ� ����
            m_CacAttTick -= Time.deltaTime;

        if (Input.GetMouseButton(1) == true)    //���콺 ��Ŭ�� �Ѿ� �߻�
        {
            if (m_CacAttTick <= 0.0f)
            {
                //���콺�� ��ǥ�� ������ǥ��� �޾Ƽ� �� ���͸� �Ű������� ����
                ShootFire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                m_CacAttTick = m_AttSpeed;
            }
        }
    }

    void KeyBoardUpdate()   //Ű���� �̵�ó��
    {
        h = Input.GetAxisRaw("Horizontal"); // -1 ~ 1
        v = Input.GetAxisRaw("Vertical");   // -1 ~ 1

        if (h != 0.0f || v != 0.0f)  //�̵� Ű���带 �����ϰ� ������
        {
            m_DirVec = (Vector3.right * h) + (Vector3.forward * v);
            if(1.0f < m_DirVec.magnitude)
                m_DirVec.Normalize();   //�����������

            //ȭ�� �ٱ����� ������ ���ϵ��� ����
            Vector3 a_Pos = Camera.main.WorldToViewportPoint(transform.position);
            if (a_Pos.x < minX)
            {
                a_Pos.x = minX + 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else if (a_Pos.x > maxX)
            {
                a_Pos.x = maxX - 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else if (a_Pos.y < minY)
            {
                a_Pos.y = minY + 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            if (a_Pos.y > maxY)
            {
                a_Pos.y = maxY - 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else
                transform.Translate(m_DirVec * m_MoveSpeed * Time.deltaTime);
        }
    }

    public void ShootFire(Vector3 a_Pos)    //��ǥ������ �Ű������� ����
    {//Ŭ�� �̺�Ʈ�� �߻����� �� �Լ� ȣ��
        GameObject a_Obj = Instantiate(GameMgr.m_BulletPrefab);

        m_CacEndVec = a_Pos - transform.position;   //��ǥ���� - ����ĳ��������
        m_CacEndVec.y = 0.0f;

        BulletCtrl a_BulletSc = a_Obj.GetComponent<BulletCtrl>();
        a_BulletSc.BulletSpawn(transform.position, m_CacEndVec.normalized, m_ShootRange);
    }
}