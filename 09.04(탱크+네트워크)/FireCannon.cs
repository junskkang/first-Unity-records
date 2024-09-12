using Photon.Pun;
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

    private PhotonView pv = null;

    TankDamage tankdamage = null;


    private void Awake()
    {
        //cannon 프리팹을 Resources 폴더에서 불러와 변수에 할당
        cannon = Resources.Load("Cannon") as GameObject;
        //포탄 발사 사운드 파일을 Resources 폴더에서 불러와 변수에 할당
        fireSfx = Resources.Load<AudioClip>("CannonFire");
        //AudioSource 컴포넌트를 할당
        sfx = GetComponent<AudioSource>();

        pv = GetComponent<PhotonView>();

        tankdamage = GetComponent<TankDamage>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //마우스 왼쪽 버튼 클릭시 발사 로직 수행
        if (pv.IsMine && Input.GetMouseButtonDown(0))
        {
            if (tankdamage != null && tankdamage.currHp <= 0) return;   //죽은상태에서 쏘지 못하게
            
            //자신의 탱크일 경우는 로컬함수를 호출해 포탄을 발사
            Fire();
            //원격 네트워크에 있는 플레이어의 탱크에 RPC로 원격으로 함수를 호출
            pv.RPC("Fire", RpcTarget.Others, null);

            //자신과 모든 네트워크 플레이어에게 함수호출
            //pv.RPC("Fire", RpcTarget.All, null);

            //Bufferd : 나중에 입장한 플레이어에게 버퍼에 저장된 내용을 쭈루룩 전달함
            //접속을 끊은 사람 것의 내용은 버퍼에 남지 않고 바로 삭제됨
            //ex) 라그 채팅창. 내가 접속하기 전 내용들이 좀 뜸
            //All : 나는 즉시 / 다른 사람들은 RPC를 통해서
            //이펙트 등의 연출을 바로 표시하여 이질감을 줄이고, 동기화의 속도는 어느정도 차이가 있음
            //AllViaServer : 나를 포함해 모두에게 RPC를 통해서 중개받음
            //한템포 느리지만 동기화의 정확성이 높다
        }
    }

    [PunRPC]
    void Fire()
    {
        //발사 사운드 재생
        sfx.PlayOneShot(fireSfx, 1.0f);
        GameObject cannon = Instantiate(this.cannon, firePos.position, firePos.rotation);
        cannon.GetComponent<Cannon>().AttackerId = pv.Owner.ActorNumber;
        //PhotonView.Owner.ActorNumber : 오너가 갖는 고유 넘버
        //IsMine과 IsMine의 아바타도 모두 Owner판정
    }
}
