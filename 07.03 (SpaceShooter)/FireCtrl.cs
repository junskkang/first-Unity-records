using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//반드시 필요한 컴포넌트를 명시해 해당 컴포넌트가 삭제되는 것을 방지하는 Attribute
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    //총알 프리팹
    public GameObject bullet;
    //총알 발사좌표
    public Transform firePos;
    //총알 오브젝트 풀 위치
    public Transform bulletPool;

    //총알을 오브젝트풀링으로 관리할 리스트
    public List<GameObject> bulletPoolList = new List<GameObject>();

    float fireTimer = 0.0f;

    //충알 발사 사운드
    public AudioClip fireSfx;
    //AudioSource 컴포넌트를 저장할 변수
    private AudioSource source = null;
    //MuzzleFlash의 MeshRenderer 컴포넌트 연결 변수
    public MeshRenderer muzzleFlash;

    //조준점 관련 변수
    public SpriteRenderer AimImg;
    // Start is called before the first frame update
    void Start()
    {
        //AudioSource 컴포넌트를 추출한 후 변수에 할당
        source = GetComponent<AudioSource>();
        //최초에 MuzzleFlash MeshRenderer를 비활성화
        muzzleFlash.enabled = false;

        ObjectPoolBullet();
    }

    void ObjectPoolBullet()
    {
        //총알을 미리 생성해 오브젝트 풀에 저장
        for (int i = 0; i < 50; i++)
        {
            //총알 프리팹을 생성
            GameObject bullet = (GameObject)Instantiate(this.bullet, firePos.position, firePos.rotation);
            //생성한 총알의 이름 설정
            bullet.name = "Bullet_" + i.ToString();
            //생성한 총알 firePos child로 저장
            bullet.transform.SetParent(bulletPool);
            //생성한 총알 비활성화 저장
            bullet.SetActive(false);
            //생성한 총알을 오브젝트 풀 리스트에 추가
            bulletPoolList.Add(bullet);
        }

        //Debug.Log(bulletPoolList.Count);
    }
    // Update is called once per frame
    void Update()
    {
        //사망 상태 시 조작 불가능
        if (GameManager.GameState == GameState.GameEnd) return;

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0.0f)
        {
            fireTimer = 0.0f;

            //마우스 왼쪽 버튼을 클릭했을 때 Fire 함수 호출
            if (Input.GetMouseButton(0))
            {
                if(GameManager.IsPointerOverUIObject() == false)
                    Fire();

                fireTimer = 0.11f;
            }
        }//if (fireTimer <= 0.0f)
         //카메라가 바라보고 있는 방향으로 총구를 틀어서 그쪽 방향으로 총알이 날아가도록 함
        firePos.transform.forward = FollowCam.riffleDir.normalized;

        Aiml();
    }

    void Fire()
    {
        //오브젝트 풀 리스트의 처음부터 끝까지 순회
        foreach (GameObject bullet in bulletPoolList)
        {
            //비활성화 여부로 사용 가능한 몬스터 판단
            if (bullet.activeSelf == false)
            {
                
                //각종 변수 초기화
                bullet.transform.position = firePos.position;
                bullet.transform.rotation = firePos.rotation; //Quaternion.LookRotation(Vector3.forward, firePos.position);
                //오브젝트 풀링에 저장된 총알 활성화
                bullet.SetActive(true);
                //오브젝트 풀에서 총알 프리팹 하나를 활성화한 후 for 루프를 빠져나감
                break;
            }
        }


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

    //MuzzleFlash 활성 / 비활성화를 짧은 시간 동안 반복
    IEnumerator ShowMuzzleFlash()
    {
        //MuzzleFlash 스케일을 불규칙하게 변경
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        //MuzzleFlash를 Z축을 기준으로 불규칙하게 회전시킴
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360.0f));
        muzzleFlash.transform.localRotation = rot;
                
        //활성화해서 보이게 함
        muzzleFlash.enabled = true;

        //불규칙적인 시간 동안 Delay한 다음 MeshRenderer를 비활성화
        yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));

        //비활성화해서 보이지 않게 함
        muzzleFlash.enabled = false;

    }//IEnumerator ShowMuzzleFlash()

    void Aiml()
    {
        Debug.DrawRay(firePos.position, firePos.transform.forward * 15.0f, Color.green);  //시작 위치, 방향 * 길이, 색
        RaycastHit hit;

        if (Physics.Raycast(firePos.position, firePos.transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.tag == "BULLET") return;

            if (AimImg != null)
            {                
                Vector3 forHere = hit.point;
                forHere.z -= 1.0f;
                AimImg.transform.position = forHere;

                float dist = Vector3.Distance(firePos.position, forHere);
                Vector3 maxSize = new Vector3(0.5f, 0.5f);
                Vector3 middleSize = new Vector3(0.3f, 0.3f);
                Vector3 minSize = new Vector3(0.1f, 0.1f);
                if (dist > 10.0f)
                {
                    AimImg.transform.localScale = Vector3.Lerp(maxSize, minSize, Time.deltaTime * 0.001f);
                }
                else //if (5.0f < dist && dist < 10.0f )
                {
                    AimImg.transform.localScale = Vector3.Lerp(minSize, maxSize, Time.deltaTime * 0.001f);
                }
                
                AimImg.transform.LookAt(Camera.main.transform.position);
            }
                

            
        }

        //레이캐스트를 쏠 때 팁!
        //카메라에서 캐릭터를 향해 쏘는 것보다 캐릭터에서 카메라를 향해 쏘는 것이 감도가 좋음
        //왜? 벽의 두께가 존재하고 벽의 내부는 비어있음 고로 raycast가 hit되지 않아
        //카메라 위치가 벽의 내부에 존재할 때는 감지하지 못한다는 의미
        //카메라의 위치가 온전히 벽을 벗어나야 그때부터 hit가 되기 시작하기 때문에
        //벽의 두께가 얇다면 크게 차이는 없겠지만 벽이 두꺼워지면 두꺼워질수록
        //그 차이가 분명할 것. 따라서 캐릭터에서부터 카메라를 향해 쏘면
        //어떠한 오브젝트를 대상으로 한다해도 확실하게 처리할 수 있음

        //isBorder = Physics.Raycast(m_PlayerVec, toCamera,
        //          out hit, dist, LayerMask.GetMask("Wall"));
        //Raycast(시작위치, 방향, 길이, 레이어마스크)
        //해당 레이캐스트가 wall이라는 레이어마스크에 닿으면 true값을 반환



        //if (isBorder)
        //{

        //    transform.position = hit.point;


        //    //float collDist = (transform.position - m_PlayerVec).magnitude;
        //    ////Debug.Log(hit.collider.name);
        //    ////hit.collider.gameObject.GetComponent<MeshRenderer>().material = materials;
        //    //if (collDist > 0.85f)
        //    //    this.dist = collDist - 0.3f;

        //}
    }
}
