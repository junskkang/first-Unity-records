using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hunter_Att : Ally_Attribute
{
    //헌터 고유 속성 추가

    public Hunter_Att()    //생성자 오버로딩 함수
    {
        //아군 공통 속성 기본값 부여
        type = AllyType.Hunter;
        unitName = "헌터";
        level = 0;
        unlockCost = 30;
        buildCost = 20;

        maxHp = 100;
        maxMp = 20;

        attackDamage = 5.0f;
        attackRange = 6.0f;
        attackSpeed = 1.5f;
        attackCool = 0.0f;

        attackCount = 0;
        skillPossible = 7;
        anyHit = false;
        skillRange = 4.0f;
        skillDamage = 15.0f;
        skillHitLimit = 5;

        attackEff = Resources.Load($"{type}AttackEff") as GameObject;
        skillEff = Resources.Load($"{type}SkillEff") as GameObject;

        //헌터 고유 속성 기본값 부여
    }

    //Ally게임 오브젝트에 빙의시킬 Ally클래스를 추가해주는 함수
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //매개변수로 받은 부모 오브젝트에 헌터 컴포넌트를 붙여줌
        AllyUnit a_RefAlly = parentObject.AddComponent<HunterUnit>();
        //이 Hunter_Att 객체를 AllyUnit 쪽의 ally_Attribute 변수에 대입해준다
        a_RefAlly.ally_Attribute = this;

        return a_RefAlly;
    }
}

public class HunterUnit : AllyUnit
{
    public GameObject bulletPrefab;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();  //부모쪽 Start() 호출 (공통 등장 준비)
        bulletPrefab = Resources.Load("BulletPrefab") as GameObject;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //헌터 고유 행동 패턴
    }

    public override void Attack()
    {
        //헌터 고유 공격 패턴
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, curAttRange);

        GameObject bullet = null;
        BulletCtrl bulletCtrl = null;
        foreach (Collider2D coll in colls)
        {
            if (!coll.tag.Contains("Monster")) continue;

            cacDir = coll.transform.position - transform.position;

            if (cacDir.magnitude <= curAttRange)
            {
                //화살 발사
                bullet = Instantiate(bulletPrefab);
                bulletCtrl = bullet.GetComponent<BulletCtrl>();
                bulletCtrl.BulletSpawn(transform.position, cacDir.normalized, curAttDamage
                                        , base.ally_Attribute.attackEff, this);
                StartCoroutine(bulletCtrl.EffectOn(coll.transform.position));
                bullet.transform.right = new Vector3(cacDir.normalized.x, cacDir.normalized.y, 0.0f);
                
                if (coll.GetComponent<Monster_Ctrl>().isFlying) //공중몹 1.5배 데미지
                    coll.GetComponent<Monster_Ctrl>().TakeDamage(curAttDamage * 1.5f, this.gameObject);
                else
                    coll.GetComponent<Monster_Ctrl>().TakeDamage(curAttDamage, this.gameObject);

                anyHit = true;
                break;
            }
        }

        if (anyHit)
        {
            attackCount++;
            curHp -= 1;
            anyHit = false;
        }
    }

    public override void Skill()
    {
        //헌터 고유 스킬 패턴
        Debug.Log("헌터 스킬 발동!");

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
                if (coll.GetComponent<Monster_Ctrl>().isFlying) //공중몹 1.5배 데미지
                    coll.GetComponent<Monster_Ctrl>().TakeDamage(skillDamage * 1.5f, this.gameObject);
                else
                    coll.GetComponent<Monster_Ctrl>().TakeDamage(skillDamage, this.gameObject);

                //이펙트 생성
                if (base.ally_Attribute.skillEff != null)
                {
                    effect = Instantiate(base.ally_Attribute.skillEff) as GameObject;
                    effect.transform.position = new Vector3(coll.transform.position.x, coll.transform.position.y + 0.2f, coll.transform.position.z);
                    Destroy(effect, 1.5f);
                }

                skillHitLimit--;
            }
        }
        attackCount = 0;
        isSkilled = false;
    }

    public override void LevelUp()
    {
        base.LevelUp();

        curAttDamage += 1.5f;
        curAttRange += 0.1f;
        curAttSpeed -= 0.1f;

        if (curLevel > 4)
        {
            skillRange += 0.2f;
            skillDamage += 2.0f;
        }
        else if (curLevel % 5 == 0)
        {
            skillPossible -= 1;
            ally_Attribute.skillHitLimit += 1;
        }
    }
}
