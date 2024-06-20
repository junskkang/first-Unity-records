using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;
    bool isBound = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor" && !isBound)    //�ٴڿ� ���� �� �����ϵ���
        {
            isBound = true;
            StartCoroutine(Explosion());
        }
    }

    IEnumerator Explosion() 
    {
        yield return new WaitForSeconds(1.0f);

        rigid.velocity = Vector3.zero;  //�ӵ� ���ֹ���
        rigid.angularVelocity = Vector3.zero;   //ȸ���ӵ��� ���ֹ���
        meshObj.SetActive(false);   //3�� �� �޽�������Ʈ ��Ȱ��ȭ
        effectObj.SetActive(true);  //��ƼŬ�ý��� true

        //���ߴ��� ��� sphere �ݶ��̴��� ������
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0,
            LayerMask.GetMask("Enemy"));

        foreach (RaycastHit hit in rayHits) 
        {
            hit.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5.0f);
    }
}
