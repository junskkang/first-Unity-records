using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class의 다형성
//상황에 따라 역할이 달라지는 것
//아래의 예시에서는 부모 virtual 함수가 자식의 함수에서 override를 통해 어떻게 역할이 바뀌는지 볼 것
//예시 : 롤에서 q,w,e,r 스킬을 부모의 virtual함수로 틀만 잡아놓고
//       실제 하위 클래스인 각자의 캐릭터에서 override하여 q,w,e,r의 고유 스킬능력을 구현하는 것

//나중에 캐릭터를 추가할 때 게임의 로직과 UI는 건들필요 없이
//캐릭터 클래스만 추가하고 리스트에 추가해주면 끝

public enum CharicType
{
    Wizard = 0,
    Barbarian,
    Archer,
    Count
}

public class CUnit
{
    public CharicType m_CharType = CharicType.Count;    //캐릭터 타입
    public string m_StrJob = "";     //직업이름 한글 저장
    public string m_Name = "";       //이름

    public int m_CurHp = 0;
    public int m_CurMana = 0;
    public int m_CurAtt = 0;   

    public CUnit(string a_CrName = "")     //생성자 오버로딩 함수
    {
        m_Name = a_CrName;
    }

    public virtual void OnUpdate()
    {
        //캐릭터 공통 행동 패턴 
    }

    public virtual string Attack()  //메인로직의 위치만 잡아놓고 구체적인 구현은 상속받는 곳으로 넘기는 것
    {
        //캐릭터 공통 공격 연출 구현
        return "";
    }
}

public class CWizard : CUnit
{
    public int m_MagicRange;    //위자드만의 고유변수 

    public CWizard(string a_CrName = "", int a_MgRange = 0) : base(a_CrName) 
    {
        m_CharType = CharicType.Wizard;
        m_StrJob = "마법사";
        //m_Name = a_CrName;

        m_CurHp = 100;
        m_CurMana = 100;
        m_CurAtt = 10;

        m_MagicRange = a_MgRange;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();  //부모쪽 함수 호출
    }

    public override string Attack()
    {
        string a_str = $"마법사 공격 : 이름 ({m_Name}), Hp({m_CurHp}), 마법범위({m_MagicRange})";
        return a_str;
    }
}

public class CBarbarian : CUnit
{
    public int m_Speed;     //바바리안만의 고유 변수
    public int m_Def;
    public CBarbarian(string a_CrName = "", int a_Speed = 1, int a_Def = 5)
    {
        m_CharType= CharicType.Barbarian;
        m_StrJob = "바바리안";
        m_Name = a_CrName;

        m_CurHp = 100;
        m_CurMana = 100;
        m_CurAtt = 10;

        m_Speed = a_Speed;
        m_Def = a_Def;

    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override string Attack()
    {
        string a_str = $"바바리안 공격 : 이름 ({m_Name}), 공격력({m_CurAtt}), 방어력({m_Def}), 속도({m_Speed})";
        return a_str;
    }
}

public class CArcher : CUnit
{
    public int m_AttackLength;      //사거리

    public CArcher(string a_CrName = "", int a_AttackLength = 50)
    {
        m_CharType = CharicType.Archer;
        m_StrJob = "아처";
        m_Name = a_CrName;

        m_CurHp = 50;
        m_CurMana = 70;
        m_CurAtt = 10;

        m_AttackLength = a_AttackLength;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
    }

    public override string Attack()
    {
        string a_str = $"아처 공격 : 이름 ({m_Name}), 공격력({m_CurAtt}), 사정거리({m_AttackLength})";
        return a_str;
    }
}

public class Test_1 : MonoBehaviour
{
    CharicType m_CurSelCT = CharicType.Count;  //현재 선택하고 있는 캐릭터 인덱스 null은 선택된 캐릭터가 없다는 뜻
    List<CUnit> g_MyChrList = new List<CUnit>();  //내가 보유하고 있는 캐릭터 리스트

    // Start is called before the first frame update
    void Start()
    {
        //내 인벤토리에 내가 보유하고 있는 캐릭터 목록 로딩 및 추가
        //생성해둔 리스트의 데이터형은 CUnit이지만 그 하위 클래스인 애들을 업캐스팅하여 리스트에 추가할 수 있음
        CWizard a_CrNode = new CWizard("간달프", 70);
        g_MyChrList.Add(a_CrNode);

        CBarbarian a_CrNodeB = new CBarbarian("바이킹", 1, 3);
        g_MyChrList.Add(a_CrNodeB);

        CArcher a_CrNodeC = new CArcher("실바나스", 10);
        g_MyChrList.Add(a_CrNodeC);

        //변수 재활용
        //CUnit a_Node = null;
        //a_Node = new CWizard("간달프", 70);
        //g_MyChrList.Add(a_CrNode);

        //a_Node = new CBarbarian("바이킹", 1, 3);
        //g_MyChrList.Add(a_CrNodeB);

        //a_Node = new CArcher("실바나스", 10);
        //g_MyChrList.Add(a_CrNodeC);
    }

    string m_AttStr = "";
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) == true)    //캐릭터 교체
        {
            m_CurSelCT++;   // 처음 한번은 3이 된다.
            if (CharicType.Count <= m_CurSelCT)
            {
                m_CurSelCT = CharicType.Wizard;
            }
            m_AttStr = "";
        }

        if (Input.GetKeyDown(KeyCode.A) == true) 
        {
            if (m_CurSelCT < CharicType.Count)
                m_AttStr = g_MyChrList[(int)m_CurSelCT].Attack();
        }

    }

    private void OnGUI()
    {
        
        string a_CrStr = "캐릭터 선택 없음";
        if(m_CurSelCT < CharicType.Count)
        {
            a_CrStr = $"{g_MyChrList[(int)m_CurSelCT].m_StrJob} 이름({g_MyChrList[(int)m_CurSelCT].m_Name}) 선택";
            
        }
        GUI.Label(new Rect(255, 35, 1000, 100), "<color=#00ff00><size=32>" + a_CrStr + "</size></color>");

        GUI.Label(new Rect(255, 110, 1000, 100), "<color=#ffff00><size=32>" + m_AttStr + "</size></color>");

    }
}
