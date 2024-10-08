using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonAIState
{
    MAI_Idle,        //숨쉬기 상태
    MAI_Patrol,      //패트롤 상태
    MAI_AggroTrace,  //적으로부터 공격을 당했을 때 추적 상태
    MAI_NormalTrace, //일반 추적 상태
    MAI_ReturnPos,   //추적을 놓쳤을 때 재자리로 돌아오는 상태
    MAI_Attack       //공격 상태
}

public class MonsterCtrl : MonoBehaviour
{
    float m_MaxHp = 100.0f;
    float m_CurHp = 100.0f;
    public Image HpBarUI = null;

    //--- 몬스터 AI 변수들...
    MonAIState m_AIState = MonAIState.MAI_Patrol;   //상태변수

    GameObject m_AggroTarget = null;    //추적해야 할 타겟 캐릭터(주인공)

    float m_AttackDist = 12.0f; //19.5f;         //공격거리
    float m_TraceDist  = 20.0f;         //추적거리

    float m_MoveVelocity = 2.0f;        //평면 초당 이동 속도...(패트롤 이동 기준)

    Vector3 m_MoveDir = Vector3.zero;   //평면 진행 방향
    float m_NowStep = 0.0f;             //이동 계산용 변수
    //--- 몬스터 AI 변수들...

    //--- 공격 주기 변수
    float m_ShootCool = 1.0f;           //주기 계산용 변수
    float m_AttackSpeed = 0.5f;         //공격 속도(공속)

    //--- 계산용 변수
    Vector3 a_CacVLen = Vector3.zero;
    float   a_CacDist = 0.0f;
    //--- 계산용 변수

    //--- 패트롤에 필요한 변수
    Vector3 m_BasePos = Vector3.zero;   //몬스터의 초기 스폰 위치(기준점이 된다.)
    bool m_bMvPtOnOff = false;          //Patrol MoveOnOff

    float m_WaitTime = 0.0f;    //Patrol시에 목표점에 도착하면 잠시 대기시키기 위한 랜덤 시간 변수
    int a_AngleRan;
    int a_LengthRan;

    Vector3 m_PatrolTarget = Vector3.zero;  //Patrol 시 움직여야 될 다음 목표 좌표
    Vector3 m_DirMvVec = Vector3.zero;      //Patrol 시 움직여야 될 방향 벡터
    double m_AddTimeCount = 0.0f;           //이동 총 누적시간 카운트용 변수
    double m_MoveDurTime = 0.0f;            //목표점까지 도착하는데 걸리는 시간
    Quaternion a_CacPtRot;
    Vector3 a_CacPtAngle = Vector3.zero;
    Vector3 a_Vert;
    //--- 패트롤에 필요한 변수

    [HideInInspector] public int m_SpawnIdx = -1;  //List<SpawnPos> m_SpawnPosList;의 인덱스
    int m_Level = 1;   //몬스터 레벨

    // Start is called before the first frame update
    void Start()
    {
        m_BasePos = transform.position;
        m_WaitTime = Random.Range(0.5f, 3.0f);
        m_bMvPtOnOff = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MonsterAI();
    }

    void OnTriggerEnter(Collider Coll)
    {
        if(Coll.gameObject.name.Contains("BulletPrefab") == true)
        {
            //if(Coll.gameObject.tag == "BT_Enemy")
            if (Coll.gameObject.CompareTag(AllyType.BT_Enemy.ToString()) == true)
                return;  //몬스터가 쏜 총알이면 제외

            BulletCtrl a_BL_Ctrl = Coll.gameObject.GetComponent<BulletCtrl>();
            TakeDamage(a_BL_Ctrl.m_Damage);

            if (a_BL_Ctrl.m_IsPool == false)
                Destroy(Coll.gameObject);
            else
                Coll.gameObject.SetActive(false);
        }
    }// void OnTriggerEnter(Collider Coll)

    public void TakeDamage(float a_Value)
    {
        if (m_CurHp <= 0.0f)
            return;

        Game_Mgr.Inst.DamageText((int)a_Value, this.transform.position);

        m_CurHp -= a_Value;
        if (m_CurHp < 0.0f)
            m_CurHp = 0.0f;

        if (HpBarUI != null)
            HpBarUI.fillAmount = m_CurHp / m_MaxHp;

        m_AggroTarget = Game_Mgr.Inst.m_RefHero.gameObject;
        m_AIState = MonAIState.MAI_AggroTrace;

        if(m_CurHp <= 0.0f)  //몬스터 사망 처리
        {
            Game_Mgr.Inst.AddMonKill();     //몬스터 Kill Count + 1

            // 보상
            ItemDrop();

            if (m_Level < 3 && Monster_Mgr.Inst != null)
                Monster_Mgr.Inst.ReSetSpawn(m_SpawnIdx);
            // 4 ~ 6초 뒤 같은 자리에서 다시 스폰 요청

            Destroy(gameObject);
        }
    }//public void TakeDamage(float a_Value)

