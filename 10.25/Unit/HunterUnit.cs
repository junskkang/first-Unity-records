using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hunter_Att : Ally_Attribute
{
    //���� ���� �Ӽ� �߰�

    public Hunter_Att()    //������ �����ε� �Լ�
    {
        //�Ʊ� ���� �Ӽ� �⺻�� �ο�
        type = AllyType.Hunter;
        unitName = "����";
        level = 1;
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

        //���� ���� �Ӽ� �⺻�� �ο�
    }

    //Ally���� ������Ʈ�� ���ǽ�ų AllyŬ������ �߰����ִ� �Լ�
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //�Ű������� ���� �θ� ������Ʈ�� ���� ������Ʈ�� �ٿ���
        AllyUnit a_RefAlly = parentObject.AddComponent<HunterUnit>();
        //�� Hunter_Att ��ü�� AllyUnit ���� ally_Attribute ������ �������ش�
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
        base.Start();  //�θ��� Start() ȣ�� (���� ���� �غ�)
        bulletPrefab = Resources.Load("BulletPrefab") as GameObject;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //���� ���� �ൿ ����
    }

    public override void Attack()
    {
        //���� ���� ���� ����
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, curAttRange);

        GameObject bullet = null;
        BulletCtrl bulletCtrl = null;
        foreach (Collider2D coll in colls)
        {
            if (!coll.tag.Contains("Monster")) continue;

            cacDir = coll.transform.position - transform.position;

            if (cacDir.magnitude <= curAttRange)
            {
                //ȭ�� �߻�
                bullet = Instantiate(bulletPrefab);
                bulletCtrl = bullet.GetComponent<BulletCtrl>();
                bulletCtrl.BulletSpawn(transform.position, cacDir.normalized, curAttDamage
                                        , base.ally_Attribute.attackEff, this);
                bullet.transform.right = new Vector3(cacDir.normalized.x, cacDir.normalized.y, 0.0f);

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
        //���� ���� ��ų ����
        Debug.Log("���� ��ų �ߵ�!");

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
                //������ �ο�
                coll.GetComponent<Monster_Ctrl>().TakeDamage(skillDamage, this.gameObject);

                //����Ʈ ����
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
}
