using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HeroCtrl : MonoBehaviour
{
    //�̵����� ����
    float hAxis;
    float vAxis;
    float moveSpeed = 10.0f;
    Vector3 moveVec;

    //ü�¹ٰ��� ����
    public Image HpBar;
    float curHp = 300.0f;
    float maxHp = 300.0f;

    //�Ѿ� ���� ���� ����
    float attackSpeed = 0.2f;
    float attackTick = 0.0f;
    //float attackRange = 40.0f;
    bool isAttack;
    public GameObject bulletPrefab;
    public Transform bulletPool;
    public GameObject shootPos = null;

    Vector3 HalfSize = Vector3.zero;

    //GameManager damageManager = null;

    //��ų���� ����
    bool isNum1;
    public GameObject skill1 = null;
    float skill1Motion = 0.0f;
    float skill1Cool = 0.0f;
    float skill1Count = 5.0f;
    public Image skill1Icon;
    public Image skill1CoolUI;
    bool isSkill1Possible = true;
    float healHp = 0.0f;

    bool isNum2;
    public GameObject skill2 = null;
    float skill2Cool = 0.0f;
    float skill2Count = 20.0f;
    public Image skill2Icon;
    public Image skill2CoolUI;
    bool isSkill2Possible = true;
    Vector3 skill2StartPos = Vector3.zero;
    Vector3 skill2EndPos = new Vector3(42.0f, 0.0f, 0.0f);
    float skill2MoveSpeed = 20.0f;


    bool isNum3;
    public GameObject skill3 = null;
    float skill3Motion = 0.0f;
    float skill3Cool = 0.0f;
    float skill3Count = 15.0f;
    public Image skill3Icon;
    public Image skill3CoolUI;
    bool isSkill3Possible = true;





    void Start()
    {
        Time.timeScale = 1.0f;
        //ĳ������ ������ ���ϱ� (���忡 �׷��� ��������Ʈ ������ ���ؿ���)
        SpriteRenderer sprRend = gameObject.GetComponentInChildren<SpriteRenderer>();
        //sprRend.bounds.size.x ��������Ʈ�� ���� ������
        //sprRend.bounds.size.y ��������Ʈ�� ���� ������
        HalfSize.x = sprRend.bounds.size.x / 2.0f - 0.23f;  //���� �̹����� ���� ������ ��������� ����
        HalfSize.y = sprRend.bounds.size.y / 2.0f - 0.05f;  //���� �̹����� ���� ������ ��������� ����
        HalfSize.z = 1.0f;

        //damageManager = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        PlayerHUD();
        Attack();
        Skill1();
        Skill2();
        Skill3();
    }

    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        isAttack = Input.GetMouseButton(0);
        isNum1 = Input.GetButtonDown("Skill1");
        isNum2 = Input.GetButtonDown("Skill2");
        isNum3 = Input.GetButtonDown("Skill3");
    }


    void Move()
    {      
        if (hAxis != 0 || vAxis != 0)
        {
            moveVec = new Vector3(hAxis, vAxis, transform.position.z);
            if(1.0f < moveVec.magnitude)
                moveVec.Normalize();

            transform.position += moveVec * moveSpeed * Time.deltaTime;
        }
            

        #region ȭ��� �̵�����
        //ȭ�� �� �̵�����
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        //if (pos.x < 0.02f) pos.x = 0.02f;
        //if (0.98f < pos.x) pos.x = 0.98f;
        //if (pos.y < 0.1f) pos.y = 0.1f;
        //if (0.9f < pos.y) pos.y = 0.9f;

        pos.x = Mathf.Clamp(pos.x, 0.03f, 0.97f);
        pos.y = Mathf.Clamp(pos.y, 0.1f, 0.93f);

        Vector3 posLimit = Camera.main.ViewportToWorldPoint(pos);
        posLimit.z = transform.position.z;
        transform.position = posLimit;
        #endregion
    }

    void PlayerHUD()
    {
        //ü�¹� ������ UI
        if (curHp <= 0.0f)
            return;
        else        
        {
            if (HpBar != null)
                HpBar.fillAmount = curHp / maxHp;
        }
    }

    void Attack()
    {
        if (isAttack == false) return;

        attackTick -= Time.deltaTime;

        if (attackTick <= 0.0f)
        {
            BulletFire();
            attackTick = attackSpeed;
        }
    }

    void BulletFire()
    {
        //GameObject obj = Resources.Load("Bullet") as GameObject;
        GameObject obj = Instantiate(bulletPrefab);
        obj.transform.SetParent(bulletPool);
        BulletCtrl bulletCtrl = obj.GetComponent<BulletCtrl>();
        bulletCtrl.BulletFire(shootPos.transform.position);
    }

    void Skill1()
    {
        //1�� ü��20%ȸ�� ��ų
        if (isNum1)
        {
            if (skill1 != null && curHp < maxHp && isSkill1Possible == true)
            {
                skill1.SetActive(true);

                healHp = maxHp * 20 / 100;

                Debug.Log(healHp);
                if (curHp + healHp >= maxHp)
                    healHp = maxHp - curHp;

                Debug.Log(healHp);
                curHp += healHp;

                if (GameManager.Inst != null)
                    GameManager.Inst.DamageText((int)healHp, this.transform.position, Color.green);

                if (curHp >= maxHp)
                    curHp = maxHp;

                skill1Count = skill1Cool;

                skill1CoolUI.fillAmount = 0.0f;

                isSkill1Possible = false;

                if (!isSkill1Possible)
                    skill1Icon.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
        else if (!isNum1)  //��Ÿ�� ����
        {
            if (skill1Count >= 5.0f)
            {
                skill1Count = 5.0f;

                isSkill1Possible = true;

                if (isSkill1Possible)
                    skill1Icon.color = new Color(1.0f, 1.0f, 1.0f);

                healHp = 0.0f;

                return;
            }

            skill1Count += Time.deltaTime;

            skill1CoolUI.fillAmount = skill1Count / 5;
        }


        //��ų�� SetAcitve �¿��� ����
        if (skill1.activeSelf == true)
        {
            skill1Motion += Time.deltaTime;

            if (skill1Motion >= 1.1f)
            {
                skill1.SetActive(false);
                skill1Motion = 0.0f;
            }
        }
    }

    void Skill2()
    {
        //��ų2 ����� ����
        if (isNum2)
        {
            if (skill2 != null && isSkill2Possible == true)
            {
                skill2StartPos = skill2.transform.position;

                skill2.SetActive(true);
                               

                skill2Count = skill2Cool;

                skill2CoolUI.fillAmount = 0.0f;

                isSkill2Possible = false;

                if (!isSkill2Possible)
                    skill2Icon.color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
        else if (!isNum2)  //��Ÿ�� ����
        {
            if (skill2Count >= 20.0f)
            {
                skill2Count = 20.0f;

                isSkill2Possible = true;

                if (isSkill2Possible)
                    skill2Icon.color = new Color(1.0f, 1.0f, 1.0f);

                return;
            }

            skill2Count += Time.deltaTime;

            skill2CoolUI.fillAmount = skill2Count / 20;
        }


        //��ų�� SetAcitve �¿��� ����
        if (skill2.activeSelf == true)
        {
            skill2.transform.position += Vector3.right * skill2MoveSpeed * Time.deltaTime;

            if (skill2.transform.position.x >= skill2EndPos.x - 0.1f)
            {
                skill2.SetActive(false);
                skill2.transform.position = skill2StartPos;
            }
        }
    }

    void Skill3()
    {
        //3�� ü��20%ȸ�� ��ų
        if (isNum3)
        {
            if (skill3 != null && isSkill3Possible == true)
            {
                skill3.SetActive(true);

                
                
                skill3Count = skill3Cool;

                skill3CoolUI.fillAmount = 0.0f;

                isSkill3Possible = false;

                if (!isSkill3Possible)
                    skill3Icon.color = new Color(0.0f, 0.5f, 0.5f);
            }
        }
        else if (!isNum3)  //��Ÿ�� ����
        {
            if (skill3Count >= 15.0f)
            {
                skill3Count = 15.0f;

                isSkill3Possible = true;

                if (isSkill3Possible)
                    skill3Icon.color = new Color(0.0f, 1.0f, 1.0f);

                return;
            }

            skill3Count += Time.deltaTime;

            skill3CoolUI.fillAmount = skill3Count / 15;
        }


        //��ų�� SetAcitve �¿��� ����
        if (skill3.activeSelf == true)
        {
            skill3Motion += Time.deltaTime;

            if (skill3Motion >= 3.0f)
            {
                
                skill3.SetActive(false);
                skill3Motion = 0.0f;
            }
        }
    }

    void LimitMove()
    {
        Vector3 m_CacCurPos = transform.position;

        if(m_CacCurPos.x < CameraResolution.m_ScWMin.x + HalfSize.x)    //ȭ���� ���ʳ�
            m_CacCurPos.x = CameraResolution.m_ScWMin.x + HalfSize.x;

        if (CameraResolution.m_ScWMax.x - HalfSize.x < m_CacCurPos.x)   //ȭ���� �����ʳ�
            m_CacCurPos.x = CameraResolution.m_ScWMax.x - HalfSize.x;
            
        if (m_CacCurPos.y < CameraResolution.m_ScWMin.y + HalfSize.y)   //ȭ���� �Ʒ��ʳ�
            m_CacCurPos.y = CameraResolution.m_ScWMin.y + HalfSize.y;

        if (CameraResolution.m_ScWMax.y - HalfSize.y < m_CacCurPos.y)   //ȭ���� ���ʳ�
            m_CacCurPos.y = CameraResolution.m_ScWMax.y - HalfSize.y;

        transform.position = m_CacCurPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Monster") == true)
        {            
            if (collision.gameObject.GetComponent<MonsterCtrl>().type == MonsterType.Boss && skill3.activeSelf == false)
                TakeDamage(100);
            if (collision.gameObject.GetComponent<MonsterCtrl>().type == MonsterType.Zombi && skill3.activeSelf == false)
                TakeDamage(50);
            if (collision.gameObject.GetComponent<MonsterCtrl>().type == MonsterType.Missile && skill3.activeSelf == false)
                TakeDamage(80);

            if (collision.gameObject.GetComponent<MonsterCtrl>().type == MonsterType.Boss) return;

            Destroy(collision.gameObject);
        }

        if (collision.gameObject.name.Contains("Coin") == true)
        {
            Destroy(collision.gameObject);

            GameManager.Inst.curGold += 100;

            Debug.Log(GameManager.Inst.curGold);
        }

        if (collision.gameObject.name.Contains("Enemy") == true && skill3.activeSelf == false)    //���� �� �Ѿ�
        {
            TakeDamage(20.0f);
            Destroy(collision.gameObject);
        }
    }

    public void TakeDamage(float value)
    {
        if (curHp <= 0.0f) return;

        value = -(value);
        curHp += value;

        if (GameManager.Inst != null)
            GameManager.Inst.DamageText((int)value, this.transform.position, Color.blue);

        if (curHp <= 0.0f)
            curHp = 0.0f;

        if (HpBar != null)
            HpBar.fillAmount = curHp / maxHp;

        if (curHp <= 0.0f)
        {
            curHp = 0;
            Time.timeScale = 0.0f;
        }
    }
}
