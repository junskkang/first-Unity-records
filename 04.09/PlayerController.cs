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

    GameObject m_OverlapBlock = null; //�����̳� ȭ�� �μ��� ���� �浹 ������ ����

    //�뽬���� ������
    ParticleSystem dashParticle;
    bool isDash = false;
    float dashPower = 30.0f;
    float dashDelayTime = 0.0f;
    float dashDelay = 3.0f;
    float dashTime = 0.0f;
    float dashMaxTime = 0.1f;
    public Image dashGaugeImg;
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        this.dashParticle = GetComponentInChildren<ParticleSystem>();
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
            (-0.05f <= this.rigid2D.velocity.y && this.rigid2D.velocity.y <= 0.02f))// && this.rigid2D.velocity.y == 0) //������ ����� ��
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

        //�뽬 ����
        if (Input.GetKeyDown(KeyCode.D))
            isDash = true;

        if (dashDelayTime < dashDelay)        //dashDelay == 3.0f ��Ÿ�� 3��
        {
            dashDelayTime += Time.deltaTime;
            isDash = false;
        }
        else //��Ÿ���� ���� �ʴµ���
        {
            DashFlicker();

            if (isDash == true)
            {
                dashParticle.Play();          //�ܻ� ����Ʈ �÷���
                dashTime += Time.deltaTime;   //dash �ߵ� �ð�
                if (dashTime <= dashMaxTime)   //dashMaTime == 0.1�� 
                    this.rigid2D.velocity = transform.up * dashPower;
                //this.rigid2D.velocity = transform.right * transform.localScale.x * dashPower;
                //key���� ���� �ش� ���⿡ dashPower��ŭ ���� �о��ֱ�
                else
                {
                    isDash = false;
                    dashDelayTime = 0.0f;     //dash ��Ÿ�� �ʱ�ȭ
                    dashTime = 0.0f;          //dash �ߵ��ð� �ʱ�ȭ
                    dashGaugeImg.color = new Color(dashGaugeImg.color.r, dashGaugeImg.color.g, dashGaugeImg.color.b, 1.0f);
                }
            }
        }
        dashGaugeImg.fillAmount = dashDelayTime / dashDelay;


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
            if (m_OverlapBlock != coll.gameObject) //���� ������Ʈ�� �ι� �浹�Ǵ� �� ����
            {
                hp -= 1.0f;                //�÷��̾� ü�� ����
                HpImgUpdate();             //UI������Ʈ
                if (hp <= 0.0f)            //���
                {
                    Die();
                }

                m_OverlapBlock = coll.gameObject;
            }
            //ȭ��� �浹
            Destroy(coll.gameObject); //�����Ǵ� ������ ���� ��ġ���� ����
            //DestroyImmediate(coll.gameObject); //�� �������� �� ��� ������Ʈ ����
        }
        else if (coll.gameObject.name.Contains("fish") == true)
        {
            if(m_OverlapBlock != coll.gameObject)
            {
                hp += 0.5f;
                
                if (hp >= 3.0f)
                    hp = 3.0f;
                HpImgUpdate();

                m_OverlapBlock = coll.gameObject;
            }
            Destroy(coll.gameObject);
        }
    }

    void Die()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    void HpImgUpdate()
    {
        float a_CacHp = 0.0f;
        for(int ii = 0; ii < HpImage.Length; ii++)
        {
            a_CacHp = hp - (float)ii;
            if (a_CacHp < 0.0f) // ü���� ���� ���
            {
                a_CacHp = 0.0f;
            }

            if (1.0f < a_CacHp)
                a_CacHp = 1.0f;

            if (0.45f < a_CacHp && a_CacHp < 0.55f)
                a_CacHp = 0.445f;

            HpImage[ii].fillAmount = a_CacHp;
        }
    }
    public float alpha = -6.0f;
    void DashFlicker()
    {
        if (dashGaugeImg == null)
            return;

        if (dashGaugeImg.color.a >= 1.0f)
            alpha = -6.0f;
        else if (dashGaugeImg.color.a <= 0.0f)
            alpha = 6.0f;

        //RGB���� 100% ������ ����
        dashGaugeImg.color = new Color(dashGaugeImg.color.r, dashGaugeImg.color.g, dashGaugeImg.color.b,
            dashGaugeImg.color.a + alpha * Time.deltaTime);
    }
}
