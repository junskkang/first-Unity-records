using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;


// C# Method
// 용도 : 코드를 재활용하기 위한 용도로 사용이 되고, 
// 절차지향적 프로그래밍 기법에서 부품(기능)을 만드는 용도로도 사용된다.
// 즉, 메서드란 코드를 재활용하거나 코드를 부품화하고 싶을 때 사용하는 문법

// [접근 수식자] 리턴형 메서드이름(데이터형 매개변수, 데이터형 매개변수, ...)
// {
//      실행코드;
// }

// 접근 수식자(= 접근 지정자) : 생략 시 private
// public : 구조체(struct)나 클래스 내부와 외부에서 모두 접근해서 사용할 수 있는 속성
// private : 해당 구조체나 클래스 내부에서만 사용할 수 있는 속성
// protected : 외부에서 접근 불가능, 자신과 상속관계의 자식 클래스까지는 접근 허용

// 리턴형과 매개변수에 따른 메서드의 종류
// 1. 리턴형과 매개변수가 모두 존재하는 경우 
// 2. return형이 없고, 매개변수가 있는 형태
// 3. return형이 없고, 매개변수가 없는 형태
// 4. return형이 있고, 매개변수가 없는 형태
public class Test_1 : MonoBehaviour
{
    // 메서드의 종류
    // 1. 리턴형과 매개변수가 모두 존재하는 경우

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

    // 2. return형이 없고, 매개변수가 있는 형태
    public void Hamsu(int Kor, int Eng, int Math)
    { //void : 리턴값이 없다는 의미 (리턴값을 돌려주지 않아도 된다)
        int a_Hap = Kor + Eng + Math;
        Debug.Log("총점 : " + a_Hap);

        if (a_Hap < 20)
            return;

        Debug.Log("평균 : " + (a_Hap / 3.0f).ToString("F2"));

    }

    // 3. return 형이 없고, 매개변수도 없는 형태
    public void PrintGuGuDan()
    {
        int a_Dan = 7;
        for (int i = 1; i < 10; i++)
            Debug.Log(a_Dan + " * " + i + " = " + a_Dan * i);
    }

    //4. return 형이 있고, 매개변수가 없는 형태
    private float m_Velocity = 10.0f;  //private 속성이기 때문에 외부에서 값 변경 불가능

    public bool IsMove()               //대신 함수로 간접적으로 접근하여 상태를 확인할 수 있어
    {
        if (m_Velocity <= 0)
            return false;

        return true;
    }
    

    void Start()
    {//유니티의 이벤트 함수 : 우리가 호출해 주지 않아도 유니티가 자동호출해 주는 함수
        // 유니티 시스템이 특정 상황에 맞게 호출해 주는 함수 : 콜백함수

        int AAA = MyMethod(1, 2);  // 메서드의 호출(메서드의 사용)

        int BBB = MyMethod(255, 255);

        int CCC = MyMethod(66, 99);

        int DDD = MyMethod(AAA, BBB);

        // 매개변수, parameter, 인자(인수)
        // 인자 : 매개변수, 함수의 정의 부분에 있는 변수
        // 인수 : 입력하는 값, 넘기는 값

        Sum(11, 25); //리턴형이 있는 함수여도 리턴값을 받아서 사용하지 않아도 에러는 없다.
                     //보통 리턴형이 있는 함수는 받아서 사용을 해줘야 해

        Hamsu(99, 10, 80);

        Hamsu(10, 5, 3);

        PrintGuGuDan();  //동일한 작업을 여러 곳에서 사용하게 될 때 함수의 재활용성을 확인가능




    }

    void Update()
    {
        
    }
}