    void MonsterAI()
    {
        if (0.0f < m_ShootCool)
            m_ShootCool -= Time.deltaTime;

        if(m_AIState == MonAIState.MAI_Patrol) //어슬렁 거리는 상태
        {
            if(Game_Mgr.Inst.m_RefHero != null)
            {
                a_CacVLen = Game_Mgr.Inst.m_RefHero.transform.position
                                            - transform.position;
                a_CacVLen.y = 0.0f;
                a_CacDist = a_CacVLen.magnitude;

                if(a_CacDist < m_TraceDist)  //추적거리
                {
                    m_AIState = MonAIState.MAI_NormalTrace;
                    m_AggroTarget = Game_Mgr.Inst.m_RefHero.gameObject;
                    return;
                }
            }//if(Game_Mgr.Inst.m_RefHero != null)

            AI_Patrol();
        }//if(m_AIState == MonAIState.MAI_Patrol) //어슬렁 거리는 상태
        else if(m_AIState == MonAIState.MAI_NormalTrace) //추적상태
        {
            if(m_AggroTarget == null)
            {
                m_AIState = MonAIState.MAI_Patrol;
                return;
            }

            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;
            if(a_CacDist < m_AttackDist)  //공격거리
            {
                m_AIState = MonAIState.MAI_Attack;
            }
            else if(a_CacDist < m_TraceDist)  //추적거리
            {
                m_MoveDir = a_CacVLen.normalized;
                m_MoveDir.y = 0.0f;
                m_NowStep = m_MoveVelocity * 1.5f * Time.deltaTime; //한걸음 크기
                //일반 패트롤 상태의 이동속도보다 1.5배 빠르게 이동
                transform.Translate(m_MoveDir * m_NowStep, Space.World);
            }
            else
            {
                m_AIState = MonAIState.MAI_Patrol;
            }

        }//else if(m_AIState == MonAIState.MAI_NormalTrace) //추적상태
        else if(m_AIState == MonAIState.MAI_AggroTrace)
        {  //어그로 추적상태 (어그로 상대를 향해 돌진)

            if(m_AggroTarget == null)
            {
                m_AIState = MonAIState.MAI_Patrol;
                return;
            }

            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;

            if(a_CacDist < m_AttackDist)  //공격거리
            {
                m_AIState = MonAIState.MAI_Attack;
            }

            if((m_AttackDist - 2.0f) < a_CacDist) //공격거리 2m 안쪽까지 바짝 쫓아오게...
            {
                m_MoveDir = a_CacVLen.normalized;
                m_MoveDir.y = 0.0f;
                //m_NowStep = m_MoveVelocity * 20.0f * Time.deltaTime;
                m_NowStep = m_MoveVelocity * 15.0f * Time.deltaTime;
                transform.Translate(m_MoveDir * m_NowStep, Space.World);
            }
        }
        else if(m_AIState == MonAIState.MAI_Attack)  //공격상태
        {
            if(m_AggroTarget == null)
            {
                m_AIState = MonAIState.MAI_Patrol;
                return;
            }

            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;

            if((m_AttackDist - 2.0f) < a_CacDist)
            {  //공격을 위해 아직 이동해야 하는 상황이면...
                m_MoveDir = a_CacVLen.normalized;
                m_MoveDir.y = 0.0f;
                m_NowStep = m_MoveVelocity * 1.5f * Time.deltaTime; //한걸음 크기
                transform.Translate(m_MoveDir * m_NowStep, Space.World);
            }//if((m_AttackDist - 2.0f) < a_CacDist)

            if(a_CacDist < m_AttackDist)  //공격거리
            {
                if(m_ShootCool <= 0.0f)
                {
                    ShootFire();        //공격
                    m_ShootCool = m_AttackSpeed;
                }
            }
            else
            {
                m_AIState = MonAIState.MAI_NormalTrace;
            }

        }//else if(m_AIState == MonAIState.MAI_Attack)  //공격상태

    }//void MonsterAI()

