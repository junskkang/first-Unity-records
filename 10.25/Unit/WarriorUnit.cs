using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
public class Warrior_Att : Ally_Atrribute
{
    //������ ���� �Ӽ� �߰�   

    public Warrior_Att()    //������ �����ε� �Լ�
    {
        //�Ʊ� ���� �Ӽ� �⺻�� �ο�
        type = AllyType.Warrior;
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
        
    }

    //Ally���� ������Ʈ�� ���ǽ�ų AllyŬ������ �߰����ִ� �Լ�
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //�Ű������� ���� �θ� ������Ʈ�� ������ ������Ʈ�� �ٿ���
        AllyUnit a_RefAlly = parentObject.AddComponent<WarriorUnit>();
        //�� Warrior_Att ��ü�� AllyUnit ���� ally_Attribute ������ �������ش�
        a_RefAlly.ally_Attribute = this;

        return a_RefAlly;
    }
}

public class WarriorUnit : AllyUnit
{
    //int skillHitLimit = 0;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();  //�θ��� Start() ȣ�� (���� ���� �غ�)        
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
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, curAttRange);

        GameObject effect = null;
        foreach (Collider2D coll in colls)
        {
            if (!coll.tag.Contains("Monster")) continue;

            cacDir = coll.transform.position - transform.position;

            if (cacDir.magnitude <= curAttRange)
            {
                //������ �ο�
                coll.GetComponent<Monster_Ctrl>().TakeDamage(curAttDamage, this.gameObject);

                //����Ʈ ����
                if (base.ally_Attribute.attackEff != null)
                {
                    effect = Instantiate(base.ally_Attribute.attackEff) as GameObject;
                    effect.transform.position = coll.transform.position;
                    Destroy(effect, 0.5f);
                }

                anyHit = true;
                break;
            }
        }

        if (anyHit)
        {
            attackCount++;
            anyHit = false;
        }
    }

    public override void Skill()
    {
        //������ ���� ��ų ����
        Debug.Log("������ ��ų �ߵ�!");

        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, skillRange);

        GameObject effect = null;
        foreach (Collider2D coll in colls)
        {
            if (skillHitLimit <= 0)
            {
                skillHitLimit = ally_Attribute.skillHitLimit;
                break;
            }
            if (!coll.tag.Contains("Monster")) continue;

            cacDir = coll.transform.position - transform.position;

            if (cacDir.magnitude <= skillRange)
            {
                //������ �ο�
                coll.GetComponent<Monster_Ctrl>().TakeDamage(skillDamage, this.gameObject);

                skillHitLimit--;
            }
        }

        //����Ʈ ����
        if (base.ally_Attribute.skillEff != null)
        {
            effect = Instantiate(base.ally_Attribute.skillEff) as GameObject;
            effect.transform.position = transform.position;
            Destroy(effect, 1.0f);
        }

        attackCount = 0;
    }
}
