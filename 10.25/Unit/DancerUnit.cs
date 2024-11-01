using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Dancer_Att : Ally_Atrribute
{
    //댄서 고유 속성 추가
    public float attackDur;
    public float bewitchedSpeed;
    public float skillDur;
    
    public Dancer_Att()    //생성자 오버로딩 함수
    {
        //아군 공통 속성 기본값 부여
        type = AllyType.Dancer;
        name = "댄서";
        level = 1;
        maxHp = 100;
        maxMp = 20;

        attackDamage = 1;
        attackRange = 4.0f;
        attackSpeed = 3.0f;
        attackCool = 0.0f;

        attackCount = 0;
        skillPossible = 2;
        anyHit = false;
        skillRange = 4.0f;
        skillDamage = 3.0f;
        skillHitLimit = 5;

        attackEff = Resources.Load($"{type}AttackEff") as GameObject;
        skillEff = Resources.Load($"{type}SkillEff") as GameObject;

        //프리스트 고유 속성 기본값 부여
        attackDur = 3.0f;
        bewitchedSpeed = 0.4f;
        skillDur = 5.0f;

        //댄서 고유 속성 기본값 부여
    }

    //Ally게임 오브젝트에 빙의시킬 Ally클래스를 추가해주는 함수
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //매개변수로 받은 부모 오브젝트에 댄서 컴포넌트를 붙여줌
        AllyUnit a_RefAlly = parentObject.AddComponent<DancerUnit>();
        //이 Dancer_Att 객체를 AllyUnit 쪽의 ally_Attribute 변수에 대입해준다
        a_RefAlly.ally_Attribute = this;

        return a_RefAlly;
    }
}

public class DancerUnit : AllyUnit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();  //부모쪽 Start() 호출 (공통 등장 준비)
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //댄서 고유 행동 패턴
    }

    public override void Attack()
    {
        StartCoroutine(Bewitch());
    }

    IEnumerator Bewitch()
    {
        //댄서 고유 공격 패턴
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
                //이펙트 생성
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
            anyHit = false;
        }

        isSkilled = false;

    }

    public override void Skill()
    {
        //댄서 고유 스킬 패턴
    }
}
