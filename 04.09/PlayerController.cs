using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid2D;
    Animator animator;
    float jumpForce = 680.0f;
    float walkForce = 30.0f;
    float maxWalkSpeed = 2.0f;
    float m_posX;

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        this.rigid2D = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
    }

 
    void Update()
    {
        //����
        if (Input.GetKeyDown(KeyCode.Space) && this.rigid2D.velocity.y == 0)
        {
            this.animator.SetTrigger("JumpTrigger");
            //Vector3.up
            this.rigid2D.AddForce(transform.up*this.jumpForce);  //���� ���ϴ� �Լ�
        }

        //�¿� �̵�
        int key = 0;
        if (Input.GetKey(KeyCode.RightArrow))
            key = 1;
        if (Input.GetKey(KeyCode.LeftArrow))
            key = -1;

        //�÷��̾��� �ӵ�
        float speedx = Mathf.Abs(this.rigid2D.velocity.x);

        //���ǵ� ����
        if (speedx < this.maxWalkSpeed)  //velocity�� ���� 2�� ���� ������ ���� �ְڴ�.
        {
            this.rigid2D.AddForce(transform.right * key * this.walkForce);
        }

        //�����̴� ���⿡ ���� �̹��� ����
        if (key != 0)
        {
            transform.localScale = new Vector3(key, 1, 1); // key���� 1 or -1
        }

        //�÷��̾� �ӵ��� ���� �ִϸ��̼� �ӵ��� �ٲ۴�.
        if (this.rigid2D.velocity.y == 0)
        {
            this.animator.speed = speedx / 2.0f;    //speedx ������� �̵��ӵ�/�ִ�ӵ�
        }
        else
        {
            this.animator.speed = 1.0f;
        }
        


        //�÷��̾ ȭ�� ������ �����ٸ� ó������
        if (transform.position.y < -10)
        {
            SceneManager.LoadScene("GameScene");
        }

        m_posX = transform.position.x;
        //�÷��̾ ��׶��� �̹��� ������ ������ ���ϵ���
        if (m_posX <= -2.65f)
        {
            m_posX = 2.65f;
            transform.position = new Vector3(-m_posX, transform.position.y, 0);
        }
        else if (2.65f <= m_posX)
        {
            m_posX = 2.65f;
            transform.position = new Vector3(m_posX, transform.position.y, 0);
        }
    }

    //�� ����
    private void OnTriggerEnter2D(Collider2D coll)
    {
        SceneManager.LoadScene("ClearScene");
    }
}
