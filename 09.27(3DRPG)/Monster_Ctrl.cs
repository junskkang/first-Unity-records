using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonType
{
    Skeleton = 0,
    Alien,
    Count
}

public class Monster_Ctrl : MonoBehaviourPunCallbacks, IPunObservable
{
    public MonType monType;

    //Hp 
    float curHp = 100;
    float maxHp = 100;
    float netHp = 100;  //네트워크 중계를 위한 Hp값
    public Image hpBarImg;
    public RectTransform HUDCanvas;

    [HideInInspector] public int m_SpawnIdx = -1;

    //몬스터의 현재 상태 정보를 저장할 Enum 변수
    AnimState m_Prestate = AnimState.idle;  //Animation 변경을 위한 변수 
    [HideInInspector] public AnimState m_CurState = AnimState.idle;  //애니메이션 변경을 위한 변수

    AnimState MonState = AnimState.idle;    //몬스터의 현재 AI 상태를 저장할 변수
    //AnimSupporter.cs 쪽에 정의되어 있음

    //인스펙터뷰에 표시할 애니메이션 클래스 변수
    public Anim anim;  //AnimSupporter.cs 쪽에 정의되어 있음
    Animation m_RefAnimation;   //Skeleton은 레거시 애니메이션을 이용하여 구현
    Animator m_RefAnimator; //Alien은 애니메이터를 이용해 구현

    //Monster AI
    [HideInInspector] public GameObject m_AggroTarget = null; //공격할 대상
    int m_AggroTgID = -1;   //이 몬스터가 공격해야 할 캐릭터의 네트워크 상 고유 번호 
    Vector3 m_MoveDir = Vector3.zero;   //수평진행 노멀 방향 벡터
    Vector3 m_CacVLen = Vector3.zero;   //주인공을 향하는 벡터
    float m_CacDist = 0.0f;             //거리 계산용 변수
    float m_TraceDist = 7.0f;           //추적 반경 변수
    float m_AttackDist = 1.8f;          //공격 거리
    Quaternion m_TargetRot;             //회전 계산용 변수
    float m_RotSpeed = 7.0f;            //회전 속도
    Vector3 m_MoveNextStep = Vector3.zero;  //이동 게산용 변수
    float m_MoveVelocity = 2.0f;        //평면 초당 이동속도

    //타겟 마크
    public GameObject targetMark = null;
    [HideInInspector] public bool isTarget = false;

    //죽는 연출 투명해지며 사라지도록
    Vector3 m_DieDir = Vector3.zero;
    float m_DieDur = 0.0f;
    float m_DieTimer = 0.0f;
    SkinnedMeshRenderer m_SMR = null;
    SkinnedMeshRenderer[] m_SMRList = null;
    MeshRenderer[] m_MeshList = null;   //무기는 그냥 MeshRenderer로 되어있음
    float m_Ratio = 0.0f;
    Color m_CalColor = Color.white;

    //Shader m_DefTexShader = null;
    //Shader g_WeaponTexShader = null;

    //네트워크 동기화를 위하여
    Vector3 curPos = Vector3.zero;
    Quaternion curRot = Quaternion.identity;
    int curAnim = 0;
    string m_OldAnim = "";
    PhotonView pv = null;

    void Awake()
    {
        pv = GetComponent<PhotonView>();

        curPos = transform.position;
        curRot = transform.rotation;
        m_DieDir = -transform.forward;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_RefAnimation = GetComponentInChildren<Animation>();
        m_RefAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        if (m_CurState == AnimState.die) return;

        if (pv.IsMine)
        {
            MonStateUpdate();
            MonActionUpdate();
            TargetMark();
        }
        else
        {
            Remote_TrUpdate();
            Remote_TakeDamage();
            Remote_Animation();
        }


    }
    public void TargetMark()
    {
        if (isTarget)
        {
            HUDCanvas.gameObject.SetActive(true);
            targetMark.gameObject.SetActive(true);
        }
        else
        {
            HUDCanvas.gameObject.SetActive(false);
            targetMark.gameObject.SetActive(false);
        }

    }

