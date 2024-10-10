using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Hero_Ctrl : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]public PhotonView pv = null;

    //--- HP 표시
    [HideInInspector] public float CurHp = 1000;
    [HideInInspector] public float MaxHP = 1000;
    float NetHp = 1000; //중계용 체력
    public Image ImgHpbar;

    //--- 키보드 이동 관련 변수 선언
    float h = 0, v = 0;

    float m_CacRotY = 0.0f;
    float m_RotKbSpeed = 150.0f;    //초당 150도 회전하려는 속도
    //--- 키보드 이동 관련 변수 선언

    float m_MoveVelocity = 5.0f;  //평면 초당 이동 속도...

    //--- JoyStick 이동 처리 변수
    float m_JoyMvLen = 0.0f;
    Vector3 m_JoyMvDir = Vector3.zero;
    //--- JoyStick 이동 처리 변수

    //--- Picking 관련 변수
    Ray m_MousePos;
    RaycastHit hitInfo;
    LayerMask LayerMask = -1;

    bool m_IsPickMoveOnOff = false;     //피킹 이동 OnOff
    Vector3 m_TargetPos = Vector3.zero; //최종 목표 위치
    Vector3 m_MoveDir = Vector3.zero;   //x, z평면 진행 방향
    double m_MoveDurTime = 0;           //목표점가지 도착하는데 걸리는 시간
    double m_AddTimeCount = 0;          //누적 시간 카운트
    Vector3 m_CacLenVec = Vector3.zero; //이동 계산용 변수
    Quaternion m_TargetRot = Quaternion.identity; //회전 계산용 변수
    float m_RotSpeed = 7.0f;            //초당 7도 회전하려는 속도
    //--- Picking 관련 변수

    //--- Animator 관련 변수
    Animator m_Animator = null;
    AnimState m_PreState = AnimState.idle;
    AnimState m_CurState = AnimState.idle;
    //--- Animator 관련 변수

    //--- 공격 관련 변수
    GameObject[] m_EnemyList = null;    //필드 상의 몬스터들을 가져오기 위한 변수
    float m_AttackDist = 1.9f;          //공격거리    
    GameObject m_TargetEnemy = null;    //공격 대상 몬스터 객체 참조 변수
    Vector3 m_CacTgVec = Vector3.zero;  //대상까지의 거리 계산용 변수
    Vector3 m_CacAtDir = Vector3.zero;  //공격시 방향 전환용 변수
    
    //--- 공격 관련 변수

    //--- Photon을 통해 위치 정보를 송수신할 때 사용할 변수 
    Vector3 curPos = Vector3.zero;
    Quaternion curRot = Quaternion.identity;

    public Text NickName;

    void Awake()
    {
        pv = GetComponent<PhotonView>();

        if (pv.IsMine)  //카메라가 isMine만 따라다니도록 함. 여러개의 캐릭터가 있을 때 무엇을 따라다닐지 모르니까
        {
            Camera_Ctrl a_CamCtrl = Camera.main.GetComponent<Camera_Ctrl>();
            if (a_CamCtrl != null)
                a_CamCtrl.InitCamera(this.gameObject);
        }        
    }

    // Start is called before the first frame update
    void Start()
    {
        if(pv.IsMine == true)
            GameMgr.Inst.m_RefHero = this;

        m_Animator = this.GetComponent<Animator>();

        LayerMask = 1 << LayerMask.NameToLayer("MyTerrain");
        LayerMask |= 1 << LayerMask.NameToLayer("MyUnit");

        //원격 Hero의 위치 및 회전 값을 처리할 변수의 초기값 설정
        curPos = transform.position;
        curRot = transform.rotation;

        NickName.text = pv.Owner.NickName;
    }

    // Update is called once per frame
    void Update()
    {
        //나가는 타이밍에 포톤 정보들이 한프레임 먼저 사라지고
        //LobbyScene()이 한프레임 늦게 호출되는 문제 해결
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.LocalPlayer == null) return;

        if (pv.IsMine)
        {
            MousePickCheck();

            EnemyMonitor();

            KeyBDMove();
            JoyStickMvUpdate();
            MousePickUpdate();

            TargetCheck();

            UpdateAnimState();
        }
        else    //!pv.IsMine의 아바타들 
        {
            if (10.0f < (transform.position - curPos).magnitude)
            {
                //딜레이 때문에 거리격차가 너무 벌어지면 의도적으로 점프시키기
                transform.position = curPos;
            }
            else
            {
                //원격 플레이어의 플레이어를 수신받은 위치까지 부드럽게 이동시키기
                transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10.0f);
            }

            //원격 플레이어의 플레이어를 수신받은 각도만큼 부드럽게 회전시키기
            transform.rotation = Quaternion.Slerp(transform.rotation, curRot, Time.deltaTime * 10.0f);

            //Remote Animation
            ChangeAnimState(m_CurState);    //아바타들 애니메이션 동기화

            Remote_TakeDamage();    //아바타들 Hp 동기화
        }
    }

    void KeyBDMove()
    {
        h = Input.GetAxisRaw("Horizontal"); //화살표키 좌우키를 누르면 -1.0f ~ 1.0f
        v = Input.GetAxisRaw("Vertical");

        if(0.0f != h || 0.0f != v)
        {
            //m_TargetEnemy = null;

            if (IsSkill()) return;  //스킬 발동 중일때는 키보드 이동 금지

            ClearMsPickMove();

            m_CacRotY = transform.eulerAngles.y;
            m_CacRotY += h * m_RotKbSpeed * Time.deltaTime;
            transform.eulerAngles = new Vector3(0.0f, m_CacRotY, 0.0f);

            Vector3 MoveVec = transform.forward * Mathf.Clamp(v, 0.0f, 1.0f);
            transform.position += MoveVec.normalized * m_MoveVelocity * Time.deltaTime;
            ChangeAnimState(AnimState.move);
        }
    }//void KeyBDMove()

    public void SetJoyStickMv(float a_JoyMvLen, Vector3 a_JoyMvDir)
    {
        m_JoyMvLen = a_JoyMvLen;
        if(0.0f < a_JoyMvLen)
        {
            //마우스 피킹 이동 취소
            ClearMsPickMove();

            //m_JoyMvDir.x = a_JoyMvDir.x;
            //m_JoyMvDir.y = 0.0f;
            //m_JoyMvDir.z = a_JoyMvDir.y;
            //m_JoyMvDir.Normalize();

            //--- 카메라가 바라보고 있는 전면을 기준으로 회전 시켜주는 코드
            Vector3 a_CamFwVec = Camera.main.transform.forward;
            a_CamFwVec.y = 0.0f;
            a_CamFwVec.Normalize();
            m_JoyMvDir = a_CamFwVec * a_JoyMvDir.y;
            //위 아래 조작(카메라가 바라보고 있는 기준으로 앞, 뒤(Z성분)로 얼만큼 이동시킬 건지?
            Vector3 a_CamRtVec = Camera.main.transform.right;
            m_JoyMvDir += a_CamRtVec * a_JoyMvDir.x;
            //좌우 조작(카메라가 바라보고 있는 기준으로 좌, 우(X성분)로 얼만큼 이동시킬 건지?
            m_JoyMvDir.y = 0.0f;
            m_JoyMvDir.Normalize();
            //--- 카메라가 바라보고 있는 전면을 기준으로 회전 시켜주는 코드

        }//if(0.0f < a_JoyMvLen)
    }

    void JoyStickMvUpdate()
    {
        if (0.0f != h || 0.0f != v)
            return;

        //--- 조이스틱 이동 처리
        if(0.0f < m_JoyMvLen)
        {
            //m_TargetEnemy = null;

            if (IsSkill()) return;  //스킬 발동 중일때는 조이스틱 이동 금지

            m_MoveDir = m_JoyMvDir;

            //--- 캐릭터 회전
            if (0.0001f < m_JoyMvDir.magnitude)
            {
                m_TargetRot = Quaternion.LookRotation(m_JoyMvDir);
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                        m_TargetRot, Time.deltaTime * m_RotSpeed);
            }
            //--- 캐릭터 회전

            transform.position += m_JoyMvDir * (m_MoveVelocity * Time.deltaTime);
            ChangeAnimState(AnimState.move);

        }//if(0.0f < m_JoyMvLen)
    }//void JoyStickMvUpdate()

    void TargetCheck()
    {
        if (m_TargetEnemy != null)
        {
            m_TargetEnemy.gameObject.GetComponent<Monster_Ctrl>().isTarget = true;

            Vector3 a_CacTgVec = m_TargetEnemy.transform.position - transform.position;
            a_CacTgVec.y = 0.0f;
            if (a_CacTgVec.magnitude > 15.0f)
                m_TargetEnemy = null;
        }
        else
        {
            m_EnemyList = GameObject.FindGameObjectsWithTag("Enemy");
            if (m_EnemyList != null)
            {
                for (int i = 0; m_EnemyList.Length > i; i++)
                {
                    m_EnemyList[i].GetComponent<Monster_Ctrl>().isTarget = false;
                }
            }
            //m_TargetEnemy.gameObject.GetComponent<Monster_Ctrl>().isTarget = false;
        }

    }
    void MousePickCheck()   //마우스 클릭 감지를 위한 함수
    {
        if(Input.GetMouseButtonDown(0) == true) //왼쪽 마우스 버튼 클릭시
        if(GameMgr.IsPointerOverUIObject() == false)    //UI가 아닌 곳을 클릭했을 때만 피킹 이동 허용
        {
            if (IsSkill()) return;  //스킬 발동 중일때는 마우스 이동 금지

            m_MousePos = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(m_MousePos, out hitInfo, Mathf.Infinity, LayerMask.value))
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("MyUnit"))
                {
                    //마우스로 몬스터를 피킹 했을 경우
                    MousePicking(hitInfo.point, hitInfo.collider.gameObject);

                    //hitInfo.collider.gameObject.GetComponent<Monster_Ctrl>().isTarget = true;
                } 
                else
                {
                    //마우스로 지형을 피킹 했을 경우
                    MousePicking(hitInfo.point);
                    
                    GameMgr.Inst.CursorMarkOn(hitInfo.point);
                }

            }
        }//if(Input.GetMouseButtonDown(0) == true) //왼쪽 마우스 버튼 클릭시
    }//void MousePickCheck()   //마우스 클릭 감지를 위한 함수

    void MousePicking(Vector3 a_PickVec, GameObject a_PickMon = null) //마우스 클릭 처리 함수
    {
        a_PickVec.y = transform.position.y;     //목표 위치
        Vector3 a_StartPos = transform.position;    //출발 위치

        m_CacLenVec = a_PickVec - a_StartPos;
        m_CacLenVec.y = 0.0f;

        //Picking enemy 공격 처리 부분
        if (a_PickMon != null)
        {
            //지금 공격하려고 하는 몬스터의 어그로 타겟이 나라면
            //몬스터가 다가오는 것과 내가 이동하는 것때문에 공격거리에 조금의 여유가 있어야
            //자연스럽게 애니메이션이 재생됨
            float a_AttDist = m_AttackDist;
            if (a_PickMon.GetComponent<Monster_Ctrl>().m_AggroTarget == this.gameObject)
            {
                a_AttDist += 1.0f;
            }

            m_CacTgVec = a_PickMon.transform.position - transform.position;

            if (m_CacTgVec.magnitude <= a_AttDist)
            {
                m_TargetEnemy = a_PickMon;
                AttackOrder();  //즉시공격
                ClearMsPickMove();

                return;
            }
        }

        if (m_CacLenVec.magnitude < 0.5f)   //너무 근거리 피킹은 스킵해 준다.
            return;

        m_TargetPos = a_PickVec;    //최종 목표 위치
        m_IsPickMoveOnOff = true;   //피킹 이동 OnOff

        m_MoveDir = m_CacLenVec.normalized;
        m_MoveDurTime = m_CacLenVec.magnitude / m_MoveVelocity; //도착하는데까지 걸리는 시간
        m_AddTimeCount = 0.0;

        //if (m_TargetEnemy != null && a_PickMon == null)
        //    m_TargetEnemy.gameObject.GetComponent<Monster_Ctrl>().isTarget = false;

        m_TargetEnemy = a_PickMon;
    }

    void MousePickUpdate()  //마우스 클릭으로 캐릭터 이동을 계산하는 함수
    {
        if(m_IsPickMoveOnOff == true)
        {
            m_CacLenVec = m_TargetPos - transform.position;
            m_CacLenVec.y = 0.0f;

            m_MoveDir = m_CacLenVec.normalized;

            //캐릭터를 이동방향으로 회전시키는 코드
            if(0.0001f < m_CacLenVec.magnitude)
            {
                m_TargetRot = Quaternion.LookRotation(m_MoveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                    m_TargetRot, Time.deltaTime * m_RotSpeed);
            }
            //캐릭터를 이동방향으로 회전시키는 코드

            m_AddTimeCount += Time.deltaTime;
            if(m_MoveDurTime <= m_AddTimeCount) //목표점에 도착한 것으로 판정한다.
            {
                ClearMsPickMove();
            }
            else
            {
                transform.position += m_MoveDir * Time.deltaTime * m_MoveVelocity;
                ChangeAnimState(AnimState.move);
            }

            //타겟을 향해 피킹 이동 공격
            if (m_TargetEnemy != null)
            {
                m_CacTgVec = m_TargetEnemy.transform.position - transform.position;

                if (m_CacTgVec.magnitude <= m_AttackDist)   //이동하다가 공격거리 안쪽으로 들어오면
                {
                    AttackOrder();
                    ClearMsPickMove();
                }
            }
        }//if (m_IsPickMoveOnOff == true)

    }//void MousePickUpdate()  //마우스 클릭으로 캐릭터 이동을 계산하는 함수

    void ClearMsPickMove()
    {
        m_IsPickMoveOnOff = false; //마우스 클릭 이동 취소

        //마우스 클릭 취소
        if (GameMgr.Inst.m_CursorMark != null)
            GameMgr.Inst.m_CursorMark.SetActive(false);

    }//void ClearMsPickMove()

    //애니메이션 상태 변경 메서드
    public void ChangeAnimState(AnimState newState, float crossTime = 0.1f, string animName = "")
    {
        if (m_Animator == null)
            return;

        if (m_PreState == newState)
            return;
        


        m_Animator.ResetTrigger(m_PreState.ToString()); //기존에 적용되어 있던 Trigger 변수를 제거

        if (0.0f < crossTime)
        {
            m_Animator.SetTrigger(newState.ToString());
        }
        else
        {
            m_Animator.Play(animName, -1, 0);
            //가운데 -1은 Layer Index, 뒤에 0는 처음부터 다시 시작 플레이 시키겠다는 의미
        }

        m_PreState = newState;
        m_CurState = newState;
    }

    //애니메이션 상태 업데이트 메서드
    void UpdateAnimState()
    {
        if (IsAttack() == true)
        {
            AttackRotUpdate();  //공격하면서 대상을 향해 회전시키기 위한 함수
            return;
        }
            

        //키보드, 조이스틱, 피킹 이동중이 아닐 때는 아이들 동작으로 돌아가게 한다.
        if((0.0f == h && 0.0f == v) && m_JoyMvLen <= 0.0f 
            && m_IsPickMoveOnOff == false && IsAttack() == false)
        {
            ChangeAnimState(AnimState.idle);
        }
    }

    //현재 공격 중인지 확인하는 메서드
    public bool IsAttack()
    {
        return m_CurState == AnimState.attack || m_CurState == AnimState.skill;
    }

    //현재 스킬 애니메이션 중인지 확인하는 메서드
    public bool IsSkill()
    {
        return m_CurState == AnimState.skill;
    }

    //공격 명령 처리 메서드
    public void AttackOrder()
    {
        if (pv.IsMine == false) return;

        if (IsAttack() == false) //공격중이거나 스킬 사용중이 아닐때만...
        {
            //키보드 컨트롤이나 조이스틱 컨트롤로 이동 중이고
            //공격키를 연타해서 누르면 달리는 애니메이션에 잠깐동안
            //공격 애니가 끼어드는 문제가 발생한다. <--- 이런 현상에 대한 예외처리
            if ((0.0f != h || 0.0f != v) || 0.0f < m_JoyMvLen)
                return;

            
            ChangeAnimState(AnimState.attack);
            ClearMsPickMove();
        }
    }//public void AttackOrder()

    //스킬 명령 처리 메서드
    public void SkillOrder(string Type, ref float CoolDur, ref float CurCool)
    {
        if (pv.IsMine == false) return;

        if (0.0f < CurCool) return;

        if (m_PreState == AnimState.skill) return;

        ChangeAnimState (AnimState.skill);

        ClearMsPickMove();

        CoolDur = 7.0f; //CoolDuration
        CurCool = CoolDur;
    }

    #region --- Event Method

    void Event_AttHit()
    {
        m_EnemyList = GameObject.FindGameObjectsWithTag("Enemy");
        int a_iCount = m_EnemyList.Length;
        float a_fCacLen = 0.0f;
        GameObject a_EffObj = null;
        Vector3 a_EffPos = Vector3.zero;
        //일정 범위내 주변 모든 몬스터를 찾아서 데미지를 준다.
        for (int i = 0; i < a_iCount; i++)
        { 
            m_CacTgVec = m_EnemyList[i].transform.position - transform.position;
            a_fCacLen = m_CacTgVec.magnitude;
            m_CacTgVec.y = 0.0f;

            //공격각도 안에 있는데... (아래 두개의 조건을 충족시키면 얼추 부채꼴 모양이 나옴)
            //45도 정도의 범위 바깥에 위치한다면 아웃
            if (Vector3.Dot(transform.forward, m_CacTgVec.normalized) < 0.45f)
                continue;
            //공격거리 밖에 있는 경우 아웃
            if (m_AttackDist + 0.1f < a_fCacLen)
                continue;

            a_EffObj = EffectPool.Inst.GetEffectObj("FX_Hit_01", Vector3.zero, Quaternion.identity);
            a_EffPos = m_EnemyList[i].transform.position;
            a_EffPos.y += 1.1f;
            a_EffObj.transform.position = a_EffPos + (-m_CacTgVec.normalized * 1.13f);
            a_EffObj.transform.LookAt(a_EffPos + (m_CacTgVec.normalized * 2.0f));
            m_EnemyList[i].GetComponent<Monster_Ctrl>().TakeDamage(this.gameObject);
        }
    }

    void Event_SkillHit()
    {
        m_EnemyList = GameObject.FindGameObjectsWithTag("Enemy");
        int a_iCount = m_EnemyList.Length;
        float a_fCacLen = 0.0f;
        GameObject a_EffObj = null;
        Vector3 a_EffPos = Vector3.zero;

        a_EffObj = EffectPool.Inst.GetEffectObj("FX_AttackCritical_01", Vector3.zero, Quaternion.identity);
        a_EffPos = transform.position;
        a_EffPos.y += 1.0f;
        a_EffObj.transform.position = a_EffPos + (transform.forward * 2.3f);
        a_EffObj.transform.LookAt(a_EffPos + (-transform.forward * 2.0f));
        for (int i = 0; i < a_iCount; ++i)
        {
            m_CacTgVec = m_EnemyList[i].transform.position - transform.position;
            //공격데미지 값은 y축까지 고려하여 넣기위해 magnitude를 먼저 해주고, y를 빼줌
            a_fCacLen = m_CacTgVec.magnitude;   
            m_CacTgVec.y = 0.0f;

            //공격 각도 제한 없이 360도 모두 데미지 주기
            if (m_AttackDist + 0.1f < a_fCacLen)
                continue;


            a_EffObj = EffectPool.Inst.GetEffectObj("FX_Attack01_01", Vector3.zero, Quaternion.identity);
            a_EffPos = m_EnemyList[i].transform.position;
            a_EffPos.y += 1.1f;
            a_EffObj.transform.position = a_EffPos + (-m_CacTgVec.normalized * 1.13f);
            a_EffObj.transform.LookAt(a_EffPos + (m_CacTgVec.normalized * 2.0f));

            m_EnemyList[i].GetComponent<Monster_Ctrl>().TakeDamage(this.gameObject, 30);
        }

    }
    //공격 종료 이벤트 처리 메서드
    void Event_AttFinish()
    {
        if (pv.IsMine == false) return; //공격 애니메이션이 끝나고 다음 행동에 대한 판단도 IsMine만
            
        if ((0.0f != h || 0.0f != v) || 0.0f < m_JoyMvLen || m_IsPickMoveOnOff)
        {
            //스킬 사용 중 키보드조작, 조이스틱이 있는 경우
            //예약 즉시 취소를 위한 코드
            ChangeAnimState(AnimState.move);
            return;
        }

        //어택상태일때는 어택 상태로 끝나야 한다.
        if (m_CurState != AnimState.attack)
            return;

        if(IsTargetEnemyActive(0.2f) == true)
        {
            ChangeAnimState(AnimState.attack);
            ClearMsPickMove();

            //Debug.Log("연속공격 호출 완료");
        }
        else
        {
            ChangeAnimState(AnimState.idle);
        }
    }

    //스킬 종료 이벤트 처리 함수
    void Event_SkillFinish()
    {
        if (pv.IsMine == false) return; //공격 애니메이션이 끝나고 다음 행동에 대한 판단도 IsMine만

        if ((0.0f != h || 0.0f != v) || 0.0f < m_JoyMvLen || m_IsPickMoveOnOff)
        {
            //스킬 사용 중 키보드조작, 조이스틱이 있는 경우
            //예약 즉시 취소를 위한 코드
            ChangeAnimState(AnimState.move);
            return;
        }

        //현재 스킬 상태인데 Attack애니메이션의 끝이 들어온 경우라면 제외 시켜버림
        //공격 애니메이션 중에 스킬 발동시 공격이 끝나는 이벤트 함수가 들어와서 
        //스킬이 취소되는 현상이 발생할 수 있기 때문임
        //스킬상태일땐 스킬상태로 끝나야 한다.

        if (m_CurState != AnimState.skill)
            return;

        if (IsTargetEnemyActive(0.2f) == true)
        {
            ChangeAnimState(AnimState.attack);
            ClearMsPickMove();
            //Debug.Log("연속공격 호출 완료");
        }
        else
        {
            ChangeAnimState(AnimState.idle);
        }
    }
