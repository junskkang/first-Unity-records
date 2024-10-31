using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Priest_Att : Ally_Atrribute
{
    //������Ʈ ���� �Ӽ� �߰�
    public float skillDur = 0;
    public Priest_Att()    //������ �����ε� �Լ�
    {
        //�Ʊ� ���� �Ӽ� �⺻�� �ο�
        type = AllyType.Priest;
        name = "������Ʈ";
        level = 1;
        maxHp = 100;
        maxMp = 20;

        attackDamage = 10;
        attackRange = 4.0f;
        attackSpeed = 3.0f;
        attackCool = 0.0f;

        attackCount = 0;
        skillPossible = 5;
        anyHit = false;
        skillRange = 4.0f;
        skillDamage = 5.0f;
        skillHitLimit = 0;

        attackEff = Resources.Load($"{type}AttackEff") as GameObject;
        skillEff = Resources.Load($"{type}SkillEff") as GameObject;

        //������Ʈ ���� �Ӽ� �⺻�� �ο�
        skillDur = 5.0f;
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
            if (!colls[i].tag.Contains("Ally")) continue;

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
            anyHit = false;
        }
    }

    public override void Skill()
    {
        //������Ʈ ���� ��ų ����
        isSkilled = false;
        attackCount = 0;
    }
}
