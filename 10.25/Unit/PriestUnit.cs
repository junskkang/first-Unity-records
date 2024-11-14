using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Priest_Att : Ally_Attribute
{
    //������Ʈ ���� �Ӽ� �߰�
    public float skillDur = 0;
    public float skillTick = 0;
    public Priest_Att()    //������ �����ε� �Լ�
    {
        //�Ʊ� ���� �Ӽ� �⺻�� �ο�
        type = AllyType.Priest;
        unitName = "������Ʈ";
        level = 0;
        unlockCost = 50;
        buildCost = 50;

        maxHp = 100;
        maxMp = 20;

        attackDamage = 5;
        attackRange = 4.0f;
        attackSpeed = 3.0f;
        attackCool = 0.0f;

        attackCount = 0;
        skillPossible = 5;
        anyHit = false;
        skillRange = 2.0f;
        skillDamage = 1.0f;
        skillHitLimit = 4;

        attackEff = Resources.Load($"{type}AttackEff") as GameObject;
        skillEff = Resources.Load($"{type}SkillEff") as GameObject;

        //������Ʈ ���� �Ӽ� �⺻�� �ο�
        skillDur = 5.0f;
        skillTick = 1.0f;
    }

    //Ally���� ������Ʈ�� ���ǽ�ų AllyŬ������ �߰����ִ� �Լ�
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //�Ű������� ���� �θ� ������Ʈ�� ������Ʈ ������Ʈ�� �ٿ���
        AllyUnit a_RefAlly = parentObject.AddComponent<PriestUnit>();
        //�� Priest_Att ��ü�� AllyUnit ���� ally_Attribute ������ �������ش�
        a_RefAlly.ally_Attribute = this;

        return a_RefAlly;
    }
}

public class PriestUnit : AllyUnit
{
    List<float> allyHpValue = new List<float>();
    List<int> whosHpIdx = new List<int>();
    float lessHp = 0;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();  //�θ��� Start() ȣ�� (���� ���� �غ�)
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //������Ʈ ���� �ൿ ����
    }

    public override void Attack()
    {
        //������Ʈ ���� ���� ����
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, curAttRange);
        
        GameObject effect = null;
        
        for (int i = 0;  i < colls.Length; i++)
        {
            if (colls[i] == null) continue;
            if (!colls[i].tag.Contains("Ally")) continue;
            if (colls[i].GetComponent<AllyUnit>().curHp == colls[i].GetComponent<AllyUnit>().maxHp) continue;

            if (colls[i].GetComponent<AllyUnit>().curHp < colls[i].GetComponent<AllyUnit>().maxHp)
            {
                //Debug.Log(colls[i].GetComponent<AllyUnit>().curHp);
                //Debug.Log(colls[i].GetComponent<AllyUnit>().maxHp);
                whosHpIdx.Add(i);
                allyHpValue.Add(colls[i].GetComponent<AllyUnit>().curHp);
            }            
        }
        //���� 10������ �ְ� �� �߿� �Ʊ��� 5���̾�. ¦���� �Ʊ��̶� ġ��
        //whosHp[0] = 2
        if (whosHpIdx.Count == 0) return;

        lessHp = allyHpValue[0];
        int idx = 0;

        for (int j = 0; j < allyHpValue.Count; j++)
        {
            if (lessHp > allyHpValue[j])
            {
                lessHp = allyHpValue[j];
                idx = j;
            }                
        }

        Debug.Log("���� ���� �Ǹ� ���� ������ �ε���" + whosHpIdx[idx]);

        if (colls[whosHpIdx[idx]].GetComponent<AllyUnit>().curHp + curAttDamage < colls[whosHpIdx[idx]].GetComponent<AllyUnit>().maxHp)
            colls[whosHpIdx[idx]].GetComponent<AllyUnit>().curHp += curAttDamage;
        else
            colls[whosHpIdx[idx]].GetComponent<AllyUnit>().curHp = colls[whosHpIdx[idx]].GetComponent<AllyUnit>().maxHp;

        Debug.Log(colls[whosHpIdx[idx]].name + "���� �� �����Ϸ�");
        anyHit = true;

        //����Ʈ ����
        if (base.ally_Attribute.attackEff != null)
        {
            effect = Instantiate(base.ally_Attribute.attackEff) as GameObject;
            effect.transform.position = colls[whosHpIdx[idx]].transform.position;
            Destroy(effect, 2.5f);
        }
        allyHpValue.Clear();
        whosHpIdx.Clear();

        if (anyHit)
        {
            attackCount++;
            curHp -= 1;
            anyHit = false;
        }
    }

    public override void Skill()
    {
        //������Ʈ ���� ��ų ����
        StartCoroutine(DoTHeal());
    }
    IEnumerator DoTHeal()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, skillRange);
        
        GameObject effect = null;
        //����Ʈ����
        foreach (Collider2D coll in colls)
        {
            if (coll == null) continue;
            if (!coll.tag.Contains("Ally")) continue;

            if (coll.GetComponent<AllyUnit>().curHp < coll.GetComponent<AllyUnit>().maxHp)
            {
                if (base.ally_Attribute.skillEff != null)
                {
                    //����Ʈ �ߺ����� �ʵ��� 
                    if (coll.GetComponent<AllyUnit>().isDoTHeal) continue;

                    effect = Instantiate(base.ally_Attribute.skillEff) as GameObject;
                    effect.transform.position = coll.transform.position;
                    coll.GetComponent<AllyUnit>().isDoTHeal = true;
                    coll.GetComponent<AllyUnit>().whosHeal = this.gameObject;
                    Destroy(effect, ((Priest_Att)ally_Attribute).skillDur);
                }
            }
        }

        //�� ������ �ο�    
        for (int i = 0; i < ((Priest_Att)ally_Attribute).skillDur / ((Priest_Att)ally_Attribute).skillTick; i++)
        {
            foreach (Collider2D coll in colls)
            {
                if (coll == null) continue;
                if (!coll.tag.Contains("Ally")) continue;

                if (coll.GetComponent<AllyUnit>().isDoTHeal &&
                    coll.GetComponent<AllyUnit>().whosHeal == this.gameObject)
                {
                    if (coll.GetComponent<AllyUnit>().curHp + skillDamage < coll.GetComponent<AllyUnit>().maxHp)
                        coll.GetComponent<AllyUnit>().curHp += skillDamage;
                    else
                        coll.GetComponent<AllyUnit>().curHp = coll.GetComponent<AllyUnit>().maxHp;
                }
            }

            yield return new WaitForSeconds(((Priest_Att)ally_Attribute).skillTick);
        }

        //��ų ���� �� ��Ʈ�� ���� ���ֱ�
        foreach (Collider2D coll in colls)
        {
            if (coll == null) continue;
            if (!coll.tag.Contains("Ally")) continue;

            if (coll.GetComponent<AllyUnit>().isDoTHeal &&
                    coll.GetComponent<AllyUnit>().whosHeal == this.gameObject)
                coll.GetComponent<AllyUnit>().isDoTHeal = false;
        }

        isSkilled = false;
        attackCount = 0;
    }
}
