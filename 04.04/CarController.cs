using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class CarController : MonoBehaviour
{
    float speed = 0;
    Vector2 startPos;

    bool IsMove = false;

    GameDirector m_GameD = null;

    void Start()
    {
        Application.targetFrameRate = 60;      //������ ����
        QualitySettings.vSyncCount = 0;        //����� �ֻ����� ������� �ʵ��� ����
        //����� �ֻ����� �ٸ� ��ǻ���� ��� ĳ���� ���۽� ������ ������ �� �ִ�.

        m_GameD = GameObject.FindObjectOfType<GameDirector>();
    }

    void Update()
    {
        ////���콺 Ŭ���� �ӵ� �ο�
        //if (Input.GetMouseButtonDown(0))       //���콺 Ŭ����
        //{
        //    this.speed = 0.2f;                 //�ӵ� �ο�
        //}
        if (m_GameD.Player.Length <= m_GameD.PlayerNum)
            return;

        //�������� �� ��ŭ �ӵ� �ο�
        if (IsMove == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //���콺�� Ŭ���� ��ǥ
                this.startPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                //���콺�� �� ��ǥ
                Vector2 endPos = Input.mousePosition;
                if (endPos.x < startPos.x)
                    return;
                else
                {
                    float swipeLength = endPos.x - startPos.x;
                    //���������� ���̸�ŭ �ӵ� �ο�
                    this.speed = swipeLength / 500.0f;   //500�� ���ڰ� ���� �����
                    
                    //ȿ���� ���
                    GetComponent<AudioSource>().Play();

                    IsMove = true;
                }
            }
        }


        transform.Translate(this.speed, 0, 0); //�ӵ���ŭ x������ �̵�
        //transform.position += new Vector3(this.speed, 0, 0);  ������ ���� �ǹ�
        this.speed *= 0.98f;                   //����

        if (IsMove == true) 
        { 
            if (this.speed <= 0.1f)  //�����̸� ���� ����
            {
                this.speed = 0;

                if (m_GameD != null)
                {
                    m_GameD.Record();
                    transform.position = new Vector3(-7, -3.7f, 0);
                    IsMove = false;
                }
            }
        }
    }
}
