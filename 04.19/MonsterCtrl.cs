using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonAIState
{
    MAI_Idle,           //숨쉬기 상태
    MAI_Patrol,         //패트롤 상태
    MAI_AggroTrace,     //적으로부터 공격을 당했을 때 추적 상태
    MAI_NormalTrace,    //일반 추적 상태
    MAI_ReturnPos,      //추적을 놓쳤을 때 제 위치로 돌아오는 상태
    MAI_Attack          //공격 상태
}

public class MonsterCtrl : MonoBehaviour
{
    float m_MaxHp = 100.0f;
    float m_CurHp = 100.0f;
    public Image HpBarUI = null;

    //몬스터 AI 변수들
    MonAIState m_AIState = MonAIState.MAI_Patrol;   //상태 변수

    GameObject m_AggroTarget = null;    //추적해야할 타겟 (캐릭터)

    float m_AttackDist = 15.0f;         //공격거리
    float m_TraceDist = 20.0f;          //추적거리

    float m_MoveVelocity = 2.0f;        //평면 초당 이동 속도(패트롤 기준)

    Vector3 m_MoveDir = Vector3.zero;   //평면 진행 방향
    float m_NowStep = 0.0f;             //이동 계산용 변수

    float m_ShootCool = 1.0f;           //공격주기 변수
    float m_AttackSpeed = 0.5f;         //공격속도

    //계산용 변수들
    Vector3 a_CacVLen = Vector3.zero;
    float a_CacDist = 0.0f;

    //패트롤 관련 변수들
    Vector3 m_BasePos = Vector3.zero;   //초기 스폰 위치 (기준점)
    bool m_bMvPtOnOff = false;          //Patrol Move OnOff

    float m_WaitTime = 0.0f;        //패트롤 시 목표점에 도착하면 잠시 대기
    int a_AngleRan;
    int a_LengthRan;

    Vector3 m_PatrolTarget = Vector3.zero;  //Patrol시 움직여야 될 다음 목표 좌표
    Vector3 m_DirMvVec = Vector3.zero;      //Patrol시 움직여야 될 방향 벡터
    double m_AddTimeCount = 0.0f;           //이동 총 누적시간 카운트용 변수
    double m_MoveDurTime = 0.0f;            //목표점까지 도착하는데 걸리는 시간 변수
    Quaternion a_CacPtRot;
    Vector3 a_CacPtAngle = Vector3.zero;
    Vector3 a_Vert;




    // Start is called before the first frame update
    void Start()
    {
        m_BasePos = transform.position;     //몬스터의 첫 스폰 위치 저장
        m_WaitTime = Random.Range(0.5f, 3.0f);
        m_bMvPtOnOff = false;
    }

