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




    // Start is called before the first frame update
    void Start()
    {
        m_BasePos = transform.position;     //������ ù ���� ��ġ ����
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
                return;         //���Ͱ� �� �Ѿ��� ���Ͱ� �¾��� �� ����. ��ų x

            TakeDamage(10.0f);

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

    void ShootFire()
    {
        if (m_AggroTarget == null)
            return;

        a_CacVLen = m_AggroTarget.transform.position - transform.position;
        a_CacVLen.y = 0.0f;

        Vector3 a_CacDir = a_CacVLen.normalized;        //���� ����

        GameObject BulletClone = Instantiate(GameMgr.m_BulletPrefab);
        BulletClone.tag = AllyType.BT_Enemy.ToString();   //"BT_Enemy";
        BulletCtrl a_BulletSc = BulletClone.GetComponent<BulletCtrl>();
        a_BulletSc.BulletSpawn(transform.position, a_CacDir, 30.0f);
        
    }
}
