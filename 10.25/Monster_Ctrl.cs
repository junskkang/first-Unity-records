using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterType
{
    normal,
    boss
}

public class Monster_Ctrl : MonoBehaviour
{
    MonsterType type = MonsterType.normal;

    //플레이어를 추적하도록 하기 위한 변수
    Hero_Ctrl refHero;  //추적하게 될 대상
    Vector3 curPos;
    Vector3 dirVec;
    int monsterType = -1;

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

    //피격 연출
    SpriteRenderer spriteRenderer;
    Color hitColor = new Color(1.0f, 0.5f, 0.5f);
    Color originColor = Color.white;
    float hitTimer = 0.0f;

    //골드값
    int money = 1;

    //라운드별 스탯 가중치
    int roundAtt = 10;
    float addSpeed = 0.0f;
    float addHp = 1.0f;
    bool isRocked = false;
    [HideInInspector] public bool isFlying = false;

    //디버프 관련 변수
    [HideInInspector] public bool isBewitched = false;
    [HideInInspector] public GameObject whosBewitch = null;
    [HideInInspector] public float bewitchedSpeed = 0.0f;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        switch (gameObject.name)
        {
            case "Monster_1(Clone)":
                monsterType = 1;
                break;
            case "Monster_2(Clone)":
                monsterType = 2;
                break;
            case "BossMonster(Clone)":
                monsterType = 1;
                type = MonsterType.boss;
                break;
        }

        RoundAttribute();

        refHero = GameObject.FindObjectOfType<Hero_Ctrl>();
        rigid2D = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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
        if (NotifyCtrl.isNotify) return;

        WayPointMove();
        ChangeAnimation();

