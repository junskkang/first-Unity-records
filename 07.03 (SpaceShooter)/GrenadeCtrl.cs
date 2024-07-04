using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeCtrl : MonoBehaviour
{
    //폭발효과 파티클 연결 변수
    public GameObject expEffect;

    //폭발 지연 타이머
    float timer = 2.0f;

    //무작위로 선택할 텍스쳐 배열
    public Texture[] textures;

    //수류탄 날아가는 속도
    float speed = 600.0f;
    Vector3 forwardDir = Vector3.zero;

    bool isRot = true;
    // Start is called before the first frame update
    void Start()
    {
        int idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material.mainTexture = textures[idx];

        //날아가는 방향 조정
        transform.forward = forwardDir;
        transform.eulerAngles = new Vector3(20.0f, transform.eulerAngles.y,
                                           transform.eulerAngles.z);

        GetComponent<Rigidbody>().AddForce(forwardDir * speed);
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < timer)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                ExpGrenade();
            }
        }

        if (isRot)
        {
            transform.Rotate(new Vector3(Time.deltaTime * 190.0f, 0.0f, 0.0f), Space.Self);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        isRot = false;
    }
    void ExpGrenade()
    {
        //폭발 효과 파티클 생성
        GameObject explosion = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(explosion, explosion.GetComponentInChildren<ParticleSystem>().main.duration + 2.0f);

        //지정한 원점을 중심으로 10.0f 반경 내에 들어와 있는 collider 객체 추출
        Collider[] colls = Physics.OverlapSphere(transform.position, 6.0f);    //위치로부터 반경까지

        //추출한 콜라이더 객체 중 몬스터에 폭발력 전달
        MonsterCtrl monsterCtrl = null;
        foreach (Collider coll in colls) 
        {
            monsterCtrl = coll.GetComponent<MonsterCtrl>(); //몬스터컨트롤이 붙어있는 애들만 찾아옴
            if (monsterCtrl == null)
                continue;

            monsterCtrl.TakeDamage(150);    //몬스터에 데미지 줌
        }

        Destroy(gameObject);
    }

    public void SetForwardDir(Vector3 dir)
    {
        forwardDir = new Vector3(dir.x, dir.y + 0.5f, dir.z);
    }
}