    void MonStateUpdate()
    {
        if (m_AggroTarget == null)  //어그로 타겟이 존재하지 않을 경우 대상 최초 설정
        {
            //새로운 어그로 타겟 찾기
            GameObject[] a_Players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < a_Players.Length; i++)
            {
                //현재 몬스터에서 플레이어를 향하는 벡터
                m_CacVLen = a_Players[i].transform.position - this.transform.position;
                m_CacVLen.y = 0.0f;
                m_MoveDir = m_CacVLen.normalized;   //주인공을 향하는 단위 벡터
                m_CacDist = m_CacVLen.magnitude;    //주인공까지의 직선거리

                if (m_CacDist <= m_AttackDist)  //플레이어까지의 직선거리가 공격범위 내에 들어왔는지 판정
                {
                    MonState = AnimState.attack;
                    m_AggroTarget = a_Players[i];
                    break;
                }
                else if (m_CacDist <= m_TraceDist)  //플레이어까지의 직선거리가 추적범위 내에 들어왔는지 판정
                {
                    MonState = AnimState.trace;
                    m_AggroTarget = a_Players[i];
                    break;
                }
            }

            if (m_AggroTarget == null)  //for문을 다 돌아도 타겟이 안정해졌다면
            {
                MonState = AnimState.idle;
                m_AggroTgID = -1;
            }
        }
        else        //어그로 타겟이 존재할 경우 갱신
        {
            m_CacVLen = m_AggroTarget.transform.position - this.transform.position;
            m_CacVLen.y = 0.0f;
            m_MoveDir = m_CacVLen.normalized;   //주인공을 향하는 벡터
            m_CacDist = m_CacVLen.magnitude;    //주인공까지의 직선거리


            if (m_CacDist <= m_AttackDist)  //플레이어까지의 직선거리가 공격범위 내에 들어왔는지 판정
            {
                MonState = AnimState.attack;

            }
            else if (m_CacDist <= m_TraceDist)
            {
                MonState = AnimState.trace;
            }
            else
            {
                MonState = AnimState.idle;
                m_AggroTarget = null;
                m_AggroTgID = -1;
            }
        }

    }

    void MonActionUpdate()
    {
        if (m_AggroTarget == null)
        {
            //타겟이 없으면 애니메이션 상태를 Idle로
            ChangeAnimState(AnimState.idle, 0.12f);
        }
        else  //타겟이 존재하면
        {
            if (MonState == AnimState.attack)
            {
                if (0.0001f < m_MoveDir.magnitude)
                {
                    m_TargetRot = Quaternion.LookRotation(m_MoveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRot, m_RotSpeed * Time.deltaTime);
                }
                ChangeAnimState(AnimState.attack, 0.12f);
            }
            else if (MonState == AnimState.trace)
            {
                if (0.0001f < m_MoveDir.magnitude)
                {
                    m_TargetRot = Quaternion.LookRotation(m_MoveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRot, m_RotSpeed * Time.deltaTime);
                }

                //아직 공격 애니메이션 중이면 공격 애니메이션이 끝난 후 추적하도록
                if (IsAttackAnim()) return;

                //이동
                m_MoveNextStep = m_MoveDir * Time.deltaTime * m_MoveVelocity;
                m_MoveNextStep.y = 0.0f;
                transform.position += m_MoveNextStep;

                ChangeAnimState(AnimState.trace, 0.12f);
            }
            else if (MonState == AnimState.idle)
            {
                ChangeAnimState(AnimState.idle, 0.12f);
            }
        }
    }

    //애니메이션 상태 변경 메서드
    void ChangeAnimState(AnimState newState, float CrossTime = 0.0f)
    {
        if (m_Prestate == newState) return; //기존 상태와 새로운 상태가 같으면 리턴

        if (m_RefAnimation != null)
        {
            string a_StrAnim = anim.Idle.name;
            if (newState == AnimState.idle)
                a_StrAnim = anim.Idle.name;
            else if (newState == AnimState.trace)
                a_StrAnim = anim.Move.name;
            else if (newState == AnimState.attack)
                a_StrAnim = anim.Attack1.name;
            else if (newState == AnimState.die)
                a_StrAnim = anim.Die.name;

            if (0.0f < CrossTime)
                m_RefAnimation.CrossFade(a_StrAnim, CrossTime);
            else
                m_RefAnimation.Play(a_StrAnim);
        }

        if (m_RefAnimator != null)
        {
            m_RefAnimator.ResetTrigger(m_Prestate.ToString());  //기존에 적용되어 있던 Trigger 변수 제거
            
            if (0.0f < CrossTime)
                m_RefAnimator.SetTrigger(newState.ToString());
            else
            {
                string animName = anim.Idle.name;
                if (newState == AnimState.die)
                    animName = anim.Die.name;

                m_RefAnimator.Play(animName, -1, 0);
            }
        }

        m_Prestate = newState;
        m_CurState = newState;
    }

    //공격애니메이션 관련 변수
    float m_CacRate = 0.0f;
    float m_NormalTime = 0.0f;

    bool IsAttackAnim() //현재 공격 애니메이션 중인지 아닌지 체크하는 함수
    {
        //if (m_RefAnimation == null) return false;

        if (m_RefAnimation != null)
        {
            if (m_RefAnimation.IsPlaying(anim.Attack1.name))
            {
                m_NormalTime = m_RefAnimation[anim.Attack1.name].time /
                                m_RefAnimation[anim.Attack1.name].length;

                //m_RefAnimation[anim.Attack1.name].time : 애니메이션의 현재 재생 시간값
                //m_RefAnimation[anim.Attack1.name].length : 애니메이션 한 동작의 총 시간값
                //time값은 루프가 걸려있으면 애니메이션 한동작이 끝나고 다음동작으로 갈 때 값이 초기화되지 않고
                //계속 누적된다.

                //그래서~ 누적된 정수값을 빼주는 것
                m_CacRate = m_NormalTime - (int)m_NormalTime;

                if (m_CacRate <= 0.95f)
                    return true;
            }
        }

        if (m_RefAnimator != null)
        {
            //현재 애니메이션 상태 정보를 가져옴
            AnimatorStateInfo stateInfo = m_RefAnimator.GetCurrentAnimatorStateInfo(0);

            //현재 상태가 공격 애니메이션인지 체크
            if (stateInfo.IsName(anim.Attack1.name))
            {
                //애니메이션의 진행도를 체크
                m_NormalTime = stateInfo.normalizedTime % 1.0f;

                //애니메이션이 아직 끝부분이 아니라면 (95% 진행)
                if (m_NormalTime < 0.95f)
                    return true;
            }
        }

        return false;

    }

    public void Event_AttHit()  //애니메이션 이벤트함수를 통해 호출
    {
        if (m_AggroTarget == null) return;  //대상이 없으면 리턴

        Vector3 a_DistVec = m_AggroTarget.transform.position - transform.position;
        float a_CacLen = a_DistVec.magnitude;
        a_DistVec.y = 0.0f;

        //공격각도 안에 있는 경우
        if (Vector3.Dot(transform.forward, a_DistVec.normalized) < 0.0f) //전반 반경 90도 범위 바깥
            return;

        //공격범위 밖에 있는 경우
        if ((m_AttackDist + 1.7f) < a_CacLen) return;

        m_AggroTarget.GetComponent<Hero_Ctrl>().TakeDamage(10);
    }

    public void TakeDamage(GameObject a_Attacker, float a_Damage = 10.0f)
    {
        if (curHp <= 0) return;

        if (pv.IsMine)
        {
            curHp -= a_Damage;

            if (curHp < 0)
                curHp = 0;

            if (hpBarImg != null)
                hpBarImg.fillAmount = curHp / maxHp;

            m_DieDir = transform.position - a_Attacker.transform.position;
            m_DieDir.y = 0.0f;
            m_DieDir.Normalize();
        }
        //else
        //{
        //    Remote_TakeDamage();
        //}

        Vector3 a_CacPos = this.transform.position;
        a_CacPos.y += 1.9f;
        GameMgr.Inst.SpawnDamageText((int)a_Damage, a_CacPos);

        //사망처리
        if (pv.IsMine)
        if (curHp <= 0)
        {
            CreateItem();

            float RespawnTime = Random.Range(3.5f, 12.0f);
            MonSpawnMgr.inst.m_SpawnPos[m_SpawnIdx].m_SpawnTime = RespawnTime;
            ChangeAnimState(AnimState.die, 0.12f); //애니메이션 상태 
            FindDefShader();    //쉐이더 찾기
            
            StartCoroutine(DieDirection()); //쥬금연출 코루틴

                //if (monType == MonType.Alien)
                //{
                //    ChangeAnimState(AnimState.die);

                //    PhotonNetwork.Destroy(this.gameObject);    //Destroy(this.gameObject, 2.0f);
                //}
                //else
                //    PhotonNetwork.Destroy(this.gameObject);    //Destroy(this.gameObject);
            }
    }

    void Remote_TrUpdate()
    {
        if (5.0f < (transform.position - curPos).magnitude)
        {
            transform.position = curPos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10.0f);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, curRot, Time.deltaTime * 10.0f);
    }

    void Remote_TakeDamage()
    {
        if (0.0f < curHp)
        {
            curHp = netHp;
            hpBarImg.fillAmount = curHp / maxHp;

            if (curHp <= 0)
            {
                curHp = 0.0f;
                //사망 연출만
                FindDefShader();    //쉐이더 찾기

                StartCoroutine(DieDirection()); //쥬금연출 코루틴
            }

        }
        else
        {
            curHp = netHp;
            hpBarImg.fillAmount = curHp / maxHp;
        }
    }

    void Remote_Animation()
    {
        ChangeAnimState(m_CurState, 0.12f);
    }

    //아이템 생성 함수
    void CreateItem()
    {
        int a_Ran = Random.Range(0, 11);
        if (2 < a_Ran) return;  // 30%확률로만 스폰되도록 함

        if (PhotonNetwork.IsMasterClient == false) return;

        Vector3 a_HPos = transform.position;
        //a_HPos.y += 1.0f;

        PhotonNetwork.InstantiateRoomObject("DiamondPrefab", a_HPos, Quaternion.identity, 0);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);    //위치
            stream.SendNext(transform.rotation);    //회전값
            stream.SendNext(curHp);                 //체력값
            stream.SendNext(m_DieDir);              //얻어맞은 방향
            stream.SendNext(m_AggroTgID);           //어그로타겟의 고유넘버

            stream.SendNext((int)m_CurState);       //애니메이션 상태  
            stream.SendNext(m_SpawnIdx);            //스폰위치 인덱스
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            netHp = (float)stream.ReceiveNext();
            m_DieDir = (Vector3)stream.ReceiveNext();
            m_AggroTgID = (int)stream.ReceiveNext();

            m_CurState = (AnimState)stream.ReceiveNext();
            m_SpawnIdx = (int)stream.ReceiveNext();
        }
    }

    void FindDefShader()
    {
        if (m_SMR == null)
        {
            //범용적인 용도를 위해서 SkinnedMeshRenderer를 배열과 그냥 하나짜리 통으로 구분해서 가져옴
            m_SMRList = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();  
            m_MeshList = gameObject.GetComponentsInChildren<MeshRenderer>();
            m_SMR = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        }
    }

    Transform m_Canvas = null;
    IEnumerator DieDirection()  //몬스터 죽는 연출
    {
        m_DieDur = 2.0f;
        m_DieTimer = 2.0f;
        //m_DieDir = transform.position - a_Attacker.transform.position;
        //m_DieDir.y = 0.0f;
        //m_DieDir.Normalize();

        BoxCollider a_BoxColl = gameObject.GetComponentInChildren<BoxCollider>();
        if (a_BoxColl != null)
            a_BoxColl.enabled = false;

        while (true)
        {
            m_Ratio = m_DieTimer / m_DieDur;    //투명도 계산
            m_Ratio = Mathf.Min(m_Ratio, 1.0f);
            m_CalColor = new Color(1.0f, 1.0f, 1.0f, m_Ratio);

            //뒤로 밀리게
            if (0.9f < m_Ratio && 0.0f < m_DieDir.magnitude)
            {
                transform.position = transform.position + m_DieDir
                    * (((m_Ratio * 0.38f) * 14.0f) * Time.deltaTime);
            }
            //HUD 끄기
            if (m_Ratio < 0.83f)    
            {
                if (m_Canvas == null)
                    m_Canvas = transform.Find("Canvas");
                if (m_Canvas != null)
                    m_Canvas.gameObject.SetActive(false);
            }
            //캐릭터 투명화
            for (int i = 0; i < m_SMRList.Length; i++) 
            {
                if (GameMgr.Inst.g_VertexLitShader != null
                    && m_SMRList[i].material.shader != GameMgr.Inst.g_VertexLitShader)
                {
                    m_SMRList[i].material.shader = GameMgr.Inst.g_VertexLitShader;
                }

                m_SMRList[i].material.SetColor("_Color", m_CalColor);
            }
            //무기 투명화
            if (m_MeshList != null)
            {
                for (int i = 0; i < m_MeshList.Length; i++) 
                {
                    if (GameMgr.Inst.g_VertexLitShader != null
                        && m_MeshList[i].material.shader != GameMgr.Inst.g_VertexLitShader)
                    {
                        m_MeshList[i].material.shader = GameMgr.Inst.g_VertexLitShader;
                    }

                    m_MeshList[i].material.SetColor("_Color", m_CalColor);
                }
            }
            //연출 타이머
            m_DieTimer -= Time.deltaTime;
            if (m_DieTimer < 0.0f)
            {
                if (pv.IsMine)
                    PhotonNetwork.Destroy(this.gameObject);
                yield break;
            }

            yield return null;
        }
    }
}
