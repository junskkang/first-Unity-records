using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//생성자 함수, 소멸자 함수
//1. 함수의 이름이 클래스 이름과 동일함
//2. 리턴형이 없음(void 등 표기x)

//클래스에서는 생성자, 소멸자를 생략할 수 있다.
//생성자, 소멸자를 생략해서 설계하고 객체를 만들게 되면
//자동으로 디폴트 생성자, 디폴트 소멸자가 만들어지고 호출된다.
public class Student
{
    public string m_Name = "";                      //초기화 순서 1번
    public int m_Jumsu = 0;
    public float m_Avg = 0.0f;

    public Student()            //생성자 함수 
    {//함수의 이름이 클래스 이름과 동일하고, 리턴형이 없음
        
        //클래스 객체를 생성할 때 자동으로 한 번 호출되는 메서드
        //주로 맴버 변수들을 초기화 해주는 용도로 사용
        Debug.Log("생성자 함수 호출");
        m_Name = "학생1";                           //초기화 순서 2번
        m_Jumsu = 1;
        m_Avg = 1.0f;

    }

    public Student(string name)                        //생성자 오버로딩 메서드
    {
        m_Name = name;
    }

    public Student(string name, int jumsu, float avg)  //생성자 오버로딩 메서드
    {
        m_Name = name;
        m_Jumsu = jumsu;
        m_Avg = avg;
    }

    ~Student()                  //소멸자 함수
    {//접근수식자, 리턴형이 없고
     //클래스명과 함수이름이 동일하고
     //그 앞에 물결표시를 붙임

        //클래스 객체의 메모리가 소멸될 때 한 번 자동 호출되는 메서드
        
        Debug.Log("소멸자 함수 호출");
    }

    public void PrintInfo()
    {
        Debug.Log($"이름 : {m_Name} 점수: ({m_Jumsu}) 평균({m_Avg})");
        
    }
}
public class Test_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Student AAA = new Student();
        AAA.PrintInfo();
        AAA.m_Name = "천사";                       //초기화 순서 3번
        AAA.m_Jumsu = 100;
        AAA.m_Avg = 1.0f;
        AAA.PrintInfo();

        int BBB;               //선언 후 초기화
        BBB = 111;
        Debug.Log(BBB);

        int CCC = 222;         //선언과 동시에 초기화
        Debug.Log(CCC);

        Student a_St = new Student();                    //선언 후 초기화
        a_St.m_Name = "드래곤";
        a_St.m_Jumsu = 87;
        a_St.m_Avg = 60.3f;

        Student a_Ss = new Student("힐러", 76, 92.1f);   //선언과 동시에 초기화
        a_Ss.PrintInfo();

        Vector3 vec = new Vector3(30.0f, 20.0f, 10.0f);
        Color col = new Color(255, 255, 255, 125);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
