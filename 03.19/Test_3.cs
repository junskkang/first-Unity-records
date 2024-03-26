using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//구조체 (Struct) - 비교적 단순
//같은 목적을 갖는 여러 데이터형 변수들과 메서드들의 집합
//cf) 같은 데이터형을 묶어놓은 변수들의 집합이 '배열'
//구조체 객체는 Value Type (int, float, double..... , struct)
//용도 : 비교적 간략한 데이터 값을 저장하는 데에 사용
//구조체는 상속 될 수 없다.


//클래스 (class) - 보다 많은 기능
//같은 목적을 갖는 여러 데이터형 변수들과 메서드들의 집합
//클래스 객체는 Reference Type (array, class객체)
//용도 : 소프트웨어를 부품화(객체지향 프로그래밍)하기 위한 도구로 사용

public struct Monster             // 구조체 설계 : 몬스터 한마리의 정보
{
    public string m_Name;         // 구조체 맴버 변수
    public int m_Hp;              // C#에서는 구조체에서 public속성을 생략하면
    public int m_Mp;              // 기본 속성은 private 속성이 된다.
    public int m_Level; 
    public float m_Attack;

    public void PrintInfo()       //구조체 맴버 메서드 정의
    {
        string str = $"{m_Name} : 체력({m_Hp}) 마나({m_Mp}) " +
                     $"레벨({m_Level}) 공격력({m_Attack.ToString("N2")})";
        Debug.Log(str);
    }
}

//클래스

public class Item        //클래스 설계
{
    public string m_Name;     //아이템 이름
    public int m_Level;       //아이템 레벨
    public int m_Star;        //아이템 성급
    public int m_Price;       //아이템 가격
    float m_AttackRate = 1.0f;  //아이템 공격 상승률   //중요 변수라 값 변하면 안돼서 private
    //속성, property
    public float AttackRate { get => m_AttackRate; 
                              set => m_AttackRate = value; } 

    
    //public float GetAttackRate()  //변수에 간접접근
    //{
    //    float Temp = m_AttackRate;

    //    //복호화

    //    return Temp;
    //}

    public void SetAttackRate(float rate)  //변수에 간접접근
    {
        //암호화

        if (1.0f < rate)
            return;
        
        m_AttackRate = rate;
    }

    public void PrintInfo()
    {
        string str = $"{m_Name} : 레벨({m_Level}) 성급({m_Star}) " +
                     $"가격({m_Price}) 공격상승률({m_AttackRate.ToString("N2")})";
        Debug.Log(str);
    }

    public void CopyItem(Item a_Item)  //일반함수가 아니라 생성자 역할을 하는 함수 '복사생성자'
    {
        m_Name = a_Item.m_Name;
        m_Level = a_Item.m_Level;
        m_Star = a_Item.m_Star;
        m_Price = a_Item.m_Price;
    }
}

// 접근수식자, 접근지정자
// public : 구조체, 클래스 내부와 외부에서 모두 접근해서 사용할 수 있는 속성
// private : 구조체, 클래스 내부에서만 사용 가능하고 외부에서는 접근할 수 없는 속성
// protected : 외부에서 접근할 수 없고, 자신과 상속관계의 자식클래스까지만 접근을 허용하는 속성
public class Test_3 : MonoBehaviour
{

    void Start()
    {
        Monster AAA;          //구조체 변수AAA 선언
        AAA.m_Name = "오크";
        AAA.m_Hp = 100;
        AAA.m_Mp = 60;
        AAA.m_Level = 1;
        AAA.m_Attack = 2.5f;
        AAA.PrintInfo();

        Monster BBB;          //구조체 변수BBB 선언
        BBB.m_Name = "울프";
        BBB.m_Hp = 200;
        BBB.m_Mp = 80;
        BBB.m_Level = 5;
        BBB.m_Attack = 7.2f;
        BBB.PrintInfo();

        Monster CCC;          //구조체 변수CCC 선언
        CCC.m_Name = "슬라임";
        CCC.m_Hp = 110;
        CCC.m_Mp = 40;
        CCC.m_Level = 2;
        CCC.m_Attack = 0.5f;
        CCC.PrintInfo();

        Monster DDD;          //구조체 변수DDD 선언
        DDD.m_Name = "인간";
        DDD.m_Hp = 500;
        DDD.m_Mp = 10;
        DDD.m_Level = 10;
        DDD.m_Attack = 10.0f;
        DDD.PrintInfo();

        //구조체는 Value Type 
        Monster a_TestVal = AAA;   //아래와 같은 맴버 대 맴버의 1:1복사가 이루어짐(얕은복사)
        //a_TestVal.m_Name = AAA.m_Name;
        //a_TestVal.m_Hp = AAA.m_Hp; 
        //            :
        //            :
        a_TestVal.PrintInfo();     // AAA의 속성을 복사하여 오크로 출력
        a_TestVal.m_Name = "상어";
        a_TestVal.m_Hp = 999;
        a_TestVal.m_Mp = 999;
        a_TestVal.m_Level = 999;
        a_TestVal.m_Attack = 999;
        Debug.Log("--- 구조체 Value Type Test ---");
        AAA.PrintInfo();           // 원래대로 오크로 출력됨
        a_TestVal.PrintInfo();     // 새로 입력한 값인 상어가 출력됨
        //따라서, 구조체는 대입시 값만 복사해오는 것을 알 수 있다.


        //클래스 변수 선언, 객체(미리 씬에 배정되어있는 것들) 선언, 인스턴스(게임 중간에 스폰되는 것들) 선언
        Item a_MyItem = new Item();     //배열 때처럼 새로운 데이터공간을 빌려온다는 의미!
        a_MyItem.m_Name = "천사의 반지";
        a_MyItem.m_Level = 4;
        a_MyItem.m_Star = 2;
        a_MyItem.m_Price = 1200;
        //a_MyItem.m_AttackRate = 1.1f;   //private속성은 객체를 통해서 외부에서 접근 불가능!
        a_MyItem.SetAttackRate(1.3f);   //위처럼 직접접근이 안되기에 간접접근을 해야함
        //a_MyItem.PrintInfo();

        //클래스는 Reference Type Test
        Item a_TestRef = a_MyItem;
        a_TestRef.PrintInfo();              // 여기서는 a_MyItem과 동일하게 출력
        a_TestRef.m_Name = "드래곤의 반지"; // 값 수정
        a_TestRef.m_Level = 999;
        a_TestRef.m_Star = 10;
        a_TestRef.m_Price = 250000;
        Debug.Log("--- 클래스 Reference Type Test ---");
        a_MyItem.PrintInfo();               // a_MyItem도 드래곤의 반지
        a_TestRef.PrintInfo();              // a_TestRef도 드래곤의 반지

        //클래스 타입인데도 불구하고 참조타입이 아니라 별개의 객체를 만들고 싶은 경우
        Item a_XXX = new Item();
        a_XXX.m_Name = a_MyItem.m_Name;
        a_XXX.m_Level = a_MyItem.m_Level; 
        //            :
        //            :
        // 하나의 아이템을 new로 선언하고 그 각각의 속성을 일일이 하나하나 일대일 복사시켜주면 됨
        // 또는! 이러한 작업을 해주는 함수를 클래스 안에 맴버 함수로 만들어놓고 
        // 함수를 호출 시키는 방법도 있어! 
        Item a_YYY = new Item();
        a_YYY.CopyItem(a_MyItem);    
    }


    void Update()
    {
        
    }
}