    // Update is called once per frame
    void Update()
    {
        MonsterAI();
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.name.Contains("BulletPrefab") == true)
        {
            if (coll.gameObject.CompareTag(AllyType.BT_Enemy.ToString()) == true)
                return;         //몬스터가 쏜 총알을 몬스터가 맞았을 때 제외. 팀킬 x

            TakeDamage(10.0f);

            Destroy(coll.gameObject);   //부딪힌 총알 제거
        }

    }

    public void TakeDamage(float a_Value)
    {
        if (m_CurHp <= 0.0f)    //디스트로이가 한프레임 늦게 되기 때문에 혹시나
            return;             //보상이 두번 들어오게 되는 경우 방지

        GameMgr.Inst.DamageText((int)a_Value, this.transform.position); //데미지 폰트 함수 호출

        m_CurHp -= a_Value;

        if (m_CurHp < 0.0f)
            m_CurHp = 0.0f;

        if(HpBarUI != null)
            HpBarUI.fillAmount = m_CurHp/m_MaxHp;   //몬스터 체력바UI 표시

        m_AggroTarget = GameMgr.Inst.m_RefHero.gameObject;
        m_AIState = MonAIState.MAI_AggroTrace;

        if (m_CurHp <= 0.0f) //몬스터 사망처리
        {
            //보상

            Destroy(gameObject);    //몬스터 제거
        }
    }

    void MonsterAI()
    {
        if (0.0f < m_ShootCool)
            m_ShootCool -= Time.deltaTime;
        
        if (m_AIState == MonAIState.MAI_Patrol)     //어슬렁거리는 상태
        {
            if (GameMgr.Inst.m_RefHero != null)
            {
                a_CacVLen = GameMgr.Inst.m_RefHero.transform.position -
                    this.gameObject.transform.position;
                a_CacVLen.y = 0.0f;
                a_CacDist = a_CacVLen.magnitude; 

                if (a_CacDist < m_TraceDist)        //추적거리보다 캐릭터와 몬스터의 직선거리가 가까워지면
                {
                    m_AIState = MonAIState.MAI_NormalTrace;
                    m_AggroTarget = GameMgr.Inst.m_RefHero.gameObject;
                    return;
                }
            }

            AI_Patrol();
        }
        else if (m_AIState == MonAIState.MAI_NormalTrace)       //추적상태 일 때
        {
            if (m_AggroTarget == null)
            {
                m_AIState = MonAIState.MAI_Patrol;
                return;
            }

            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;
            if (a_CacDist < m_AttackDist)       //공격거리보다 캐릭터와의 거리가 가까워지면
            {
                m_AIState = MonAIState.MAI_Attack;
            }
            else if (a_CacDist < m_TraceDist)
            {
                m_MoveDir = a_CacVLen.normalized;
                m_MoveDir.y = 0.0f;
                m_NowStep = m_MoveVelocity * 1.5f * Time.deltaTime;     //한걸음 크기
                //일반 패트롤 상태의 이동속도 보다 1.5배 빠르게
                transform.Translate(m_MoveDir*m_NowStep, Space.World);
            }
            else
            {
                m_AIState = MonAIState.MAI_Patrol;
            }
        }
        else if (m_AIState == MonAIState.MAI_AggroTrace)        //어그로 추적 상태
        {
            if (m_AggroTarget == null)
            {
                m_AIState = MonAIState.MAI_Patrol;
                return;
            }

            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;

            if (a_CacDist < m_AttackDist)
            {
                m_AIState = MonAIState.MAI_Attack;
            }

            if ((m_AttackDist - 2.0f) < a_CacDist)
            {
                m_MoveDir = a_CacVLen.normalized;
                m_MoveDir.y = 0.0f;
                m_NowStep = m_MoveVelocity + 5.0f * Time.deltaTime;
                transform.Translate(m_MoveDir* m_NowStep, Space.World);
            }
        }
        else if (m_AIState == MonAIState.MAI_Attack)            //공격 상태
        {
            if (m_AggroTarget == null)
            {
                m_AIState = MonAIState.MAI_Patrol;
                return;
            }

            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;
            if ((m_AttackDist - 2.0f) < a_CacDist)      //공격을 위해 아직 이동해야 하는 상황이면
            {
                m_MoveDir = a_CacVLen.normalized;
                m_MoveDir.y = 0.0f;
                m_NowStep = m_MoveVelocity * 1.5f * Time.deltaTime;
                transform.Translate(m_MoveDir*m_NowStep, Space.World);
            }

            if (a_CacDist < m_AttackDist)       //공격 가능한 거리
            {
                //공격
                if (m_ShootCool <= 0.0f)
                {
                    ShootFire();
                    m_ShootCool = m_AttackSpeed;
                }
            }
            else
            {
                m_AIState = MonAIState.MAI_NormalTrace;
            }

        }
    }

    void AI_Patrol()
    {
        if (m_bMvPtOnOff == true)
        {
            m_DirMvVec = m_PatrolTarget - transform.position;   //목표지점까지의 벡터
            m_DirMvVec.y = 0.0f;
            m_DirMvVec.Normalize();

            m_AddTimeCount += Time.deltaTime;
            if (m_MoveDurTime <= m_AddTimeCount)    //목표점에 도착한 것으로 판정
                m_bMvPtOnOff = false;
            else
                transform.Translate(m_DirMvVec*Time.deltaTime*m_MoveVelocity, Space.World);
        }
        else
        {
            m_WaitTime -= Time.deltaTime;       //도착 후 잠깐 머물렀다가 다시 이동
            if (0.0f < m_WaitTime)
                return;

            m_WaitTime = 0.0f;
            a_AngleRan = Random.Range(30, 301);     //랜덤 회전값
            a_LengthRan = Random.Range(3, 8);       //랜덤 이동 반경 값

            m_DirMvVec = transform.position - m_BasePos;    //기본위치에서 현재위치를 향하는 벡터
            m_DirMvVec.y = 0.0f;

            if (m_DirMvVec.magnitude < 1.0f)        //처음 시작할 때 밖에 없음
                a_CacPtRot = Quaternion.LookRotation(transform.forward);    
            else
                a_CacPtRot = Quaternion.LookRotation(m_DirMvVec);           

            a_CacPtAngle = a_CacPtRot.eulerAngles;      //eulerAnlge 가져옴
            a_CacPtAngle.y = a_CacPtAngle.y + (float)a_AngleRan;        //y축 회전을 위해 랜덤값을 더해줌
            a_CacPtRot.eulerAngles = a_CacPtAngle;      //다시 오일러 앵글에 담아주어 회전시킴
            a_Vert = new Vector3(0, 0, 1);
            a_Vert = a_CacPtRot * a_Vert;   //쿼터니온에 벡터를 곱하면 벡터가 쿼터니온 값으로 회전이 됨
            a_Vert.Normalize();             //단위벡터화

            m_PatrolTarget = m_BasePos + (a_Vert * (float)a_LengthRan); //회전된 방향으로 랜덤한 반경의 좌표

            m_DirMvVec = m_PatrolTarget - transform.position;
            m_DirMvVec.y = 0.0f;
            m_MoveDurTime = m_DirMvVec.magnitude/m_MoveVelocity;    //도착하는데 걸리는 시간
            // 속도 = 거리/시간, 거리 = 속도 * 시간, 시간 = 거리/속도
            m_AddTimeCount = 0.0f;
            m_DirMvVec.Normalize();

            m_WaitTime = Random.Range(0.2f, 3.0f);
            m_bMvPtOnOff = true;
        }
    }

    void ShootFire()
    {
        if (m_AggroTarget == null)
            return;

        a_CacVLen = m_AggroTarget.transform.position - transform.position;
        a_CacVLen.y = 0.0f;

        Vector3 a_CacDir = a_CacVLen.normalized;        //방향 벡터

        GameObject BulletClone = Instantiate(GameMgr.m_BulletPrefab);
        BulletClone.tag = AllyType.BT_Enemy.ToString();   //"BT_Enemy";
        BulletCtrl a_BulletSc = BulletClone.GetComponent<BulletCtrl>();
        a_BulletSc.BulletSpawn(transform.position, a_CacDir, 30.0f);
        
    }
}
