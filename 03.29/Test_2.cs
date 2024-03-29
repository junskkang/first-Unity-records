using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Ŭ���� ���
//�θ� Ŭ������ �ɹ������� �ɹ��Լ��� �ڽ� Ŭ������ ��ӹ޾Ƽ� ����ϰڴٴ� ����
//�ڵ��� ��Ȱ���� ���� ���ȴ�.
//���� ������ �� �� ����� ��� Ŭ������ ������ �Ļ�Ŭ������ Ȯ���ؼ� ����� ������ڴ� ����
//���� ����� ����� ���� �θ𿡼� ����
//���ڸ����� ��������� ���� ����
//������������ ����(�̷��� �߰� ����� �����ϱ� ���� ����) to be continued

//<��� Ư���� ���� �з�>
//1. Ȯ���� : ���� -> ����, ���� -> �Ĺ�
//2. ������� : ���� -> ���� -> ������ -> ���
//3. ���߻�� : �ڵ���, ī�޶� -> ī�޶���
//              ������, �ѽ�, ��ĳ�� -> ���ձ�
//              C#���� ���߻���� ������ ���� �������̽� Ŭ������ ����
//              Interface class�� �θ� Ŭ������ ���� �� ���� = ��Ӱ��谡 ���� ���� ����


public class Parent //Ŭ���� �տ� public�� ���� �Ǹ� �ٸ� Ŭ�������� public ��� ������ ����� �� ����.
{
    public int num;      // �ɹ� ����
    public int m_Age;
    protected int m_Height;    //�ڽĵ������ ��� ���
    
    public Parent()   //������ �Լ�
    {
        m_Age = 10;
        m_Height = 130;
        Debug.Log("�θ� Ŭ������ �����ڰ� ȣ��Ǿ����ϴ�.");
    }


    public void PrintInfo()        //�ɹ� �Լ�
    {
        Debug.Log($"num : {this.num}");

    }
}

public class Child : Parent     // ParentŬ������ ��ӹ޴� Child Ŭ���� ����
{
    new public int num;   //�θ� Ŭ������ ���� �������� Parent.num�� ������� newŰ���带 �տ� ���̸� �ȴ�.
    public int m_Kg;

    public Child(int a_Num)    //������ �����ε� �Լ�
    {
        this.num = a_Num;        //this. �ڱ� Ŭ���� ���� �ִ� ������ �Լ��� ���� ����
        base.num = a_Num * 100;  //base. �θ� Ŭ���� ���� �ִ� ������ �Լ��� ���� ����
        Debug.Log("�ڽ� Ŭ������ �����ڰ� ȣ��Ǿ����ϴ�.");
    }

    public void DisplayValue()
    {
        Debug.Log($"�θ��� num : {base.num}, �ڽ��� num : {this.num}");
        Debug.Log($"���� : {m_Age}, Ű : {m_Height}, ������ : {m_Kg}");

        PrintInfo();
    }
}

public class Test_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Parent pt = new Parent();
        pt.num = 80;
        pt.PrintInfo();

        Child cd = new Child(20);
        cd.DisplayValue();
        Debug.Log(cd.m_Age);
        //Debug.Log(cd.m_Height);
        cd.num = 77;
        cd.DisplayValue() ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
