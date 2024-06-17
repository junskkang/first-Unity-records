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
    public float speed = 1000.0f;

    FireCtrl fireCtrl;

    // Start is called before the first frame update
    void Start()
    {
        speed = 3000.0f;

        GetComponent<Rigidbody>().AddForce(transform.forward * speed);

        fireCtrl = GetComponent<FireCtrl>();

        //Destroy(gameObject, 4.0f);

        //오브젝트 풀에 다시 환원
        StartCoroutine(PushObjectPool());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator PushObjectPool(float time = 4.0f)
    {
        yield return new WaitForSeconds(time);

        //각종 변수 초기화
        transform.position = fireCtrl.firePos.transform.position;

        //총알 비활성화
        gameObject.SetActive(false);
    }
}
