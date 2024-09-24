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

    //��ź�� �߻��� �÷��̾��� ID ����
    [HideInInspector] public int AttackerId = -1;
    //���� �� �Ѿ������� �����ϱ� ���� ����
    //��ų ���� / ų ī��Ʈ�� ���Ͽ� 
    [HideInInspector] public string teamColor = "";

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
            //Debug.Log(_rigid.isKinematic);
            _rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
            _rigid.isKinematic = false;
            _rigid.velocity = Vector3.zero;
            _rigid.isKinematic = true;
        }
        
        //ī�޶�� ��ź�� �Ÿ��� ����Ͽ� �� �Ÿ��� ����
        //����Ʈ�� ������ ��ġ�� ��������
        Vector3 cacDist = transform.position - Camera.main.transform.position;
        float backLength = cacDist.magnitude * 0.1f;
        if (9.0f < backLength)
            backLength = 9.0f;

        //���������� ���� ����
        //�� �� �� ���̰� �ϱ� ���ؼ� �߻��� �������� ��¦ ����Ʈ�� ������ ����
        GameObject go = Instantiate(expEffect, transform.position - (transform.forward * backLength), Quaternion.identity);

        Destroy(go, 1.0f);

        //Trail renderer�� �Ҹ�Ǳ���� ��� ����� ���� ó��
        Destroy(this.gameObject, 1.0f);
       
    }
}
