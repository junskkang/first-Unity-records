using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Moving
{
    IsMoving,
    Stop,
    GameOver
}

public class PlayerCtrl : MonoBehaviour
{
    public static Moving state;

    //�̵��� ���� ����
    float h = 0.0f;
    float v = 0.0f;

    float moveSpeed = 20.0f;        //�̵��ӵ�
    Vector3 moveDir = Vector3.zero; //�̵�����

    //ȸ���� ���� ����
    float rotSpeed = 350.0f;
    Vector3 m_CacVec = Vector3.zero;

    //������ ���� ����
    Rigidbody rigid;
    float jumpPower = 1000.0f;
    int m_ReserveJump;



    void Start()
    {
        state = Moving.Stop;
        this.rigid = GetComponent<Rigidbody>();
    }


    void Update()
    {
        //�̵� ��ư ���� �� ���� ��ȯ
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            state = Moving.IsMoving;
        else
            state = Moving.Stop;

        //ī�޶� ȸ��
        if (Input.GetMouseButton(1) == true)    
        {
            m_CacVec = transform.eulerAngles; // 0~359��
            //Mouse X : ���콺���¿�� �����̴� �� ����
            m_CacVec.y += (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse X"));
            m_CacVec.x -= (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse Y"));

            if (180.0f < m_CacVec.x && m_CacVec.x < 330.0f)
                m_CacVec.x = 330.0f;
            if (15.0f < m_CacVec.x && m_CacVec.x <= 180.0f)
                m_CacVec.x = 15.0f;

            transform.eulerAngles = m_CacVec;
        }
        //�̵� ���� GetAxis�� float���̱� ������ �̵��ÿ� �����̳� ������ ��. 
        //���� �������� ������
        h = Input.GetAxis("Horizontal");     // -1.0 ~ 1.0f; 
        v = Input.GetAxis("Vertical");       // -1.0 ~ 1.0f;

        //�����¿� �̵� ���� ���� ���
        moveDir = (Vector3.forward * v) + (Vector3.right * h); //���ʹ������θ� �����̸� ���⺤�Ͱ� 1 
        if (1.0f < moveDir.magnitude)   //�밢������ ������ �� ���⺤�Ͱ� 1�� �ǰԲ� �ϴ� �ڵ�
            moveDir.Normalize();

        //Translate(�̵����� * Time.deltaTime * �ӵ�, ������ǥ��);
        transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);

        ////������ǥ�� ����
        //moveDir = (transform.forward * v) + (transform.right * h);
        //if (1.0f < moveDir.magnitude)
        //    moveDir.Normalize();
        //transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.World);

        if (this.transform.position.y <= 5.2f)
        {
            float a_CacPosY = GameManager.Inst.m_RefMap.SampleHeight(transform.position);
            transform.position = new Vector3(transform.position.x, 5 + a_CacPosY, transform.position.z);
        }
        //���� ����
        //�ѹ� �����̽��� ������ �� 3���� �����ӵ��� ������ ������ų ��ȸ�� ��
        if (Input.GetKeyDown(KeyCode.Space) == true) //�����̽��� ������ ������ ����
        {
            m_ReserveJump = 3;
        }

        //����, ��Ȯ�� velocity�� 0�� �������� ���� ��쿡 ����Ͽ�....
        if ((0 < m_ReserveJump) &&
            (4.95f <= this.rigid.transform.position.y && this.transform.position.y <= 5.2f))// && this.rigid2D.velocity.y == 0) //������ ����� ��
        {
            this.rigid.velocity = new Vector3(rigid.velocity.x, 0.0f, rigid.velocity.z);
            this.rigid.AddForce(transform.up * this.jumpPower);  //���� ���ϴ� �Լ�

            m_ReserveJump = 0;
        }

        if (0 < m_ReserveJump)
        {
            m_ReserveJump--;
        }
    }
}
