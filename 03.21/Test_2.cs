using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTeam;
using System.Net.Mail;

public class Item         //������ �̹� �����ϴ� Ŭ����
{
    public string m_Nick;
    public int m_Star;
    public float m_AttRate;
}

namespace MyClass
{
    public class Item     //���� ���� ����� ���� ���ο� Ŭ����
    {
        public string m_Name;
        public int m_Level;
        public int m_Price;
    }
}
public class Test_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if KOREA
        Debug.Log("�̰��� �ѱ� �����Դϴ�.");
#elif ENGLISH
        Debug.Log("�̰��� ���� �����Դϴ�.");
#elif CHINA
        Debug.Log("�̰��� �߱��� �����Դϴ�.");
#endif
        Item AAA = new Item();
        AAA.m_Nick = "�ȶ��� ��";
        AAA.m_Star = 5;
        AAA.m_AttRate = 1.0f;
        Debug.Log(AAA.m_Nick + " : " + AAA.m_Star + " : " + AAA.m_AttRate);

        MyClass.Item BBB = new MyClass.Item();
        BBB.m_Name = "�巡���� ����";
        BBB.m_Level = 4;
        BBB.m_Price = 1000;
        Debug.Log(BBB.m_Name + " : " + BBB.m_Level + " : " + BBB.m_Price);

        MyTeam.Monster CCC = new MyTeam.Monster();
        CCC.m_Name = "����";
        CCC.m_Hp = 100;
        CCC.m_Mp = 100;
        CCC.m_Attack = 20;
        CCC.PrintInfo();

        MyTeam.Monster DDD = new MyTeam.Monster();
        DDD.m_Name = "��ũ";
        DDD.m_Hp = 200;
        DDD.m_Mp = 50;
        DDD.m_Attack = 50;
        DDD.PrintInfo();


        // alt + ctrl clickEEE
        Monster EEE = new Monster();
        EEE.m_Name = "����";
        EEE.m_Hp = 1000;
        EEE.m_Mp = 0;
        EEE.m_Attack = 10;
        EEE.PrintInfo();

    }   // Update is called once per frame
    void Update()
    {
        
    }
}
