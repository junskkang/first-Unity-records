using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer_Stat : Chr_Stat
{
    public int m_HealPower;   //����
    public int m_Shield;      //��ȣ��


    public Healer_Stat(string a_CrName = "", int a_HealPower = 30, int a_shield = 10)   //������ �����ε� �Լ�
    {
        m_CharType = CharicType.Healer;
        m_StrJob = "����";
        m_Name = a_CrName;
        m_RscFile = "Images/HealRenderer";  //�ش� ���ҽ��� ����ϰڴٴ� �ǹ�
        m_MaxHp = 50;
        m_MaxMana = 200;
        m_Attack = 10;
        m_HealPower = a_HealPower;
        m_Shield = a_shield;
    }

    //ĳ������ ���� ������ ������� ������ ���ƴٴϴ� ĳ���� ��ü �߰��Ϸ��� �� ��
    //Hero���� ������Ʈ�� ���� ��Ű�� ���ϴ� Ŭ���� �߰� �Լ�
    public override CUnit MyAddComponent(GameObject a_ParentGObj)
    {
        //�Ű������� ���� Hero GameObject�� CWizard ������Ʈ �ٿ��ְ�
        CUnit a_RefHero = a_ParentGObj.AddComponent<CHealer>();
        a_RefHero.m_ChrStat = this;  //���� �� Barbarian_Stat��ü�� CUnit�� m_ChrStat ������ �������ش�.
        //this == GlobalValue.g_CurSelCStat
        return a_RefHero;
    }
}
public class CHealer : CUnit
{
    //1.Awake()
    //2.MyAddComponent() : a_RefHero.m_ChrShat = this;
    //3.Start()
    void Awake()   //Override ���ϸ� �̰͸� ȣ���.
    {

    }

    protected override void Start()
    {
        base.Start();  //�θ��� Start() ȣ�� (���� ���� �غ�)

        if (m_RefGameMgr != null)
        {
            m_RefGameMgr.LogMsg($"{m_ChrStat.m_Name} : ���� ���� ���� �غ�");
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        //�ٹٸ����� ���� AI �ൿ ���� �ڵ�
    }

    public override void Attack()
    {
        if (m_RefGameMgr != null)
        {
            m_RefGameMgr.LogMsg($"���� ���� : �̸�({m_ChrStat.m_Name}), Hp({m_CurHp}), ���ݷ�({m_CurAtt})");
        }
    }

    public override void UseSkill()
    {
        if (m_RefGameMgr != null)
            m_RefGameMgr.LogMsg($"���� ��ų : �̸�({m_ChrStat.m_Name}), " +
                                $"ġ����({((Healer_Stat)m_ChrStat).m_HealPower}), " +
                                $"���差({((Healer_Stat)m_ChrStat).m_Shield})");
    }
}
