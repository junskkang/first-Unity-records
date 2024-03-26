using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyItem
{
    public string m_Name = "";
    public int m_Level = 0;
    public float m_AttRate = 1.0f;
    public int m_Price = 100;

    public MyItem(string a_Name = "", int a_Level = 0, float a_Ar = 0.0f, int a_Price = 0)
    {//������ �����ε� �Լ�
        m_Name = a_Name;
        m_Level = a_Level;  
        m_AttRate = a_Ar;
        m_Price = a_Price;
    }

    public void PrintInfo()
    {
        Debug.Log($"�̸�({m_Name}) : ����({m_Level}) : ���ݻ�·�({m_AttRate}) : ����({m_Price})");
    }
}

public class Test_2 : MonoBehaviour
{
    List<MyItem> m_ItList = new List<MyItem>();   //�����ϸ� ó���� �ƹ��� ������ ���� Count = 0

    int PriceASC(MyItem a, MyItem b)             //���� �������� ����
    {
        return a.m_Price.CompareTo(b.m_Price);
    }

    int LevelDESC(MyItem a, MyItem b)            //���� �������� ����
    {
        return b.m_Level.CompareTo(a.m_Level);
    }

    // Start is called before the first frame update
    void Start()
    {
        //int AAA;      //�Ϲ� ������ ����
        //AAA = 10;     //�Ϲ� ������ �ʱ�ȭ
        //int BBB = 10; //�Ϲ� ������ ����� ���ÿ� �ʱ�ȭ


        ////Ŭ���� 1�� �ʱ�ȭ ��� : ��ü�� ���� �� �ʱ�ȭ
        //MyItem a_Node = new MyItem();
        //a_Node.m_Name = "õ���� ����";
        //a_Node.m_Level = 3;
        //a_Node.m_AttRate = 2.0f;
        //a_Node.m_Price = 2500;

        ////Ŭ���� 2�� �ʱ�ȭ ��� : ��ü ����� ���ÿ� �ʱ�ȭ(������ �����ε� �Լ�)
        //MyItem a_Node2 = new MyItem("õ���ǹ���", 3, 2.0f, 2500);

        ////Ŭ���� 3�� �ʱ�ȭ ��� : ��ü ����� ���ÿ� ���������� ���� �ʱ�ȭ
        //MyItem a_Node3 = new MyItem { m_Name = "õ���� ����", m_Price = 2500 };
        //a_Node3.PrintInfo();

        
        
        //����Ʈ ��� �߰�
        MyItem a_Node = new MyItem("õ���� ����", 3, 2.0f, 2500);
        m_ItList.Add(a_Node);

        a_Node = new MyItem("�ȶ��� ��", 1, 1.2f, 1200);
        m_ItList.Add(a_Node);

        a_Node = new MyItem("�ü��� Ȱ", 4, 1.7f, 1500);
        m_ItList.Add(a_Node);

        a_Node = new MyItem("�ź����� ����", 5, 0.5f, 3000);
        m_ItList.Add(a_Node);

        //for���� ���� �ҽ� Ȯ��
        for(int ii = 0;  ii < m_ItList.Count; ii++)
            m_ItList[ii].PrintInfo();

        Debug.Log("---------------------");
        ////��� ����
        ////m_ItList.RemoveAt(0);      // m_ItList[0] ��� ����   
        //Debug.Log("ù��° ��� ���� ���");
        //foreach (MyItem a_It in m_ItList)
        //    a_It.PrintInfo();

        ////��������� ����
        //if (0 < m_ItList.Count)
        //{
        //    m_ItList.RemoveAt(m_ItList.Count - 1);
        //    Debug.Log("������ ��� ���� ���");
        //    foreach (MyItem a_It in m_ItList)
        //        a_It.PrintInfo();
        //}

        ////����� ������ ã�� �����ϴ� ���
        //MyItem a_FNode = m_ItList[1];
        //if(m_ItList.Contains(a_FNode) == true) //a_FNode �ν��Ͻ��� ����Ʈ�� �����ϴ��� Ȯ��
        //{
        //    m_ItList.Remove(a_FNode);
        //    //Remove�Լ��� ��ü�� ����Ʈ�� �������� ���� ���¿��� ���� �õ��ص� ������ ���� ����
        //}

        //foreach (MyItem a_It in m_ItList)
        //    a_It.PrintInfo();

        ////�߰� �߰��� ���ǿ� �����ϴ� ��츸 ���������� �����ϱ�
        //for(int ii = 0;ii < m_ItList.Count;)
        //{
        //    if (m_ItList[ii].m_Price < 2000)
        //    {
        //        m_ItList.RemoveAt(ii);
        //    }
        //    else
        //        ii++;
        //}

        //for (int ii = 0; ii < m_ItList.Count; ii++)
        //    m_ItList[ii].PrintInfo();

        //�߰��� �߰��ϱ�
        a_Node = new MyItem("����� �̻�", 4, 1.2f, 12000);
        m_ItList.Insert(1, a_Node);
        Debug.Log("1�� �ε����� �߰��� �߰� ��� Ȯ��");


        foreach (MyItem a_It in m_ItList)
            a_It.PrintInfo();


        //m_ItList.Add(new MyItem("����� �̻�", 4, 1.2f, 12000));
        //m_ItList.Insert(1, new MyItem("����� �̻�", 4, 1.2f, 12000));

        //����
        //���Ľÿ� ���� ������ ����Ʈ�� ���纻�� ���� �¸� �����Ͽ� ȭ������ �����ִ���
       
        //������ ���������� ���������� ����
        List<MyItem> a_CopyList = m_ItList.ToList(); //����Ʈ ���� using System.Linq; �߰� �ʿ�

        Debug.Log("���� �������� ����");
        a_CopyList.Sort(PriceASC);

        foreach (MyItem a_It in a_CopyList)
            a_It.PrintInfo();
        
        //������ ���������� ���������� ����
        List<MyItem> a_CloneList = m_ItList.ToList(); //����Ʈ ���� using System.Linq; �߰� �ʿ�
   
        Debug.Log("���� �������� ����");
        a_CloneList.Sort(LevelDESC);
        
        foreach (MyItem a_It in a_CloneList)
            a_It.PrintInfo();

        Debug.Log("�߰��� ������ ����");
        foreach (MyItem a_It in m_ItList)
            a_It.PrintInfo();
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
