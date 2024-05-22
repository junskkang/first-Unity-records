using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonAIState
{
    MAI_Idle,           //������ ����
    MAI_Patrol,         //��Ʈ�� ����
    MAI_AggroTrace,     //�����κ��� ������ ������ �� ���� ����
    MAI_NormalTrace,    //�Ϲ� ���� ����
    MAI_ReturnPos,      //������ ������ �� �� ��ġ�� ���ƿ��� ����
    MAI_Attack          //���� ����
}

public class MonsterCtrl : MonoBehaviour
{
    float m_MaxHp = 100.0f;
    float m_CurHp = 100.0f;
    public Image HpBarUI = null;

    //���� AI ������
    MonAIState m_AIState = MonAIState.MAI_Patrol;   //���� ����

    GameObject m_AggroTarget = null;    //�����ؾ��� Ÿ�� (ĳ����)

    float m_AttackDist = 15.0f;         //���ݰŸ�
    float m_TraceDist = 20.0f;          //�����Ÿ�

    float m_MoveVelocity = 2.0f;        //��� �ʴ� �̵� �ӵ�(��Ʈ�� ����)

    Vector3 m_MoveDir = Vector3.zero;   //��� ���� ����
    float m_NowStep = 0.0f;             //�̵� ���� ����

    float m_ShootCool = 1.0f;           //�����ֱ� ����
    float m_AttackSpeed = 0.5f;         //���ݼӵ�

    //���� ������
    Vector3 a_CacVLen = Vector3.zero;
    float a_CacDist = 0.0f;

    //��Ʈ�� ���� ������
    Vector3 m_BasePos = Vector3.zero;   //�ʱ� ���� ��ġ (������)
    bool m_bMvPtOnOff = false;          //Patrol Move OnOff

    float m_WaitTime = 0.0f;        //��Ʈ�� �� ��ǥ���� �����ϸ� ��� ���
    int a_AngleRan;
    int a_LengthRan;

    Vector3 m_PatrolTarget = Vector3.zero;  //Patrol�� �������� �� ���� ��ǥ ��ǥ
    Vector3 m_DirMvVec = Vector3.zero;      //Patrol�� �������� �� ���� ����
    double m_AddTimeCount = 0.0f;           //�̵� �� �����ð� ī��Ʈ�� ����
    double m_MoveDurTime = 0.0f;            //��ǥ������ �����ϴµ� �ɸ��� �ð� ����
    Quaternion a_CacPtRot;
    Vector3 a_CacPtAngle = Vector3.zero;
    Vector3 a_Vert;

    [HideInInspector] public int m_SpawnIdx = -1;   //List<SpawnPos> m_SpawnPosList�� �ε���
    int m_Level = 1;        //���� ����




