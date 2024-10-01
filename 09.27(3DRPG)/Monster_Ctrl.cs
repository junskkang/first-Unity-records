using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonType
{
    Skeleton = 0,
    Alien,
    Count
}

public class Monster_Ctrl : MonoBehaviour
{
    public MonType monType;

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

    public GameObject targetMark = null;
    [HideInInspector] public bool isTarget = false;

    // Start is called before the first frame update
    void Start()
    {
        m_RefAnimation = GetComponentInChildren<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurState == AnimState.die) return;

        MonStateUpdate();
        MonActionUpdate();
        TargetMark();
    }
    public void TargetMark()
    {
        if (isTarget)
        {
            targetMark.gameObject.SetActive(true);
        }
        else
        {
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
            else if  (newState == AnimState.attack)
                a_StrAnim = anim.Attack1.name;
            else if (newState == AnimState.die)
                a_StrAnim = anim.Die.name;

            if (0.0f < CrossTime)
                m_RefAnimation.CrossFade(a_StrAnim, CrossTime);
            else
                m_RefAnimation.Play(a_StrAnim);
        }

        m_Prestate = newState;
        m_CurState = newState;
    }

    //공격애니메이션 관련 변수
    float m_CacRate = 0.0f;
    float m_NormalTime = 0.0f;

    bool IsAttackAnim()
    {
        if (m_RefAnimation == null) return false;

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
}
