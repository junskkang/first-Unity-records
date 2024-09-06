using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster_Ctrl : MonoBehaviour
{
    //�÷��̾ �����ϵ��� �ϱ� ���� ����
    Hero_Ctrl refHero;  //�����ϰ� �� ���
    float speed = 1.0f; //�̵��ӵ�
    Vector3 curPos;
    Vector3 dirVec;
    int monsterType = 0;

    //rigidbody�� �̵���Ű�� ���� ����
    Rigidbody2D rigid2D;

    Animator anim;      //�ִϸ��̼� ������ ���� ����

    //ü�� ���� ����
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
         if(0.01f < dirVec.magnitude) //�̵� ���� ��
        {
            if(Mathf.Abs(dirVec.x) > Mathf.Abs(dirVec.y))  //�¿� �̵�
            {
                if (dirVec.x > 0) //���������� �̵� ���� ��
                    anim.Play($"Mon{monsterType}_Right_Walk");
                else  //�������� �̵� ���� ��
                    anim.Play($"Mon{monsterType}_Left_Walk");
            }
            else  //���� �̵�
            {
                if (dirVec.y < 0)
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
