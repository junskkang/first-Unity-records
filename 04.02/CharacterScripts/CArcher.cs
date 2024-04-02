using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Stat : Chr_Stat
{
    public int m_AttackRange;   //�����Ÿ�


    public Archer_Stat(string a_CrName = "", int a_AttackRange = 0)   //������ �����ε� �Լ�
    {
        m_CharType = CharicType.Archer;
        m_StrJob = "��ó";
        m_Name = a_CrName;
        m_RscFile = "Images/ArcRenderer";  //�ش� ���ҽ��� ����ϰڴٴ� �ǹ�
        m_MaxHp = 70;
        m_MaxMana = 80;
        m_Attack = 80;
        m_AttackRange = a_AttackRange;
    }

    //ĳ������ ���� ������ ������� ������ ���ƴٴϴ� ĳ���� ��ü �߰��Ϸ��� �� ��
    //Hero���� ������Ʈ�� ���� ��Ű�� ���ϴ� Ŭ���� �߰� �Լ�
    public override CUnit MyAddComponent(GameObject a_ParentGObj)
    {
        //�Ű������� ���� Hero GameObject�� CWizard ������Ʈ �ٿ��ְ�
        CUnit a_RefHero = a_ParentGObj.AddComponent<CArcher>();
        a_RefHero.m_ChrStat = this;  //���� �� Barbarian_Stat��ü�� CUnit�� m_ChrStat ������ �������ش�.
        //this == GlobalValue.g_CurSelCStat
        return a_RefHero;
    }
}
public class CArcher : CUnit
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
            m_RefGameMgr.LogMsg($"{m_ChrStat.m_Name} : ��ó ���� ���� �غ�");
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        //��ó�� ���� AI �ൿ ���� �ڵ�
    }

    public override void Attack()
    {
        if (m_RefGameMgr != null)
        {
            m_RefGameMgr.LogMsg($"��ó ���� : �̸�({m_ChrStat.m_Name}), Hp({m_CurHp}), ���ݷ�({m_CurAtt})");
        }
    }

    public override void UseSkill()
    {
        if (m_RefGameMgr != null)
            m_RefGameMgr.LogMsg($"��ó ��ų : �̸�({m_ChrStat.m_Name}), " +
                                $"�̼�({((Archer_Stat)m_ChrStat).m_AttackRange})");
    }
}
