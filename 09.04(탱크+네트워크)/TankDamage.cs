using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//HP 에너지 동기화가 안맞는 현상 고치기
//IsMine 기준으로 판정하고 중계한다.
//현재 IsMine과 아바타가 발사하는 캐넌의 구분을 하지 않고
//각자가 모두 Instantiate를 하고 있기 때문에 위치들이 정확하게 일치하지 않기 때문에
//아바타가 발사한 캐넌이 먼저 적중할 수도 있음. 그래서 IsMine을 가지고 판정하는 것.
//정밀하게 총알 원격충돌처리를 하려면
//총알을 맞았을 때 IsMine으로 총알을 맞았다는 RPC함수를 쏘고
//여기서 RPC 함수의 총알 고유번호를 같이 보내서 중복 데미지가 일어나지 않도록 처리한다.
//아바타이던 IsMine이던 총알에 맞으면 무조건 일단 IsMine에 총알을 맞았다고
//신호를 보내고 IsMine에서 데미지 계산을 해준 후 Hp를 중계하여 동기화를 맞춰줘야 한다.
public class TankDamage : MonoBehaviourPunCallbacks, IPunObservable
{
    //탱크 폭파 후 투명처리를 위한 MeshRenderer 컴포넌트 배열
    private MeshRenderer[] _renderers;

    //탱크 폭발 효과 프리팹을 연결할 변수
    private GameObject expEffect = null;

    //탱크의 초기 생명치
    private int initHp = 200;
    //탱크의 현재 생명치
    [HideInInspector] public int currHp = 0;
    int NetHp = 200;    //아바타탱크의 Hp값을 동기화 시켜주기 위한 변수


    //HUD에 연결할 변수
    public Canvas hudCanvas;
    public Image hpBarImg;
    Color startColor = Color.white;


    PhotonView pv = null;
    //플레이어의 고유번호를 저장할 변수
    public int PlayerId = -1;

    //Kill Count 동기화에 필요한 변수
    //탱크 HUD에 표시할 스코어 Text UI 항목
    public Text txtKillCount;

    //적 탱크 파괴 스코어를 CustomProperties를 통해 중계하기 위한 변수들
    int killCount = 0;
    int lastAttackId = -1;  //막타를 누가 쳤는지 아이디를 받아놓을 변수

    ExitGames.Client.Photon.Hashtable KillProps = new ExitGames.Client.Photon.Hashtable();

    [HideInInspector] public float m_ResetTime = 0.0f;

