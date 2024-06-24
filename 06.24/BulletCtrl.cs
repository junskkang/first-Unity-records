using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static MonsterCtrl;

public class BulletCtrl : MonoBehaviour
{
    //총알의 파괴력
    public int damage = 20;
    //총알 발사 속도
    public float speed = 3000.0f;

    Vector3 firstPos = Vector3.zero;

    FireCtrl fireCtrl;

    GameObject firePos;

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

        Invoke("PushObjectPool", 4.0f);
        
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
}
