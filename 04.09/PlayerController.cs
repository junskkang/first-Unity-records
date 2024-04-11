using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigid2D;
    Animator animator;
    float jumpForce = 680.0f;
    //float walkForce = 30.0f;
    //float maxWalkSpeed = 2.0f;
    float walkSpeed = 3.0f;
    float m_posX;

    int m_ReserveJump = 0;      //���� ���Է� ����

    float hp = 3.0f;
    public Image[] HpImage;

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        this.rigid2D = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
    }

 
    void Update()
    {
        //�ѹ� �����̽��� ������ �� 3���� �����ӵ��� ������ ������ų ��ȸ�� ��
        if (Input.GetKeyDown(KeyCode.Space) == true) //�����̽��� ������ ������ ����
        {
            m_ReserveJump = 3;
        }

        //����, ��Ȯ�� velocity�� 0�� �������� ���� ��쿡 ����Ͽ�....
        if ((0 < m_ReserveJump) &&
            (-0.02f <= this.rigid2D.velocity.y && this.rigid2D.velocity.y <= 0.02f))// && this.rigid2D.velocity.y == 0) //������ ����� ��
        {
            this.animator.SetTrigger("JumpTrigger");
            this.rigid2D.velocity = new Vector2(rigid2D.velocity.x, 0.0f);
            this.rigid2D.AddForce(transform.up*this.jumpForce);  //���� ���ϴ� �Լ�
            
            m_ReserveJump = 0;
        }

        if(0 < m_ReserveJump)
        {
            m_ReserveJump--;
        }

        //�ٴڿ� ����� ���
        if (this.rigid2D.velocity.y == 0)
        {
            this.animator.SetTrigger("WalkTrigger");
        }

        //�¿� �̵�
        int key = 0;
        if (Input.GetKey(KeyCode.RightArrow))
            key = 1;
        if (Input.GetKey(KeyCode.LeftArrow))
            key = -1;

        //�÷��̾��� �ӵ�
        float speedx = Mathf.Abs(this.rigid2D.velocity.x);

        ////���ǵ� ����
        //if (speedx < this.maxWalkSpeed)  //velocity�� ���� 2�� ���� ������ ���� �ְڴ�.
        //{
        //    this.rigid2D.AddForce(transform.right * key * this.walkForce);
        //}

        //ĳ���� �̵�
        rigid2D.velocity = new Vector2((key * walkSpeed), rigid2D.velocity.y);

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
        //������ Ǯ��
        Vector3 pos = transform.position;
        if (pos.x < -2.65f) pos.x = -2.65f;
        if (pos.x > 2.65f) pos.x = 2.65f;
        transform.position = pos;
    }

    //�� ����
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.name.Contains("flag") == true)
        {
            SceneManager.LoadScene("ClearScene");
        }
        else if (coll.gameObject.name.Contains("WaterRoot") == true)
        {
            //���� �浹
            Die();
        }
        else if (coll.gameObject.name.Contains("arrowPrefab") == true)
        {
            //ȭ��� �浹
            Destroy(coll.gameObject); //ȭ�� ����

            //�÷��̾� ü�� ����
        }
    }

    void Die()
    {
        SceneManager.LoadScene("ClearScene");
    }
}
