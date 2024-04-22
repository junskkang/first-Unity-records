using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster
{
    string[] ranName = { "�ٺ�", "���", "����", "����", "�ΰ�", "��ɰ�", "�巡��", "�Ÿӵ�", "���ÿ�", "���̳�Ҿ�" };
    
    public string m_Name = ""; //�̸� //�̸��� ���� ���� �̸��� ���ڼ��� : 2�� ~ 5�ڱ���...
    public int m_Level = 0; //���� 1 ~ 99 �⺻��

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
        Debug.Log("<���� �ڵ� ������>");

        for (int ii = 0; ii < m_MonList.Count; ii++)
        {
            Debug.Log(string.Format("{0}�� : �̸�({1}) ����({2})",
            ii + 1, m_MonList[ii].m_Name, m_MonList[ii].m_Level));
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}