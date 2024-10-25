using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster_Ctrl : MonoBehaviour
{
    //플레이어를 추적하도록 하기 위한 변수
    Hero_Ctrl refHero;  //추적하게 될 대상
    float speed = 5.0f; //이동속도
    Vector3 curPos;
    Vector3 dirVec;
    int monsterType = 0;

    //웨이포인트 관련 이동 변수
    public GameObject wayPointParent;
    Transform[] wayPoint;
    int wayPointIdx = 1;
    Vector3 toNextPoint = Vector3.zero;

    float moveSpeed = 1.0f;

    //rigidbody로 이동시키기 위한 변수
    Rigidbody2D rigid2D;

    Animator anim;      //애니메이션 변경을 위한 변수

    //체력 관련 변수
    public Image Hpbar;
    float curHp = 20;
    float maxHp = 20;

    // Start is called before the first frame update
    void Start()
    {
        refHero = GameObject.FindObjectOfType<Hero_Ctrl>();
        rigid2D = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        switch (gameObject.name)
        {
            case "Monster_1(Clone)":
                monsterType = 1;
                break;
            case "Monster_2(Clone)":
                monsterType = 2;
                break;
        }

        if (wayPointParent == null)
        {
            wayPointParent = GameObject.Find("WayPointParent") as GameObject;
            wayPoint = wayPointParent.GetComponentsInChildren<Transform>();
        }            

        transform.position = wayPoint[wayPointIdx].position;
    }

    // Update is called once per frame
    void Update()
    {
        //if (refHero != null)
        //{
        //    dirVec = refHero.transform.position - transform.position;
        //    dirVec.Normalize();

        //    //transform.position += (dirVec * speed * Time.deltaTime);
        //    rigid2D.velocity = dirVec * speed;
        //}
        //else
        //{
        //    dirVec = Vector3.zero;
        //    rigid2D.velocity = Vector3.zero;
        //}
        WayPointMove();
        ChangeAnimation();
    }
    void WayPointMove()
    {
        toNextPoint = wayPoint[wayPointIdx].position - transform.position;

        if (wayPointIdx == (wayPoint.Length - 1) && toNextPoint.magnitude < 0.1f)
        {
            //도착판정
            MonsterGenerator.inst.curMonCount--;
            Destroy(this.gameObject);
            //Debug.Log(wayPointIdx);
            return;
        }
        else if (toNextPoint.magnitude < 0.1f)
            wayPointIdx++;

        //toNextPoint.Normalize();
        //transform.rotation = Quaternion.LookRotation(transform.forward, toNextPoint);
        transform.position = Vector3.MoveTowards(transform.position, wayPoint[wayPointIdx].position, moveSpeed * Time.deltaTime);
    }
    void ChangeAnimation()
    {
         if(0.01f < toNextPoint.magnitude) //이동 중일 때
        {
            if(Mathf.Abs(toNextPoint.x) > Mathf.Abs(toNextPoint.y))  //좌우 이동
            {
                if (toNextPoint.x > 0) //오른쪽으로 이동 중일 때
                    anim.Play($"Mon{monsterType}_Right_Walk");
                else  //왼쪽으로 이동 중일 때
                    anim.Play($"Mon{monsterType}_Left_Walk");
            }
            else  //상하 이동
            {
                if (toNextPoint.y < 0)
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

    public void TakeDamage(float value, Ally_Ctrl whosAttack)
    {
        if (curHp < 0) return;

        curHp -= value;
        Hpbar.fillAmount = curHp / maxHp;

        if (curHp < 0) curHp = 0;

        if (curHp <= 0)
        {
            //사망
            Destroy(gameObject);
            whosAttack.monKill++;
            if (whosAttack.monKill % 10 == 0)
                whosAttack.Levelup();
        }



    }
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.name.Contains("Hero"))
        {
            MonsterGenerator.inst.curMonCount--;
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
            MonsterGenerator.inst.curMonCount--;
            GameManager.Inst.AddPoint(10);
        }
    }
}
