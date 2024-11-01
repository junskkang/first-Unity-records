using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster_Ctrl : MonoBehaviour
{
    //�÷��̾ �����ϵ��� �ϱ� ���� ����
    Hero_Ctrl refHero;  //�����ϰ� �� ���
    Vector3 curPos;
    Vector3 dirVec;
    int monsterType = 0;

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

    //����� ���� ����
    [HideInInspector] public bool isBewitched = false;
    [HideInInspector] public GameObject whosBewitch = null;
    [HideInInspector] public float bewitchedSpeed = 0.0f; 

    // Start is called before the first frame update
    void Start()
    {
        refHero = GameObject.FindObjectOfType<Hero_Ctrl>();
        rigid2D = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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
            MonsterGenerator.inst.curMonCount--;
            Destroy(this.gameObject);
            //Debug.Log(wayPointIdx);
            return;
        }
        else if (toNextPoint.magnitude < 0.1f)
            wayPointIdx++;

        //toNextPoint.Normalize();
        //transform.rotation = Quaternion.LookRotation(transform.forward, toNextPoint);
        float speed = isBewitched ? bewitchedSpeed : moveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, wayPoint[wayPointIdx].position, speed * Time.deltaTime);
    }
    void ChangeAnimation()
    {
         if(0.01f < toNextPoint.magnitude) //�̵� ���� ��
        {
            if(Mathf.Abs(toNextPoint.x) > Mathf.Abs(toNextPoint.y))  //�¿� �̵�
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

    public void TakeDamage(float value, GameObject whosAttack)
    {
        if (curHp < 0) return;

        curHp -= value;
        hitTimer = 0.1f;
        Hpbar.fillAmount = curHp / maxHp;

        if (curHp < 0) curHp = 0;

        if (curHp <= 0)
        {
            //���
            
            MonsterGenerator.inst.curMonCount--;
            whosAttack.GetComponent<AllyUnit>().monKill++;
            if (whosAttack.GetComponent<AllyUnit>().monKill % 10 == 0)
                whosAttack.GetComponent<AllyUnit>().Levelup();

            Destroy(gameObject);
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
}
