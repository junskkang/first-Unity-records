using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;

    Material material; //�ǰ�ȿ���� ���� 
    bool isDamaged; //��ź�� �� ���� ���ݴ��ϱ� ���� ����

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponent<MeshRenderer>().material;
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
