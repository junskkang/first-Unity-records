using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Mage_Att : Ally_Attribute
{
    //메이지 고유 속성 추가
    public int multiMin = 0;
    public int multiMax = 0;
    public float skillDur = 0;
    public float skillTick = 0;
    public GameObject[] attackEffs = new GameObject[4];
    public GameObject[] skillEffs = new GameObject[2];

    public Mage_Att()    //생성자 오버로딩 함수
    {
        //아군 공통 속성 기본값 부여
        type = AllyType.Mage;
        unitName = "메이지";
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
        //메이지 고유 속성 기본값 부여
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

    //Ally게임 오브젝트에 빙의시킬 Ally클래스를 추가해주는 함수
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //매개변수로 받은 부모 오브젝트에 메이지 컴포넌트를 붙여줌
        AllyUnit a_RefAlly = parentObject.AddComponent<MageUnit>();
        //이 Mage_Att 객체를 AllyUnit 쪽의 ally_Attribute 변수에 대입해준다
        a_RefAlly.ally_Attribute = this;

        return a_RefAlly;
    }
}

public class MageUnit : AllyUnit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();  //부모쪽 Start() 호출 (공통 등장 준비)
        //Debug.Log(((Mage_Att)ally_Attribute).attackEffs.Length);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //메이지 고유 행동 패턴
    }

    public override void Attack()
    { 

        //메이지 고유 공격 패턴
        int ran = Random.Range(0, 4);   //지화수풍 
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
                //데미지 부여
                colls[i].GetComponent<Monster_Ctrl>().TakeDamage(curAttDamage, this.gameObject);

                //이펙트 생성
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
        //메이지 고유 스킬 패턴
        Debug.Log("메이지 스킬 발동!");

        //이펙트 생성
        StartCoroutine(MeteorEffect());

        //데미지 발생
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
        //데미지 부여
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
                    //데미지 부여
                    coll.GetComponent<Monster_Ctrl>().TakeDamage(skillDamage, this.gameObject);
                    //Debug.Log("메테오 데미지" + skillDamage);
                }
            }
            yield return new WaitForSeconds(((Mage_Att)ally_Attribute).skillTick);
        }
    }
}
