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

    int m_ReserveJump = 0;      //점프 선입력 변수

    float hp = 3.0f;
    public Image[] HpImage;

    GameObject m_OverlapBlock = null; //보상이나 화살 두세번 연속 충돌 방지용 변수

    //대쉬관련 변수들
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
        //한번 스페이스를 눌렀을 때 3번의 프레임동안 점프에 성공시킬 기회를 줌
        if (Input.GetKeyDown(KeyCode.Space) == true) //스페이스를 누르면 변수값 충전
        {
            m_ReserveJump = 3;
        }

        //점프, 정확히 velocity가 0에 떨어지지 않을 경우에 대비하여....
        if ((0 < m_ReserveJump) &&
            (-0.05f <= this.rigid2D.velocity.y && this.rigid2D.velocity.y <= 0.02f))// && this.rigid2D.velocity.y == 0) //구름에 닿았을 때
        {
            this.animator.SetTrigger("JumpTrigger");
            this.rigid2D.velocity = new Vector2(rigid2D.velocity.x, 0.0f);
            this.rigid2D.AddForce(transform.up*this.jumpForce);  //힘을 가하는 함수
            
            m_ReserveJump = 0;
        }

        if(0 < m_ReserveJump)
        {
            m_ReserveJump--;
        }

        //바닥에 닿았을 경우
        if (this.rigid2D.velocity.y == 0)
        {
            this.animator.SetTrigger("WalkTrigger");
        }

        //좌우 이동
        int key = 0;
        if (Input.GetKey(KeyCode.RightArrow))
            key = 1;
        if (Input.GetKey(KeyCode.LeftArrow))
            key = -1;

        //플레이어의 속도
        float speedx = Mathf.Abs(this.rigid2D.velocity.x);

        ////스피드 제한
        //if (speedx < this.maxWalkSpeed)  //velocity의 값이 2를 넘지 않으면 힘을 주겠다.
        //{
        //    this.rigid2D.AddForce(transform.right * key * this.walkForce);
        //}

        //캐릭터 이동
        rigid2D.velocity = new Vector2((key * walkSpeed), rigid2D.velocity.y);

        //움직이는 방향에 따라 이미지 반전
        if (key != 0)
        {
            transform.localScale = new Vector3(key, 1, 1); // key값은 1 or -1
        }

        //플레이어 속도에 맞춰 애니메이션 속도를 바꾼다.
        if (this.rigid2D.velocity.y == 0)
        {
            this.animator.speed = speedx / 2.0f;    //speedx 고양이의 이동속도/최대속도
        }
        else
        {
            this.animator.speed = 1.0f;
        }

        //대쉬 구현
        if (Input.GetKeyDown(KeyCode.D))
            isDash = true;

        if (dashDelayTime < dashDelay)        //dashDelay == 3.0f 쿨타임 3초
        {
            dashDelayTime += Time.deltaTime;
            isDash = false;
        }
        else //쿨타임이 돌지 않는동안
        {
            DashFlicker();

            if (isDash == true)
            {
                dashParticle.Play();          //잔상 이펙트 플레이
                dashTime += Time.deltaTime;   //dash 발동 시간
                if (dashTime <= dashMaxTime)   //dashMaTime == 0.1초 
                    this.rigid2D.velocity = transform.up * dashPower;
                //this.rigid2D.velocity = transform.right * transform.localScale.x * dashPower;
                //key값에 따라 해당 방향에 dashPower만큼 세게 밀어주기
                else
                {
                    isDash = false;
                    dashDelayTime = 0.0f;     //dash 쿨타임 초기화
                    dashTime = 0.0f;          //dash 발동시간 초기화
                    dashGaugeImg.color = new Color(dashGaugeImg.color.r, dashGaugeImg.color.g, dashGaugeImg.color.b, 1.0f);
                }
            }
        }
        dashGaugeImg.fillAmount = dashDelayTime / dashDelay;


        //플레이어가 화면 밖으로 나간다면 처음으로
        if (transform.position.y < -10)
        {
            SceneManager.LoadScene("GameScene");
        }

        m_posX = transform.position.x;
        //플레이어가 백그라운드 이미지 밖으로 나가지 못하도록
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
        //선생님 풀이
        Vector3 pos = transform.position;
        if (pos.x < -2.65f) pos.x = -2.65f;
        if (pos.x > 2.65f) pos.x = 2.65f;
        transform.position = pos;
    }

    //골 도착
    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.gameObject.name.Contains("flag") == true)
        {
            SceneManager.LoadScene("ClearScene");
        }
        else if (coll.gameObject.name.Contains("WaterRoot") == true)
        {
            //물과 충돌
            Die();
        }
        else if (coll.gameObject.name.Contains("arrowPrefab") == true)
        {
            if (m_OverlapBlock != coll.gameObject) //같은 오브젝트에 두번 충돌되는 것 방지
            {
                hp -= 1.0f;                //플레이어 체력 감소
                HpImgUpdate();             //UI업데이트
                if (hp <= 0.0f)            //사망
                {
                    Die();
                }

                m_OverlapBlock = coll.gameObject;
            }
            //화살과 충돌
            Destroy(coll.gameObject); //삭제되는 시점이 쓰인 위치보다 느려
            //DestroyImmediate(coll.gameObject); //그 시점에서 그 즉시 오브젝트 삭제
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
            if (a_CacHp < 0.0f) // 체력이 없는 경우
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

        //RGB값은 100% 투명도만 조정
        dashGaugeImg.color = new Color(dashGaugeImg.color.r, dashGaugeImg.color.g, dashGaugeImg.color.b,
            dashGaugeImg.color.a + alpha * Time.deltaTime);
    }
}
