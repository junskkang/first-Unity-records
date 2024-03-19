using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

//메서드의 오버로딩 (오버라이딩과 다른 문법!)
//하나의 메서드 이름으로 여러 개의 메서드를 구현 하는 것
//매개변수의 형식만 다르면 같은 메서드 이름을 사용할 수 있다
//기본용도는 하나의 함수 이름으로 다양한 데이터형 매개변수를 편하게 사용하기 위해서 사용되는 문법

public class Test_1 : MonoBehaviour
{
    int Plus(int a, int b)
    { 
        return a + b;
    }

    double Plus(double a, double b) //1. 메서드의 오버로딩의 예
    {
        return (a + b); 
    }

    int Plus(int a, int b, int c)  //2. 메서드의 오버로딩 예
    {
        return a+b+c;
    }

    double Plus(int a, double b)  //3. 메서드의 오버로딩의 예
    {
        return (a + b); 
    }

    // Start is called before the first frame update
    void Start()
    {
        int ret = Plus(45, 70);     // 첫번째 Plus()로 실행

        Debug.Log(Plus(1.8, 2.4));  // 두번째 Plus()로 실행
        Debug.Log(Plus(4, 7, 9));   // 세번째 Plus()로 실행
        Debug.Log(Plus(1, 2.4));    // 네번째 Plus()로 실행


        //유니티 C#에서 지원하는 메서드 오버로딩의 예
        int iRan = Random.Range(1, 7);            // 1~6
        float fRan = Random.Range(1.5f, 3.14f);   // 1.5~ 3.14
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
