using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barbarian_Stat : Chr_Stat
{
    public int m_Speed;   //�̼�
    public int m_Def;     //����

    public Barbarian_Stat(string a_CrName = "", int a_Speed = 1, int a_Def = 5)   //������ �����ε� �Լ�
    {
        m_CharType = CharicType.Barbarian;
        m_StrJob = "�ٹٸ���";
        m_Name = a_CrName;
        m_RscFile = "Images/BbrRenderer";  //�ش� ���ҽ��� ����ϰڴٴ� �ǹ�
        m_MaxHp = 200;
        m_MaxMana = 50;
        m_Attack = 50;
        m_Speed = a_Speed;
        m_Def = a_Def;
    }

    //ĳ������ ���� ������ ������� ������ ���ƴٴϴ� ĳ���� ��ü �߰��Ϸ��� �� ��
    //Hero���� ������Ʈ�� ���� ��Ű�� ���ϴ� Ŭ���� �߰� �Լ�
    public override CUnit MyAddComponent(GameObject a_ParentGObj)
    {
        //�Ű������� ���� Hero GameObject�� CWizard ������Ʈ �ٿ��ְ�
        CUnit a_RefHero = a_ParentGObj.AddComponent<CBarbarian>();
        a_RefHero.m_ChrStat = this;  //���� �� Barbarian_Stat��ü�� CUnit�� m_ChrStat ������ �������ش�.
        //this == GlobalValue.g_CurSelCStat
        return a_RefHero;
    }
}
public class CBarbarian : CUnit
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
            m_RefGameMgr.LogMsg($"{m_ChrStat.m_Name} : �ٹٸ��� ���� ���� �غ�");
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
            m_RefGameMgr.LogMsg($"�ٹٸ��� ���� : �̸�({m_ChrStat.m_Name}), Hp({m_CurHp}), ���ݷ�({m_CurAtt})");
        }
    }

    public override void UseSkill()
    {
        if (m_RefGameMgr != null)
            m_RefGameMgr.LogMsg($"�ٹٸ��� ��ų : �̸�({m_ChrStat.m_Name}), " +
                                $"�̼�({((Barbarian_Stat)m_ChrStat).m_Speed}), " +
                                $"����({((Barbarian_Stat)m_ChrStat).m_Def})");
    }
}
