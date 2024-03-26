using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

//클래스 소속의 static(정적) 변수, 정적메서드
//특징
//1. 객체 생성없이 클래스이름.변수명, 클래스이름.메서드이름();
//2. 프로그램이 시작할 때 메모리가 확보되고 프로그램이 종료될 때까지 유지된다.
//3. 클래스 소속이지만 메모리를 클래스와 별도로 생성하고 공유된다.

public class Hero
{
    public string m_Name;    //일반 멤버 변수(인스턴스 멤버 변수) 
    public int m_Hp;         //인스턴스가 있어야지만 사용할 수 있는 변수라는 의미

    public static int s_UserPoint = 0;  //정적 멤버 변수 (클래스 멤버 변수)

    public void AddUserPoint(int a_Point)   //일반 멤버 메서드
    {
        s_UserPoint += a_Point;
    }

    public int GetUserPoint()               //일반 멤버 메서드
    {
        return s_UserPoint;
    }

    public static void StaticPrint()    //정적 멤버 메서드 (클래스 멤버 메서드)
    {
       //m_Name = "";                  //클래스 멤버 메서드에서 일반 멤버변수는 안돼!

        int a_ABC = 100;                //지역 변수
        s_UserPoint = 1234;             //정적 멤버 변수 사용 바로 가능
        Debug.Log(s_UserPoint);
    }

}
public class Test_1 : MonoBehaviour
{

    void Start()
    {
#if KOREA
        Debug.Log("이것은 한글 버전입니다.");
#elif ENGLISH
        Debug.Log("이것은 영어 버전입니다.");
#elif CHINA
        Debug.Log("이것은 중국어 버전입니다.");
#endif
        Hero.s_UserPoint = 100;    // 정적 멤버 변수의 사용
        Hero.StaticPrint();        // 정적 멤버 메서드의 사용

        //유니티에서 제공해주는 static method 예시
        Debug.Log("");
        Random.Range(0, 100);
        Vector3 a_Vec = Vector3.zero;
        


        // 3:3 턴 방식 RPG일 때 캐릭터 3명을 선택해서 게임에 들어간 상태
        Hero hunter = new Hero();
        hunter.m_Name = "사냥꾼";
        hunter.m_Hp = 123;
        Debug.Log(hunter.m_Name + " : " + hunter.m_Hp);
        //hunter.s_UserPoint = 100; //static 변수는 객체로 접근하는게 아니어서 에러남
        hunter.AddUserPoint(100);

        Hero Warrior = new Hero();
        Warrior.m_Name = "전사";
        Warrior.m_Hp = 200;
        Debug.Log(Warrior.m_Name + " : " + Warrior.m_Hp);
        

        Hero healer = new Hero();
        healer.m_Name = "힐러";
        healer.m_Hp = 80;
        Debug.Log(healer.m_Name + " : " + healer.m_Hp);
        healer.AddUserPoint(30);
        

        Debug.Log(Hero.s_UserPoint + " : " + hunter.GetUserPoint() + Warrior.GetUserPoint() + healer.GetUserPoint());

    }


    void Update()
    {
        
    }
}
