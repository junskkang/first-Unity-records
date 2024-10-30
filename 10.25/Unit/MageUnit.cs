using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Mage_Att : Ally_Atrribute
{
    //������ ���� �Ӽ� �߰�
    public GameObject[] attackEffs = new GameObject[4];
    public GameObject[] skillEffs = new GameObject[2];

    public Mage_Att()    //������ �����ε� �Լ�
    {
        //�Ʊ� ���� �Ӽ� �⺻�� �ο�
        type = AllyType.Mage;
        name = "������";
        level = 1;
        maxHp = 100;
        maxMp = 20;

        attackDamage = 10;
        attackRange = 2.0f;
        attackSpeed = 2.0f;
        attackCool = 0.0f;

        attackCount = 0;
        skillPossible = 5;
        anyHit = false;
        skillRange = 3.0f;
        skillDamage = 20.0f;
        skillHitLimit = 3;
        

        attackEff = Resources.Load($"{type}AttackEff") as GameObject;
        skillEff = Resources.Load($"{type}SkillEff") as GameObject;
        //������ ���� �Ӽ� �⺻�� �ο�
        for (int i = 0; i < attackEffs.Length; i++)
        {
            attackEffs[i] = Resources.Load($"{type}AttackEff{i}") as GameObject;
        }    
        for (int i = 0; i < skillEffs.Length; i++)
        {
            skillEffs[i] = Resources.Load($"{type}SkillEff{i}") as GameObject;
        }
    }

    //Ally���� ������Ʈ�� ���ǽ�ų AllyŬ������ �߰����ִ� �Լ�
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //�Ű������� ���� �θ� ������Ʈ�� ������ ������Ʈ�� �ٿ���
        AllyUnit a_RefAlly = parentObject.AddComponent<MageUnit>();
        //�� Mage_Att ��ü�� AllyUnit ���� ally_Attribute ������ �������ش�
        a_RefAlly.ally_Attribute = this;

        return a_RefAlly;
    }
}

public class MageUnit : AllyUnit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();  //�θ��� Start() ȣ�� (���� ���� �غ�)
        Debug.Log(((Mage_Att)ally_Attribute).attackEffs.Length);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //������ ���� �ൿ ����
    }

    public override void Attack()
    {
        //������ ���� ���� ����
        int ran = Random.Range(0, 4);
    }

    public override void Skill()
    {
        //������ ���� ��ų ����
    }
}