    void AI_Patrol()
    {
        if(m_bMvPtOnOff == true)
        {
            m_DirMvVec = m_PatrolTarget - transform.position;
            m_DirMvVec.y = 0.0f;
            m_DirMvVec.Normalize();

            m_AddTimeCount += Time.deltaTime;
            if (m_MoveDurTime <= m_AddTimeCount)  //목표점에 도착한 것으로 판정한다.
                m_bMvPtOnOff = false;
            else
                transform.Translate((m_DirMvVec * Time.deltaTime * m_MoveVelocity), Space.World);
        }//if(m_bMvPtOnOff == true)
        else
        {
            m_WaitTime -= Time.deltaTime;
            if (0.0f < m_WaitTime)
                return;

            m_WaitTime = 0.0f;
            a_AngleRan = Random.Range(30, 301);
            a_LengthRan = Random.Range(3, 8);

            m_DirMvVec = transform.position - m_BasePos;
            m_DirMvVec.y = 0.0f;

            if (m_DirMvVec.magnitude < 1.0f)
                a_CacPtRot = Quaternion.LookRotation(transform.forward);
            else
                a_CacPtRot = Quaternion.LookRotation(m_DirMvVec);

            a_CacPtAngle = a_CacPtRot.eulerAngles;
            a_CacPtAngle.y = a_CacPtAngle.y + (float)a_AngleRan;
            a_CacPtRot.eulerAngles = a_CacPtAngle;
            a_Vert = new Vector3(0, 0, 1);
            a_Vert = a_CacPtRot * a_Vert;
            a_Vert.Normalize();

            m_PatrolTarget = m_BasePos + (a_Vert * (float)a_LengthRan);

            m_DirMvVec = m_PatrolTarget - transform.position;
            m_DirMvVec.y = 0.0f;
            m_MoveDurTime = m_DirMvVec.magnitude / m_MoveVelocity; //도착하는데 걸리는 시간
            // 속도  = 거리 / 시간     속도 * 시간 = 거리     시간 = 거리 / 속도
            m_AddTimeCount = 0.0f;
            m_DirMvVec.Normalize();

            m_WaitTime = Random.Range(0.2f, 3.0f);
            m_bMvPtOnOff = true;

        }//else
    }

    ////--- 회오리 발사
    //float m_ShootAngle = 0.0f;
    ////--- 회오리 발사

    void ShootFire()
    {
        if (m_AggroTarget == null)
            return;

        if (m_Level == 1)
        {
            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;
            Vector3 a_CacDir = a_CacVLen.normalized;

            //GameObject a_BLClone = Instantiate(Game_Mgr.m_BulletPrefab);
            //BulletCtrl a_BulletSc = a_BLClone.GetComponent<BulletCtrl>();
            BulletCtrl a_BulletSc = BulletPool_Mgr.Inst.GetBulletPool();
            a_BulletSc.gameObject.tag = AllyType.BT_Enemy.ToString(); //"BT_Enemy";  //테그 변경
            a_BulletSc.BulletSpawn(transform.position, a_CacVLen, 30.0f);
        }    
        else if (m_Level == 2)  //--- 레벨2 몬스터일 경우 360도 발사하기...
        {
            //    //--- 360도 발사
            //    Vector3 a_TargetV = Vector3.zero;
            //    GameObject a_NewObj = null;
            //    BulletCtrl a_BL_Sc = null;
            //    for(float Angle = 0.0f; Angle < 360.0f; Angle += 15.0f)
            //    {
            //        a_TargetV.x = Mathf.Cos(Angle * Mathf.Deg2Rad);
            //        a_TargetV.y = 0.0f;
            //        a_TargetV.z = Mathf.Sin(Angle * Mathf.Deg2Rad);
            //        a_TargetV.Normalize();
            //
            //        a_NewObj = Instantiate(Game_Mgr.m_BulletPrefab);
            //        a_BL_Sc = a_NewObj.GetComponent<BulletCtrl>();
            //        a_NewObj.tag = AllyType.BT_Enemy.ToString();
            //        a_BL_Sc.BulletSpawn(transform.position, a_TargetV, 30.0f);
            //    }
            //
            //    //--- 360도 발사

            //--- 부채꼴 모양으로 발사하기
            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;
            Quaternion a_CacRot = Quaternion.identity;
            Vector3 a_DirVec = Vector3.forward;
            //GameObject a_CloneObj = null;
            BulletCtrl a_BL_Sc = null;
            for (float a_Ang = -30.0f; a_Ang <= 30.0f; a_Ang += 15.0f)
            {
                a_CacRot = Quaternion.LookRotation(a_CacVLen.normalized);
                a_CacRot.eulerAngles = new Vector3(a_CacRot.eulerAngles.x,
                                                   a_CacRot.eulerAngles.y + a_Ang,
                                                   a_CacRot.eulerAngles.z);
                a_DirVec = a_CacRot * Vector3.forward;
                a_DirVec.Normalize();

                //a_CloneObj = Instantiate(Game_Mgr.m_BulletPrefab);
                //a_BL_Sc = a_CloneObj.GetComponent<BulletCtrl>();
                //a_CloneObj.tag = AllyType.BT_Enemy.ToString();
                a_BL_Sc = BulletPool_Mgr.Inst.GetBulletPool();
                a_BL_Sc.gameObject.tag = AllyType.BT_Enemy.ToString();
                a_BL_Sc.BulletSpawn(transform.position, a_DirVec, 30.0f);
            }
            //--- 부채꼴 모양으로 발사하기

            ////--- 회오리 발사 (시계방향 발사)
            //Vector3 a_TargetV = Vector3.zero;
            //a_TargetV.x = Mathf.Sin(m_ShootAngle * Mathf.Deg2Rad);
            //a_TargetV.y = 0.0f;
            //a_TargetV.z = Mathf.Cos(m_ShootAngle * Mathf.Deg2Rad);
            //a_TargetV.Normalize();

            //GameObject a_CloneObj = Instantiate(Game_Mgr.m_BulletPrefab);
            //BulletCtrl a_BL_Sc = a_CloneObj.GetComponent<BulletCtrl>();
            //a_CloneObj.tag = AllyType.BT_Enemy.ToString();
            //a_BL_Sc.BulletSpawn(transform.position, a_TargetV, 30.0f);

            //m_ShootAngle += 15.0f;
            ////--- 회오리 발사 (시계방향 발사)

        }//if(m_Level == 2)        
        else if (m_Level == 3)  //--- 레벨 3 몬스터일 경우 렌덤한 각도로 발사하기...
        {
            //--- 랜덤 각도로 발사하기
            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;

            Quaternion a_CacRot = Quaternion.LookRotation(a_CacVLen.normalized);
            float a_CacRan = Random.Range(-15.0f, 15.0f);
            a_CacRot.eulerAngles = new Vector3(a_CacRot.eulerAngles.x,
                                                a_CacRot.eulerAngles.y + a_CacRan,
                                                a_CacRot.eulerAngles.z);
            Vector3 a_DirVec = a_CacRot * Vector3.forward;
            a_DirVec.Normalize();

            //GameObject a_CloneObj = Instantiate(Game_Mgr.m_BulletPrefab);
            //BulletCtrl a_BL_Sc = a_CloneObj.GetComponent<BulletCtrl>();
            //a_CloneObj.tag = AllyType.BT_Enemy.ToString();
            BulletCtrl a_BulletSc = BulletPool_Mgr.Inst.GetBulletPool();
            a_BulletSc.gameObject.tag = AllyType.BT_Enemy.ToString();
            a_BulletSc.BulletSpawn(transform.position, a_DirVec, 30.0f);
            //--- 랜덤 각도로 발사하기
        }
        //--- 레벨 3 몬스터일 경우 렌덤한 각도로 발사하기...
    
    }//void ShootFire()

