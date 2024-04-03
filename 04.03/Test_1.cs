using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//추상 클래스와 인터페이스 클래스

//추상 클래스 : 상속 받는 차일드 클래스 쪽에서 가상함수의 구현을 의무화 하는 클래스

//abstract class Weapon  //순수 가상함수가 하나라도 존재하는 클래스를 추상클래스라 한다.
//{
//    //public virtual void Attack()  //일반 가상함수는 차일드 쪽에서 오버라이딩을 안해도 된다.
//    //{

//    //}

//    public abstract void Attack();  //순수 가상함수
//    //public, protected는 가능, private는 불가능
//}
public class Item
{
    public int m_Level = 1;
    public int m_Star = 3;
    public int m_Grade = 4;

    public void ShowInfo()
    {
        Debug.Log($"아이템레벨({m_Level}) 아이템성급({m_Star}) 아이템등급({m_Grade})");
    }

}

public abstract class Inven
{

}

interface Weapon
{
    //private int price = 1000;   //인터페이스 클래스는 일반변수를 만들 수 없다.

    int Price { get; set; }     //프로퍼티로 만드는 것은 가능하다

    void Attack(); //접근수식자 생략하면 기본이 public속성
}

interface Armor
{
    void Defense();
}

class Knife : Item, Weapon, Armor
{
    int m_Price = 1000;
    public int Price   //프로퍼티를 의무적으로 만들어줘야 한다.
    {
        get { return m_Price; }
        set { m_Price = value; }  //value : set 접근자 사용시 암묵적 매개변수
    }
    
    public int m_Power = 10;
    

    public void PrintInfo()
    {
        Debug.Log(m_Power);
    }

    public void Attack()  //추상 클래스를 상속받은 클래스는 반드시 오버라이딩을 해야 한다.
    {
        Debug.Log("칼로 공격");
    }

    public void Defense()
    {

    }

}

public class Test_1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Weapon a_Wp = new Weapon();  //추상함수를 가지고 있는 클래스는 인스턴스를 만들 수 없다.

        Weapon a_Wp2 = new Knife();  //업캐스팅을 이용한 사용은 가능!

        Knife a_Knife = new Knife();
        a_Knife.PrintInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
