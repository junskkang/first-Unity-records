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
    //float shootCool = 0.0f;

    //체력 관련 변수
    public Image Hpbar;
    float maxHp = 20.0f;
    [HideInInspector] public float curHp = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_Anim = GetComponentInChildren<Animator>();    //Animator 컴포넌트 찾아오기...
        rigid2D = GetComponent<Rigidbody2D>();          //Rigidbody2D 컴포넌트 찾아오기

        curHp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (NotifyCtrl.isNotify) return;

        if (curHp <= 0.0f)
            return;

        KeyBDUpdate();
        ChangeAnimation();
        //FireUpdate();
    }

    void KeyBDUpdate()
    {
        if (CameraResolution.isZoom == true) return;

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

            Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

            pos.x = Mathf.Clamp(pos.x, 0.03f, 0.97f);
            pos.y = Mathf.Clamp(pos.y, 0.07f, 0.95f);

            Vector3 converterToWorld = Camera.main.ViewportToWorldPoint(pos);
            converterToWorld.z = transform.position.z;
            transform.position = converterToWorld;
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

    //void fireupdate()
    //{
    //    if (0.0f < shootcool)
    //        shootcool -= time.deltatime;

    //    if (input.getmousebutton(0))
    //    {
    //        if (shootcool <= 0.0f)
    //        {
    //            //발사주기 생성
    //            shootcool = 0.3f;

    //            //마우스 좌표를 월드좌표로 변환
    //            vector3 targetpostion = input.mouseposition;
    //            targetpostion = camera.main.screentoworldpoint(targetpostion);
    //            targetpostion.z = 0.0f;

    //            //현재 위치에서 마우스를 향하는 방향벡터 구하기
    //            vector3 dirv = targetpostion - transform.position; 
    //            dirv.normalize();

    //            //총알 생성 및 좌표 넘겨주기
    //            gameobject bullet = instantiate(bulletprefab);
    //            bulletctrl bulletctrl = bullet.getcomponent<bulletctrl>();
    //            bulletctrl.bulletspawn(transform.position, dirv);

    //            //총알 회전
    //            bullet.transform.right = new vector3(dirv.x, dirv.y, 0.0f);

    //        }
    //    }
    //}

    public void TakeDamage(float damage = 1f)
    {
        if (curHp <= 0.0f)
            return;

        curHp -= damage;

        if (curHp <= 0.0f)
            curHp = 0.0f;

        if(Hpbar != null)
            Hpbar.fillAmount = curHp/maxHp;

        Debug.Log("남은 체력 : " + curHp);
        if (curHp <= 0.0f)
        {
            //사망처리
            StartCoroutine(GameManager.Inst.GameOver());
            Time.timeScale = 0.0f;            
        }
    }
}
