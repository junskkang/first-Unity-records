using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCtrl : MonoBehaviour
{
    //이동관련 변수
    float hAxis;
    float vAxis;
    float moveSpeed = 10.0f;
    Vector3 moveVec;

    //체력바관련 변수
    public Image HpBar;
    float curHp = 300.0f;
    float maxHp = 300.0f;

    //총알 공격 관련 변수
    float attackSpeed = 0.2f;
    float attackTick = 0.0f;
    float attackRange = 40.0f;
    bool isAttack;
    public GameObject bulletPrefab;
    public Transform bulletPool;
    public GameObject shootPos = null;

    Vector3 HalfSize = Vector3.zero;

    DamageManager damageManager = null;

    //재화 관련 변수
    int gold = 0;


        
    void Start()
    {
        Time.timeScale = 1.0f;
        //캐릭터의 사이즈 구하기 (월드에 그려진 스프라이트 사이즈 구해오기)
        SpriteRenderer sprRend = gameObject.GetComponentInChildren<SpriteRenderer>();
        //sprRend.bounds.size.x 스프라이트의 가로 사이즈
        //sprRend.bounds.size.y 스프라이트의 세로 사이즈
        HalfSize.x = sprRend.bounds.size.x / 2.0f - 0.23f;  //원본 이미지의 여백 때문에 상수값으로 보정
        HalfSize.y = sprRend.bounds.size.y / 2.0f - 0.05f;  //원본 이미지의 여백 때문에 상수값으로 보정
        HalfSize.z = 1.0f;

        damageManager = GameObject.FindObjectOfType<DamageManager>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        PlayerHUD();
        Attack();
    }

    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
        isAttack = Input.GetMouseButton(0);
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
            

        #region 화면밖 이동제어
        //화면 밖 이동금지
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
        //체력바 게이지 UI
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

    void LimitMove()
    {
        Vector3 m_CacCurPos = transform.position;

        if(m_CacCurPos.x < CameraResolution.m_ScWMin.x + HalfSize.x)    //화면의 왼쪽끝
            m_CacCurPos.x = CameraResolution.m_ScWMin.x + HalfSize.x;

        if (CameraResolution.m_ScWMax.x - HalfSize.x < m_CacCurPos.x)   //화면의 오른쪽끝
            m_CacCurPos.x = CameraResolution.m_ScWMax.x - HalfSize.x;
            
        if (m_CacCurPos.y < CameraResolution.m_ScWMin.y + HalfSize.y)   //화면의 아래쪽끝
            m_CacCurPos.y = CameraResolution.m_ScWMin.y + HalfSize.y;

        if (CameraResolution.m_ScWMax.y - HalfSize.y < m_CacCurPos.y)   //화면의 위쪽끝
            m_CacCurPos.y = CameraResolution.m_ScWMax.y - HalfSize.y;

        transform.position = m_CacCurPos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Monster") == true)
        {
            Destroy(collision.gameObject);

            if (collision.gameObject.GetComponent<MonsterCtrl>().type == MonsterType.Zombi)
                TakeDamage(50);
            if (collision.gameObject.GetComponent<MonsterCtrl>().type == MonsterType.Missile)
                TakeDamage(80);
        }

        if (collision.gameObject.name.Contains("Coin") == true)
        {
            Destroy(collision.gameObject);

            gold += 100;

            Debug.Log(gold);
        }
    }

    public void TakeDamage(float value)
    {
        if (curHp <= 0.0f) return;

        curHp -= value;

        if (damageManager != null)
            damageManager.DamageText((int)value, this.transform.position, Color.blue);

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