    // Start is called before the first frame update
    void Start()
    {
        m_BasePos = transform.position;     //������ ù ���� ��ġ ����
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
                return;         //���Ͱ� �� �Ѿ��� ���Ͱ� �¾��� �� ����. ��ų x

            BulletCtrl a_Bl_Ctrl = coll.gameObject.GetComponent<BulletCtrl>();
            TakeDamage(a_Bl_Ctrl.m_Damage);

            if (a_Bl_Ctrl.m_IsPool == true)
                coll.gameObject.SetActive(false);
            else
                Destroy(coll.gameObject);   //�ε��� �Ѿ� ����
        }

    }

    public void TakeDamage(float a_Value)
    {
        if (m_CurHp <= 0.0f)    //��Ʈ���̰� �������� �ʰ� �Ǳ� ������ Ȥ�ó�
            return;             //������ �ι� ������ �Ǵ� ��� ����

        GameMgr.Inst.DamageText((int)a_Value, this.transform.position); //������ ��Ʈ �Լ� ȣ��

        m_CurHp -= a_Value;

        if (m_CurHp < 0.0f)
            m_CurHp = 0.0f;

        if(HpBarUI != null)
            HpBarUI.fillAmount = m_CurHp/m_MaxHp;   //���� ü�¹�UI ǥ��

        m_AggroTarget = GameMgr.Inst.m_RefHero.gameObject;
        m_AIState = MonAIState.MAI_AggroTrace;

        if (m_CurHp <= 0.0f) //���� ���ó��
        {
            //����
            ItemDrop();
            GameMgr.Inst.monKillCount++;

            //4~6�� �� ���� �ڸ����� �ٽ� ���� ��û
            if (m_Level < 3 && Monster_Mgr.Inst != null)        //3���� ���ʹ� ����� ������x ��ȹ�ǵ�
                Monster_Mgr.Inst.ResetSpawn(m_SpawnIdx);

            Destroy(gameObject);    //���� ����
        }
    }

    void MonsterAI()
    {
        if (0.0f < m_ShootCool)
            m_ShootCool -= Time.deltaTime;
        
        if (m_AIState == MonAIState.MAI_Patrol)     //����Ÿ��� ����
        {
            if (GameMgr.Inst.m_RefHero != null)
            {
                a_CacVLen = GameMgr.Inst.m_RefHero.transform.position -
                    this.gameObject.transform.position;
                a_CacVLen.y = 0.0f;
                a_CacDist = a_CacVLen.magnitude; 

                if (a_CacDist < m_TraceDist)        //�����Ÿ����� ĳ���Ϳ� ������ �����Ÿ��� ���������
                {
                    m_AIState = MonAIState.MAI_NormalTrace;
                    m_AggroTarget = GameMgr.Inst.m_RefHero.gameObject;
                    return;
                }
            }

            AI_Patrol();
        }
        else if (m_AIState == MonAIState.MAI_NormalTrace)       //�������� �� ��
        {
            if (m_AggroTarget == null)
            {
                m_AIState = MonAIState.MAI_Patrol;
                return;
            }

            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;
            if (a_CacDist < m_AttackDist)       //���ݰŸ����� ĳ���Ϳ��� �Ÿ��� ���������
            {
                m_AIState = MonAIState.MAI_Attack;
            }
            else if (a_CacDist < m_TraceDist)
            {
                m_MoveDir = a_CacVLen.normalized;
                m_MoveDir.y = 0.0f;
                m_NowStep = m_MoveVelocity * 1.5f * Time.deltaTime;     //�Ѱ��� ũ��
                //�Ϲ� ��Ʈ�� ������ �̵��ӵ� ���� 1.5�� ������
                transform.Translate(m_MoveDir*m_NowStep, Space.World);
            }
            else
            {
                m_AIState = MonAIState.MAI_Patrol;
            }
        }
        else if (m_AIState == MonAIState.MAI_AggroTrace)        //��׷� ���� ����
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
        else if (m_AIState == MonAIState.MAI_Attack)            //���� ����
        {
            if (m_AggroTarget == null)
            {
                m_AIState = MonAIState.MAI_Patrol;
                return;
            }

            a_CacVLen = m_AggroTarget.transform.position - transform.position;
            a_CacVLen.y = 0.0f;

            a_CacDist = a_CacVLen.magnitude;
            if ((m_AttackDist - 2.0f) < a_CacDist)      //������ ���� ���� �̵��ؾ� �ϴ� ��Ȳ�̸�
            {
                m_MoveDir = a_CacVLen.normalized;
                m_MoveDir.y = 0.0f;
                m_NowStep = m_MoveVelocity * 1.5f * Time.deltaTime;
                transform.Translate(m_MoveDir*m_NowStep, Space.World);
            }

            if (a_CacDist < m_AttackDist)       //���� ������ �Ÿ�
            {
                //����
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
            m_DirMvVec = m_PatrolTarget - transform.position;   //��ǥ���������� ����
            m_DirMvVec.y = 0.0f;
            m_DirMvVec.Normalize();

            m_AddTimeCount += Time.deltaTime;
            if (m_MoveDurTime <= m_AddTimeCount)    //��ǥ���� ������ ������ ����
                m_bMvPtOnOff = false;
            else
                transform.Translate(m_DirMvVec*Time.deltaTime*m_MoveVelocity, Space.World);
        }
        else
        {
            m_WaitTime -= Time.deltaTime;       //���� �� ��� �ӹ����ٰ� �ٽ� �̵�
            if (0.0f < m_WaitTime)
                return;

            m_WaitTime = 0.0f;
            a_AngleRan = Random.Range(30, 301);     //���� ȸ����
            a_LengthRan = Random.Range(3, 8);       //���� �̵� �ݰ� ��

            m_DirMvVec = transform.position - m_BasePos;    //�⺻��ġ���� ������ġ�� ���ϴ� ����
            m_DirMvVec.y = 0.0f;

            if (m_DirMvVec.magnitude < 1.0f)        //ó�� ������ �� �ۿ� ����
                a_CacPtRot = Quaternion.LookRotation(transform.forward);    
            else
                a_CacPtRot = Quaternion.LookRotation(m_DirMvVec);           

            a_CacPtAngle = a_CacPtRot.eulerAngles;      //eulerAnlge ������
            a_CacPtAngle.y = a_CacPtAngle.y + (float)a_AngleRan;        //y�� ȸ���� ���� �������� ������
            a_CacPtRot.eulerAngles = a_CacPtAngle;      //�ٽ� ���Ϸ� �ޱۿ� ����־� ȸ����Ŵ
            a_Vert = new Vector3(0, 0, 1);
            a_Vert = a_CacPtRot * a_Vert;   //���ʹϿ¿� ���͸� ���ϸ� ���Ͱ� ���ʹϿ� ������ ȸ���� ��
            a_Vert.Normalize();             //��������ȭ

            m_PatrolTarget = m_BasePos + (a_Vert * (float)a_LengthRan); //ȸ���� �������� ������ �ݰ��� ��ǥ

            m_DirMvVec = m_PatrolTarget - transform.position;
            m_DirMvVec.y = 0.0f;
            m_MoveDurTime = m_DirMvVec.magnitude/m_MoveVelocity;    //�����ϴµ� �ɸ��� �ð�
            // �ӵ� = �Ÿ�/�ð�, �Ÿ� = �ӵ� * �ð�, �ð� = �Ÿ�/�ӵ�
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

            Vector3 a_CacDir = a_CacVLen.normalized;        //���� ����

            //GameObject BulletClone = Instantiate(GameMgr.m_BulletPrefab);

            //BulletCtrl a_BulletSc = BulletClone.GetComponent<BulletCtrl>();
            BulletCtrl a_BulletSc = BulletPool_Mgr.Inst.GetBulletPool();
            a_BulletSc.gameObject.tag = AllyType.BT_Enemy.ToString();   //"BT_Enemy";
            a_BulletSc.BulletSpawn(transform.position, a_CacDir, 30.0f);
        }
        else if (m_Level == 2)
        {
            //���� ������ �߻��ϱ�
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
            //360�� ����
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

            //Ÿ���� �߽����� �� ��ä�� ����
            //a_CacVLen = m_AggroTarget.transform.position - transform.position;
            //a_CacVLen.y = 0.0f;
            //Quaternion a_CacRot = Quaternion.identity;
            //Vector3 a_DirVec = Vector3.forward;     //�ð踦 �������� �� 12�� �����̹Ƿ� eulerangle = 0.0f
            //GameObject a_CloneObj = null;
            //BulletCtrl a_BL_Sc = null;
            //for (float Angle = -30.0f; Angle <= 30.0f; Angle += 15.0f)
            //{
            //    a_CacRot = Quaternion.LookRotation(a_CacVLen.normalized);   //����� ���ϴ� ���Ⱚ�� ������
            //    a_CacRot.eulerAngles = new Vector3(a_CacRot.eulerAngles.x,
            //                                       a_CacRot.eulerAngles.y + Angle,
            //                                       a_CacRot.eulerAngles.z); //�� ���Ⱚ�� �߽����� +- ���� ������
            //    a_DirVec = a_CacRot * Vector3.forward;  //0�ø� �������� �ش� ������ �������� ���ͷ�

            //    //a_CloneObj = Instantiate(GameMgr.m_BulletPrefab);
            //    //a_CloneObj.tag = AllyType.BT_Enemy.ToString();   //"BT_Enemy";
            //    //a_BL_Sc = a_CloneObj.GetComponent<BulletCtrl>();
            //    a_BL_Sc = BulletPool_Mgr.Inst.GetBulletPool();
            //    a_BL_Sc.gameObject.tag = AllyType.BT_Enemy.ToString();
            //    a_BL_Sc.BulletSpawn(transform.position, a_DirVec, 30.0f);
            //}
            //return;

            //ȸ���� �߻�
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

        if (a_Rnd == 1)     //��ӵǴ� ������ �� 1���� ��ź�� ������� �ʵ���
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
        //ȸ���� �߻� �ÿ�
        if (m_Level == 3)
            m_AttackSpeed = 0.1f;
            
        //if (m_Level == 2) //360�� �߻�ÿ�
        //    m_AttackSpeed = 1.3f;

            //���� �̹��� ��ü
            if (Monster_Mgr.Inst == null) return;

        int ImgIdx = m_Level - 1;
        ImgIdx = Mathf.Clamp(ImgIdx, 0, 2);     //���� ���� ���ڰ� ���� ��� �ش� ���� ���� ���ڷ� ��������
        Texture a_RefMonImg = Monster_Mgr.Inst.m_MonImg[ImgIdx];

        MeshRenderer a_MeshList = gameObject.GetComponentInChildren<MeshRenderer>();
        if (a_MeshList != null)
            a_MeshList.material.SetTexture("_MainTex", a_RefMonImg);

    }
}
