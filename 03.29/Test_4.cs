using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//다차 상속

public class GrandParent
{
    public string m_UserName;
    public long m_UserId;
    
    //public GrandParent()
    //{
    //    Debug.Log("GrandParent 클래스의 생성자 함수 호출");
    //}

    public void CallPrint()
    {
        Debug.Log("GrandParent 클래스의 CallPrint() 함수 호출");
    }
    public virtual void MyPrint()
    {
        Debug.Log("GrandParent 클래스의 MyPrint()함수 호출");
    }
}


public class Parent2 : GrandParent
{
    public int m_UserAge;
    //public Parent2()
    //{
    //    Debug.Log("Parent2 클래스의 생성자 함수 호출");
    //}
    public new void CallPrint()
    {
        Debug.Log("Parent2 클래스의 CallPrint() 함수 호출");
    }
    public override void MyPrint()
    {
        Debug.Log("Parent2 클래스의 MyPrint()함수 호출");
    }
}

public class Child2 : Parent2
{
    public int m_Height;

    //public Child2()
    //{
    //    Debug.Log("Child2 클래스의 생성자 함수 호출");
    //}
    public new void CallPrint()
    {
        Debug.Log("Child2 클래스의 CallPrint() 함수 호출");
    }
    public override void MyPrint()
    {
        Debug.Log("Child2 클래스의 MyPrint()함수 호출");
    }
}

public class Test_4 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Child2 a_Cld = new Child2();
        GrandParent AAA = a_Cld;          //업캐스팅

        GrandParent BBB = new Child2();   //생성과 동시에 업캐스팅
        
        BBB.MyPrint();         //오버라이딩을 통해 Child2 함수 호출
        BBB.CallPrint();       //업캐스팅이지만 원래 클래스인 GrandParent 함수 호출
        ((Parent2)BBB).CallPrint(); //다운캐스팅을 통해 Parent2의 CallPrint 함수 호출
        ((Child2)(BBB)).CallPrint(); //다운캐스팅을 통해 Child2의 CallPrint 함수 호출


        GrandParent CCC = new Parent2();
        CCC.MyPrint();         //오버라이딩을 통해 Parent2 함수 호출
        CCC.CallPrint();       //업캐스팅이지만 원래 클래스인 GrandParent 함수 호출
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
