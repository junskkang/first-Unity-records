using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTeam;
using System.Net.Mail;

public class Item         //기존에 이미 존재하는 클래스
{
    public string m_Nick;
    public int m_Star;
    public float m_AttRate;
}

namespace MyClass
{
    public class Item     //내가 새로 만들고 싶은 새로운 클래스
    {
        public string m_Name;
        public int m_Level;
        public int m_Price;
    }
}
public class Test_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if KOREA
        Debug.Log("이것은 한글 버전입니다.");
#elif ENGLISH
        Debug.Log("이것은 영어 버전입니다.");
#elif CHINA
        Debug.Log("이것은 중국어 버전입니다.");
#endif
        Item AAA = new Item();
        AAA.m_Nick = "팔라독의 검";
        AAA.m_Star = 5;
        AAA.m_AttRate = 1.0f;
        Debug.Log(AAA.m_Nick + " : " + AAA.m_Star + " : " + AAA.m_AttRate);

        MyClass.Item BBB = new MyClass.Item();
        BBB.m_Name = "드래곤의 방패";
        BBB.m_Level = 4;
        BBB.m_Price = 1000;
        Debug.Log(BBB.m_Name + " : " + BBB.m_Level + " : " + BBB.m_Price);

        MyTeam.Monster CCC = new MyTeam.Monster();
        CCC.m_Name = "늑대";
        CCC.m_Hp = 100;
        CCC.m_Mp = 100;
        CCC.m_Attack = 20;
        CCC.PrintInfo();

        MyTeam.Monster DDD = new MyTeam.Monster();
        DDD.m_Name = "오크";
        DDD.m_Hp = 200;
        DDD.m_Mp = 50;
        DDD.m_Attack = 50;
        DDD.PrintInfo();


        // alt + ctrl clickEEE
        Monster EEE = new Monster();
        EEE.m_Name = "좀비";
        EEE.m_Hp = 1000;
        EEE.m_Mp = 0;
        EEE.m_Attack = 10;
        EEE.PrintInfo();

    }   // Update is called once per frame
    void Update()
    {
        
    }
}
