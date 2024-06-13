using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    //폭발 효과 파티클 연결 변수
    public GameObject explosionEffect;

    //무작위로 선택할 텍스쳐 배열
    public Texture[] textures;

    //총알 맞은 횟수를 누적시킬 변수
    private int hitCount = 0;

    float massTimer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        //텍스쳐 랜덤 교체
        int idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[idx];
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < massTimer)
        {
            massTimer -= Time.deltaTime;
            if (massTimer <= 0.0f)
            {
                Rigidbody rbody = this.GetComponent<Rigidbody>();
                if (rbody != null)
                    rbody.mass = 20.0f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "BULLET")
        {
            //충돌한 총알 제거
            Destroy(collision.gameObject);

            //총알 맞은 횟수를 증가시키고 3회 이상이면 폭발 처리
            if (++hitCount >= 3)
                ExpBarrel();
        }

        //if (collision.collider.tag == "Barrel")
        //{
        //    ExpBarrel();
        //}
    }

    public void ExpBarrel()
    {
        //폭발 효과 파티클 생성
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(explosion, explosion.GetComponentInChildren<ParticleSystem>().main.duration + 3.0f);

        //지정한 원점을 중심으로 10.0f 반경 내에 들어와 있는 Collider 객체 추출
        Collider[] colls = Physics.OverlapSphere(transform.position, 5.0f);
        BarrelCtrl barrel = null;
        Rigidbody rbody = null;
        //추출한 Collider 객체에 폭발력 전달
        foreach (Collider coll in colls)
        {
            barrel = coll.GetComponent<BarrelCtrl>();
            if (barrel == null) continue; 
            //barrel이 null이라는 것은 배럴컨트롤을 갖고 있지 않다는 의미 == 배럴이 아니다

            rbody = coll.GetComponent<Rigidbody>();
            if (rbody != null)
            {
                rbody.mass = 1.0f;
                //폭발력, 발생위치, 반경, 위로 솟구치는 힘
                rbody.AddExplosionForce(1000.0f, transform.position, 10.0f, 300, 0f);
                barrel.massTimer = 0.1f;

                //연쇄폭발을 위한 인보크함수
                barrel.Invoke("ExpBarrel", 1.5f);
            }
        }

        //5초 후에 드럼통 제거
        Destroy(gameObject, 2.0f);
    }
}
