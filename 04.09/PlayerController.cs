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

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

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
            (-0.02f <= this.rigid2D.velocity.y && this.rigid2D.velocity.y <= 0.02f))// && this.rigid2D.velocity.y == 0) //구름에 닿았을 때
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
            //화살과 충돌
            Destroy(coll.gameObject); //화살 삭제

            //플레이어 체력 감소
        }
    }

    void Die()
    {
        SceneManager.LoadScene("ClearScene");
    }
}
