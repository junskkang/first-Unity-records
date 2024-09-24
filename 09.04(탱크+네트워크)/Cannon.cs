using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    //포탄의 속도
    public float speed = 6000.0f;

    //폭발 효과 프리팹을 연결할 변수
    public GameObject expEffect;
    CapsuleCollider _collider;
    Rigidbody _rigid;

    //포탄을 발사한 플레이어의 ID 저장
    [HideInInspector] public int AttackerId = -1;
    //누가 쏜 총알인지를 저장하기 위한 변수
    //팀킬 방지 / 킬 카운트를 위하여 
    [HideInInspector] public string teamColor = "";

    void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        _rigid = GetComponent<Rigidbody>();

        _rigid.AddForce(transform.forward * speed);

        //3초가 지난 후 자동으로 폭발하는 코루틴 실행
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
            //지면 또는 적 탱크에 충돌할 경우 즉시 폭발하도록 코루틴 실행
            StartCoroutine(this.ExplosionCannon(0.0f));
        }


    }

    IEnumerator ExplosionCannon(float timer)
    {
        yield return new WaitForSeconds(timer);
        //충돌 콜백 함수가 발생하지 않도록 Collider를 비활성화
        if(_collider != null)
            _collider.enabled = false;
        //물리엔진의 영향을 받을 필요 없음
        if (_rigid != null)
        {
            //Debug.Log(_rigid.isKinematic);
            _rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
            _rigid.isKinematic = false;
            _rigid.velocity = Vector3.zero;
            _rigid.isKinematic = true;
        }
        
        //카메라와 포탄의 거리를 계산하여 그 거리에 따라
        //이펙트가 터지는 위치를 보정해줌
        Vector3 cacDist = transform.position - Camera.main.transform.position;
        float backLength = cacDist.magnitude * 0.1f;
        if (9.0f < backLength)
            backLength = 9.0f;

        //폭발프리팹 동적 생성
        //좀 더 잘 보이게 하기 위해서 발사한 방향으로 살짝 이펙트를 앞으로 꺼냄
        GameObject go = Instantiate(expEffect, transform.position - (transform.forward * backLength), Quaternion.identity);

        Destroy(go, 1.0f);

        //Trail renderer가 소멸되기까지 잠시 대기후 삭제 처리
        Destroy(this.gameObject, 1.0f);
       
    }
}
