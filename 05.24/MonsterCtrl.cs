using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterType
{
    Zombi, Missile, Boss
}

public class MonsterCtrl : MonoBehaviour
{
    public MonsterType type;
    
    public GameObject shootPos = null;

    public Image HpBar;
    float curHp = 200.0f;
    float maxHp = 200.0f;

    float moveSpeed = 6.0f;
    Vector3 moveVec = Vector3.zero;
    Vector3 moveDir = Vector3.zero;
    Vector3 spawnPos = Vector3.zero;

    float cacPosY = 0.0f;   //이동용 Sin함수에 들어갈 각도 계산용 변수
    float ranY = 0.0f;      //랜덤한 진폭값 저장용 변수
    float cycleSpeed = 0.0f;//랜덤한 진동 속도 변수

    public GameObject CoinPrefab;
    HeroCtrl refHero = null;

    DamageManager damageManager = null;

    Vector3 bossMoveY = Vector3.zero;
    bool isBossUp = true;
    float bossMove = 1.2f;
    float angryBossTime = 5.0f;
    float countDownAngry = 0.0f;
    Color angryColor = new Color(1.0f, 0.3f, 0.3f, 1.0f);
    SpriteRenderer bossColor;


    void Start()
    {
        spawnPos = transform.position;
        bossMoveY = new Vector3(transform.position.x, 6.5f, transform.position.z);
        ranY = Random.Range(0.5f, 2.0f);    //Sin함수의 랜덤 진폭
        cycleSpeed = Random.Range(1.8f, 5.0f);  //진동수 랜덤값
        refHero = GameObject.FindObjectOfType<HeroCtrl>();
        damageManager = GameObject.FindObjectOfType<DamageManager>();

        if( bossColor == null)
            bossColor = this.gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    
    void Update()
    {
        if (type == MonsterType.Zombi)
        {
            ZombiAIUpdate();
        }
        else if (type == MonsterType.Missile)
        {
            MissileAIUpdate();            
        }
        else if (type == MonsterType.Boss)
        {            
            BossAIUpdate();
        }


        if (transform.position.x < CameraResolution.m_ScWMin.x - 2.0f) Destroy(gameObject);
    }

    void BossAIUpdate()
    {
        //if (isBossUp == true)
        //{
        //    if(transform.localPosition.y < bossMoveY.y)
        //        transform.localPosition = Vector3.MoveTowards(spawnPos, bossMoveY, bossMove * Time.deltaTime);
        //    if (transform.localPosition.y >= bossMoveY.y - 0.2f)
        //        isBossUp = false;
        //}
        //else if (isBossUp == false)
        //{
        //    if(spawnPos.y < transform.localPosition.y)
        //        transform.localPosition = Vector3.MoveTowards(bossMoveY, spawnPos, bossMove * Time.deltaTime);
        //    if (transform.localPosition.y <= spawnPos.y + 0.1f)
        //        isBossUp = true;
        //}
        countDownAngry += Time.deltaTime;
        if (countDownAngry >= angryBossTime)
        {
            countDownAngry = angryBossTime;

            bossColor.color = angryColor;
            bossMove = 3.0f;
        }

        if (isBossUp == true)
        {
            if (transform.position.y < bossMoveY.y)
                transform.position = Vector3.MoveTowards(transform.position, bossMoveY, bossMove * Time.deltaTime);
            if (transform.position.y >= bossMoveY.y)
                isBossUp = false;
        }
        else if (isBossUp == false)
        {
            if (spawnPos.y < transform.position.y)
                transform.position = Vector3.MoveTowards(transform.position, spawnPos, bossMove * Time.deltaTime);
            if (transform.position.y <= spawnPos.y)
                isBossUp = true;
        }


    }
    void MissileAIUpdate()
    {
        moveVec = transform.position;
        moveVec = Vector3.left * moveSpeed / 2.2f * Time.deltaTime;
        transform.position += moveVec;
        moveDir = refHero.transform.position - transform.position;
        
        if (moveDir.magnitude < 10.0f)
        {
            if (moveDir.x >= 0) return;

            moveVec = moveDir.normalized;
            transform.position += moveVec * moveSpeed * 1.7f * Time.deltaTime;
        }

    }

    void ZombiAIUpdate()
    {
        moveVec = transform.position;
        moveVec += Vector3.left * moveSpeed * Time.deltaTime;
        cacPosY += (Time.deltaTime * cycleSpeed);
        moveVec.y = spawnPos.y + Mathf.Sin(cacPosY) * ranY;
        transform.position = moveVec;

        if (transform.position.y < CameraResolution.m_ScWMin.y + 2.0f)   //화면의 아래쪽끝
            moveVec.y = CameraResolution.m_ScWMin.y + 2.0f;

        if (CameraResolution.m_ScWMax.y - 2.0f < transform.position.y)   //화면의 위쪽끝
            moveVec.y = CameraResolution.m_ScWMax.y - 2.0f;

        transform.position = moveVec;
    }

    public void TakeDamage(float value)
    {
        if (curHp <= 0.0f) return;

        if (type == MonsterType.Missile)
        {
            value = value / 3 * 2;
        }
        else if (type == MonsterType.Boss)
        {
            value = value / 4;
        }

        curHp -= value;

        if (damageManager != null)
            damageManager.DamageText((int)value, this.transform.position, Color.red);

        if (curHp <= 0.0f)
            curHp = 0.0f;   

        if(HpBar != null)
            HpBar.fillAmount = curHp / maxHp;

        if (curHp <= 0.0f)
        {
            Destroy(gameObject);
            GoldDrop();
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "AllyBullet")
        {
            TakeDamage(80.0f);
            Destroy(coll.gameObject);
        }
    }

    void GoldDrop()
    {
        if (CoinPrefab != null)
        {
            GameObject go = Instantiate(CoinPrefab);
            go.transform.position = transform.position;
        }
    }

}
