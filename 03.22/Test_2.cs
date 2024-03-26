using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyItem
{
    public string m_Name = "";
    public int m_Level = 0;
    public float m_AttRate = 1.0f;
    public int m_Price = 100;

    public MyItem(string a_Name = "", int a_Level = 0, float a_Ar = 0.0f, int a_Price = 0)
    {//생성자 오버로딩 함수
        m_Name = a_Name;
        m_Level = a_Level;  
        m_AttRate = a_Ar;
        m_Price = a_Price;
    }

    public void PrintInfo()
    {
        Debug.Log($"이름({m_Name}) : 레벨({m_Level}) : 공격상승률({m_AttRate}) : 가격({m_Price})");
    }
}

public class Test_2 : MonoBehaviour
{
    List<MyItem> m_ItList = new List<MyItem>();   //선언하면 처음엔 아무런 공간이 없음 Count = 0

    int PriceASC(MyItem a, MyItem b)             //가격 오름차순 정렬
    {
        return a.m_Price.CompareTo(b.m_Price);
    }

    int LevelDESC(MyItem a, MyItem b)            //레벨 내림차순 정렬
    {
        return b.m_Level.CompareTo(a.m_Level);
    }

    // Start is called before the first frame update
    void Start()
    {
        //int AAA;      //일반 변수의 선언
        //AAA = 10;     //일반 변수의 초기화
        //int BBB = 10; //일반 변수의 선언과 동시에 초기화


        ////클래스 1번 초기화 방법 : 객체를 선언 후 초기화
        //MyItem a_Node = new MyItem();
        //a_Node.m_Name = "천사의 반지";
        //a_Node.m_Level = 3;
        //a_Node.m_AttRate = 2.0f;
        //a_Node.m_Price = 2500;

        ////클래스 2번 초기화 방법 : 객체 선언과 동시에 초기화(생성자 오버로딩 함수)
        //MyItem a_Node2 = new MyItem("천사의반지", 3, 2.0f, 2500);

        ////클래스 3번 초기화 방법 : 객체 선언과 동시에 선택적으로 변수 초기화
        //MyItem a_Node3 = new MyItem { m_Name = "천사의 반지", m_Price = 2500 };
        //a_Node3.PrintInfo();

        
        
        //리스트 노드 추가
        MyItem a_Node = new MyItem("천사의 반지", 3, 2.0f, 2500);
        m_ItList.Add(a_Node);

        a_Node = new MyItem("팔라독의 검", 1, 1.2f, 1200);
        m_ItList.Add(a_Node);

        a_Node = new MyItem("궁수의 활", 4, 1.7f, 1500);
        m_ItList.Add(a_Node);

        a_Node = new MyItem("거북이의 갑옷", 5, 0.5f, 3000);
        m_ItList.Add(a_Node);

        //for문을 통해 소스 확인
        for(int ii = 0;  ii < m_ItList.Count; ii++)
            m_ItList[ii].PrintInfo();

        Debug.Log("---------------------");
        ////노드 삭제
        ////m_ItList.RemoveAt(0);      // m_ItList[0] 노드 삭제   
        //Debug.Log("첫번째 노드 삭제 결과");
        //foreach (MyItem a_It in m_ItList)
        //    a_It.PrintInfo();

        ////마지막노드 삭제
        //if (0 < m_ItList.Count)
        //{
        //    m_ItList.RemoveAt(m_ItList.Count - 1);
        //    Debug.Log("마지막 노드 삭제 결과");
        //    foreach (MyItem a_It in m_ItList)
        //        a_It.PrintInfo();
        //}

        ////노드의 내용을 찾아 삭제하는 방법
        //MyItem a_FNode = m_ItList[1];
        //if(m_ItList.Contains(a_FNode) == true) //a_FNode 인스턴스가 리스트에 존재하는지 확인
        //{
        //    m_ItList.Remove(a_FNode);
        //    //Remove함수는 객체가 리스트에 존재하지 않은 상태에서 제거 시도해도 에러가 나지 않음
        //}

        //foreach (MyItem a_It in m_ItList)
        //    a_It.PrintInfo();

        ////중간 중간에 조건에 만족하는 경우만 선택적으로 삭제하기
        //for(int ii = 0;ii < m_ItList.Count;)
        //{
        //    if (m_ItList[ii].m_Price < 2000)
        //    {
        //        m_ItList.RemoveAt(ii);
        //    }
        //    else
        //        ii++;
        //}

        //for (int ii = 0; ii < m_ItList.Count; ii++)
        //    m_ItList[ii].PrintInfo();

        //중간값 추가하기
        a_Node = new MyItem("상어의 이빨", 4, 1.2f, 12000);
        m_ItList.Insert(1, a_Node);
        Debug.Log("1번 인덱스에 중간값 추가 결과 확인");


        foreach (MyItem a_It in m_ItList)
            a_It.PrintInfo();


        //m_ItList.Add(new MyItem("상어의 이빨", 4, 1.2f, 12000));
        //m_ItList.Insert(1, new MyItem("상어의 이빨", 4, 1.2f, 12000));

        //정렬
        //정렬시에 보통 아이템 리스트의 복사본을 만들어서 걔를 정렬하여 화면으로 보여주는편
       
        //가격이 낮은순에서 높은순으로 정렬
        List<MyItem> a_CopyList = m_ItList.ToList(); //리스트 복사 using System.Linq; 추가 필요

        Debug.Log("가격 오름차순 정렬");
        a_CopyList.Sort(PriceASC);

        foreach (MyItem a_It in a_CopyList)
            a_It.PrintInfo();
        
        //레벨이 높은순에서 낮은순으로 정렬
        List<MyItem> a_CloneList = m_ItList.ToList(); //리스트 복사 using System.Linq; 추가 필요
   
        Debug.Log("레벨 내림차순 정렬");
        a_CloneList.Sort(LevelDESC);
        
        foreach (MyItem a_It in a_CloneList)
            a_It.PrintInfo();

        Debug.Log("추가된 순으로 보기");
        foreach (MyItem a_It in m_ItList)
            a_It.PrintInfo();
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
