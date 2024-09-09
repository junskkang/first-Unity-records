using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    //��ź�� �ӵ�
    public float speed = 6000.0f;

    //���� ȿ�� �������� ������ ����
    public GameObject expEffect;
    CapsuleCollider _collider;
    Rigidbody _rigid;

    void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        _rigid = GetComponent<Rigidbody>();

        _rigid.AddForce(transform.forward * speed);

        //3�ʰ� ���� �� �ڵ����� �����ϴ� �ڷ�ƾ ����
        StartCoroutine(this.ExplosionCannon(3.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (!coll.tag.Contains("Cannon"))
        {
            //���� �Ǵ� �� ��ũ�� �浹�� ��� ��� �����ϵ��� �ڷ�ƾ ����
            StartCoroutine(this.ExplosionCannon(0.0f));
        }


    }

    IEnumerator ExplosionCannon(float timer)
    {
        yield return new WaitForSeconds(timer);
        //�浹 �ݹ� �Լ��� �߻����� �ʵ��� Collider�� ��Ȱ��ȭ
        if(_collider != null)
            _collider.enabled = false;
        //���������� ������ ���� �ʿ� ����
        if (_rigid != null)
        {
            _rigid.velocity  = Vector3.zero;
            _rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
            _rigid.isKinematic = true;
        }
            

        //���������� ���� ����
        //�� �� �� ���̰� �ϱ� ���ؼ� �߻��� �������� ��¦ ����Ʈ�� ������ ����
        GameObject go = Instantiate(expEffect, transform.position - (transform.forward * 9.0f), Quaternion.identity);

        Destroy(go, 1.0f);

        //Trail renderer�� �Ҹ�Ǳ���� ��� ����� ���� ó��
        Destroy(this.gameObject, 1.0f);
       
    }
}
