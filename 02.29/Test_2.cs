using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 //< C# 변수 적용범위 분류>
 //1.지역변수
 //2.class 소속 맴버 변수 : 클래스 소속의 변수를 맴버변수라고 한다.
 //3. 클래스 소속 정적맴버변수 : 생명주기가 프로그램 시작해서 끝날 때까지의 변수

 //<c, c++ 변수의 적용범위 분류>
 //1. 지역변수
 //2. 클래스 소속 맴버변수
 //3. 클래스 소속 정적맴버변수
 //4. 전역변수
 //5. 지역 정적변수

// 지역변수의 특징 : 자기 지역 소속 내에서만 사용 가능한 변수 ex) {}
// 1. 자기 소속의 지역을 벗어나서는 사용할 수 없다.
// 2. 변수 선언 전에는 사용 할 수 없다.
// 3. 자기 소속의 하위 중괄호(지역) 안에서는 사용할 수 있다.
// 4. 같은 지역 내에서는 같은 이름의 변수를 선언할 수 없다.
// 5. 소속이 다르면 같은 이름의 지역변수를 만들 수 있다.
public class Test_2 : MonoBehaviour
{    // 짝지어진 괄호를 찾아가는 단축키 < Ctrl + ] >

    public static int s_CDE = 0; 
    //클래스 소속의 정적맴버변수 static을 붙여서 사용


    public int m_ABC = 0; // 클래스 소속(중괄호)의 변수를 클래스 소속 맴버변수라고 함.
    public int m_zzz = 0;

    //클래스 소속의 맴버변수는 선언 앞에 public만 붙여주면!
    //소속을 벗어나서도 해당 변수를 사용할 수 있음
    
    void Start()
    {
        Debug.Log(m_ABC);
        {
            int m_ABC = 123; // 지역변수는 맴버변수와 같은 이름으로 선언하여 사용할 수 있다.
            m_ABC = 5324; // 여기서는 m_ABC는 가까운 쪽이 우선된다는 규칙에 의해 지역변수를 가리킴
            Debug.Log(m_ABC);
            Debug.Log(this.m_ABC); //명시적으로 맴버변수에 접근하는 방법
            //여기서 this의 의미는 Test_2 Class 자기자신을 의미한다.
        }



        //a_AAA = 123; // 변수를 선언하기 전에는 해당 변수를 사용할 수 없음

        int a_AAA = 0; // Start 함수 내의 지역 변수의 선언
        a_AAA = 777; // Start 함수 범위 내에서 값을 초기화하는 것도 정상
        //Debug.Log(a_AAA); // Start 함수 범위 내에서 값을 확인하는 것도 정상

        {
            int a_BBB = 5; // 지역변수 a_BBB의 선언 및 초기화
            a_BBB = 10; // 지역변수 a_BBB의 초기화
            a_AAA = a_BBB; // 자기 소속의 "하위" 중괄호 안에서는 사용할 수 있다.
            //Debug.Log(a_AAA);
        }

        //a_BBB = 77; // 자기 소속 지역을 벗어나서는 사용할 수 없다.

        {
            int a_CCC = 10;
            //int a_CCC = 20; // 같은 지역 내에서는 같은 이름의 변수를 선언할 수 없다.
        }
    } //void Start()

    //a_AAA=123; 
    // 자기 지역을 벗어나서 사용할 수 없음
    // 클래스의 중괄호 안에서는 코드를 작성할 수 없음

    void Update()
    {
        //a_AAA = 888; //Start 함수 내에서만 유효한 지역변수이기 때문에 오류남
        
    } //void Update()
}//public class Test_2 : MonoBehaviour
