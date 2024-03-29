using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//클래스 오버라이딩 (낫 오버로드) : virtual, override 키워드
// 상속 관계에 있는 부모쪽 함수명을 자식 클래스에 같은 함수명으로 생성해서 재정의하여 사용하는 것
//<오버라이딩을 하기 위한 조건>
//1. 함수의 이름이 같아야 한다.
//2. 매개변수가 같아야 한다.
//3. 반환타입이 같아야 한다.
//4. 부모클래스에 virtual 키워드가 필요하다.


//업캐스팅과 다운캐스팅 : (=형변환)
//class Animal {}
//class Dog : Animal {}

//Dog dog = new Dog();
//Animal animal = dog;     // 업캐스팅. 자식의 객체를 부모의 객체에 대입할 수 있음

//Animal a_Animal2 = new Animal();//다운캐스팅. 부모의 객체를 자식의 객체에 대입할 수 있음
//Dog a_Dog2 = (Dog)a_Animal2;  // 얘는 에러
//Dog a_Down = (Dog)animal;    //업캐스팅 되어있던 객체만 다시 다운캐스팅 가능하다.

class MyParent
{
    public int m_ii;

    public MyParent()
    {
        Debug.Log("MyParent의 생성자 함수 호출");

    }

    public void Method()
    {
        Debug.Log("MyParent의 Method() 호출");
    }
    public virtual void MyPrint()
    {
        Debug.Log("부모 클래스의 MyPrint()메서드 호출");
    }
}

class MyBoy : MyParent
{
    public MyBoy()
    {
        m_ii = 10;
    }

    public new void Method()
    {//new 키워드를 사용하여 함수의 오버라이딩을 대신할 수 있다.
        Debug.Log("MyBoy의 Method() 호출");
    }

    public override void MyPrint()
    {
        Debug.Log("MyBoy 클래스의 MyPrint()메서드 호출");
    }

}

class MyDaughter : MyParent
{
    public MyDaughter()
    {

    }

    public override void MyPrint()
    {
        Debug.Log("MyDaughter 클래스의 MyPrint()메서드 호출");
    }

}

public class Test_3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MyParent a_pp = new MyParent();  //"부모 클래스의 MyPrint()메서드 호출"
        a_pp.MyPrint();

        MyBoy xy = new MyBoy(); //"MyBoy 클래스의 MyPrint()메서드 호출"
        xy.MyPrint();

        MyDaughter xx = new MyDaughter(); //"MyDaughter 클래스의 MyPrint()메서드 호출"
        xx.MyPrint();

        MyParent a_yy = xy;           //업캐스팅. 자식을 부모객체에 대입
        a_yy.MyPrint();       //"MyBoy 클래스의 MyPrint()메서드 호출"

        a_pp.Method();       //부모 클래스꺼 함수 호출

        xy.Method();         // 아들 클래스꺼 함수 호출

        a_yy.Method();       //(업캐스팅된 애) 부모 클래스꺼 함수 호출

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
