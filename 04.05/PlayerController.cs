using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float m_PlayerSpeed = 10.0f;
    GameDirector m_GameD = null;
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
            if (Input.GetKey(KeyCode.LeftArrow))  //���ʹ���Ű�� ������ ��
            {
                transform.Translate(-m_PlayerSpeed * Time.deltaTime, 0, 0);
                if (transform.position.x <= -8)
                {
                    transform.position = new Vector2(-8, -3.6f);
                }

            }

            if (Input.GetKey(KeyCode.RightArrow)) //�����ʹ���Ű�� ������ ��
            {
                transform.Translate(m_PlayerSpeed * Time.deltaTime, 0, 0);
                if (transform.position.x >= 8)
                {
                    transform.position = new Vector2(8, -3.6f);
                }
            }
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
}

