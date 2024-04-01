using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharicType
{
    Wizard,
    Barbarian,
    Archer,
    Count
}


public class Chr_Stat      //Stat, Atrribute ����������
{
    public CharicType m_CharType;
    public string m_StrJob = "";
    public string m_Name = "";
    public string m_RscFile = "";          //���ҽ� ������ �̸�
    public int m_MaxHp = 0;
    public int m_MaxMana = 0;
    public int m_Attack = 0;

    //ĳ������ ���� �������� ������� ������ ���ƴٴϴ� ĳ���� ��ü�� �߰��Ϸ��� �� ��
    //Hero ���� ������Ʈ�� ���� ��Ű�� ���ϴ� Ŭ���� �߰� �Լ�
    public virtual CUnit MyAddComponent(GameObject a_Hero)
    {
        CUnit a_RefHero = null;
        return a_RefHero;
    }
}

public class CUnit : MonoBehaviour
{
    [HideInInspector] public Chr_Stat m_ChrStat = null;

    [HideInInspector] public int m_CurHp = 0;     //���� �߿� ���ϴ� Hp
    [HideInInspector] public int m_CurMana = 0;   //���� �߿� ���ϴ� Mana
    [HideInInspector] public int m_CurAtt = 0;    //���� �߿� ���ϴ� ���ݷ�

    [HideInInspector] public InGameManager m_RefGameMgr = null;  //InGameMgr�� ������ ���� ��ü

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //ĳ���� ���� ���� �غ�
        m_RefGameMgr = FindObjectOfType<InGameManager>(); //InGameManager�� ������ ���� ��ü�� ã�� ����

        //ĳ���� ����� ȣ���Լ� (���� �𵨸� �ε� ���ҽ� ����, ĳ���� ���� ����Ʈ �޸�Ǯ �غ�)
        //��ӹ޴� �� ���� ���� �ڵ�(���� ��ġ ��)
        
        //���ҽ� �ε�
        GameObject a_ChrSprite = Resources.Load(m_ChrStat.m_RscFile)as GameObject;
        if (a_ChrSprite != null)
        {
            GameObject a_Clone = Instantiate(a_ChrSprite); //���� �� Hero GameObject�� ���� ��������Ʈ ����
            a_Clone.transform.SetParent(this.transform, false); //Hero �ؿ� child�� ��������Ʈ ���̱�
        }
        //����ġ ���� ������ ����
        m_CurHp = m_ChrStat.m_MaxHp;
        m_CurMana = m_ChrStat.m_MaxMana;
        m_CurAtt = m_ChrStat.m_Attack;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //ĳ������ ���� �ൿ���� ����ó��
    }

    public virtual void Attack()
    {
        //ĳ������ ���� ���� ����ó��
    }

    public virtual void UseSkill()
    {
        //ĳ������ ���� ��ų ����ó��
    }
}
