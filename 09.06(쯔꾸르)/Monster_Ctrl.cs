using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster_Ctrl : MonoBehaviour
{
    //플레이어를 추적하도록 하기 위한 변수
    Hero_Ctrl refHero;  //추적하게 될 대상
    float speed = 1.0f; //이동속도
    Vector3 curPos;
    Vector3 dirVec;
    int monsterType = 0;

    //rigidbody로 이동시키기 위한 변수
    Rigidbody2D rigid2D;

    Animator anim;      //애니메이션 변경을 위한 변수

    //체력 관련 변수
    public Image Hpbar;

    // Start is called before the first frame update
    void Start()
    {
        refHero = GameObject.FindObjectOfType<Hero_Ctrl>();
        rigid2D = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        switch (gameObject.name)
        {
            case "Monster_1":
                monsterType = 1;
                break;
            case "Monster_2":
                monsterType = 2;
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (refHero != null)
        {
            dirVec = refHero.transform.position - transform.position;
            dirVec.Normalize();

            //transform.position += (dirVec * speed * Time.deltaTime);
            rigid2D.velocity = dirVec * speed;
        }
        else
        {
            dirVec = Vector3.zero;
            rigid2D.velocity = Vector3.zero;
        }

        ChangeAnimation();
    }
    void ChangeAnimation()
    {
         if(0.01f < dirVec.magnitude) //이동 중일 때
        {
            if(Mathf.Abs(dirVec.x) > Mathf.Abs(dirVec.y))  //좌우 이동
            {
                if (dirVec.x > 0) //오른쪽으로 이동 중일 때
                    anim.Play($"Mon{monsterType}_Right_Walk");
                else  //왼쪽으로 이동 중일 때
                    anim.Play($"Mon{monsterType}_Left_Walk");
            }
            else  //상하 이동
            {
                if (dirVec.y < 0)
                    anim.Play($"Mon{monsterType}_Front_Walk");
                else
                    anim.Play($"Mon{monsterType}_Back_Walk");
            }

            anim.speed = 0.6f;    //애니메이션 플레이 속도 조절  
        }
        else  //멈춰 있을 때
        {
            anim.speed = 0.0f;    //애니메이션 속도를 0으로 설정하여 멈춤
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name.Contains("Hero"))
        {
            Destroy(this.gameObject);
            refHero.TakeDamage(10.0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "AllyBullet")
        {
            Destroy(coll.gameObject);
            Destroy(this.gameObject);

            GameManager.Inst.AddPoint(10);
        }
    }
}
