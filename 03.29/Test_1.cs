using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//제네릭 (Generic)   C++템플릿
//제네릭은 컴파일러가 인수를 적용하여 컴파일 중에 클래스나 함수를 만들어내는 형틀이다.
//같은 동작을 하는 함수의 오버로딩을 데이터형에 따라 하나하나 만들어야하나... 

public class MyGeneric<T>
{
    T a_ii;
    public MyGeneric(T a_in)      //생성자 오버로딩 함수
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
        return $"{m_Name} : 국어({m_Kor}) 영어({m_Eng}) 수학({m_Math}) 총점({m_Total}) 평균({m_Avg:N2})";
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
    //void MySwap(ref float a, ref float b)     //같은 동작의 데이터형이 다른 오버로딩
    //{
    //    float temp = a;
    //    a = b;
    //    b = temp;
    //}

    //void MySwap(ref double a, ref double b)     //같은 동작의 데이터형이 다른 오버로딩
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

        //int temp = a;       //대입방법에서 쓰는 스왑방법
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

        Student EEE = new Student("야옹이", 50, 30, 12);
        MyGeneric<Student> FFF = new MyGeneric<Student>(EEE);
        Debug.Log(FFF.Get().m_Name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
