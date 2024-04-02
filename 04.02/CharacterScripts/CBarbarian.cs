using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barbarian_Stat : Chr_Stat
{
    public int m_Speed;   //이속
    public int m_Def;     //방어력

    public Barbarian_Stat(string a_CrName = "", int a_Speed = 1, int a_Def = 5)   //생성자 오버로딩 함수
    {
        m_CharType = CharicType.Barbarian;
        m_StrJob = "바바리안";
        m_Name = a_CrName;
        m_RscFile = "Images/BbrRenderer";  //해당 리소스를 사용하겠다는 의미
        m_MaxHp = 200;
        m_MaxMana = 50;
        m_Attack = 50;
        m_Speed = a_Speed;
        m_Def = a_Def;
    }

    //캐릭터의 스탯 정보를 기반으로 지형에 돌아다니는 캐릭터 객체 추가하려고 할 때
    //Hero게임 오브젝트에 빙의 시키길 원하는 클래스 추가 함수
    public override CUnit MyAddComponent(GameObject a_ParentGObj)
    {
        //매개변수로 받은 Hero GameObject에 CWizard 컴포넌트 붙여주고
        CUnit a_RefHero = a_ParentGObj.AddComponent<CBarbarian>();
        a_RefHero.m_ChrStat = this;  //지금 이 Barbarian_Stat객체를 CUnit쪽 m_ChrStat 변수에 대입해준다.
        //this == GlobalValue.g_CurSelCStat
        return a_RefHero;
    }
}
public class CBarbarian : CUnit
{
    //1.Awake()
    //2.MyAddComponent() : a_RefHero.m_ChrShat = this;
    //3.Start()
    void Awake()   //Override 안하면 이것만 호출됨.
    {

    }

    protected override void Start()
    {
        base.Start();  //부모쪽 Start() 호출 (공통 등장 준비)

        if (m_RefGameMgr != null)
        {
            m_RefGameMgr.LogMsg($"{m_ChrStat.m_Name} : 바바리안 고유 등장 준비");
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        //바바리안의 고유 AI 행동 패턴 코드
    }

    public override void Attack()
    {
        if (m_RefGameMgr != null)
        {
            m_RefGameMgr.LogMsg($"바바리안 공격 : 이름({m_ChrStat.m_Name}), Hp({m_CurHp}), 공격력({m_CurAtt})");
        }
    }

    public override void UseSkill()
    {
        if (m_RefGameMgr != null)
            m_RefGameMgr.LogMsg($"바바리안 스킬 : 이름({m_ChrStat.m_Name}), " +
                                $"이속({((Barbarian_Stat)m_ChrStat).m_Speed}), " +
                                $"방어력({((Barbarian_Stat)m_ChrStat).m_Def})");
    }
}