    private void Awake()
    {
        //탱크 모델의 모든 MeshRenderer 컴포넌트를 추출한 후 배열에 할당
        _renderers = GetComponentsInChildren<MeshRenderer>();
        
        pv = GetComponent<PhotonView>();

        //현재 생명치를 초기 생명치로 초기값 설정
        currHp = initHp;
        //탱크 폭발시 생성시킬 폭발 효과를 로드
        expEffect = Resources.Load("ExplosionMobile") as GameObject;

        startColor = hpBarImg.color;

        PlayerId = pv.Owner.ActorNumber;

        InitCustomProperties(pv);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    int updateCk = 2;
    // Update is called once per frame
    void Update()
    {
        //탱크는 방에 처음 입장하면 안보이도록 처리
        //타이밍 상 모두 update를 돌고난 후에 적용되어야 UI가 깨지지 않는다
        if (0 < updateCk) 
        {
            updateCk--;
            if (updateCk <= 0)
            {
                ReadyStateTank();
            }
        }

        if (m_ResetTime > 0.0f)
        {
            m_ResetTime -= Time.deltaTime;
        }

        if (PhotonNetwork.CurrentRoom == null ||
            PhotonNetwork.LocalPlayer == null) return;
        //동기화 가능한 상태일때만 업데이트를 계산해 준다.
        //로비로 돌아갈 때 포톤네트워크 룸에서 먼저 나가고
        //그 다음 씬 이동을 하게 되는데
        //씬 이동 전에 포톤관련 함수가 돌아가게 되면 에러가 뜸
        
        //아바타 탱크일 때 동기화 코드
        if (pv.IsMine == false)
        {
            //원격 플레이어일때
            AvatarUpdate();

            //아바타 탱크들 입장에서 KillCount 중계 받기
            ReceiveKillCount();
        }

        if (txtKillCount != null)
            txtKillCount.text = killCount.ToString();   //킬 카운트 UI 갱신
    }

    public void ReadyStateTank()
    {
        if (GameManager.gameState != GameState.GS_Ready) return;

        StartCoroutine(this.WaitReadyTank());
    }

    //게임 시작 대기 시
    IEnumerator WaitReadyTank()
    {
        //HUD 비활성화
        hudCanvas.enabled = false;

        //탱크 투명처리
        SetTankVisible(false);

        while (GameManager.gameState == GameState.GS_Ready)
            yield return null;

        //탱크 특정한 위치에 스폰 및 투명화 제거
        float pos = Random.Range(-100.0f, 100.0f);
        Vector3 sitPos = new Vector3(pos, 20.0f, pos);

        string a_TeamKind = ReceiveSelTeam(pv.Owner);
        int a_SitPosIdx = ReceiveSitPosIdx(pv.Owner);
        if (0 <= a_SitPosIdx && a_SitPosIdx < 4)
        {
            if (a_TeamKind == "blue")
            {
                sitPos = GameManager.m_Team1Pos[a_SitPosIdx];
                this.gameObject.transform.eulerAngles = new Vector3(0.0f, 201.0f, 0.0f);
            }
            else if (a_TeamKind == "red")
            {
                sitPos = GameManager.m_Team2Pos[a_SitPosIdx];
                this.gameObject.transform.eulerAngles = new Vector3(0.0f, 19.5f, 0.0f);
            }
        }

        transform.position = sitPos;
        //HUD 초기화
        hpBarImg.fillAmount = 1.0f;
        hpBarImg.color = startColor;
        hudCanvas.enabled = true;

        if (pv != null && pv.IsMine)
            currHp = initHp;

        //리스폰 무적 시간 초기화
        m_ResetTime = 5.0f;


        SetTankVisible(true);        
    }

    private void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider의 태그 비교
        if (currHp > 0 && coll.tag.Contains("Cannon"))
        {
            //충돌한 캐넌의 아이디를 받아서 넘기기
            int att_Id = -1;
            Cannon refCannon = coll.gameObject.GetComponent<Cannon>();
            if ((refCannon) != null)
                att_Id = refCannon.AttackerId;

            if ((string)pv.Owner.CustomProperties["MyTeam"] == refCannon.teamColor)
                return;

            TakeDamage(att_Id);

            //Debug.Log((string)pv.Owner.CustomProperties["MyTeam"] + " : " + refCannon.teamColor);
            

            //Debug.Log("충돌은 되네?");

            //currHp -= 20;

            ////현재 생명치 HUD에 표기
            //hpBarImg.fillAmount = (float)currHp / (float)initHp;
            ////생명 수치에 따른 체력바 색상 변경
            //if (hpBarImg.fillAmount <= 0.5f)
            //    hpBarImg.color = Color.yellow;

            //if (hpBarImg.fillAmount <= 0.3f)
            //    hpBarImg.color = Color.red;

            //if (currHp <= 0)
            //{
            //    StartCoroutine(this.ExplosionTank());
            //}
        }
    }

