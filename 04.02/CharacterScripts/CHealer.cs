using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer_Stat : Chr_Stat
{
    public int m_HealPower;   //힐량
    public int m_Shield;      //보호막


    public Healer_Stat(string a_CrName = "", int a_HealPower = 30, int a_shield = 10)   //생성자 오버로딩 함수
    {
        m_CharType = CharicType.Healer;
        m_StrJob = "힐러";
        m_Name = a_CrName;
        m_RscFile = "Images/HealRenderer";  //해당 리소스를 사용하겠다는 의미
        m_MaxHp = 50;
        m_MaxMana = 200;
        m_Attack = 10;
        m_HealPower = a_HealPower;
        m_Shield = a_shield;
    }

    //캐릭터의 스탯 정보를 기반으로 지형에 돌아다니는 캐릭터 객체 추가하려고 할 때
    //Hero게임 오브젝트에 빙의 시키길 원하는 클래스 추가 함수
    public override CUnit MyAddComponent(GameObject a_ParentGObj)
    {
        //매개변수로 받은 Hero GameObject에 CWizard 컴포넌트 붙여주고
        CUnit a_RefHero = a_ParentGObj.AddComponent<CHealer>();
        a_RefHero.m_ChrStat = this;  //지금 이 Barbarian_Stat객체를 CUnit쪽 m_ChrStat 변수에 대입해준다.
        //this == GlobalValue.g_CurSelCStat
        return a_RefHero;
    }
}
public class CHealer : CUnit
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
            m_RefGameMgr.LogMsg($"{m_ChrStat.m_Name} : 힐러 고유 등장 준비");
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
            m_RefGameMgr.LogMsg($"힐러 공격 : 이름({m_ChrStat.m_Name}), Hp({m_CurHp}), 공격력({m_CurAtt})");
        }
    }

    public override void UseSkill()
    {
        if (m_RefGameMgr != null)
            m_RefGameMgr.LogMsg($"힐러 스킬 : 이름({m_ChrStat.m_Name}), " +
                                $"치유량({((Healer_Stat)m_ChrStat).m_HealPower}), " +
                                $"쉴드량({((Healer_Stat)m_ChrStat).m_Shield})");
    }
}
