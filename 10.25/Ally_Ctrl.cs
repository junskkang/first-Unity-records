using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally_Ctrl : MonoBehaviour
{
    public AllyType allyType;
    public Canvas canvas;

    float attackRange = 2.0f;
    float attackSpeed = 2.0f;
    float attackCool = 0.0f;
    float attackDamage = 10.0f;
    public GameObject attackEff;
    public GameObject skillEff;
    int attackCount = 0;
    bool anyHit = false;
    int skillPossible = 5;
    float skillRange = 3.0f;
    float skillDamage = 20.0f;
    int level = 1;
    public int monKill = 0; 

    Vector3 cacDir = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        attackEff = Resources.Load($"{allyType.ToString()}AttackEff") as GameObject;
        skillEff = Resources.Load($"{allyType.ToString()}SkillEff") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        attackCool -= Time.deltaTime;

        if (attackCool <= 0)
        {
            Attack();
            attackCool = attackSpeed;
        }

        if (attackCount == skillPossible)
            Skill();        
    }

    void Attack()
    {
        //GameObject[] monster = GameObject.FindGameObjectsWithTag("Monster");        
        ////Debug.Log(monster.Length);

        //for (int i = 0; i < monster.Length; i++) 
        //{
        //    cacRange = monster[i].transform.position - transform.position;

        //    if (attackRange >= cacRange.magnitude)
        //    {
        //        monster[i].GetComponent<Monster_Ctrl>().TakeDamage(10.0f);
        //    }
        //}

        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, attackRange);
        
        GameObject effect = null;
        foreach (Collider2D coll in colls)
        {
            if (!coll.tag.Contains("Monster")) continue;

            cacDir = coll.transform.position - transform.position;

            if (cacDir.magnitude <= attackRange)
            {
                //데미지 부여
                coll.GetComponent<Monster_Ctrl>().TakeDamage(attackDamage, this);

                //이펙트 생성
                if (attackEff != null)
                {
                    effect = Instantiate(attackEff) as GameObject;
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
            
        //Debug.Log(attackCount);
    }

    void Skill()
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, skillRange);

        GameObject effect = null;
        foreach (Collider2D coll in colls)
        {
            if (!coll.tag.Contains("Monster")) continue;

            cacDir = coll.transform.position - transform.position;

            if (cacDir.magnitude <= skillRange)
            {
                //데미지 부여
                coll.GetComponent<Monster_Ctrl>().TakeDamage(skillDamage, this);
            }
        }
        
        //이펙트 생성
        if (skillEff != null)
        {
            effect = Instantiate(skillEff) as GameObject;
            effect.transform.position = transform.position;
            Destroy(effect, 1.0f);
        }

        attackCount = 0;
    }

    public void Levelup()
    {
        if (level >= 10) return;

        attackRange += 0.1f;

        attackSpeed -= 0.1f;        

        skillRange += 0.2f;

        attackDamage += 1.0f;

        skillDamage += 2.0f;

        level++;
        
        if (level % 4 == 0)
        {
            skillPossible -= 1;
        }
    }
}
