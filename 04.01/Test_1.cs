using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class�� ������
//��Ȳ�� ���� ������ �޶����� ��
//�Ʒ��� ���ÿ����� �θ� virtual �Լ��� �ڽ��� �Լ����� override�� ���� ��� ������ �ٲ���� �� ��
//���� : �ѿ��� q,w,e,r ��ų�� �θ��� virtual�Լ��� Ʋ�� ��Ƴ���
//       ���� ���� Ŭ������ ������ ĳ���Ϳ��� override�Ͽ� q,w,e,r�� ���� ��ų�ɷ��� �����ϴ� ��

//���߿� ĳ���͸� �߰��� �� ������ ������ UI�� �ǵ��ʿ� ����
//ĳ���� Ŭ������ �߰��ϰ� ����Ʈ�� �߰����ָ� ��

public enum CharicType
{
    Wizard = 0,
    Barbarian,
    Archer,
    Count
}

public class CUnit
{
    public CharicType m_CharType = CharicType.Count;    //ĳ���� Ÿ��
    public string m_StrJob = "";     //�����̸� �ѱ� ����
    public string m_Name = "";       //�̸�

    public int m_CurHp = 0;
    public int m_CurMana = 0;
    public int m_CurAtt = 0;   

    public CUnit(string a_CrName = "")     //������ �����ε� �Լ�
    {
        m_Name = a_CrName;
    }

    public virtual void OnUpdate()
    {
        //ĳ���� ���� �ൿ ���� 
    }

    public virtual string Attack()  //���η����� ��ġ�� ��Ƴ��� ��ü���� ������ ��ӹ޴� ������ �ѱ�� ��
    {
        //ĳ���� ���� ���� ���� ����
        return "";
    }
}

public class CWizard : CUnit
{
    public int m_MagicRange;    //���ڵ常�� �������� 

    public CWizard(string a_CrName = "", int a_MgRange = 0) : base(a_CrName) 
    {
        m_CharType = CharicType.Wizard;
        m_StrJob = "������";
        //m_Name = a_CrName;

        m_CurHp = 100;
        m_CurMana = 100;
        m_CurAtt = 10;

        m_MagicRange = a_MgRange;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();  //�θ��� �Լ� ȣ��
    }

    public override string Attack()
    {
        string a_str = $"������ ���� : �̸� ({m_Name}), Hp({m_CurHp}), ��������({m_MagicRange})";
        return a_str;
    }
}

public class CBarbarian : CUnit
{
    public int m_Speed;     //�ٹٸ��ȸ��� ���� ����
    public int m_Def;
    public CBarbarian(string a_CrName = "", int a_Speed = 1, int a_Def = 5)
    {
        m_CharType= CharicType.Barbarian;
        m_StrJob = "�ٹٸ���";
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
        string a_str = $"�ٹٸ��� ���� : �̸� ({m_Name}), ���ݷ�({m_CurAtt}), ����({m_Def}), �ӵ�({m_Speed})";
        return a_str;
    }
}

public class CArcher : CUnit
{
    public int m_AttackLength;      //��Ÿ�

    public CArcher(string a_CrName = "", int a_AttackLength = 50)
    {
        m_CharType = CharicType.Archer;
        m_StrJob = "��ó";
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
        string a_str = $"��ó ���� : �̸� ({m_Name}), ���ݷ�({m_CurAtt}), �����Ÿ�({m_AttackLength})";
        return a_str;
    }
}

public class Test_1 : MonoBehaviour
{
    CharicType m_CurSelCT = CharicType.Count;  //���� �����ϰ� �ִ� ĳ���� �ε��� null�� ���õ� ĳ���Ͱ� ���ٴ� ��
    List<CUnit> g_MyChrList = new List<CUnit>();  //���� �����ϰ� �ִ� ĳ���� ����Ʈ

    // Start is called before the first frame update
    void Start()
    {
        //�� �κ��丮�� ���� �����ϰ� �ִ� ĳ���� ��� �ε� �� �߰�
        //�����ص� ����Ʈ�� ���������� CUnit������ �� ���� Ŭ������ �ֵ��� ��ĳ�����Ͽ� ����Ʈ�� �߰��� �� ����
        CWizard a_CrNode = new CWizard("������", 70);
        g_MyChrList.Add(a_CrNode);

        CBarbarian a_CrNodeB = new CBarbarian("����ŷ", 1, 3);
        g_MyChrList.Add(a_CrNodeB);

        CArcher a_CrNodeC = new CArcher("�ǹٳ���", 10);
        g_MyChrList.Add(a_CrNodeC);

        //���� ��Ȱ��
        //CUnit a_Node = null;
        //a_Node = new CWizard("������", 70);
        //g_MyChrList.Add(a_CrNode);

        //a_Node = new CBarbarian("����ŷ", 1, 3);
        //g_MyChrList.Add(a_CrNodeB);

        //a_Node = new CArcher("�ǹٳ���", 10);
        //g_MyChrList.Add(a_CrNodeC);
    }

    string m_AttStr = "";
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) == true)    //ĳ���� ��ü
        {
            m_CurSelCT++;   // ó�� �ѹ��� 3�� �ȴ�.
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
        
        string a_CrStr = "ĳ���� ���� ����";
        if(m_CurSelCT < CharicType.Count)
        {
            a_CrStr = $"{g_MyChrList[(int)m_CurSelCT].m_StrJob} �̸�({g_MyChrList[(int)m_CurSelCT].m_Name}) ����";
            
        }
        GUI.Label(new Rect(255, 35, 1000, 100), "<color=#00ff00><size=32>" + a_CrStr + "</size></color>");

        GUI.Label(new Rect(255, 110, 1000, 100), "<color=#ffff00><size=32>" + m_AttStr + "</size></color>");

    }
}
