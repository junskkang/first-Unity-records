using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float m_PlayerSpeed = 10.0f;
    GameDirector m_GameD = null;

    bool m_IsRBtnDown = false;  //true�� �� ��ư�� ������ �ִٴ� �ǹ�
    bool m_IsLBtnDown = false;  //true�� �� ��ư�� ������ �ִٴ� �ǹ�
    void Start()
    {
        Application.targetFrameRate = 60;   //������ ����
        QualitySettings.vSyncCount = 0;     //����� �ֻ��� ���� x

        m_GameD = GameObject.FindObjectOfType<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameDirector.m_State == GameState.GameIng)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || m_IsLBtnDown == true)  //���ʹ���Ű�� ������ ��
            {
                transform.Translate(-m_PlayerSpeed * Time.deltaTime, 0, 0);
                //transform.position += new Vector3(-m_PlayerSpeed * Time.deltaTime, 0, 0)
                if (transform.position.x <= -8)
                {
                    transform.position = new Vector2(-8, -3.6f);
                }

            }

            if (Input.GetKey(KeyCode.RightArrow) || m_IsRBtnDown == true) //�����ʹ���Ű�� ������ ��
            {
                transform.Translate(m_PlayerSpeed * Time.deltaTime, 0, 0);
                //transform.position += new Vector3(m_PlayerSpeed * Time.deltaTime, 0, 0)
                if (transform.position.x >= 8)
                {
                    transform.position = new Vector2(8, -3.6f);
                }
            }

            //ĳ���Ͱ� ��� �ٱ����� ������ ���ϵ��� ���� ó��
            Vector3 a_vPos = transform.position;
            if (8.0f < a_vPos.x)
                a_vPos.x = 8.0f;
            if (a_vPos.x < -8.0f)
                a_vPos.x = -8.0f;
            transform.position = a_vPos;
        }
    }

    public void LButtonDown()
    {
        transform.Translate(-m_PlayerSpeed * Time.deltaTime, 0, 0);
        if (transform.position.x <= -8)
        {
            transform.position = new Vector2(-8, -3.6f);
        }
    }

    public void RButtonDown()
    {
        transform.Translate(m_PlayerSpeed * Time.deltaTime, 0, 0);
        if (transform.position.x >= 8)
        {
            transform.position = new Vector2(8, -3.6f);
        }
    }

    //Event Trigger ó�� �Լ�
    public void OnRBtnDown()  //������ ��ư�� ���� ��
    {
        m_IsRBtnDown = true;
    }

    public void OnRBtnUp()    //������ ��ư�� �� ��
    {
        m_IsRBtnDown = false;
    }

    public void OnLBtnDown()  //���� ��ư�� ���� ��
    {
        m_IsLBtnDown = true;
    }

    public void OnLBtnUp()    //���� ��ư�� �� ��
    {
        m_IsLBtnDown = false;
    }
}

