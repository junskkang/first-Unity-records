using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �޼����� ����Ʈ �Ű����� (C��� : �Լ��� ����Ʈ �μ�)
// �޼����� �Ű������� �⺻���� ���� �� �ִ�.
// �޼����� ȣ�� �� ���� ������ ������, ������ �⺻ ���� ����Ѵ�.
public class Test_2 : MonoBehaviour
{
    void Test(int AAA, bool isShow = true)
    {
        if(isShow==true)
        {
            int Result = AAA;
            Debug.Log(Result);
        }
        else
        {
            Debug.Log("Hide");
        }
    }

    //void Person(string name)
    //{
    //    Debug.Log(name);
    //}

    //void Person(string name, int age)
    //{
    //    Debug.Log(name + " : " +  age);
    //}

    //void Person(string name, int age, string address)
    //{
    //    Debug.Log(name + " : " + age + " : " + address);
    //}

    void Person(string name, int age = 0, string address = "None")
    {
        string a_Temp = name;
        if (0 < age)
            a_Temp += " : " + age;
        if (address != "")
            a_Temp += " : " + address;

        Debug.Log(a_Temp);
    }

    // ����Ʈ �Ű����� ��Ģ
    // �ڿ� �ִ� �Ű��������� ���� �ο��� �� �ְ�, �߰��� �ǳʶ�� �ο��� �� ����.

    void CacHap(int a_Strat, int a_End = 100, int a_Add = 1)
    {
        int a_Hap = 0;
        for(int i = a_Strat; i < a_End; i += a_Add)
        {
            Debug.Log(a_Hap + " + " + i + " = " + (a_Hap + i));
            a_Hap = a_Hap + i;
        }    

    }

    void Start()
    {
        Person("��ȣ");
        Person("��ȣ", 23);
        Person("��ȣ", 23, "�����");

        //C# 4.0���� �����ϴ� ���� �Ű�����
        Person("��ȣ", address: "����Ư����", age : 30);     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
