using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCannon : MonoBehaviour
{
    //Canoon 프리팹을 연결할 변수
    public GameObject cannon = null;
    //Cannon 발사 지점
    public Transform firePos;

    //포탄 발사 사운드 파일
    private AudioClip fireSfx = null;
    //AudioSource 컴포넌트를 할당할 변수
    private AudioSource sfx = null;


    private void Awake()
    {
        //cannon 프리팹을 Resources 폴더에서 불러와 변수에 할당
        cannon = Resources.Load("Cannon") as GameObject;
        //포탄 발사 사운드 파일을 Resources 폴더에서 불러와 변수에 할당
        fireSfx = Resources.Load<AudioClip>("CannonFire");
        //AudioSource 컴포넌트를 할당
        sfx = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //마우스 왼쪽 버튼 클릭시 발사 로직 수행
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {
        //발사 사운드 재생
        sfx.PlayOneShot(fireSfx, 1.0f);
        Instantiate(cannon, firePos.position, firePos.rotation);
    }
}