        if (hitTimer > 0.0f)
        {
            hitTimer -= Time.deltaTime;
            spriteRenderer.color = hitColor;
        }
        else if (hitTimer <= 0.0f)
        {
            hitTimer = 0.0f;
            spriteRenderer.color = originColor;
        }        
    }
    void WayPointMove()
    {
        toNextPoint = wayPoint[wayPointIdx].position - transform.position;

        if (wayPointIdx == (wayPoint.Length - 1) && toNextPoint.magnitude < 0.1f)
        {
            //도착판정
            if (type == MonsterType.boss)
            {
                wayPointIdx = 1;
                transform.position = wayPoint[wayPointIdx].position;
                
                //목숨 깎기
                refHero.TakeDamage(5);
            }
            else if (type == MonsterType.normal)
            {
                MonsterGenerator.inst.curMonCount--;
                //목숨깎기
                refHero.TakeDamage();
                Destroy(this.gameObject);
            }            
            
            return;
        }
        else if (toNextPoint.magnitude < 0.1f)
            wayPointIdx++;

        //toNextPoint.Normalize();
        //transform.rotation = Quaternion.LookRotation(transform.forward, toNextPoint);

        float speed = 0.0f;

        speed = isBewitched ? bewitchedSpeed * moveSpeed : moveSpeed;


        transform.position = Vector3.MoveTowards(transform.position, wayPoint[wayPointIdx].position, speed * Time.deltaTime);
    }
    void ChangeAnimation()
    {
        if (type == MonsterType.normal)
        {         
            if (0.01f < toNextPoint.magnitude) //이동 중일 때
            {
                if (Mathf.Abs(toNextPoint.x) > Mathf.Abs(toNextPoint.y))  //좌우 이동
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
        else if (type == MonsterType.boss)
        {
            if (0.01f < toNextPoint.magnitude) //이동 중일 때
            {
                if (Mathf.Abs(toNextPoint.x) > Mathf.Abs(toNextPoint.y))  //좌우 이동
                {
                    if (toNextPoint.x > 0) //오른쪽으로 이동 중일 때
                        anim.Play($"Boss{monsterType}_Right_Walk");
                    else  //왼쪽으로 이동 중일 때
                        anim.Play($"Boss{monsterType}_Left_Walk");
                }
                else  //상하 이동
                {
                    if (toNextPoint.y < 0)
                        anim.Play($"Boss{monsterType}_Front_Walk");
                    else
                        anim.Play($"Boss{monsterType}_Back_Walk");
                }

                anim.speed = 0.6f;    //애니메이션 플레이 속도 조절  
            }
            else  //멈춰 있을 때
            {
                anim.speed = 0.0f;    //애니메이션 속도를 0으로 설정하여 멈춤
            }
        }
    }

    public void TakeDamage(float value, GameObject whosAttack)
    {
        if (curHp < 0) return;

        curHp -= isRocked? 1 : value;
        hitTimer = 0.1f;

        if (GameManager.Inst != null)
            GameManager.Inst.DamageText(-(int)value, this.transform.position, Color.red);

        Hpbar.fillAmount = curHp / maxHp;

        if (curHp < 0) curHp = 0;

        if (curHp <= 0)
        {
            //사망
            
            MonsterGenerator.inst.curMonCount--;
            whosAttack.GetComponent<AllyUnit>().monKill++;
            //if (whosAttack.GetComponent<AllyUnit>().monKill % 10 == 0)
            //    whosAttack.GetComponent<AllyUnit>().Levelup();
            GameManager.Inst.GetGold(money);
            Destroy(gameObject);
        }
    }
    void RoundAttribute()
    {
        //라운드별 몬스터 특성 살리기
        if (GameManager.Inst.round > 45)
            roundAtt = 1;
        else if (GameManager.Inst.round > 40)
            roundAtt = 2;
        else if (GameManager.Inst.round > 35)
            roundAtt = 3;
        else if (GameManager.Inst.round > 30)
            roundAtt = 4;
        else if (GameManager.Inst.round > 25)
            roundAtt = 5;
        else if (GameManager.Inst.round > 20)
            roundAtt = 6;
        else if (GameManager.Inst.round > 15)
            roundAtt = 7;
        else if (GameManager.Inst.round > 10)
            roundAtt = 8;
        else if (GameManager.Inst.round > 5)
            roundAtt = 9;

        if(GameManager.Inst.round > 5)
        {        
            if (GameManager.Inst.round % 5 == 1)      //이속이 빠른 라운드
            {
                addHp = 0.8f;
                addSpeed = 2.5f;
                isFlying = true;
            }
            else if (GameManager.Inst.round % 5 == 3) //체력이 많은 라운드   
            {
                addHp = 2.0f;
                addSpeed *= 0.7f;
            }
            else if (GameManager.Inst.round % 5 == 4) //체력은 낮지만 모든 데미지가 1씩만 들어오는 라운드
            { 
                isRocked = true;
            }
        }

        if (type == MonsterType.normal)
        {
            if (GameManager.Inst.round == 1) return;

            moveSpeed = moveSpeed * (1 + (float)GameManager.Inst.round / (roundAtt * 10) + addSpeed);

            maxHp = isRocked ? GameManager.Inst.round : (maxHp * (1 + ((float)GameManager.Inst.round / roundAtt))) * addHp;
            curHp = maxHp;

            money = money * (1 + (int)GameManager.Inst.round);
        }
        else if (type == MonsterType.boss)
        {
            moveSpeed = moveSpeed * (1 + (float)GameManager.Inst.round / (roundAtt * 5));

            maxHp = maxHp * (10 * (float)GameManager.Inst.round / roundAtt * addHp);
            curHp = maxHp;

            money = money * (10 + (int)GameManager.Inst.round);
        }        

        Debug.Log($"{GameManager.Inst.round}라운드 [{type}]체력 : {curHp}, 이동속도 : {moveSpeed}");
    }
    //private void OnCollisionEnter2D(Collision2D coll)
    //{
    //    if (coll.gameObject.name.Contains("Hero"))
    //    {
    //        MonsterGenerator.inst.curMonCount--;
    //        Destroy(this.gameObject);
    //        refHero.TakeDamage(10.0f);
    //    }
    //}

    IEnumerator BossPatern()
    {
        yield break;
    }
}
