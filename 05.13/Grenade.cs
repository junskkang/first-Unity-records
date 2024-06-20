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
        if (collision.gameObject.tag == "Floor" && !isBound)    //바닥에 닿은 후 폭발하도록
        {
            isBound = true;
            StartCoroutine(Explosion());
        }
    }

    IEnumerator Explosion() 
    {
        yield return new WaitForSeconds(1.0f);

        rigid.velocity = Vector3.zero;  //속도 없애버려
        rigid.angularVelocity = Vector3.zero;   //회전속도도 없애버려
        meshObj.SetActive(false);   //3초 뒤 메쉬오브젝트 비활성화
        effectObj.SetActive(true);  //파티클시스템 true

        //적중당한 모든 sphere 콜라이더를 가져옴
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0,
            LayerMask.GetMask("Enemy"));

        foreach (RaycastHit hit in rayHits) 
        {
            hit.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 5.0f);
    }
}
