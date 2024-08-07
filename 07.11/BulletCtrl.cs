using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    //총알의 파괴력
    public int damage = 20;
    //총알 발사 속도
    public float speed = 1000.0f;

    //스파크 파티클 프리팹 연결할 변수
    public GameObject sparkEffect;

    // Start is called before the first frame update
    void Start()
    {
        speed = 3000.0f;

        if (gameObject.tag == "E_BULLET")  //몬스터가 쏜 초알일 때 
        {
            speed = 800.0f;
        }
        else  //주인공이 쏜 총알일 때
        {            
            transform.forward = FollowCam.m_RifleDir.normalized;
        }

        GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        Destroy(gameObject, 4.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.name.Contains("Player") == true)
            return;

        if (coll.gameObject.name.Contains("Barrel") == true)
            return;

        if (coll.gameObject.name.Contains("Monster_") == true)
            return;

        if (coll.collider.tag == "SideWall")
            return;

        if (coll.collider.tag == "BULLET")
            return;

        if (coll.collider.tag == "E_BULLET")
            return;

        //스파크 파티클을 동적으로 생성
        GameObject spark = Instantiate(sparkEffect, transform.position, Quaternion.identity);
        //ParticleSystem 컴포넌트의 수행시간(duration)이 지난 후 삭제 처리
        Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

        //충돌한 게임오브젝트 삭제
        Destroy(gameObject);

    }//void OnCollisionEnter(Collision coll)
}
