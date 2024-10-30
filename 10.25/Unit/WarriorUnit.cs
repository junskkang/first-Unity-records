using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
public class Warrior_Att : Ally_Atrribute
{
    //워리어 고유 속성 추가   

    public Warrior_Att()    //생성자 오버로딩 함수
    {
        //아군 공통 속성 기본값 부여
        type = AllyType.Warrior;
        name = "워리어";
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
        //워리어 고유 속성 기본값 부여
        
    }

    //Ally게임 오브젝트에 빙의시킬 Ally클래스를 추가해주는 함수
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //매개변수로 받은 부모 오브젝트에 워리어 컴포넌트를 붙여줌
        AllyUnit a_RefAlly = parentObject.AddComponent<WarriorUnit>();
        //이 Warrior_Att 객체를 AllyUnit 쪽의 ally_Attribute 변수에 대입해준다
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
        base.Start();  //부모쪽 Start() 호출 (공통 등장 준비)        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //워리어 고유 행동 패턴
    }

    public override void Attack()
    {
        //워리어 고유 공격 패턴
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, curAttRange);

        GameObject effect = null;
        foreach (Collider2D coll in colls)
        {
            if (!coll.tag.Contains("Monster")) continue;

            cacDir = coll.transform.position - transform.position;

            if (cacDir.magnitude <= curAttRange)
            {
                //데미지 부여
                coll.GetComponent<Monster_Ctrl>().TakeDamage(curAttDamage, this.gameObject);

                //이펙트 생성
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
        //워리어 고유 스킬 패턴
        Debug.Log("워리어 스킬 발동!");

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
                //데미지 부여
                coll.GetComponent<Monster_Ctrl>().TakeDamage(skillDamage, this.gameObject);

                skillHitLimit--;
            }
        }

        //이펙트 생성
        if (base.ally_Attribute.skillEff != null)
        {
            effect = Instantiate(base.ally_Attribute.skillEff) as GameObject;
            effect.transform.position = transform.position;
            Destroy(effect, 1.0f);
        }

        attackCount = 0;
    }
}
