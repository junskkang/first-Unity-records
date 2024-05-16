using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SpawnPos  //스폰 위치를 리스트로 관리 예정이고 리스트이기 때문에 인덱스를 갖게 된다.
{
    public Vector3 m_Pos = Vector3.zero;        //스폰 위치
    public float m_SpDelay = 0.0f;      //리스폰 주기
    public int m_Level = 1;     //스폰될 몬스터의 레벨은 증가됨

    public SpawnPos()   //생성자 함수
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
    //몬스터 위치 랜덤생성 관련 변수
    public GameObject MonsterPrefab;
    Transform MonsterMgr = null;
    int monMaxCount = 30;

    Transform m_EnemyGroup = null;
    GameObject m_MonPrefab = null;
    List<SpawnPos> m_SpawnPosList = new List<SpawnPos>();

    public Texture[] m_MonImg = null;

    //싱글턴 패턴
    public static Monster_Mgr Inst;

    private void Awake()
    {
        Inst = this;

        //몬스터 랜덤위치 첫생성
        MonFirstSpawn();

        //몬스터 스폰 위치 리스트로 저장해 놓기
        m_EnemyGroup = this.transform;
        m_MonPrefab = Resources.Load("MonsterRoot") as GameObject;

        //MonsterCtrl 컴포넌트가 붙어있는 차일드를 모두 배열로 불러오기
        MonsterCtrl[] a_MonsterList;
        a_MonsterList = transform.GetComponentsInChildren<MonsterCtrl>();

        //배열을 쭉 돌면서 리스트에 추가하기
        for (int i = 0; i < a_MonsterList.Length; i++)
        {
            SpawnPos a_Node = new SpawnPos();
            a_Node.m_Pos = a_MonsterList[i].gameObject.transform.position;
            a_MonsterList[i].m_SpawnIdx = i;    //MonsterCtrl class 쪽에 인덱스를 셋팅
            Destroy(a_MonsterList[i].gameObject);   //기존에 설치되어 있던 몬스터 제거
            m_SpawnPosList.Add(a_Node);
        }

        //Debug.Log(m_SpawnPosList.Count);

    }
    void Start()
    {
        //몬스터 레벨 랜덤하게 바꾸기
        for (int i = 0; i < m_SpawnPosList.Count; i++)
        {
            int a_Rnd = Random.Range(0, 10);
            if (9 <= a_Rnd)
                m_SpawnPosList[i].m_Level = 3;  //10% 확률로 몬스터 레벨 3
            else if (7 <= a_Rnd)
                m_SpawnPosList[i].m_Level = 2;  //20% 확률로 몬스터 레벨 2
            else
                m_SpawnPosList[i].m_Level = 1;  //70% 확률로 몬스터 레벨 1

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

            //죽은 몬스터 리스폰
            GameObject newObj = Instantiate(m_MonPrefab);
            newObj.transform.SetParent(m_EnemyGroup);
            newObj.transform.position = m_SpawnPosList[i].m_Pos;
            newObj.GetComponent<MonsterCtrl>().m_SpawnIdx = i;  //리스폰된 몬스터도 다시 인덱스 저장

            //레벨에 따른 몬스터 능력치 셋팅
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
            m_SpawnPosList[idx].m_Level = 3;  //10% 확률로 몬스터 레벨 3
        else if (7 <= a_Rnd)
            m_SpawnPosList[idx].m_Level = 2;  //20% 확률로 몬스터 레벨 2
        else
            m_SpawnPosList[idx].m_Level = 1;  //70% 확률로 몬스터 레벨 1

        m_SpawnPosList[idx].m_SpDelay = Random.Range(4.0f, 6.0f);
    }
}
