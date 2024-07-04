using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum BulletType
{
    BULLET,
    E_BULLET
}
public class BulletCtrl : MonoBehaviour
{
    //총알의 파괴력
    public int damage = 20;
    //총알 발사 속도
    public float speed = 3000.0f;

    Vector3 firstPos = Vector3.zero;

    FireCtrl fireCtrl;

    GameObject firePos;

    //스파크 이펙트 연결 변수
    public GameObject sparkEffect;

    private void Awake()
    {
        speed = 3000.0f;

        fireCtrl = GameObject.FindObjectOfType<FireCtrl>();

        firePos = GameObject.Find("FirePos");
    }
    private void OnEnable()
    {

        //GetComponent<TrailRenderer>().time = 0.3f;

        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        //오브젝트 풀에 다시 환원
        //StartCoroutine(PushObjectPool2());
    }

    void Start()
    {
        //Destroy(gameObject, 4.0f);

        if (gameObject.tag == "E_BULLET")
        {
            Destroy(gameObject, 4.0f);
            return;
        }
        else
        {
            Invoke("PushObjectPool", 4.0f);
        }       
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public IEnumerator PushObjectPool2(float time = 4.0f)
    //{
    //    yield return new WaitForSeconds(time);

    //    GetComponent<TrailRenderer>().time = -1;
    //    //총알 비활성화
    //    gameObject.SetActive(false);


    //    yield break;

    //}

    public void PushObjectPool()
    {
        //각종 변수 초기화
        //GetComponent<TrailRenderer>().time = -1;
        //트레일 렌더러 초기화

        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);  //물리적으로 남아있는 속도값 제거
        GetComponent<TrailRenderer>().Clear();

        //총알 비활성화
        gameObject.SetActive(false);

    }

    private void OnCollisionEnter(Collision coll)
    {

        //아래의 제외사항은 각 컨트롤에서 별도로 하고 있기 때문에 중복을 피하기 위해서
        if (coll.gameObject.name.Contains("Player") == true) return;

        if (coll.gameObject.name.Contains("Barrel") == true) return;

        if (coll.gameObject.name.Contains("Monster_") == true ) return;

        if (coll.gameObject.tag == "SideWall") return;

        if (coll.gameObject.tag == "BULLET") return;

        if (coll.gameObject.tag == "E_BULLET") return;

        
        //여기까지 오면 충돌체는 건물만! 남게 됨
        GameObject spark = Instantiate(sparkEffect, transform.position, Quaternion.identity);
        
        //ParticleSystem 컴포넌트의 수행시간(duration)이 지난 후 삭제 처리
        Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f);

        //충돌한 게임오브젝트 삭제
        //BulletCtrl bulletCtrl = coll.collider.GetComponent<BulletCtrl>();
        //충돌한 총알 제거
        PushObjectPool();
    }
}
