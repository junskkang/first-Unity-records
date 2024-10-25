using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Ally_Atrribute //�Ӽ�
{
    public AllyType type;
    public string name = "";
    public int level = 0;

    public float maxHp = 0;
    public float maxMp = 0;
    public float attack = 0;

    //ĳ������ ���� �������� ������� ������ ���ƴٴϴ� ĳ���� ��ü�� �߰��Ϸ��� �� ��
    //Hero ���� ������Ʈ�� ���� ��Ű�� ���ϴ� Ŭ���� �߰� �Լ�
    public virtual AllyUnit MyAddComponent(GameObject parentObject)
    {
        AllyUnit refAlly = null;
        return refAlly;
    }  
}

public class AllyUnit : MonoBehaviour
{
    [HideInInspector] public Ally_Atrribute ally_Attribute = null;

    [HideInInspector] public float m_CurHp = 0;     //���� �߿� ���ϴ� Hp
    [HideInInspector] public float m_CurMana = 0;   //���� �߿� ���ϴ� Mana
    [HideInInspector] public float m_CurAtt = 0;    //���� �߿� ���ϴ� ���ݷ�

    [HideInInspector] public GameManager m_RefGameMgr = null;  //InGameMgr�� ������ ���� ��ü
    // Start is called before the first frame update
    protected virtual void Start()
    {
        //ĳ���� ���� ���� �غ�
        m_RefGameMgr = FindObjectOfType<GameManager>(); //InGameManager�� ������ ���� ��ü�� ã�� ����

        //ĳ���� ����� ȣ���Լ� (���� �𵨸� �ε� ���ҽ� ����, ĳ���� ���� ����Ʈ �޸�Ǯ �غ�)
        //��ӹ޴� �� ���� ���� �ڵ�(���� ��ġ ��)

        //���ҽ� �ε� >> ���������� ����
        //GameObject a_ChrSprite = Resources.Load(Ally_Atrribute.m_RscFile) as GameObject;
        //if (a_ChrSprite != null)
        //{
        //    GameObject a_Clone = Instantiate(a_ChrSprite); //���� �� Hero GameObject�� ���� ��������Ʈ ����
        //    a_Clone.transform.SetParent(this.transform, false); //Hero �ؿ� child�� ��������Ʈ ���̱�
        //}
        //����ġ ���� ������ ����
        m_CurHp = ally_Attribute.maxHp;
        m_CurMana = ally_Attribute.maxMp;
        m_CurAtt = ally_Attribute.attack;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
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
