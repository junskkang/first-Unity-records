using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;
    public Transform target;
    public bool isChase;

    Rigidbody rigid;
    BoxCollider boxCollider;

    NavMeshAgent nav;
    Animator anim;

    Material material; //�ǰ�ȿ���� ���� 
    bool isDamaged; //��ź�� �� ���� ���ݴ��ϱ� ���� ����

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("ChaseStart", 2.0f);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void Update()
    {
        if (isChase)
            nav.SetDestination(target.position);

    }

    private void FixedUpdate()
    {
        FreezeVelocity();
    }

    void FreezeVelocity() //�ǵ�ġ ���� �浹�� ���� ������ �����ϱ� ���� �Լ�
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;   //AugularVelocity : ���� ȸ�� �ӵ�
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet")
        {
            BulletCtrl bullet = other.GetComponent<BulletCtrl>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;

            StartCoroutine(OnDamage(reactVec));
            Destroy(other.gameObject);
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        if (!isDamaged)
        {
            isDamaged = true;

            curHealth -= 100;
            Vector3 reactVec = transform.position - explosionPos;
            StartCoroutine(OnDamage(reactVec, true));

            isDamaged = false;
        }

    }
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade = false)
    {
        material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            //����
            material.color = Color.white;
        }
        else
        {
            //���
            material.color = Color.gray;    //�� ��ȯ
            gameObject.layer = 12;          //EnemyDead ���̾�� ����
            anim.SetTrigger("doDie");
            isChase = false;
            nav.enabled = false;

            if (isGrenade) //��ź���� ������ �� ���ϰ� ����ϴ� ����
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false; //��ų��� ����������̼��� ����
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);    //��¦ �˹�Ǵ� ����
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);  //ȸ�� ����
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);    //��¦ �˹�Ǵ� ����
            }

            Destroy(gameObject, 4.0f);
        }
    }
}
