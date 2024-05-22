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

    [HideInInspector] public int m_SpawnIdx = -1;   //List<SpawnPos> m_SpawnPosList의 인덱스
    int m_Level = 1;        //몬스터 레벨




    // Start is called before the first frame update
    void Start()
    {
        m_BasePos = transform.position;     //몬스터의 첫 스폰 위치 저장
        m_WaitTime = Random.Range(0.5f, 3.0f);
        m_bMvPtOnOff = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MonsterAI();
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.name.Contains("BulletPrefab") == true)
        {
            if (coll.gameObject.CompareTag(AllyType.BT_Enemy.ToString()) == true)
                return;         //몬스터가 쏜 총알을 몬스터가 맞았을 때 제외. 팀킬 x

            BulletCtrl a_Bl_Ctrl = coll.gameObject.GetComponent<BulletCtrl>();
            TakeDamage(a_Bl_Ctrl.m_Damage);

            if (a_Bl_Ctrl.m_IsPool == true)
                coll.gameObject.SetActive(false);
            else
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
            ItemDrop();
            GameMgr.Inst.monKillCount++;

            //4~6초 뒤 같은 자리에서 다시 스폰 요청
            if (m_Level < 3 && Monster_Mgr.Inst != null)        //3레벨 몬스터는 사망시 리스폰x 기획의도
                Monster_Mgr.Inst.ResetSpawn(m_SpawnIdx);

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
    float m_ShootAngle;
    void ShootFire()
    {
        if (m_AggroTarget == null)
            return;

        if (m_Level == 1)
        {
            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;

            Vector3 a_CacDir = a_CacVLen.normalized;        //방향 벡터

            //GameObject BulletClone = Instantiate(GameMgr.m_BulletPrefab);

            //BulletCtrl a_BulletSc = BulletClone.GetComponent<BulletCtrl>();
            BulletCtrl a_BulletSc = BulletPool_Mgr.Inst.GetBulletPool();
            a_BulletSc.gameObject.tag = AllyType.BT_Enemy.ToString();   //"BT_Enemy";
            a_BulletSc.BulletSpawn(transform.position, a_CacDir, 30.0f);
        }
        else if (m_Level == 2)
        {
            //랜덤 각도로 발사하기
            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;

            Quaternion a_CacRot = Quaternion.LookRotation(a_CacVLen.normalized);
            float a_CacRan = Random.Range(-15.0f, 15.0f);
            a_CacRot.eulerAngles = new Vector3(a_CacRot.eulerAngles.x,
                                               a_CacRot.eulerAngles.y + a_CacRan,
                                               a_CacRot.eulerAngles.z);
            Vector3 a_DirVec = a_CacRot * Vector3.forward;
            a_DirVec.Normalize();

            //GameObject a_CloneObj = Instantiate(GameMgr.m_BulletPrefab);
            //a_CloneObj.tag = AllyType.BT_Enemy.ToString();   //"BT_Enemy";
            //BulletCtrl a_BL_Sc = a_CloneObj.GetComponent<BulletCtrl>();
            BulletCtrl a_BL_Sc = BulletPool_Mgr.Inst.GetBulletPool();
            a_BL_Sc.gameObject.tag = AllyType.BT_Enemy.ToString();   //"BT_Enemy";
            a_BL_Sc.BulletSpawn(transform.position, a_DirVec, 30.0f);
        }
        else if (m_Level == 3)
        {
            //360도 공격
            //Vector3 a_TargetV = Vector3.zero;
            //GameObject a_NewObj = null;
            //BulletCtrl a_Bl_Sc = null;
            //for (float Angle = 0.0f; Angle < 360.0f; Angle += 15.0f)
            //{
            //    a_TargetV.x = Mathf.Cos(Angle * Mathf.Deg2Rad);
            //    a_TargetV.y = 0.0f;
            //    a_TargetV.z = Mathf.Sin(Angle * Mathf.Deg2Rad);
            //    a_TargetV.Normalize();

            //    //a_NewObj = Instantiate(GameMgr.m_BulletPrefab);
            //    //a_Bl_Sc = a_NewObj.GetComponent<BulletCtrl>();
            //    //a_NewObj.tag = AllyType.BT_Enemy.ToString();
            //    a_Bl_Sc = BulletPool_Mgr.Inst.GetBulletPool();
            //    a_Bl_Sc.gameObject.tag = AllyType.BT_Enemy.ToString();
            //    a_Bl_Sc.BulletSpawn(transform.position, a_TargetV, 30.0f);
            //}
            //return;

            //타겟을 중심으로 한 부채꼴 공격
            //a_CacVLen = m_AggroTarget.transform.position - transform.position;
            //a_CacVLen.y = 0.0f;
            //Quaternion a_CacRot = Quaternion.identity;
            //Vector3 a_DirVec = Vector3.forward;     //시계를 생각했을 때 12시 방향이므로 eulerangle = 0.0f
            //GameObject a_CloneObj = null;
            //BulletCtrl a_BL_Sc = null;
            //for (float Angle = -30.0f; Angle <= 30.0f; Angle += 15.0f)
            //{
            //    a_CacRot = Quaternion.LookRotation(a_CacVLen.normalized);   //대상을 향하는 방향값을 가져옴
            //    a_CacRot.eulerAngles = new Vector3(a_CacRot.eulerAngles.x,
            //                                       a_CacRot.eulerAngles.y + Angle,
            //                                       a_CacRot.eulerAngles.z); //그 방향값을 중심으로 +- 값을 더해줌
            //    a_DirVec = a_CacRot * Vector3.forward;  //0시를 기준으로 해당 방향의 각도값을 벡터로

            //    //a_CloneObj = Instantiate(GameMgr.m_BulletPrefab);
            //    //a_CloneObj.tag = AllyType.BT_Enemy.ToString();   //"BT_Enemy";
            //    //a_BL_Sc = a_CloneObj.GetComponent<BulletCtrl>();
            //    a_BL_Sc = BulletPool_Mgr.Inst.GetBulletPool();
            //    a_BL_Sc.gameObject.tag = AllyType.BT_Enemy.ToString();
            //    a_BL_Sc.BulletSpawn(transform.position, a_DirVec, 30.0f);
            //}
            //return;

            //회오리 발사
            Vector3 a_TargetV = Vector3.zero;
            a_TargetV.x = Mathf.Sin(m_ShootAngle * Mathf.Deg2Rad);
            a_TargetV.y = 0.0f;
            a_TargetV.z = Mathf.Cos(m_ShootAngle * Mathf.Deg2Rad);
            a_TargetV.Normalize();

            //GameObject a_CloneObj = Instantiate(GameMgr.m_BulletPrefab);
            //a_CloneObj.tag = AllyType.BT_Enemy.ToString();   //"BT_Enemy";
            //BulletCtrl a_BL_Sc = a_CloneObj.GetComponent<BulletCtrl>();
            BulletCtrl a_BL_Sc = BulletPool_Mgr.Inst.GetBulletPool();
            a_BL_Sc.gameObject.tag = AllyType.BT_Enemy.ToString();
            a_BL_Sc.BulletSpawn(transform.position, a_TargetV, 30.0f);

            m_ShootAngle += 15.0f;
        }        
    }

    public void ItemDrop()
    {
        int a_Rnd = Random.Range(0, 6);

        if (a_Rnd == 1)     //드롭되는 아이템 중 1번인 폭탄이 드랍되지 않도록
            a_Rnd = 0;

        GameObject a_Item = null;
        a_Item = (GameObject)Instantiate(Resources.Load("Item_Obj"));
        a_Item.transform.position = new Vector3(transform.position.x, 0.7f, transform.position.z);

        if (a_Rnd == 0)
        {
            a_Item.name = "coin_Item_Obj";
        }
        else if (a_Rnd == 1)
        {
            a_Item.name = "bomb_Item_Obj";
        }
        else
        {
            Item_Type a_ItType = (Item_Type)a_Rnd;
            a_Item.name = a_ItType.ToString() + "_Item_Obj";
        }

        ItemObjInfo a_RefItemInfo = a_Item.GetComponent<ItemObjInfo>();
        if(a_RefItemInfo != null)
        {
            a_RefItemInfo.InitItem((Item_Type)a_Rnd, a_Item.name, Random.Range(1, 6), Random.Range(1, 6));
        }
    }

    public void SetSpawnInfo(int Idx, int a_Level, float a_MaxHp, float a_AttSpeed, float a_MvSpeed)
    {
        m_SpawnIdx = Idx;
        m_Level = a_Level;
        m_MaxHp = a_MaxHp;
        m_CurHp = a_MaxHp;
        m_AttackSpeed = a_AttSpeed;
        m_MoveVelocity = a_MvSpeed;

        if (m_Level == 2)
            m_AttackSpeed = 0.1f;
        //회오리 발사 시에
        if (m_Level == 3)
            m_AttackSpeed = 0.1f;
            
        //if (m_Level == 2) //360도 발사시에
        //    m_AttackSpeed = 1.3f;

            //몬스터 이미지 교체
            if (Monster_Mgr.Inst == null) return;

        int ImgIdx = m_Level - 1;
        ImgIdx = Mathf.Clamp(ImgIdx, 0, 2);     //범위 외의 숫자가 나올 경우 해당 범위 내의 숫자로 만들어버림
        Texture a_RefMonImg = Monster_Mgr.Inst.m_MonImg[ImgIdx];

        MeshRenderer a_MeshList = gameObject.GetComponentInChildren<MeshRenderer>();
        if (a_MeshList != null)
            a_MeshList.material.SetTexture("_MainTex", a_RefMonImg);

    }
}
