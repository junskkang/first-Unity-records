using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Mage_Att : Ally_Attribute
{
    //������ ���� �Ӽ� �߰�
    public int multiMin = 0;
    public int multiMax = 0;
    public float skillDur = 0;
    public float skillTick = 0;
    public GameObject[] attackEffs = new GameObject[4];
    public GameObject[] skillEffs = new GameObject[2];

    public Mage_Att()    //������ �����ε� �Լ�
    {
        //�Ʊ� ���� �Ӽ� �⺻�� �ο�
        type = AllyType.Mage;
        unitName = "������";
        level = 0;
        unlockCost = 50;
        buildCost = 30;

        maxHp = 100;
        maxMp = 20;

        attackDamage = 7;
        attackRange = 5.0f;
        attackSpeed = 4.0f;
        attackCool = 0.0f;

        attackCount = 0;
        skillPossible = 3;
        anyHit = false;
        skillRange = 4.0f;
        skillDamage = 0.5f;
        skillHitLimit = 0;        

        attackEff = Resources.Load($"{type}AttackEff") as GameObject;
        skillEff = Resources.Load($"{type}SkillEff") as GameObject;
        //������ ���� �Ӽ� �⺻�� �ο�
        multiMin = 1;
        multiMax = 5;
        skillDur = 5.0f;
        skillTick = 0.2f;
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
        //Debug.Log(((Mage_Att)ally_Attribute).attackEffs.Length);
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
        int ran = Random.Range(0, 4);   //��ȭ��ǳ 
        int multi = Random.Range(((Mage_Att)ally_Attribute).multiMin, ((Mage_Att)ally_Attribute).multiMax);
        //int i = 0;
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, curAttRange);

        GameObject effect = null;

        for (int i = 0; i < colls.Length; i++) 
        {
            //Debug.Log(multi);
            if (i == multi) break;
            if (!colls[i].tag.Contains("Monster")) continue;

            cacDir = colls[i].transform.position - transform.position;

            if (cacDir.magnitude <= curAttRange)
            {
                //������ �ο�
                colls[i].GetComponent<Monster_Ctrl>().TakeDamage(curAttDamage, this.gameObject);

                //����Ʈ ����
                if (((Mage_Att)ally_Attribute).attackEffs != null)
                {
                    effect = Instantiate(((Mage_Att)ally_Attribute).attackEffs[ran]) as GameObject;
                    effect.transform.position = colls[i].transform.position;
                    Destroy(effect, 1.0f);
                }

                anyHit = true;
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

        //����Ʈ ����
        StartCoroutine(MeteorEffect());

        //������ �߻�
        StartCoroutine(MeteorDamage());

        attackCount = 0;
    }

    IEnumerator MeteorEffect()
    {

        GameObject effect1 = Instantiate(((Mage_Att)ally_Attribute).skillEffs[0]) as GameObject;
        effect1.transform.position = transform.position;
        Destroy(effect1, ((Mage_Att)ally_Attribute).skillDur);

        GameObject effect2 = null;
        int ranX, ranY;

        for (int i = 0; i < ((Mage_Att)ally_Attribute).skillDur / 0.3f; i++)
        {
            ranX = Random.Range(-(int)skillRange, (int)skillRange);
            ranY = Random.Range(-(int)skillRange, (int)skillRange);
            effect2 = Instantiate(((Mage_Att)ally_Attribute).skillEffs[1]) as GameObject;
            effect2.transform.position = new Vector3(transform.position.x + ranX,
                                        transform.position.y + ranY, transform.position.z);
            Destroy(effect2, 1.0f);
            yield return new WaitForSeconds(0.3f);
        }

        isSkilled = false;
    }

    IEnumerator MeteorDamage()
    {        
        //������ �ο�
        Collider2D[] colls;

        for (int i = 0; i < ((Mage_Att)ally_Attribute).skillDur / ((Mage_Att)ally_Attribute).skillTick; i++)
        {
            colls = Physics2D.OverlapCircleAll(transform.position, skillRange);
            //Debug.Log(colls.Length);
            foreach (Collider2D coll in colls)
            {
                if (!coll.tag.Contains("Monster")) continue;

                cacDir = coll.transform.position - transform.position;

                if (cacDir.magnitude <= skillRange)
                {
                    //������ �ο�
                    coll.GetComponent<Monster_Ctrl>().TakeDamage(skillDamage, this.gameObject);
                    //Debug.Log("���׿� ������" + skillDamage);
                }
            }
            yield return new WaitForSeconds(((Mage_Att)ally_Attribute).skillTick);
        }
    }
}
