using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//클래스 상속
//부모 클래스의 맴버변수나 맴버함수를 자식 클래스가 상속받아서 사용하겠다는 개념
//코드의 재활용을 위해 사용된다.
//공통 사항은 한 번 만들고 기반 클래스를 가지고 파생클래스를 확장해서 만들어 사용하자는 개념
//공통 기능을 상속을 통해 부모에서 구현
//각자마다의 고유기능은 각자 구현
//다형성에서도 사용됨(미래의 추가 기능을 대응하기 위한 구조) to be continued

//<상속 특성에 따른 분류>
//1. 확장상속 : 생물 -> 동물, 생물 -> 식물
//2. 다차상속 : 생물 -> 동물 -> 포유류 -> 사람
//3. 다중상속 : 핸드폰, 카메라 -> 카메라폰
//              프린터, 팩스, 스캐너 -> 복합기
//              C#에서 다중상속이 가능한 것은 인터페이스 클래스만 가능
//              Interface class는 부모 클래스를 가질 수 없음 = 상속관계가 꼬일 일이 없음


public class Parent //클래스 앞에 public을 빼게 되면 다른 클래스에서 public 멤버 변수로 사용할 수 없다.
{
    public int num;      // 맴버 변수
    public int m_Age;
    protected int m_Height;    //자식들까지는 사용 허용
    
    public Parent()   //생성자 함수
    {
        m_Age = 10;
        m_Height = 130;
        Debug.Log("부모 클래스의 생성자가 호출되었습니다.");
    }


    public void PrintInfo()        //맴버 함수
    {
        Debug.Log($"num : {this.num}");

    }
}

public class Child : Parent     // Parent클래스를 상속받는 Child 클래스 설계
{
    new public int num;   //부모 클래스의 같은 변수명인 Parent.num을 숨기려면 new키워드를 앞에 붙이면 된다.
    public int m_Kg;

    public Child(int a_Num)    //생성자 오버로딩 함수
    {
        this.num = a_Num;        //this. 자기 클래스 내에 있는 변수나 함수를 지명 접근
        base.num = a_Num * 100;  //base. 부모 클래스 내에 있는 변수나 함수를 지명 접근
        Debug.Log("자식 클래스의 생성자가 호출되었습니다.");
    }

    public void DisplayValue()
    {
        Debug.Log($"부모의 num : {base.num}, 자식의 num : {this.num}");
        Debug.Log($"나이 : {m_Age}, 키 : {m_Height}, 몸무게 : {m_Kg}");

        PrintInfo();
    }
}

public class Test_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Parent pt = new Parent();
        pt.num = 80;
        pt.PrintInfo();

        Child cd = new Child(20);
        cd.DisplayValue();
        Debug.Log(cd.m_Age);
        //Debug.Log(cd.m_Height);
        cd.num = 77;
        cd.DisplayValue() ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