#endregion

    // 타겟 적이 유효한지 확인하는 함수
    bool IsTargetEnemyActive(float extraRange = 0.0f)
    {
        if(m_TargetEnemy == null)
            return false;

        // 타겟이 활성화되어 있지 않으면 타겟 해제
        if(m_TargetEnemy.activeSelf == false)
            return false;

        //죽어있어도 해제
        Monster_Ctrl a_Unit = m_TargetEnemy.GetComponent<Monster_Ctrl>();
        if (a_Unit.m_CurState == AnimState.die)
        {
            m_TargetEnemy = null;
            return false;
        }

        m_CacTgVec = m_TargetEnemy.transform.position - transform.position;
        m_CacTgVec.y = 0.0f;
        if (m_AttackDist + extraRange < m_CacLenVec.magnitude)
        {
            //공격거리 + 추가범위 바깥쪽에 있을 경우 타겟 무효화
            return false;
        }

        return true;
    }

    void EnemyMonitor()
    {
        //마우스 피킹을 시도했고 이동 중이면 타겟을 다시 잡지 않는다.
        if (m_IsPickMoveOnOff) return;

        //보간 때문에 정확히 정밀한 공격 애니메이션을 하고 있을 때만
        if (!IsAttack()) return;

        //공격 애니메이션 중이고 타겟이 무효화 되었다면 타겟을 새로 잡아준다.
        //타겟의 교체는 공격거리보다는 조금 더 여유있게 
        if (IsTargetEnemyActive(0.5f) == false) 
            FindEnemyTarget();
    }

    void FindEnemyTarget()
    {
        m_EnemyList = GameObject.FindGameObjectsWithTag("Enemy");
        float a_MinLen = float.MaxValue;
        int a_iCount = m_EnemyList.Length;
        m_TargetEnemy = null;     //타겟 초기화 해놓고 시작
        for (int i = 0; i < a_iCount; i++)
        {
            m_CacTgVec = m_EnemyList[i].transform.position - transform.position;
            m_CacTgVec.y = 0.0f;
            if (m_CacTgVec.magnitude <= m_AttackDist)
            {
                //공격거리 안쪽에 있을 경우 타겟 잡기
                if (m_CacTgVec.magnitude < a_MinLen)    //주인공과의 거리가 가장 가까운 대상
                {
                    a_MinLen = m_CacTgVec.magnitude;
                    m_TargetEnemy = m_EnemyList[i];                    
                    
                    //Debug.Log(m_TargetEnemy.name);
                }
            }
        }

    }



    float m_CacRotSpeed = 0.0f;
    public void AttackRotUpdate()
    {
        if (m_TargetEnemy == null) return;
        //Debug.Log(m_TargetEnemy.name);

        //if (h != 0.0f || v != 0.0f || 0.0f < m_CacLenVec.magnitude || 0.0f < m_JoyMvLen) return;
       

        m_CacTgVec = m_TargetEnemy.transform.position - transform.position;
        m_CacTgVec.y = 0.0f;

        if (m_CacTgVec.magnitude <= m_AttackDist + 0.3f)    //공격거리
        {
            m_CacAtDir = m_CacTgVec.normalized;
            if (0.0001f < m_CacAtDir.magnitude)
            {
                m_CacRotSpeed = m_RotSpeed * 3.0f;  //초당 회전속도
                Quaternion a_TargetRot = Quaternion.LookRotation(m_CacAtDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, a_TargetRot, m_CacRotSpeed * Time.deltaTime);
            }
        }
    }
    public void TakeDamage(float damage = 10)
    {
        if (CurHp <= 0.0f) return;

        if (pv.IsMine)
        {
            //실제 데미지는 isMine일 때만 깎아주기
            CurHp -= damage;
            if (CurHp < 0.0f)
                CurHp = 0.0f;

            ImgHpbar.fillAmount = CurHp / MaxHP;
        }            

        Vector3 cacPos = this.transform.position;
        cacPos.y += 1.7f;
        GameMgr.Inst.SpawnDamageText((int)damage, cacPos, 1);

        
        if (pv.IsMine)  
        if (CurHp <= 0.0f)  //사망처리
        {
                //Die();
        }        
    }

    void Remote_TakeDamage()    //아바타 Hp동기화 함수
    {
        if (0.0f < CurHp)
        {
            CurHp = NetHp;

            //Image UI 항목의 fillAmount 속성을 조절해 생명 게이지 값 조정
            ImgHpbar.fillAmount = CurHp / (float)MaxHP;

            if (CurHp <= 0.0f)  //사망처리 한번만 호출
            {
                //Die();
            }

        }
    }
    [PunRPC]
    public void TakeItemRPC(int a_ItemType)
    {
        //자기가 조종하고 있는 Player를 기준으로 처리해준다.
        if (!pv.IsMine) return;

        if (CurHp <= 0.0f) return;

        CurHp += 100;

        if (MaxHP <= CurHp)
            CurHp = MaxHP;

        Vector3 cacPos = this.transform.position;
        cacPos.y += 1.7f;
        GameMgr.Inst.SpawnDamageText((int)100, cacPos, 1);

        ImgHpbar.fillAmount = CurHp / (float)MaxHP;

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보 송신
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(CurHp);
            stream.SendNext((int)m_CurState);
        }
        else //원격 플레이어의 위치 정보 수신
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            NetHp = (float)stream.ReceiveNext();
            m_CurState = (AnimState)stream.ReceiveNext();

        }
    }
}
