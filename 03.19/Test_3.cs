using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//����ü (Struct) - ���� �ܼ�
//���� ������ ���� ���� �������� ������� �޼������ ����
//cf) ���� ���������� ������� �������� ������ '�迭'
//����ü ��ü�� Value Type (int, float, double..... , struct)
//�뵵 : ���� ������ ������ ���� �����ϴ� ���� ���
//����ü�� ��� �� �� ����.


//Ŭ���� (class) - ���� ���� ���
//���� ������ ���� ���� �������� ������� �޼������ ����
//Ŭ���� ��ü�� Reference Type (array, class��ü)
//�뵵 : ����Ʈ��� ��ǰȭ(��ü���� ���α׷���)�ϱ� ���� ������ ���

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
    public float AttackRate { get => m_AttackRate; 
                              set => m_AttackRate = value; } 

    
    //public float GetAttackRate()  //������ ��������
    //{
    //    float Temp = m_AttackRate;

    //    //��ȣȭ

    //    return Temp;
    //}

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
    }
}

// ���ټ�����, ����������
// public : ����ü, Ŭ���� ���ο� �ܺο��� ��� �����ؼ� ����� �� �ִ� �Ӽ�
// private : ����ü, Ŭ���� ���ο����� ��� �����ϰ� �ܺο����� ������ �� ���� �Ӽ�
// protected : �ܺο��� ������ �� ����, �ڽŰ� ��Ӱ����� �ڽ�Ŭ���������� ������ ����ϴ� �Ӽ�
public class Test_3 : MonoBehaviour
{

    void Start()
    {
        Monster AAA;          //����ü ����AAA ����
        AAA.m_Name = "��ũ";
        AAA.m_Hp = 100;
        AAA.m_Mp = 60;
        AAA.m_Level = 1;
        AAA.m_Attack = 2.5f;
        AAA.PrintInfo();

        Monster BBB;          //����ü ����BBB ����
        BBB.m_Name = "����";
        BBB.m_Hp = 200;
        BBB.m_Mp = 80;
        BBB.m_Level = 5;
        BBB.m_Attack = 7.2f;
        BBB.PrintInfo();

        Monster CCC;          //����ü ����CCC ����
        CCC.m_Name = "������";
        CCC.m_Hp = 110;
        CCC.m_Mp = 40;
        CCC.m_Level = 2;
        CCC.m_Attack = 0.5f;
        CCC.PrintInfo();

        Monster DDD;          //����ü ����DDD ����
        DDD.m_Name = "�ΰ�";
        DDD.m_Hp = 500;
        DDD.m_Mp = 10;
        DDD.m_Level = 10;
        DDD.m_Attack = 10.0f;
        DDD.PrintInfo();

        //����ü�� Value Type 
        Monster a_TestVal = AAA;   //�Ʒ��� ���� �ɹ� �� �ɹ��� 1:1���簡 �̷����(��������)
        //a_TestVal.m_Name = AAA.m_Name;
        //a_TestVal.m_Hp = AAA.m_Hp; 
        //            :
        //            :
        a_TestVal.PrintInfo();     // AAA�� �Ӽ��� �����Ͽ� ��ũ�� ���
        a_TestVal.m_Name = "���";
        a_TestVal.m_Hp = 999;
        a_TestVal.m_Mp = 999;
        a_TestVal.m_Level = 999;
        a_TestVal.m_Attack = 999;
        Debug.Log("--- ����ü Value Type Test ---");
        AAA.PrintInfo();           // ������� ��ũ�� ��µ�
        a_TestVal.PrintInfo();     // ���� �Է��� ���� �� ��µ�
        //����, ����ü�� ���Խ� ���� �����ؿ��� ���� �� �� �ִ�.


        //Ŭ���� ���� ����, ��ü(�̸� ���� �����Ǿ��ִ� �͵�) ����, �ν��Ͻ�(���� �߰��� �����Ǵ� �͵�) ����
        Item a_MyItem = new Item();     //�迭 ��ó�� ���ο� �����Ͱ����� �����´ٴ� �ǹ�!
        a_MyItem.m_Name = "õ���� ����";
        a_MyItem.m_Level = 4;
        a_MyItem.m_Star = 2;
        a_MyItem.m_Price = 1200;
        //a_MyItem.m_AttackRate = 1.1f;   //private�Ӽ��� ��ü�� ���ؼ� �ܺο��� ���� �Ұ���!
        a_MyItem.SetAttackRate(1.3f);   //��ó�� ���������� �ȵǱ⿡ ���������� �ؾ���
        //a_MyItem.PrintInfo();

        //Ŭ������ Reference Type Test
        Item a_TestRef = a_MyItem;
        a_TestRef.PrintInfo();              // ���⼭�� a_MyItem�� �����ϰ� ���
        a_TestRef.m_Name = "�巡���� ����"; // �� ����
        a_TestRef.m_Level = 999;
        a_TestRef.m_Star = 10;
        a_TestRef.m_Price = 250000;
        Debug.Log("--- Ŭ���� Reference Type Test ---");
        a_MyItem.PrintInfo();               // a_MyItem�� �巡���� ����
        a_TestRef.PrintInfo();              // a_TestRef�� �巡���� ����

        //Ŭ���� Ÿ���ε��� �ұ��ϰ� ����Ÿ���� �ƴ϶� ������ ��ü�� ����� ���� ���
        Item a_XXX = new Item();
        a_XXX.m_Name = a_MyItem.m_Name;
        a_XXX.m_Level = a_MyItem.m_Level; 
        //            :
        //            :
        // �ϳ��� �������� new�� �����ϰ� �� ������ �Ӽ��� ������ �ϳ��ϳ� �ϴ��� ��������ָ� ��
        // �Ǵ�! �̷��� �۾��� ���ִ� �Լ��� Ŭ���� �ȿ� �ɹ� �Լ��� �������� 
        // �Լ��� ȣ�� ��Ű�� ����� �־�! 
        Item a_YYY = new Item();
        a_YYY.CopyItem(a_MyItem);    
    }


    void Update()
    {
        
    }
}
