using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 메서드의 디폴트 매개변수 (C언어 : 함수의 디폴트 인수)
// 메서드의 매개변수는 기본값을 가질 수 있다.
// 메서드의 호출 시 값의 전달이 없으면, 정해진 기본 값을 사용한다.
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

    // 디폴트 매개변수 규칙
    // 뒤에 있는 매개변수부터 값을 부여할 수 있고, 중간을 건너띄고 부여할 수 없다.

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
        Person("민호");
        Person("민호", 23);
        Person("민호", 23, "서울시");

        //C# 4.0부터 지원하는 명명된 매개변수
        Person("승호", address: "서울특별시", age : 30);     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
