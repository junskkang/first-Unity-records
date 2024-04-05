using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class CarController : MonoBehaviour
{
    float speed = 0;
    Vector2 startPos;

    //bool IsMove = false;  //�ڵ����� ���� ����

    GameDirector m_GameD = null;  //���ӵ��� ��ũ��Ʈ�� ã�ƿ��� ���� ����

    void Start()
    {
        Application.targetFrameRate = 60;      //������ ����
        QualitySettings.vSyncCount = 0;        //����� �ֻ����� ������� �ʵ��� ����
        //����� �ֻ����� �ٸ� ��ǻ���� ��� ĳ���� ���۽� ������ ������ �� �ִ�.

        m_GameD = GameObject.FindObjectOfType<GameDirector>();

        ////���ӵ��� ã�� ���� ���
        //GameObject a_Obj = GameObject.Find("GameDirector"); //���̾��Ű�� �ִ� ������Ʈ �ҷ���
        //if(a_Obj != null )
        //    m_GameD = a_Obj.GetComponent<GameDirector>(); 
        ////������Ʈ�� �پ��ִ� ���ӵ��� ������Ʈ(��ũ��Ʈ)�� �ҷ���
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
        //if (IsMove == false) //�ڵ����� �������� ���� ���� �����ϵ���
        if(GameDirector.s_State == GameState.Ready)
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

                    m_GameD.RecordTimer = 2.0f;
                        
                    GameDirector.s_State = GameState.Move;
                }
            }
        }
        //else if (IsMove == true)  //�ڵ����� �����̴� ����
        else if (GameDirector.s_State == GameState.Move)
        {
            transform.Translate(this.speed, 0, 0); //�ӵ���ŭ x������ �̵�
                                                   //transform.position += new Vector3(this.speed, 0, 0);  ������ ���� �ǹ�
            this.speed *= 0.98f;                   //����

            if (this.speed <= 0.005f)  //�����̸� ���� ����
            {
                this.speed = 0;

                m_GameD.RecordTimer -= Time.deltaTime;  //2�� �� ��� ����
                if (m_GameD.RecordTimer <= 0)
                    m_GameD.RecordTimer = 0;

                if (m_GameD != null && m_GameD.RecordTimer == 0) 
                {
                    m_GameD.Record();  //�̹� �÷��̾��� ��� ����
                    transform.position = new Vector3(-7, -3.7f, 0);  //�ڵ��� ��ġ �ʱ�ȭ
                    //IsMove = false;  //�ڵ��� ���� ����
                    GameDirector.s_State = GameState.Ready;
                }
            }
        }
    }
}
