using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Mage_Att : Ally_Atrribute
{
    //메이지 고유 속성 추가
    public GameObject[] attackEffs = new GameObject[4];
    public GameObject[] skillEffs = new GameObject[2];

    public Mage_Att()    //생성자 오버로딩 함수
    {
        //아군 공통 속성 기본값 부여
        type = AllyType.Mage;
        name = "메이지";
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
        //메이지 고유 속성 기본값 부여
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
        Debug.Log(((Mage_Att)ally_Attribute).attackEffs.Length);
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
        int ran = Random.Range(0, 4);
    }

    public override void Skill()
    {
        //메이지 고유 스킬 패턴
    }
}
