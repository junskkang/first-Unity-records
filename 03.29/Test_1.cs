using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���׸� (Generic)   C++���ø�
//���׸��� �����Ϸ��� �μ��� �����Ͽ� ������ �߿� Ŭ������ �Լ��� ������ ��Ʋ�̴�.
//���� ������ �ϴ� �Լ��� �����ε��� ���������� ���� �ϳ��ϳ� �������ϳ�... 

public class MyGeneric<T>
{
    T a_ii;
    public MyGeneric(T a_in)      //������ �����ε� �Լ�
    {
        a_ii = a_in;
    }

    public T Get()
    {
        return a_ii;
    }
}
public class Student
{
    public string m_Name;
    public int m_Kor;
    public int m_Eng;
    public int m_Math;
    public int m_Total;
    public float m_Avg;

    public Student(string a_Name = "", int a_Kor = 0, int a_Eng = 0, int a_Math = 0)
    {
        m_Name = a_Name;
        m_Kor = a_Kor;
        m_Eng = a_Eng;
        m_Math = a_Math;
        m_Total = m_Kor + m_Eng + m_Math;
        m_Avg = m_Total / 3.0f;
    }

    public string GetInfoStr()
    {
        return $"{m_Name} : ����({m_Kor}) ����({m_Eng}) ����({m_Math}) ����({m_Total}) ���({m_Avg:N2})";
    }
}

public class Test_1 : MonoBehaviour
{
    void MySwap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }
    //void MySwap(ref int a, ref int b)
    //{
    //    int temp = a;
    //    a = b;
    //    b = temp;
    //}
    //void MySwap(ref float a, ref float b)     //���� ������ ���������� �ٸ� �����ε�
    //{
    //    float temp = a;
    //    a = b;
    //    b = temp;
    //}

    //void MySwap(ref double a, ref double b)     //���� ������ ���������� �ٸ� �����ε�
    //{
    //    double temp = a;
    //    a = b;
    //    b = temp;
    //}
    // Start is called before the first frame update
    void Start()
    {
        int a = 111;
        int b = 999;

        //int temp = a;       //���Թ������ ���� ���ҹ��
        //b = a;
        //b = temp;

        MySwap(ref a, ref b);
        //MySwap<int>(ref a, ref b);

        Debug.Log($"a({a}), b({b})");    //a999 b111

        double c = 10.1, d = 20.2;
        MySwap(ref c, ref d);
        Debug.Log($"c({c}), d({d})");    //c 20.2 d 10.1

        float e = 7.31f, f = 3.14f;
        MySwap(ref e, ref f);
        Debug.Log($"e({e}), f({f})");    //e 3.14 f 7.31

        MyGeneric<int> AAA = new MyGeneric<int>(10);
        Debug.Log(AAA.Get());

        MyGeneric<string> BBB = new MyGeneric<string>("GDragon");
        Debug.Log(BBB.Get());

        List<MyGeneric<int>> CCC = new List<MyGeneric<int>>();
        Dictionary<string, MyGeneric<int>> DDD = new Dictionary<string, MyGeneric<int>>();

        Student EEE = new Student("�߿���", 50, 30, 12);
        MyGeneric<Student> FFF = new MyGeneric<Student>(EEE);
        Debug.Log(FFF.Get().m_Name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
