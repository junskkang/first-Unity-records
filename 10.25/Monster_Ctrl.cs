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

    //�÷��̾ �����ϵ��� �ϱ� ���� ����
    Hero_Ctrl refHero;  //�����ϰ� �� ���
    Vector3 curPos;
    Vector3 dirVec;
    int monsterType = -1;

    //��������Ʈ ���� �̵� ����
    public GameObject wayPointParent;
    Transform[] wayPoint;
    int wayPointIdx = 1;
    Vector3 toNextPoint = Vector3.zero;

    float moveSpeed = 1.0f;

    //rigidbody�� �̵���Ű�� ���� ����
    Rigidbody2D rigid2D;

    Animator anim;      //�ִϸ��̼� ������ ���� ����

    //ü�� ���� ����
    public Image Hpbar;
    float curHp = 20;
    float maxHp = 20;

    //�ǰ� ����
    SpriteRenderer spriteRenderer;
    Color hitColor = new Color(1.0f, 0.5f, 0.5f);
    Color originColor = Color.white;
    float hitTimer = 0.0f;

    //��尪
    int money = 1;

    //���庰 ���� ����ġ
    int roundAtt = 10;
    float addSpeed = 0.0f;
    float addHp = 1.0f;
    bool isRocked = false;
    [HideInInspector] public bool isFlying = false;

    //����� ���� ����
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
            //��������
            if (type == MonsterType.boss)
            {
                wayPointIdx = 1;
                transform.position = wayPoint[wayPointIdx].position;
                
                //��� ���
                refHero.TakeDamage(5);
            }
            else if (type == MonsterType.normal)
            {
                MonsterGenerator.inst.curMonCount--;
                //������
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
            if (0.01f < toNextPoint.magnitude) //�̵� ���� ��
            {
                if (Mathf.Abs(toNextPoint.x) > Mathf.Abs(toNextPoint.y))  //�¿� �̵�
                {
                    if (toNextPoint.x > 0) //���������� �̵� ���� ��
                        anim.Play($"Mon{monsterType}_Right_Walk");
                    else  //�������� �̵� ���� ��
                        anim.Play($"Mon{monsterType}_Left_Walk");
                }
                else  //���� �̵�
                {
                    if (toNextPoint.y < 0)
                        anim.Play($"Mon{monsterType}_Front_Walk");
                    else
                        anim.Play($"Mon{monsterType}_Back_Walk");
                }

                anim.speed = 0.6f;    //�ִϸ��̼� �÷��� �ӵ� ����  
            }
            else  //���� ���� ��
            {
                anim.speed = 0.0f;    //�ִϸ��̼� �ӵ��� 0���� �����Ͽ� ����
            }
        }
        else if (type == MonsterType.boss)
        {
            if (0.01f < toNextPoint.magnitude) //�̵� ���� ��
            {
                if (Mathf.Abs(toNextPoint.x) > Mathf.Abs(toNextPoint.y))  //�¿� �̵�
                {
                    if (toNextPoint.x > 0) //���������� �̵� ���� ��
                        anim.Play($"Boss{monsterType}_Right_Walk");
                    else  //�������� �̵� ���� ��
                        anim.Play($"Boss{monsterType}_Left_Walk");
                }
                else  //���� �̵�
                {
                    if (toNextPoint.y < 0)
                        anim.Play($"Boss{monsterType}_Front_Walk");
                    else
                        anim.Play($"Boss{monsterType}_Back_Walk");
                }

                anim.speed = 0.6f;    //�ִϸ��̼� �÷��� �ӵ� ����  
            }
            else  //���� ���� ��
            {
                anim.speed = 0.0f;    //�ִϸ��̼� �ӵ��� 0���� �����Ͽ� ����
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
            //���
            
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
        //���庰 ���� Ư�� �츮��
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
            if (GameManager.Inst.round % 5 == 1)      //�̼��� ���� ����
            {
                addHp = 0.8f;
                addSpeed = 2.5f;
                isFlying = true;
            }
            else if (GameManager.Inst.round % 5 == 3) //ü���� ���� ����   
            {
                addHp = 2.0f;
                addSpeed *= 0.7f;
            }
            else if (GameManager.Inst.round % 5 == 4) //ü���� ������ ��� �������� 1���� ������ ����
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

        Debug.Log($"{GameManager.Inst.round}���� [{type}]ü�� : {curHp}, �̵��ӵ� : {moveSpeed}");
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
