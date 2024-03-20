using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//������ �Լ�, �Ҹ��� �Լ�
//1. �Լ��� �̸��� Ŭ���� �̸��� ������
//2. �������� ����(void �� ǥ��x)

//Ŭ���������� ������, �Ҹ��ڸ� ������ �� �ִ�.
//������, �Ҹ��ڸ� �����ؼ� �����ϰ� ��ü�� ����� �Ǹ�
//�ڵ����� ����Ʈ ������, ����Ʈ �Ҹ��ڰ� ��������� ȣ��ȴ�.
public class Student
{
    public string m_Name = "";                      //�ʱ�ȭ ���� 1��
    public int m_Jumsu = 0;
    public float m_Avg = 0.0f;

    public Student()            //������ �Լ� 
    {//�Լ��� �̸��� Ŭ���� �̸��� �����ϰ�, �������� ����
        
        //Ŭ���� ��ü�� ������ �� �ڵ����� �� �� ȣ��Ǵ� �޼���
        //�ַ� �ɹ� �������� �ʱ�ȭ ���ִ� �뵵�� ���
        Debug.Log("������ �Լ� ȣ��");
        m_Name = "�л�1";                           //�ʱ�ȭ ���� 2��
        m_Jumsu = 1;
        m_Avg = 1.0f;

    }

    public Student(string name)                        //������ �����ε� �޼���
    {
        m_Name = name;
    }

    public Student(string name, int jumsu, float avg)  //������ �����ε� �޼���
    {
        m_Name = name;
        m_Jumsu = jumsu;
        m_Avg = avg;
    }

    ~Student()                  //�Ҹ��� �Լ�
    {//���ټ�����, �������� ����
     //Ŭ������� �Լ��̸��� �����ϰ�
     //�� �տ� ����ǥ�ø� ����

        //Ŭ���� ��ü�� �޸𸮰� �Ҹ�� �� �� �� �ڵ� ȣ��Ǵ� �޼���
        
        Debug.Log("�Ҹ��� �Լ� ȣ��");
    }

    public void PrintInfo()
    {
        Debug.Log($"�̸� : {m_Name} ����: ({m_Jumsu}) ���({m_Avg})");
        
    }
}
public class Test_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Student AAA = new Student();
        AAA.PrintInfo();
        AAA.m_Name = "õ��";                       //�ʱ�ȭ ���� 3��
        AAA.m_Jumsu = 100;
        AAA.m_Avg = 1.0f;
        AAA.PrintInfo();

        int BBB;               //���� �� �ʱ�ȭ
        BBB = 111;
        Debug.Log(BBB);

        int CCC = 222;         //����� ���ÿ� �ʱ�ȭ
        Debug.Log(CCC);

        Student a_St = new Student();                    //���� �� �ʱ�ȭ
        a_St.m_Name = "�巡��";
        a_St.m_Jumsu = 87;
        a_St.m_Avg = 60.3f;

        Student a_Ss = new Student("����", 76, 92.1f);   //����� ���ÿ� �ʱ�ȭ
        a_Ss.PrintInfo();

        Vector3 vec = new Vector3(30.0f, 20.0f, 10.0f);
        Color col = new Color(255, 255, 255, 125);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
