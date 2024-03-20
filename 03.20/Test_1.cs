using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


//����ü value Type�� Ŭ���� Reference Type : �޼����� �Ű������� ���� ���� Ư¡



public struct Monster             // ����ü ���� : ���� �Ѹ����� ����
{
    public string m_Name;         // ����ü �ɹ� ����
    public int m_Hp;              // C#������ ����ü���� public�Ӽ��� �����ϸ�
    public int m_Mp;              // �⺻ �Ӽ��� private �Ӽ��� �ȴ�.
    public int m_Level;
    public float m_Attack;

    public void PrintInfo()       //����ü �ɹ� �޼��� ����
    {
        string str = $"{m_Name} : ü��({m_Hp}) ����({m_Mp}) " +
                     $"����({m_Level}) ���ݷ�({m_Attack.ToString("N2")})";
        Debug.Log(str);
    }
}

//Ŭ����

public class Item        //Ŭ���� ����
{
    public string m_Name;     //������ �̸�
    public int m_Level;       //������ ����
    public int m_Star;        //������ ����
    public int m_Price;       //������ ����
    float m_AttackRate = 1.0f;  //������ ���� ��·�   //�߿� ������ �� ���ϸ� �ȵż� private
    //�Ӽ�, property
    public float AttackRate
    {
        get => m_AttackRate;
        set => m_AttackRate = value;
    }


    public float GetAttackRate()  //������ ��������
    {
        float Temp = m_AttackRate;

        //��ȣȭ

        return Temp;
    }

    public void SetAttackRate(float rate)  //������ ��������
    {
        //��ȣȭ

        if (1.0f < rate)
            return;

        m_AttackRate = rate;
    }

    public void PrintInfo()
    {
        string str = $"{m_Name} : ����({m_Level}) ����({m_Star}) " +
                     $"����({m_Price}) ���ݻ�·�({m_AttackRate.ToString("N2")})";
        Debug.Log(str);
    }

    public void CopyItem(Item a_Item)  //�Ϲ��Լ��� �ƴ϶� ������ ������ �ϴ� �Լ� '���������'
    {
        m_Name = a_Item.m_Name;
        m_Level = a_Item.m_Level;
        m_Star = a_Item.m_Star;
        m_Price = a_Item.m_Price;
        m_AttackRate = a_Item.GetAttackRate();
    }
}
public class Test_1 : MonoBehaviour
{
    public void Value(int aa)           //�Ű������� �Ϲݺ�����
    {
        aa = 1000;
    }

    public void Reference(int[] aa)     //�Ű������� �迭��
    {
        aa[0] = 1000;
    }

    public void MonsterMethod(Monster a_Mon)  //����ü�� �Ű������� �޴� �Լ�
    {
        a_Mon.m_Name = "��������ü";
        a_Mon.m_Level = 999;
        a_Mon.m_Hp = 999;
        a_Mon.m_Mp = 999;
        a_Mon.m_Attack = 999.9f;
    }

    public void ItemMethod(Item a_It)         //Ŭ������ �Ű������� �޴� �Լ� 
    {
        a_It.m_Name = "���۷���Ŭ����";
        a_It.m_Level = 999;
        a_It.m_Star = 999;
        a_It.m_Price = 999;
    }

    // Start is called before the first frame update
    void Start()
    {
        int xx = 0;
        Value(xx);
        Debug.Log("xx : " + xx);    // 0

        int[] yy = { 0 };
        Reference(yy);
        Debug.Log("yy : " + yy[0]);    // 1000

        //����ü
        Monster AAA;
        AAA.m_Name = "�巡��";
        AAA.m_Level = 0;
        AAA.m_Hp = 0;
        AAA.m_Mp = 0;   
        AAA.m_Attack = 0.0f;
        MonsterMethod(AAA);
        AAA.PrintInfo();          //�巡��

        //Ŭ����
        Item BBB = new Item();
        BBB.m_Name = "õ���� ����";
        BBB.m_Level = 0;
        BBB.m_Star = 0;
        BBB.m_Price = 0;
        ItemMethod(BBB);
        BBB.PrintInfo();          //���۷���Ŭ����
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
