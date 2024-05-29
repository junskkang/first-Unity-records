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

    HeroCtrl refHero = null;
    public Transform bulletPool;
    public GameObject CoinPrefab;

    //총알 발사 변수
    public GameObject shootPos = null;
    public GameObject bulletPrefab;
    float shootTime = 0.0f;     //발사 주기용 변수
    float shootDelay = 2.5f;    //발사 쿨타임
    float shootSpeed = 10.0f;   //총알 이동속도

    //GameManager damageManager = null;

    //보스 관련 변수
    Vector3 bossMoveY = Vector3.zero;
    bool isBossUp = true;
    float bossMove = 1.2f;
    float angryBossTime = 20.0f;
    float countDownAngry = 0.0f;
    bool isBossAngry = false;
    Color angryColor = new Color(1.0f, 0.3f, 0.3f, 1.0f);
    SpriteRenderer bossColor;
    float bossAttTime = 3.0f;
    float bossGatling = 0.3f;
    float bossGatlingTime = 0.0f;
    int gatlingCount = 0;
    int gatlingCountMax = 5;
    float bossDelay = 5.0f;
    int bossPattern = 0;
    int pattern1Count = 0;
    float roundAtt = 0.5f;
    float roundAttTime = 0.0f;
    float roundCount = 0;



    void Start()
    {
        spawnPos = transform.position;
        bossMoveY = new Vector3(transform.position.x, 6.5f, transform.position.z);
        ranY = Random.Range(0.5f, 2.0f);    //Sin함수의 랜덤 진폭
        cycleSpeed = Random.Range(1.8f, 5.0f);  //진동수 랜덤값
        refHero = GameObject.FindObjectOfType<HeroCtrl>();
        //damageManager = GameObject.FindObjectOfType<GameManager>();

        if( bossColor == null)
            bossColor = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        if (type == MonsterType.Boss)
        {
            curHp = 500.0f;
            maxHp = 500.0f;
        }
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
        //광폭화
        if (isBossAngry == false)
            countDownAngry += Time.deltaTime;

        if (countDownAngry >= angryBossTime)
        {
            countDownAngry = angryBossTime;

            isBossAngry = true;
            bossColor.color = angryColor;
            bossMove = 3.0f;
            bossDelay = 3.0f;
            gatlingCountMax = 7;
            bossGatling = 0.2f;
            shootSpeed = 25.0f;
        }
        
        //보스 움직임
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

        //총알 발사
        if (bulletPrefab == null) return;

        bossAttTime += Time.deltaTime;
        if (bossDelay <= bossAttTime)
        {
            if (bossPattern == 0)       //플레이어를 향한 개틀링 총알발사
            {
                bossGatlingTime += Time.deltaTime;

                if (bossGatling <= bossGatlingTime)
                {
                    bossGatlingTime = 0.0f;
                    Vector3 toPlayerVec = refHero.transform.position - this.transform.position;
                    toPlayerVec.Normalize();
                    GameObject go = Instantiate(bulletPrefab);
                    go.transform.SetParent(bulletPool);
                    BulletCtrl bulletCtrl = go.GetComponent<BulletCtrl>();
                    bulletCtrl.BulletSpawn(shootPos.transform.position, toPlayerVec, shootSpeed);
                    gatlingCount++;

                    if (gatlingCount >= gatlingCountMax)
                    {
                        gatlingCount = 0;
                        bossAttTime = 0.0f;
                        pattern1Count++;
                    }

                    if (pattern1Count == 3) //개틀링 패턴 3번후 다음 패턴 ㄱ
                    {
                        pattern1Count = 0;
                        bossPattern = 1;
                    }
                }
            }
            else if (bossPattern == 1)
            {
                roundAttTime += Time.deltaTime;
                if (roundAtt <= roundAttTime)
                {
                    Vector3 roundAttack = Vector3.zero;
                    GameObject go = null;
                    BulletCtrl goBulletCtrl;
                    for (float angle = 0.0f; angle < 360.0f; angle += 15.0f)
                    {
                        roundAttack.x = Mathf.Sin(angle * Mathf.Deg2Rad);
                        roundAttack.y = Mathf.Cos(angle * Mathf.Deg2Rad);
                        roundAttack.z = 0.0f;
                        roundAttack.Normalize();

                        go = Instantiate(bulletPrefab);
                        go.transform.SetParent(bulletPool);
                        goBulletCtrl = go.GetComponent<BulletCtrl>();
                        goBulletCtrl.BulletSpawn(shootPos.transform.position, roundAttack, shootSpeed/0.6f);
                    }
                    roundCount++;
                    roundAttTime = 0.0f;

                    if (roundCount == 3)
                    {
                        roundCount = 0;
                        bossPattern = 0;
                        bossAttTime = 0.0f;
                    }
                }
            }
        }
    }
    void MissileAIUpdate()
    {
        //moveVec = transform.position;
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

        //총알 발사
        if (bulletPrefab == null) return;

        shootTime += Time.deltaTime;
        if (shootDelay <= shootTime)
        {
            shootTime = 0.0f;

            GameObject go = Instantiate(bulletPrefab);
            go.transform.SetParent(bulletPool);
            BulletCtrl bulletCtrl = go.GetComponent<BulletCtrl>();
            bulletCtrl.BulletSpawn(shootPos.transform.position, Vector3.left, shootSpeed);
        }
    }

    public void TakeDamage(float value)
    {
        if (curHp <= 0.0f) return;

        ////막타를 남은 체력만큼만 표시되게끔 하기
        //float lastPang = value;
        //if (curHp < value)
        //    lastPang = curHp;

        value = -value;

        if (type == MonsterType.Missile)
        {
            value = value / 3 * 2;
        }
        else if (type == MonsterType.Boss)
        {
            value = value / 4;

            if (isBossAngry == true)    //광폭화 들어가면 보스 방어력 약해지게
                value = value * 2;
        }

        curHp += value;

        if (GameManager.inst != null)
            GameManager.inst.DamageText((int)value, this.transform.position, Color.red);

        if (curHp <= 0.0f)
            curHp = 0.0f;   

        if(HpBar != null)
            HpBar.fillAmount = curHp / maxHp;

        if (curHp <= 0.0f)
        {
            GoldDrop1(this.transform.position, 100);
            Destroy(gameObject);
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

    public void GoldDrop1(Vector3 spawnPos, float value)
    {
        if (CoinPrefab != null)
        {
            GameObject gold = Instantiate(CoinPrefab);
            gold.transform.position = spawnPos;
        }
    }

}
