using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���� ���

public class GrandParent
{
    public string m_UserName;
    public long m_UserId;
    
    //public GrandParent()
    //{
    //    Debug.Log("GrandParent Ŭ������ ������ �Լ� ȣ��");
    //}

    public void CallPrint()
    {
        Debug.Log("GrandParent Ŭ������ CallPrint() �Լ� ȣ��");
    }
    public virtual void MyPrint()
    {
        Debug.Log("GrandParent Ŭ������ MyPrint()�Լ� ȣ��");
    }
}


public class Parent2 : GrandParent
{
    public int m_UserAge;
    //public Parent2()
    //{
    //    Debug.Log("Parent2 Ŭ������ ������ �Լ� ȣ��");
    //}
    public new void CallPrint()
    {
        Debug.Log("Parent2 Ŭ������ CallPrint() �Լ� ȣ��");
    }
    public override void MyPrint()
    {
        Debug.Log("Parent2 Ŭ������ MyPrint()�Լ� ȣ��");
    }
}

public class Child2 : Parent2
{
    public int m_Height;

    //public Child2()
    //{
    //    Debug.Log("Child2 Ŭ������ ������ �Լ� ȣ��");
    //}
    public new void CallPrint()
    {
        Debug.Log("Child2 Ŭ������ CallPrint() �Լ� ȣ��");
    }
    public override void MyPrint()
    {
        Debug.Log("Child2 Ŭ������ MyPrint()�Լ� ȣ��");
    }
}

public class Test_4 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Child2 a_Cld = new Child2();
        GrandParent AAA = a_Cld;          //��ĳ����

        GrandParent BBB = new Child2();   //������ ���ÿ� ��ĳ����
        
        BBB.MyPrint();         //�������̵��� ���� Child2 �Լ� ȣ��
        BBB.CallPrint();       //��ĳ���������� ���� Ŭ������ GrandParent �Լ� ȣ��
        ((Parent2)BBB).CallPrint(); //�ٿ�ĳ������ ���� Parent2�� CallPrint �Լ� ȣ��
        ((Child2)(BBB)).CallPrint(); //�ٿ�ĳ������ ���� Child2�� CallPrint �Լ� ȣ��


        GrandParent CCC = new Parent2();
        CCC.MyPrint();         //�������̵��� ���� Parent2 �Լ� ȣ��
        CCC.CallPrint();       //��ĳ���������� ���� Ŭ������ GrandParent �Լ� ȣ��
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
