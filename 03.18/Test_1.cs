using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;


// C# Method
// �뵵 : �ڵ带 ��Ȱ���ϱ� ���� �뵵�� ����� �ǰ�, 
// ���������� ���α׷��� ������� ��ǰ(���)�� ����� �뵵�ε� ���ȴ�.
// ��, �޼���� �ڵ带 ��Ȱ���ϰų� �ڵ带 ��ǰȭ�ϰ� ���� �� ����ϴ� ����

// [���� ������] ������ �޼����̸�(�������� �Ű�����, �������� �Ű�����, ...)
// {
//      �����ڵ�;
// }

// ���� ������(= ���� ������) : ���� �� private
// public : ����ü(struct)�� Ŭ���� ���ο� �ܺο��� ��� �����ؼ� ����� �� �ִ� �Ӽ�
// private : �ش� ����ü�� Ŭ���� ���ο����� ����� �� �ִ� �Ӽ�
// protected : �ܺο��� ���� �Ұ���, �ڽŰ� ��Ӱ����� �ڽ� Ŭ���������� ���� ���

// �������� �Ű������� ���� �޼����� ����
// 1. �������� �Ű������� ��� �����ϴ� ��� 
// 2. return���� ����, �Ű������� �ִ� ����
// 3. return���� ����, �Ű������� ���� ����
// 4. return���� �ְ�, �Ű������� ���� ����
public class Test_1 : MonoBehaviour
{
    // �޼����� ����
    // 1. �������� �Ű������� ��� �����ϴ� ���

    public int Sum(int a, int b)
    {
        int c = a + b;
        return c;
    }
    public int MyMethod(int a, int b)
    {
        int c = a + b;
        return c;
    }

    // 2. return���� ����, �Ű������� �ִ� ����
    public void Hamsu(int Kor, int Eng, int Math)
    { //void : ���ϰ��� ���ٴ� �ǹ� (���ϰ��� �������� �ʾƵ� �ȴ�)
        int a_Hap = Kor + Eng + Math;
        Debug.Log("���� : " + a_Hap);

        if (a_Hap < 20)
            return;

        Debug.Log("��� : " + (a_Hap / 3.0f).ToString("F2"));

    }

    // 3. return ���� ����, �Ű������� ���� ����
    public void PrintGuGuDan()
    {
        int a_Dan = 7;
        for (int i = 1; i < 10; i++)
            Debug.Log(a_Dan + " * " + i + " = " + a_Dan * i);
    }

    //4. return ���� �ְ�, �Ű������� ���� ����
    private float m_Velocity = 10.0f;  //private �Ӽ��̱� ������ �ܺο��� �� ���� �Ұ���

    public bool IsMove()               //��� �Լ��� ���������� �����Ͽ� ���¸� Ȯ���� �� �־�
    {
        if (m_Velocity <= 0)
            return false;

        return true;
    }
    

    void Start()
    {//����Ƽ�� �̺�Ʈ �Լ� : �츮�� ȣ���� ���� �ʾƵ� ����Ƽ�� �ڵ�ȣ���� �ִ� �Լ�
        // ����Ƽ �ý����� Ư�� ��Ȳ�� �°� ȣ���� �ִ� �Լ� : �ݹ��Լ�

        int AAA = MyMethod(1, 2);  // �޼����� ȣ��(�޼����� ���)

        int BBB = MyMethod(255, 255);

        int CCC = MyMethod(66, 99);

        int DDD = MyMethod(AAA, BBB);

        // �Ű�����, parameter, ����(�μ�)
        // ���� : �Ű�����, �Լ��� ���� �κп� �ִ� ����
        // �μ� : �Է��ϴ� ��, �ѱ�� ��

        Sum(11, 25); //�������� �ִ� �Լ����� ���ϰ��� �޾Ƽ� ������� �ʾƵ� ������ ����.
                     //���� �������� �ִ� �Լ��� �޾Ƽ� ����� ����� ��

        Hamsu(99, 10, 80);

        Hamsu(10, 5, 3);

        PrintGuGuDan();  //������ �۾��� ���� ������ ����ϰ� �� �� �Լ��� ��Ȱ�뼺�� Ȯ�ΰ���




    }

    void Update()
    {
        
    }
}
