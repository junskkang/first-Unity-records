using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SpawnPos  //���� ��ġ�� ����Ʈ�� ���� �����̰� ����Ʈ�̱� ������ �ε����� ���� �ȴ�.
{
    public Vector3 m_Pos = Vector3.zero;        //���� ��ġ
    public float m_SpDelay = 0.0f;      //������ �ֱ�
    public int m_Level = 1;     //������ ������ ������ ������

    public SpawnPos()   //������ �Լ�
    {
        m_SpDelay = 0.0f;
    }

    public bool Update_SpPos(float a_DeltaTime)
    {
        if (0.0f < m_SpDelay)
        {
            m_SpDelay -= a_DeltaTime;
            if (m_SpDelay <= 0.0f) 
            {
                m_SpDelay = 0.0f;
                return true;
            }
        }

        return false;
    }

}

public class Monster_Mgr : MonoBehaviour
{
    //���� ��ġ �������� ���� ����
    public GameObject MonsterPrefab;
    Transform MonsterMgr = null;
    int monMaxCount = 30;

    Transform m_EnemyGroup = null;
    GameObject m_MonPrefab = null;
    List<SpawnPos> m_SpawnPosList = new List<SpawnPos>();

    public Texture[] m_MonImg = null;

    //�̱��� ����
    public static Monster_Mgr Inst;

    private void Awake()
    {
        Inst = this;

        //���� ������ġ ù����
        MonFirstSpawn();

        //���� ���� ��ġ ����Ʈ�� ������ ����
        m_EnemyGroup = this.transform;
        m_MonPrefab = Resources.Load("MonsterRoot") as GameObject;

        //MonsterCtrl ������Ʈ�� �پ��ִ� ���ϵ带 ��� �迭�� �ҷ�����
        MonsterCtrl[] a_MonsterList;
        a_MonsterList = transform.GetComponentsInChildren<MonsterCtrl>();

        //�迭�� �� ���鼭 ����Ʈ�� �߰��ϱ�
        for (int i = 0; i < a_MonsterList.Length; i++)
        {
            SpawnPos a_Node = new SpawnPos();
            a_Node.m_Pos = a_MonsterList[i].gameObject.transform.position;
            a_MonsterList[i].m_SpawnIdx = i;    //MonsterCtrl class �ʿ� �ε����� ����
            Destroy(a_MonsterList[i].gameObject);   //������ ��ġ�Ǿ� �ִ� ���� ����
            m_SpawnPosList.Add(a_Node);
        }

        //Debug.Log(m_SpawnPosList.Count);

    }
    void Start()
    {
        //���� ���� �����ϰ� �ٲٱ�
        for (int i = 0; i < m_SpawnPosList.Count; i++)
        {
            int a_Rnd = Random.Range(0, 10);
            if (9 <= a_Rnd)
                m_SpawnPosList[i].m_Level = 3;  //10% Ȯ���� ���� ���� 3
            else if (7 <= a_Rnd)
                m_SpawnPosList[i].m_Level = 2;  //20% Ȯ���� ���� ���� 2
            else
                m_SpawnPosList[i].m_Level = 1;  //70% Ȯ���� ���� ���� 1

            m_SpawnPosList[i].m_SpDelay = 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RespawnMoster();
    }

    void MonFirstSpawn()
    {
        MonsterMgr = gameObject.GetComponent<Transform>();


        if (MonsterPrefab != null)
        {
            for (int i = 0; i < monMaxCount; i++)
            {

                GameObject monster = Instantiate(MonsterPrefab);
                monster.transform.SetParent(MonsterMgr, false);
                int ranX = Random.Range(-95, 95);
                int ranZ = Random.Range(-95, 95);
                monster.transform.position = new Vector3((int)ranX, 1, (int)ranZ);
            }
        }
    }

    void RespawnMoster()
    {
        for (int i = 0; i < m_SpawnPosList.Count; i++)
        {
            if (m_SpawnPosList[i].Update_SpPos(Time.deltaTime) == false)
                continue;

            //���� ���� ������
            GameObject newObj = Instantiate(m_MonPrefab);
            newObj.transform.SetParent(m_EnemyGroup);
            newObj.transform.position = m_SpawnPosList[i].m_Pos;
            newObj.GetComponent<MonsterCtrl>().m_SpawnIdx = i;  //�������� ���͵� �ٽ� �ε��� ����

            //������ ���� ���� �ɷ�ġ ����
            int a_Lv = m_SpawnPosList[i].m_Level;
            float a_MaxHp = 100.0f + (a_Lv * 50.0f);
            if (1000.0f < a_MaxHp) 
                a_MaxHp = 1000.0f;

            float a_AttSpeed = 0.5f - (a_Lv * 0.1f);
            if (a_AttSpeed < 0.1f)
                a_AttSpeed = 0.1f;

            float a_MvSpeed = 2.0f + (a_Lv * 0.5f);
            if (5.0f < a_MvSpeed)
                a_MvSpeed = 5.0f;

            newObj.GetComponent<MonsterCtrl>().SetSpawnInfo(i, a_Lv, a_MaxHp, a_AttSpeed, a_MvSpeed);
        }
    }

    public void ResetSpawn(int idx)
    {
        if(idx < 0 || m_SpawnPosList.Count <= idx) return;

        int a_Rnd = Random.Range(0, 10);
        if (9 <= a_Rnd)
            m_SpawnPosList[idx].m_Level = 3;  //10% Ȯ���� ���� ���� 3
        else if (7 <= a_Rnd)
            m_SpawnPosList[idx].m_Level = 2;  //20% Ȯ���� ���� ���� 2
        else
            m_SpawnPosList[idx].m_Level = 1;  //70% Ȯ���� ���� ���� 1

        m_SpawnPosList[idx].m_SpDelay = Random.Range(4.0f, 6.0f);
    }
}