    public void TakeDamage(int Attacker = -1)
    {
        //자기가 쏜 총알은 자기가 맞으면 안되도록 처리
        if (Attacker == PlayerId) return;

        if (currHp <= 0.0f) return;

        //피격연출

        if (!pv.IsMine) return;     //이 함수에 IsMine만 적용되도록 예외처리

        if (0.0f < m_ResetTime) return; //리스폰 후 무적타임



        //pv.IsMine일때
        lastAttackId = Attacker;
        currHp -= 20;
        if(currHp <0)
            currHp = 0;
        
        //현재 생명치 HUD에 표기
        hpBarImg.fillAmount = (float)currHp / (float)initHp;
        //생명 수치에 따른 체력바 색상 변경
        if (hpBarImg.fillAmount <= 0.5f)
            hpBarImg.color = Color.yellow;

        if (hpBarImg.fillAmount <= 0.3f)
            hpBarImg.color = Color.red;

        if (currHp <= 0)    //죽는 처리 (아바타 탱크들은 중계 받아서 처리)
            StartCoroutine(this.ExplosionTank());


    }
    //폭발 효과 생성 및 리스폰 코루틴 함수
    IEnumerator ExplosionTank()
    {
        //폭발 효과 생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);

        Destroy(effect, 3.0f);

        //HUD를 비활성화
        hudCanvas.enabled = false;

        //탱크 투명 처리
        SetTankVisible(false); yield return null;

        //if (pv != null && pv.IsMine)    //IsMine일때만 여기를 통해 되살리기
        //{
        //    //3초동안 기다렸다가 활성화하는 로직을 수행
        //    yield return new WaitForSeconds(5.0f);

        //    //HUD 초기화
        //    hpBarImg.fillAmount = 1.0f;
        //    hpBarImg.color = startColor;
        //    hudCanvas.enabled = true;

        //    //리스폰 시 생명 초기값 설정
        //    currHp = initHp;

        //    //리스폰 무적 시간 초기화
        //    m_ResetTime = 5.0f;

        //    //탱크를 다시 보이게 처리
        //    SetTankVisible(true);
        //}
        //else
        //{   //아바타 탱크들은 중계받아서 되살리기 위해 아무것도 하지 않고 나가기
        //    yield return null;
        //}

    }

    //MeshRenderer를 활성/비활성화하는 함수
    void SetTankVisible(bool isVisible)
    {
        foreach (MeshRenderer renderer in _renderers)
        {
            renderer.enabled = isVisible;
        }

        Rigidbody[] rigids = GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody rig in rigids)
        {
            rig.isKinematic = !isVisible;
        }

        BoxCollider[] boxColliders = GetComponentsInChildren<BoxCollider>(true);
        foreach (BoxCollider boxCollider in boxColliders)
        {
            boxCollider.enabled = isVisible;
        }

