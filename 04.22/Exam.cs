using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    string[] ranName = { "바보", "상어", "엘프", "좀비", "인간", "사냥개", "드래곤", "매머드", "원시용", "다이노소어" };
    
    public string m_Name = ""; //이름 //이름은 랜덤 생성 이름의 글자수는 : 2자 ~ 5자까지...
    public int m_Level = 0; //레벨 1 ~ 99 기본값

    public Monster()
    {
        int a_Idx = Random.Range(0, ranName.Length);
        m_Name = ranName[a_Idx];
        m_Level = Random.Range(1, 100);
    }
};

public class Exam : MonoBehaviour
{
    List<Monster> m_MonList = new List<Monster>();

    static void MakeMonster(List<Monster> m_RefMonList, int a_MonNum = 10)
    {
        Monster monster = new Monster();
        for (int i = 0; i < a_MonNum; i++)
        {
            monster = new Monster();
            m_RefMonList.Add(monster);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_MonList.Clear();
        MakeMonster(m_MonList, 5);
        Debug.Log("<몬스터 자동 생성기>");

        for (int ii = 0; ii < m_MonList.Count; ii++)
        {
            Debug.Log(string.Format("{0}번 : 이름({1}) 레벨({2})",
            ii + 1, m_MonList[ii].m_Name, m_MonList[ii].m_Level));
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}