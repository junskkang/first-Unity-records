using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero_Ctrl : MonoBehaviour
{
    //--- 키보드 이동 관련 변수 선언
    float h, v;                 //키보드 입력값을 받기 위한 변수
    float m_MoveSpeed = 2.8f;   //초당 2.8m 이동 속도

    Vector3 m_DirVec;           //이동하려는 방향 벡터 변수
    Rigidbody2D rigid2D;
    //--- 키보드 이동 관련 변수 선언

    Animator m_Anim;            //Animator 컴포넌트를 참조할 변수

    //총알 발사를 위한 변수
    public GameObject bulletPrefab;
    float shootCool = 0.0f;

    //체력 관련 변수
    public Image Hpbar;
    float maxHp = 200.0f;
    [HideInInspector] public float curHp = 200.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_Anim = GetComponentInChildren<Animator>();    //Animator 컴포넌트 찾아오기...
        rigid2D = GetComponent<Rigidbody2D>();          //Rigidbody2D 컴포넌트 찾아오기
    }

    // Update is called once per frame
    void Update()
    {
        if (curHp <= 0.0f)
            return;

        KeyBDUpdate();
        ChangeAnimation();
        FireUpdate();
    }

    void KeyBDUpdate()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if(h != 0 || v != 0) //이동 키보드를 조작하고 있으면...
        {
            //m_DirVec = (transform.right * h) + (transform.up * v);

            m_DirVec = (Vector3.right * h) + (Vector3.up * v);
            if (1.0f < m_DirVec.magnitude)
                m_DirVec.Normalize();

            //transform.position += (m_DirVec * m_MoveSpeed * Time.deltaTime);

            rigid2D.velocity = (m_DirVec * m_MoveSpeed);
        }
        else  // 멈춰 있을 때
        {
            m_DirVec = Vector3.zero;
            rigid2D.velocity = (m_DirVec * m_MoveSpeed);
        }
    }//void KeyBDUpdate()

    void ChangeAnimation()
    {
        if(0.01f < m_DirVec.magnitude) //이동 중일 때
        {
            if(Mathf.Abs(m_DirVec.x) > Mathf.Abs(m_DirVec.y))  //좌우 이동
            {
                if (m_DirVec.x > 0) //오른쪽으로 이동 중일 때
                    m_Anim.Play("Warrior_Right_Walk");
                else  //왼쪽으로 이동 중일 때
                    m_Anim.Play("Warrior_Left_Walk");
            }
            else  //상하 이동
            {
                if (m_DirVec.y < 0)
                    m_Anim.Play("Warrior_Front_Walk");
                else
                    m_Anim.Play("Warrior_Back_Walk");
            }

            m_Anim.speed = 0.8f;    //애니메이션 플레이 속도 조절  
        }
        else  //멈춰 있을 때
        {
            m_Anim.speed = 0.0f;    //애니메이션 속도를 0으로 설정하여 멈춤
        }
    }

    void FireUpdate()
    {
        if (0.0f < shootCool)
            shootCool -= Time.deltaTime;

        if (Input.GetMouseButton(0))
        {
            if (shootCool <= 0.0f)
            {
                //발사주기 생성
                shootCool = 0.3f;

                //마우스 좌표를 월드좌표로 변환
                Vector3 targetPostion = Input.mousePosition;
                targetPostion = Camera.main.ScreenToWorldPoint(targetPostion);
                targetPostion.z = 0.0f;

                //현재 위치에서 마우스를 향하는 방향벡터 구하기
                Vector3 dirV = targetPostion - transform.position; 
                dirV.Normalize();

                //총알 생성 및 좌표 넘겨주기
                GameObject bullet = Instantiate(bulletPrefab);
                BulletCtrl bulletCtrl = bullet.GetComponent<BulletCtrl>();
                bulletCtrl.BulletSpawn(transform.position, dirV);

                //총알 회전
                bullet.transform.right = new Vector3(dirV.x, dirV.y, 0.0f);

            }
        }
    }

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
            //사망처리
            Time.timeScale = 0.0f;
        }
    }
}
