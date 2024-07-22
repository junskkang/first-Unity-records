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

    Material material; //피격효과를 위해 
    bool isDamaged; //폭탄에 한 번만 공격당하기 위한 변수

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

    void FreezeVelocity() //의도치 않은 충돌로 인한 추적을 지속하기 위한 함수
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;   //AugularVelocity : 물리 회전 속도
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
            //생존
            material.color = Color.white;
        }
        else
        {
            //사망
            material.color = Color.gray;    //색 전환
            gameObject.layer = 12;          //EnemyDead 레이어로 변경
            anim.SetTrigger("doDie");
            isChase = false;
            nav.enabled = false;

            if (isGrenade) //폭탄으로 죽으면 더 과하게 사망하는 연출
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3;

                rigid.freezeRotation = false; //잠궈놨던 프리즈로테이션을 해제
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);    //살짝 넉백되는 연출
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);  //회전 연출
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);    //살짝 넉백되는 연출
            }

            Destroy(gameObject, 4.0f);
        }
    }
}
