using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


//구조체 value Type과 클래스 Reference Type : 메서드의 매개변수로 사용될 때의 특징



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
    public float AttackRate
    {
        get => m_AttackRate;
        set => m_AttackRate = value;
    }


    public float GetAttackRate()  //변수에 간접접근
    {
        float Temp = m_AttackRate;

        //복호화

        return Temp;
    }

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
        m_AttackRate = a_Item.GetAttackRate();
    }
}
public class Test_1 : MonoBehaviour
{
    public void Value(int aa)           //매개변수를 일반변수로
    {
        aa = 1000;
    }

    public void Reference(int[] aa)     //매개변수를 배열로
    {
        aa[0] = 1000;
    }

    public void MonsterMethod(Monster a_Mon)  //구조체를 매개변수로 받는 함수
    {
        a_Mon.m_Name = "벨류구조체";
        a_Mon.m_Level = 999;
        a_Mon.m_Hp = 999;
        a_Mon.m_Mp = 999;
        a_Mon.m_Attack = 999.9f;
    }

    public void ItemMethod(Item a_It)         //클래스를 매개변수로 받는 함수 
    {
        a_It.m_Name = "레퍼런스클래스";
        a_It.m_Level = 999;
        a_It.m_Star = 999;
        a_It.m_Price = 999;
    }

    // Start is called before the first frame update
    void Start()
    {
        int xx = 0;
        Value(xx);
        Debug.Log("xx : " + xx);    // 0

        int[] yy = { 0 };
        Reference(yy);
        Debug.Log("yy : " + yy[0]);    // 1000

        //구조체
        Monster AAA;
        AAA.m_Name = "드래곤";
        AAA.m_Level = 0;
        AAA.m_Hp = 0;
        AAA.m_Mp = 0;   
        AAA.m_Attack = 0.0f;
        MonsterMethod(AAA);
        AAA.PrintInfo();          //드래곤

        //클래스
        Item BBB = new Item();
        BBB.m_Name = "천사의 반지";
        BBB.m_Level = 0;
        BBB.m_Star = 0;
        BBB.m_Price = 0;
        ItemMethod(BBB);
        BBB.PrintInfo();          //레퍼런스클래스
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
