using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//--- SpawnPos
class SpawnPos //리스트로 관리 예정이고 리스트이기 때문에 인덱스를 갖게 된다.
{
    public Vector3 m_Pos = Vector3.zero;
    public float m_SpDelay = 0.0f;
    public int m_Level = 1; //스폰될 몬스터의 레벨은 증가될 것이다.

    public SpawnPos() //생성자 함수
    {
        m_SpDelay = 0.0f;
    }

    public bool Update_SpPos(float a_DeltaTime)
    {
        if(0.0f < m_SpDelay)
        {
            m_SpDelay -= a_DeltaTime;
            if(m_SpDelay <= 0.0f)
            {
                m_SpDelay = 0.0f;
                return true;
            }
        }

        return false;
    }//public bool Update_SpPos(float a_DeltaTime)
}
//-------SpawnPos

public class Monster_Mgr : MonoBehaviour
{
    Transform m_EnemyGroup = null;
    GameObject m_MonPrefab = null;
    List<SpawnPos> m_SpawnPosList = new List<SpawnPos>();

    public Texture[] m_MonImg = null;

    //--- 싱글턴 패턴
    public static Monster_Mgr Inst;

    void Awake()
    {
        Inst = this;

        //--- 몬스터 스폰 위치 리스트로 저장해 놓기...
        m_EnemyGroup = this.transform;
        m_MonPrefab = Resources.Load("MonsterRoot") as GameObject;

        MonsterCtrl[] a_MonSterList;
        a_MonSterList = transform.GetComponentsInChildren<MonsterCtrl>();
        // MonsterCtrl 컴포넌트가 붙어있는 애들만 찾아오기...
        for(int i = 0; i < a_MonSterList.Length; i++)
        {
            SpawnPos a_Node = new SpawnPos();
            a_Node.m_Pos = a_MonSterList[i].gameObject.transform.position;
            a_MonSterList[i].m_SpawnIdx = i;  //MonsterCtrl class 쪽에 m_SpawnIdx 인덱스변수를 만들어 준다.
            Destroy(a_MonSterList[i].gameObject);  //기존에 설치되어 있던 몬스터는 모두 제거해 준다.
            m_SpawnPosList.Add(a_Node);
        }
        //--- 몬스터 스폰 위치 리스트로 저장해 놓기...
    }
    //--- 싱글턴 패턴

    // Start is called before the first frame update
    void Start()
    {
        //--- 몬스터 스폰 랜덤하게 바꾸기...
        for(int i = 0; i < m_SpawnPosList.Count; i++)
        {
            int a_Rnd = Random.Range(0, 10);
            if (9 <= a_Rnd)
                m_SpawnPosList[i].m_Level = 3;
            else if (7 <= a_Rnd)
                m_SpawnPosList[i].m_Level = 2;
            else
                m_SpawnPosList[i].m_Level = 1;

            m_SpawnPosList[i].m_SpDelay = 0.01f;
        }
        //--- 몬스터 스폰 랜덤하게 바꾸기...
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < m_SpawnPosList.Count; i++)
        {
            if (m_SpawnPosList[i].Update_SpPos(Time.deltaTime) == false)
                continue;

            //새로 스폰 시킨다.
            GameObject newObj = Instantiate(m_MonPrefab);
            newObj.transform.SetParent(m_EnemyGroup);
            newObj.transform.position = m_SpawnPosList[i].m_Pos;
            //newObj.GetComponent<MonsterCtrl>().m_SpawnIdx = i;  //m_SpawnPos 의 인덱스를 셋팅해 준다.

            //--- 레벨에 따른 몬스터 능력치 셋팅
            int a_Lv = m_SpawnPosList[i].m_Level;
            float a_MaxHp = 100.0f + (m_SpawnPosList[i].m_Level * 50.0f);
            if (1000.0f < a_MaxHp)
                a_MaxHp = 1000.0f;  //최대체력 100.0f ~ 1000.0f 까지
            float a_AttSpeed = 0.5f - (m_SpawnPosList[i].m_Level * 0.1f);
            if (a_AttSpeed < 0.1f)
                a_AttSpeed = 0.1f;  //공속 0.5f ~ 0.1f 까지
            float a_MvSpeed = 2.0f + (m_SpawnPosList[i].m_Level * 0.5f);
            if (5.0f < a_MvSpeed)
                a_MvSpeed = 5.0f;   //이속 2.0f ~ 5.0f 까지
            newObj.GetComponent<MonsterCtrl>().SetSpawnInfo(i, a_Lv, a_MaxHp, a_AttSpeed, a_MvSpeed);
            //--- 레벨에 따른 몬스터 능력치 셋팅
        }
    }// void Update()

    public void ReSetSpawn(int idx)
    {
        if (idx < 0 || m_SpawnPosList.Count <= idx)
            return;

        //int a_Rnd = Random.Range(0, 10);
        //if (9 <= a_Rnd)
        //    m_SpawnPosList[idx].m_Level = 3;
        //else if (7 <= a_Rnd)
        //    m_SpawnPosList[idx].m_Level = 2;
        //else
        //    m_SpawnPosList[idx].m_Level = 1;

        m_SpawnPosList[idx].m_Level++;
        if (3 < m_SpawnPosList[idx].m_Level)
            m_SpawnPosList[idx].m_Level = 3;

        m_SpawnPosList[idx].m_SpDelay = Random.Range(4.0f, 6.0f);
    }
}
