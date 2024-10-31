using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;


public class Ally_Atrribute //�Ӽ�
{
    public AllyType type;
    public string name = "";
    public int level = 0;

    public float maxHp = 0;
    public float maxMp = 0;
    public float attackDamage = 0;
    public float attackRange = 0;
    public float attackSpeed = 0;
    public float attackCool = 0;

    public int attackCount = 0;
    public int skillPossible = 0;
    public bool anyHit = false;
    public float skillRange = 0;
    public float skillDamage = 0;
    public int skillHitLimit = 0;

    public GameObject attackEff;
    public GameObject skillEff;
    
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

    [HideInInspector] public int curLevel = 0;

    [HideInInspector] public float maxHp = 0;     //���� �߿� ���ϴ� Hp
    public float curHp = 0;     //���� �߿� ���ϴ� Hp
    [HideInInspector] public float curMp = 0;   //���� �߿� ���ϴ� Mana
    [HideInInspector] public float curAttDamage = 0;    //���� �߿� ���ϴ� ���ݷ�
    [HideInInspector] public float curAttRange = 0;
    [HideInInspector] public float curAttSpeed = 0;
    [HideInInspector] public float curAttCool = 0;

    [HideInInspector] public int attackCount = 0;
    [HideInInspector] public int skillPossible = 0;
    [HideInInspector] public bool anyHit = false;
    [HideInInspector] public float skillRange = 0;
    [HideInInspector] public float skillDamage = 0;
    [HideInInspector] public int skillHitLimit = 0;
    public int monKill = 0;

    [HideInInspector] public bool isSkilled = false;

    [HideInInspector] public Vector3 cacDir = Vector3.zero;
    
    [HideInInspector] public GameManager m_RefGameMgr = null;  //InGameMgr�� ������ ���� ��ü

    public Canvas canvas = null;
    public Image hpBar = null;
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
        StatSetUp();

        //UI����
        UISetUp();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        curAttCool -= Time.deltaTime;
        if (curAttCool <= 0 && attackCount == skillPossible && !isSkilled)
        {
            isSkilled = true;
            Skill();
            curAttCool = curAttSpeed;
        }

        if (curAttCool <= 0 && !isSkilled)
        {
            Attack();
            curAttCool = curAttSpeed;

            curHp -= curAttDamage / 2;
        }

        UIUpdate();
    }

    public virtual void Attack()
    {
        //ĳ������ ���� ���� ����ó��
    }

    public virtual void Skill()
    {
        //ĳ������ ���� ��ų ����ó��
    }

    void StatSetUp()
    {
        curLevel = ally_Attribute.level;
        maxHp = ally_Attribute.maxHp;
        curHp = maxHp;
        curMp = ally_Attribute.maxMp;

        curAttDamage = ally_Attribute.attackDamage;
        curAttRange = ally_Attribute.attackRange;
        curAttSpeed = ally_Attribute.attackSpeed;
        curAttCool = curAttSpeed;

        attackCount = ally_Attribute.attackCount;
        skillPossible = ally_Attribute.skillPossible;
        anyHit = ally_Attribute.anyHit;
        skillRange = ally_Attribute.skillRange;
        skillDamage = ally_Attribute.skillDamage;
        skillHitLimit = ally_Attribute.skillHitLimit;

        Debug.Log("���� ���� �Ϸ�");
    }

    void UISetUp()
    {
        if (canvas == null)
            canvas = GetComponentInChildren<Canvas>();

        if (canvas != null)
            hpBar = canvas.transform.Find("Hpbar").GetComponent<Image>();       
    }

    void UIUpdate()
    {
        if (hpBar != null)
            hpBar.fillAmount = curHp / maxHp;
    }

    public void Levelup()
    {
        if (curLevel >= 10) return;
        curLevel++;

        curAttDamage += 1.0f;
        curAttRange += 0.1f;
        curAttSpeed -= 0.1f;

        skillRange += 0.2f;
        skillDamage += 2.0f;

        if (curLevel % 4 == 0)
        {
            skillPossible -= 1;
            skillHitLimit += 1;
        }
    }
}
