using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Priest_Att : Ally_Attribute
{
    //프리스트 고유 속성 추가
    public float skillDur = 0;
    public float skillTick = 0;
    public Priest_Att()    //생성자 오버로딩 함수
    {
        //아군 공통 속성 기본값 부여
        type = AllyType.Priest;
        unitName = "프리스트";
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

        //프리스트 고유 속성 기본값 부여
        skillDur = 5.0f;
        skillTick = 1.0f;
    }

    //Ally게임 오브젝트에 빙의시킬 Ally클래스를 추가해주는 함수
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //매개변수로 받은 부모 오브젝트에 프리스트 컴포넌트를 붙여줌
        AllyUnit a_RefAlly = parentObject.AddComponent<PriestUnit>();
        //이 Priest_Att 객체를 AllyUnit 쪽의 ally_Attribute 변수에 대입해준다
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
        base.Start();  //부모쪽 Start() 호출 (공통 등장 준비)
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //프리스트 고유 행동 패턴
    }

    public override void Attack()
    {
        //프리스트 고유 공격 패턴
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
        //만약 10마리가 있고 그 중에 아군이 5명이야. 짝수가 아군이라 치자
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

        Debug.Log("제일 낮은 피를 가진 유닛의 인덱스" + whosHpIdx[idx]);

        if (colls[whosHpIdx[idx]].GetComponent<AllyUnit>().curHp + curAttDamage < colls[whosHpIdx[idx]].GetComponent<AllyUnit>().maxHp)
            colls[whosHpIdx[idx]].GetComponent<AllyUnit>().curHp += curAttDamage;
        else
            colls[whosHpIdx[idx]].GetComponent<AllyUnit>().curHp = colls[whosHpIdx[idx]].GetComponent<AllyUnit>().maxHp;

        Debug.Log(colls[whosHpIdx[idx]].name + "에게 힐 시전완료");
        anyHit = true;

        //이펙트 생성
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
        //프리스트 고유 스킬 패턴
        StartCoroutine(DoTHeal());
    }
    IEnumerator DoTHeal()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, skillRange);
        
        GameObject effect = null;
        //이펙트생성
        foreach (Collider2D coll in colls)
        {
            if (coll == null) continue;
            if (!coll.tag.Contains("Ally")) continue;

            if (coll.GetComponent<AllyUnit>().curHp < coll.GetComponent<AllyUnit>().maxHp)
            {
                if (base.ally_Attribute.skillEff != null)
                {
                    //이펙트 중복되지 않도록 
                    if (coll.GetComponent<AllyUnit>().isDoTHeal) continue;

                    effect = Instantiate(base.ally_Attribute.skillEff) as GameObject;
                    effect.transform.position = coll.transform.position;
                    coll.GetComponent<AllyUnit>().isDoTHeal = true;
                    coll.GetComponent<AllyUnit>().whosHeal = this.gameObject;
                    Destroy(effect, ((Priest_Att)ally_Attribute).skillDur);
                }
            }
        }

        //힐 데미지 부여    
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

        //스킬 종료 후 도트힐 상태 꺼주기
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
