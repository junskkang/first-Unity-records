using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//반드시 필요한 컴포넌트를 명시해서 해당 컴포넌트가 삭제되는 것을 방지하는 Attribute
[RequireComponent(typeof(AudioSource))]

public class FireCtrl : MonoBehaviour
{
    //총알 프리팹
    public GameObject bullet;
    //총알 발사좌표
    public Transform firePos;

    float fireTimer = 0.0f;

    //총알 발사 사운드 
    public AudioClip fireSfx;
    //AudiosSource 컴포넌트를 저장할 변수
    private AudioSource source = null;

    //MuzzleFlash의 MeshRenderer 컴포넌트 연결 변수
    public MeshRenderer muzzleFlash;


    // Start is called before the first frame update
    void Start()
    {
        //AudioSource 컴포넌트를 추출한 후 변수에 할당
        source = GetComponent<AudioSource>();

        //최초에 MuzzleFlash MeshRenderer를 비활성화
        muzzleFlash.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0.0f)
        {
            fireTimer = 0.0f;

            //마우스 왼쪽 버튼을 클릭했을 때 Fire 함수 호출
            if (Input.GetMouseButton(0))
            {
                Fire();

                fireTimer = 0.11f;
            }
        }//if (fireTimer <= 0.0f)
    }

    void Fire()
    {
        //동적으로 총알을 생성하는 함수
        CreateBullet();

        //사운드 발생 함수
        source.PlayOneShot(fireSfx, 0.9f);

        //잠시 기다리는 루틴을 위해 코루틴 함수로 호출
        StartCoroutine(this.ShowMuzzleFlash());
    }

    void CreateBullet()
    {
        //Bullet 프리팹을 동적으로 생성
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    //MuzzleFlash 활성/비활성화를 짧은 시간 동안 반복
    IEnumerator ShowMuzzleFlash()
    {
        //MuzzleFlash 스케일을 불규칙하게 변경
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        //MuzzleFalsh를 z축 기준으로 불규칙하게 회전시킴
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360.0f));
        muzzleFlash.transform.localRotation = rot;

        //메시 렌더러를 활성화해서 보이게 함
        muzzleFlash.enabled = true;

        //불규칙적인 시간동안  Delay시킴
        yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));
        //WaitForSeconds동안 return을 다른 쪽에 cpu사용을 양보하겠다(yield)는 의미
        //리턴을 잠시 미루겠다는 의미

        //비활성화해서 보이지 않게 함
        muzzleFlash.enabled = false;

        //for (int i = 0; i < 100; i++)
        //{
        //    Debug.Log("포문 주기에도 딜레이 가능?" + i);

        //    yield return new WaitForSeconds(1.0f);
        //}
    }
}
