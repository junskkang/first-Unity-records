using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//델리게이트 함수 Delegate (대리자 함수) c, c++ 함수포인터

public class DLT_Class
{
    public delegate void DLT_StrType(string c);   // 델리게이트 데이터형 선언
    static DLT_StrType DltStrMtd;                 // 델리게이트 변수 선언(소켓)

    public static void AddListener(DLT_StrType a_DltMtd)
    {
        DltStrMtd = a_DltMtd;
    }

    public static void PrintTest(string a_Str)
    {
        if(DltStrMtd != null) 
            DltStrMtd(a_Str); 
    }
}

public class Test_2 : MonoBehaviour
{
    delegate int DLT_Type(int x);    //델리게이트 데이터형 선언
    DLT_Type DltMethod;              //델리게이트 변수 선언

    Button m_TempBtn;

    
    int Hamsu2X(int a)
    {
        a = a * 2;
        return a;
    }

    int Hamsu3X(int a)
    {
        a = a * 3;
        return a;
    }

    int Hamsu4X(int a)
    {
        a = a * 4;
        return a;
    }

    void Skill_1(string a_Name)
    {
        Debug.Log(a_Name + "님이 스턴스킬을 사용하셨습니다.");
    }
    
    void Skill_2(string a_Name)
    {
        Debug.Log(a_Name + "님이 흡혈스킬을 사용");
    }

    void Skill_3(string a_Name)
    {
        Debug.Log(a_Name + "님이 도끼 휘두르기 스킬을 사용");
    }

    List<int> ABC = new List<int>();
    int MyComp(int a, int b)
    {
        return a.CompareTo(b);
    }

    // Start is called before the first frame update
    void Start()
    {
        DLT_Class.AddListener(Skill_1);

        m_TempBtn.onClick.AddListener(TempClick);   //유니티에서 제공해주는 Delegate 함수

        ABC.Sort(MyComp);                           //유니티에서 제공해주는 Delegate 함수
    }

    void TempClick()
    {

    }


    int m_Index = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (m_Index == 0)
                DltMethod = Hamsu2X;
            else if (m_Index == 1)
                DltMethod = Hamsu3X;
            else if (m_Index ==2)
                DltMethod = Hamsu4X;
            
            m_Index++;
            if (3 <= m_Index)
                m_Index = 0;
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            if(DltMethod != null)
                Debug.Log(DltMethod(11));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            DLT_Class.PrintTest("SBS전사");
        }

    }
}