        //if (isVisible && pv.IsMine)
        //{
        //    float pos = Random.Range(-100.0f, 100.0f);
        //    transform.position = new Vector3(pos, 20.0f, pos);
        //}
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(lastAttackId);  //이번에 -20깎인게 누구 때문인지 보내려는 의도
            stream.SendNext(currHp);
        }
        else
        {
            lastAttackId = (int)stream.ReceiveNext();   //아바타들도 누구때문에 피가 깎였는지 저장
            NetHp = (int)stream.ReceiveNext();
            //아바타 입장에서 사망 시점을 알기 위해 NetHp라는 별도의 변수를 만들어
            //IsMine에서 송신해준 Hp값을 받아옴
        }
    }

    void AvatarUpdate()     //아바타의 Hp Update 처리 함수
    {
        if (0 < currHp)
        {
            currHp = NetHp;
            //현재 생명치 백분율 
            hpBarImg.fillAmount = (float)currHp / (float)initHp;

            //생명 수치에 따른 체력바 색상 변경
            if (hpBarImg.fillAmount <= 0.5f)
                hpBarImg.color = Color.yellow;

            if (hpBarImg.fillAmount <= 0.3f)
                hpBarImg.color = Color.red;

            if (currHp <= 0) //죽는처리 (아바타 탱크들은 중계 받아서 처리)
            {
                currHp = 0;

                if (0 <= lastAttackId)  //공격자의 id가 유효할 때
                {
                    //나에게 막타를 친 탱크가 누구인지를 파악해서 
                    //KillCount를 올려줘야함.
                    //자신을 파괴시킨 적 탱크의 스코어를 증가시키는 함수를 호출
                    //'죽는 탱크 입장'에서 나를 죽인 대상은 다른 플레이어의 '아바타'임
                    //그래서 내 기준에서 존재하는 아바타들 중에서 lastAttackId와 동일한
                    //아이디를 갖고 있는 IsMine을 찾아서 그곳에서 킬 카운트를 올리는
                    //함수를 호출시켜줘야 함.                   
                    SaveKillCount(lastAttackId);
                }
                StartCoroutine(ExplosionTank());
            }
        }
        else
        {   //죽어 있을 때 계속 NetHp는 0으로 계속 들어오게 되고
            //되살려야 하는 상황 처리
            currHp = NetHp;
            if ((int)(initHp * 0.95) < currHp)  
            {
                //이번에 들어온 Hp가 최대 에너지로 들어오면 되살려야하는 것으로 판단

                //Filled 이미지 초기값으로 환원
                hpBarImg.fillAmount = 1.0f;
                //이미지 색상 원복
                hpBarImg.color = startColor;
                //HUD 활성화 
                hudCanvas.enabled = true;

                //리스폰시 새생명값으로 설정
                currHp = initHp;

                //리스폰 무적 시간 초기화
                m_ResetTime = 5.0f;

                //탱크를 다시 보이게 처리
                SetTankVisible(true);

            }
        }
    }

    //자신을 파괴시킨 적 탱크를 찾아서 스코어를 증가시켜주는 함수
    void SaveKillCount(int AttackerId)
    {
        //탱크 태그로 지정된 모든 오브젝트를 가져와 배열로 저장
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("TANK");
        foreach (GameObject tank in tanks)
        {
            var tankDamage = tank.GetComponent<TankDamage>();
            if (tankDamage != null && tankDamage.PlayerId == AttackerId)
            {
                //탱크의 PlayerId와 포탄의 AttackerID가 동일한지 판단
                if (tankDamage.IncKillCount())
                {
                    return;
                }
            }
        }
    }

    public bool IncKillCount()  
    {
        //때린 탱크 IsMine 입장에서 이 함수가 호출되어야 한다.
        if (pv != null && pv.IsMine)
        {
            //IsMine 한군데서만 KillCount를 증가시키는 이유는
            //IsMine과 아바타의 구분없이 모두 각각 KillCount를 증가시켜 중계하다보면
            //KillCount가 어긋날 수 있기 때문
            killCount++;

            //IsMine일때만 중계함
            SendKillCount(killCount);

            return true;
        }
        
        return false;
    }

    void InitCustomProperties(PhotonView pv)
    {
        //버퍼를 미리 만들어 놓기 위한 함수
        if (pv != null && pv.IsMine)
        {
            //pv.IsMine == true 내가 조정하고 있는 탱크이고 스폰 시점에 초기화
            KillProps.Clear();
            KillProps.Add("KillCount", 0);  //키값과 밸류로 이루어져 있음
            pv.Owner.SetCustomProperties(KillProps);
        }
    }

    void SendKillCount(int killCount = 0)
    {
        if (pv == null) return;

        if (!pv.IsMine) return; //IsMine에서만 중계를 보냄

        if (KillProps == null)
        {
            KillProps = new ExitGames.Client.Photon.Hashtable();
            KillProps.Clear();
        }

        if (KillProps.ContainsKey("KillCount"))
            KillProps["KillCount"] = killCount;
        else
            KillProps.Add("KillCount", killCount);

        pv.Owner.SetCustomProperties(KillProps);    //중계하는 곳   
    }

    void ReceiveKillCount()
    {
        if(pv == null) return;

        if(pv.IsMine) return;   //아바타인 탱크들만 받게 함

        if (pv.Owner == null) return;

        if (pv.Owner.CustomProperties.ContainsKey("KillCount"))
        {
            killCount = (int)pv.Owner.CustomProperties["KillCount"];
        }
    }

    private int ReceiveSitPosIdx(Player a_Player)
    {
        int a_SitPosidx = -1;

        if (a_Player == null)
            return a_SitPosidx;

        if (a_Player.CustomProperties.ContainsKey("SitPosInx"))
        {
            a_SitPosidx = (int)a_Player.CustomProperties["SitPosInx"];
        }

        return a_SitPosidx;
    }

    private string ReceiveSelTeam(Player a_Player)
    {
        string a_TeamKind = "blue";

        if (a_Player == null)
            return a_TeamKind;

        if (a_Player.CustomProperties.ContainsKey("MyTeam"))
        {
            a_TeamKind = (string)a_Player.CustomProperties["MyTeam"];
        }

        return a_TeamKind;
    }

}
