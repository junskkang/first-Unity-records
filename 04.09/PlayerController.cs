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
        //점프
        if (Input.GetKeyDown(KeyCode.Space) && this.rigid2D.velocity.y == 0)
        {
            this.animator.SetTrigger("JumpTrigger");
            //Vector3.up
            this.rigid2D.AddForce(transform.up*this.jumpForce);  //힘을 가하는 함수
        }

        //좌우 이동
        int key = 0;
        if (Input.GetKey(KeyCode.RightArrow))
            key = 1;
        if (Input.GetKey(KeyCode.LeftArrow))
            key = -1;

        //플레이어의 속도
        float speedx = Mathf.Abs(this.rigid2D.velocity.x);

        //스피드 제한
        if (speedx < this.maxWalkSpeed)  //velocity의 값이 2를 넘지 않으면 힘을 주겠다.
        {
            this.rigid2D.AddForce(transform.right * key * this.walkForce);
        }

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
    }

    //골 도착
    private void OnTriggerEnter2D(Collider2D coll)
    {
        SceneManager.LoadScene("ClearScene");
    }
}