    public void ItemDrop()
    {
        int a_Rnd = Random.Range(0, 6);

        if (a_Rnd == 1)
            a_Rnd = 0;

        GameObject a_Item = null;
        a_Item = (GameObject)Instantiate(Resources.Load("Item_Obj"));
        a_Item.transform.position = new Vector3(transform.position.x, 0.7f, 
                                                transform.position.z);
        if(a_Rnd == 0)
        {
            a_Item.name = "coin_Item_Obj";
        }
        else if(a_Rnd == 1)
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
            a_RefItemInfo.InitItem((Item_Type)a_Rnd, a_Item.name,
                                    Random.Range(1, 6), Random.Range(1, 6) );
        }
    }//public void ItemDrop()


    public void SetSpawnInfo(int Idx, int a_Lv, float a_MaxHp, float a_AttSpeed, float a_MvSpeed)
    {
        m_SpawnIdx = Idx;
        m_Level = a_Lv;
        m_MaxHp = a_MaxHp;
        m_CurHp = a_MaxHp;
        m_AttackSpeed = a_AttSpeed;
        m_MoveVelocity = a_MvSpeed;

        ////--- 회오리 발사
        //if (m_Level == 2)
        //    m_AttackSpeed = 0.1f;
        ////--- 회오리 발사

        //---- 360도 발사, 부채꼴 발사
        if (m_Level == 2)
            m_AttackSpeed = 0.8f;
        //---- 360도 발사, 부채꼴 발사

        //--- 이미지 교체 코드
        if (Monster_Mgr.Inst == null)
            return;

        int ImgIdx = m_Level - 1;
        ImgIdx = Mathf.Clamp(ImgIdx, 0, 2);
        Texture a_RefMonImg = Monster_Mgr.Inst.m_MonImg[ImgIdx];

        MeshRenderer a_MeshList = gameObject.GetComponentInChildren<MeshRenderer>();
        if (a_MeshList != null)
            a_MeshList.material.SetTexture("_MainTex", a_RefMonImg);
        //--- 이미지 교체 코드
    }

}
