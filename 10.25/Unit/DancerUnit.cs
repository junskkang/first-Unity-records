using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Dancer_Att : Ally_Attribute
{
    //�� ���� �Ӽ� �߰�
    public float attackDur;
    public float bewitchedSpeed;
    public float skillDur;
    public GameObject[] skillEffs = new GameObject[2];

    public Dancer_Att()    //������ �����ε� �Լ�
    {
        //�Ʊ� ���� �Ӽ� �⺻�� �ο�
        type = AllyType.Dancer;
        unitName = "��";
        level = 0;
        unlockCost = 70;
        buildCost = 100;

        maxHp = 100;
        maxMp = 20;

        attackDamage = 1;
        attackRange = 4.0f;
        attackSpeed = 3.0f;
        attackCool = 0.0f;

        attackCount = 0;
        skillPossible = 4;
        anyHit = false;
        skillRange = 3.0f;
        skillDamage = 1.2f;
        skillHitLimit = 4;

        attackEff = Resources.Load($"{type}AttackEff") as GameObject;
        skillEff = Resources.Load($"{type}SkillEff") as GameObject;

        //�� ���� �Ӽ� �⺻�� �ο�
        attackDur = 3.0f;
        bewitchedSpeed = 0.4f;
        skillDur = 3.0f;
        
        for (int i = 0; i < skillEffs.Length; i++)
        {
            skillEffs[i] = Resources.Load($"{type}SkillEff{i}") as GameObject;
        }
    }

    //Ally���� ������Ʈ�� ���ǽ�ų AllyŬ������ �߰����ִ� �Լ�
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //�Ű������� ���� �θ� ������Ʈ�� �� ������Ʈ�� �ٿ���
        AllyUnit a_RefAlly = parentObject.AddComponent<DancerUnit>();
        //�� Dancer_Att ��ü�� AllyUnit ���� ally_Attribute ������ �������ش�
        a_RefAlly.ally_Attribute = this;

        return a_RefAlly;
    }
}

public class DancerUnit : AllyUnit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();  //�θ��� Start() ȣ�� (���� ���� �غ�)
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //�� ���� �ൿ ����
    }

    public override void Attack()
    {
        StartCoroutine(Bewitch());
    }

    IEnumerator Bewitch()
    {
        //�� ���� ���� ����
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, curAttRange);

        isSkilled = true;

        GameObject effect = null;

        foreach (Collider2D coll in colls)
        {
            if (skillHitLimit <= 0)
            {
                skillHitLimit = ally_Attribute.skillHitLimit;
                break;
            }
            if (coll == null) continue;
            if (!coll.tag.Contains("Monster")) continue;
            if (coll.GetComponent<Monster_Ctrl>().isBewitched) continue;

            cacDir = coll.transform.position - transform.position;

            if (cacDir.magnitude <= curAttRange)
            {
                //����Ʈ ����
                if (base.ally_Attribute.attackEff != null)
                {
                    effect = Instantiate(base.ally_Attribute.attackEff) as GameObject;
                    effect.transform.position = coll.transform.position;
                    effect.transform.SetParent(coll.transform);
                    coll.GetComponent<Monster_Ctrl>().isBewitched = true;
                    coll.GetComponent<Monster_Ctrl>().bewitchedSpeed = ((Dancer_Att)ally_Attribute).bewitchedSpeed;
                    coll.GetComponent<Monster_Ctrl>().whosBewitch = this.gameObject;
                    Destroy(effect, ((Dancer_Att)ally_Attribute).attackDur);
                }
                skillHitLimit--;
                anyHit = true;
            }
        }

        yield return new WaitForSeconds(((Dancer_Att)ally_Attribute).attackDur);

        foreach (Collider2D coll in colls)
        {
            if (coll == null) continue;
            if (!coll.tag.Contains("Monster")) continue;
            if (coll.GetComponent<Monster_Ctrl>().isBewitched &&
                coll.GetComponent<Monster_Ctrl>().whosBewitch == this.gameObject)
            {
                coll.GetComponent<Monster_Ctrl>().isBewitched = false;
            }
        }
        
        if (anyHit)
        {
            attackCount++;
            curHp -= 1;
            anyHit = false;
        }

        isSkilled = false;

    }

    public override void Skill()
    {
        //�� ���� ��ų ����
        StartCoroutine(Accelerando());
    }

    IEnumerator Accelerando()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, skillRange);

        GameObject effect1;
        GameObject effect2;

        SpriteRenderer[] randomSR;
        int ran;

        //����Ʈ����
        foreach (Collider2D coll in colls)
        {
            if (coll == null) continue;
            if (!coll.tag.Contains("Ally")) continue;
                       
            if (((Dancer_Att)ally_Attribute).skillEffs != null)
            {
                //����Ʈ �ߺ����� �ʵ��� 
                if (coll.GetComponent<AllyUnit>().isAccel) continue;

                effect1 = Instantiate(((Dancer_Att)ally_Attribute).skillEffs[1]) as GameObject;
                effect1.transform.position = coll.transform.position;

                effect2 = Instantiate(((Dancer_Att)ally_Attribute).skillEffs[0]) as GameObject;
                effect2.transform.position = coll.transform.position;
                randomSR = effect2.GetComponentsInChildren<SpriteRenderer>(true);
                //Debug.Log(randomSR.Length);
                ran = Random.Range(0, randomSR.Length);
                randomSR[ran].gameObject.SetActive(true);

                coll.GetComponent<AllyUnit>().isAccel = true;
                coll.GetComponent<AllyUnit>().accel = skillDamage;
                coll.GetComponent<AllyUnit>().whosAccel = this.gameObject;

                //ȿ�� �ο�

                //����Ʈ ���� ����
                Destroy(effect1, ((Dancer_Att)ally_Attribute).skillDur);
                Destroy(effect2, ((Dancer_Att)ally_Attribute).skillDur);
            }            
        }

        yield return new WaitForSeconds(((Dancer_Att)ally_Attribute).skillDur);

        //��ų ���� �� ��ÿ������ ���� ���ֱ�
        foreach (Collider2D coll in colls)
        {
            if (coll == null) continue;
            if (!coll.tag.Contains("Ally")) continue;

            if (coll.GetComponent<AllyUnit>().isAccel &&
                coll.GetComponent<AllyUnit>().whosAccel == this.gameObject)
                coll.GetComponent<AllyUnit>().isAccel = false;
        }

        isSkilled = false;
        attackCount = 0;
    }

    public override void LevelUp()
    {
        base.LevelUp();

        curAttDamage += 1.0f;
        curAttRange += 0.05f;
        curAttSpeed -= 0.1f;

        if (curLevel > 4)
        {
            skillRange += 0.15f;
            skillDamage += 0.2f;
        }
        else if (curLevel % 5 == 0)
        {
            ((Dancer_Att)ally_Attribute).skillDur += 0.5f;
            ((Dancer_Att)ally_Attribute).attackDur += 0.5f;
            ((Dancer_Att)ally_Attribute).bewitchedSpeed -= 0.1f;
            ally_Attribute.skillHitLimit += 1;
            skillPossible -= 1;
        }
    }
}
