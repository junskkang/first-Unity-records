using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero_Ctrl : MonoBehaviour
{
    //--- Ű���� �̵� ���� ���� ����
    float h, v;                 //Ű���� �Է°��� �ޱ� ���� ����
    float m_MoveSpeed = 2.8f;   //�ʴ� 2.8m �̵� �ӵ�

    Vector3 m_DirVec;           //�̵��Ϸ��� ���� ���� ����
    Rigidbody2D rigid2D;
    //--- Ű���� �̵� ���� ���� ����

    Animator m_Anim;            //Animator ������Ʈ�� ������ ����

    //�Ѿ� �߻縦 ���� ����
    public GameObject bulletPrefab;
    //float shootCool = 0.0f;

    //ü�� ���� ����
    public Image Hpbar;
    float maxHp = 200.0f;
    [HideInInspector] public float curHp = 200.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_Anim = GetComponentInChildren<Animator>();    //Animator ������Ʈ ã�ƿ���...
        rigid2D = GetComponent<Rigidbody2D>();          //Rigidbody2D ������Ʈ ã�ƿ���
    }

    // Update is called once per frame
    void Update()
    {
        if (curHp <= 0.0f)
            return;

        KeyBDUpdate();
        ChangeAnimation();
        //FireUpdate();
    }

    void KeyBDUpdate()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if(h != 0 || v != 0) //�̵� Ű���带 �����ϰ� ������...
        {
            //m_DirVec = (transform.right * h) + (transform.up * v);

            m_DirVec = (Vector3.right * h) + (Vector3.up * v);
            if (1.0f < m_DirVec.magnitude)
                m_DirVec.Normalize();

            //transform.position += (m_DirVec * m_MoveSpeed * Time.deltaTime);

            rigid2D.velocity = (m_DirVec * m_MoveSpeed);
        }
        else  // ���� ���� ��
        {
            m_DirVec = Vector3.zero;
            rigid2D.velocity = (m_DirVec * m_MoveSpeed);
        }
    }//void KeyBDUpdate()

    void ChangeAnimation()
    {
        if(0.01f < m_DirVec.magnitude) //�̵� ���� ��
        {
            if(Mathf.Abs(m_DirVec.x) > Mathf.Abs(m_DirVec.y))  //�¿� �̵�
            {
                if (m_DirVec.x > 0) //���������� �̵� ���� ��
                    m_Anim.Play("Warrior_Right_Walk");
                else  //�������� �̵� ���� ��
                    m_Anim.Play("Warrior_Left_Walk");
            }
            else  //���� �̵�
            {
                if (m_DirVec.y < 0)
                    m_Anim.Play("Warrior_Front_Walk");
                else
                    m_Anim.Play("Warrior_Back_Walk");
            }

            m_Anim.speed = 0.8f;    //�ִϸ��̼� �÷��� �ӵ� ����  
        }
        else  //���� ���� ��
        {
            m_Anim.speed = 0.0f;    //�ִϸ��̼� �ӵ��� 0���� �����Ͽ� ����
        }
    }

    //void fireupdate()
    //{
    //    if (0.0f < shootcool)
    //        shootcool -= time.deltatime;

    //    if (input.getmousebutton(0))
    //    {
    //        if (shootcool <= 0.0f)
    //        {
    //            //�߻��ֱ� ����
    //            shootcool = 0.3f;

    //            //���콺 ��ǥ�� ������ǥ�� ��ȯ
    //            vector3 targetpostion = input.mouseposition;
    //            targetpostion = camera.main.screentoworldpoint(targetpostion);
    //            targetpostion.z = 0.0f;

    //            //���� ��ġ���� ���콺�� ���ϴ� ���⺤�� ���ϱ�
    //            vector3 dirv = targetpostion - transform.position; 
    //            dirv.normalize();

    //            //�Ѿ� ���� �� ��ǥ �Ѱ��ֱ�
    //            gameobject bullet = instantiate(bulletprefab);
    //            bulletctrl bulletctrl = bullet.getcomponent<bulletctrl>();
    //            bulletctrl.bulletspawn(transform.position, dirv);

    //            //�Ѿ� ȸ��
    //            bullet.transform.right = new vector3(dirv.x, dirv.y, 0.0f);

    //        }
    //    }
    //}

    public void TakeDamage(float damage = 10.0f)
    {
        if (curHp <= 0.0f)
            return;

        curHp -= damage;

        if (curHp <= 0.0f)
            curHp = 0.0f;

        if(Hpbar != null)
            Hpbar.fillAmount = curHp/maxHp;

        if (curHp <= 0.0f)
        {
            //���ó��
            Time.timeScale = 0.0f;
        }
    }
}
