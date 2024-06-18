using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public enum MonsterType
{
    Normal,
    Elite,
    Boss
}

public class MonsterCtrl : MonoBehaviour
{
    public MonsterType type;

    public Transform playerTarget;

    public Image Hpbar;
    float curHp;
    float maxHp;

    //노말카드 회전 관련
    Quaternion firstRot = Quaternion.identity;
    Quaternion lastRot = new Quaternion(0, 180, 0, 0);
    float rotTimer = 0.0f;

    Vector3 spawnPos;
    //엘리트카드 움직임 관련
    Vector3 eliteMoveY = Vector3.zero;
    bool isUp = true;
    float eliteMoveSpeed = 2.0f;

    //보스카드 움직임 관련
    Vector3 bossMaxX = Vector3.zero;
    Vector3 bossMinX = Vector3.zero;
    bool isBossMove = true;
    float bossMoveSpeed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (type == MonsterType.Normal)
        {
            maxHp = 100.0f;
            curHp = maxHp;
        }
        else if (type == MonsterType.Elite)
        {
            spawnPos = transform.position;
            eliteMoveY = new Vector3(transform.position.x, 3.5f, transform.position.z);
            maxHp = 200.0f;
            curHp = maxHp;
        }
        else if (type == MonsterType.Boss)
        {
            bossMaxX = new Vector3(20.0f, transform.position.y, transform.position.z);
            bossMinX = new Vector3(-20.0f, transform.position.y, transform.position.z);
            maxHp = 500.0f;
            curHp = maxHp;

            int ran = Random.Range(0, 2); //처음 이동방향 정해줄 변수
            switch (ran)
            {
                case 0:
                    isBossMove = false; break;
                case 1:
                    isBossMove = true; break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (type == MonsterType.Normal)
        {
            StartCoroutine(NormalAI());
        }
        else if (type == MonsterType.Elite)
        {
            EliteAI();
        }
        else if (type == MonsterType.Boss)
        {
            BossAI();
        }
    }


    IEnumerator NormalAI()
    {
        Destroy(gameObject, 7.0f);

        yield return new WaitForSeconds(1.5f);

        rotTimer += Time.deltaTime;
        transform.rotation = Quaternion.Lerp(firstRot, lastRot, rotTimer / 10);
    }

    void EliteAI()
    {
        if (isUp == true)
        {
            if (transform.position.y < eliteMoveY.y)
                transform.position = Vector3.MoveTowards(transform.position, eliteMoveY, eliteMoveSpeed * Time.deltaTime);
            if (transform.position.y >= eliteMoveY.y)
                isUp = false;
        }
        else if (isUp == false)
        {
            if (spawnPos.y < transform.position.y)
                transform.position = Vector3.MoveTowards(transform.position, spawnPos, eliteMoveSpeed * Time.deltaTime);
            if (transform.position.y <= spawnPos.y)
                isUp = true;
        }
    }

    void BossAI()
    {
        if (isBossMove == true)
        {
            if (transform.position.x < bossMaxX.x)
                transform.position = Vector3.MoveTowards(transform.position, bossMaxX, bossMoveSpeed * Time.deltaTime);
            if (transform.position.x >= bossMaxX.x)
                isBossMove = false;
        }
        else if (isBossMove == false)
        {
            if (bossMinX.x < transform.position.x)
                transform.position = Vector3.MoveTowards(transform.position, bossMinX, bossMoveSpeed * Time.deltaTime);
            if (transform.position.x <= bossMinX.x)
                isBossMove = true;
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "AllyBam")
        {
            TakeDamage(50.0f);

            Destroy(coll.gameObject, 0.7f);
        }
    }

    void TakeDamage(float damage)
    {
        if (curHp <= 0.0f) return;

        curHp -= damage;

        if (curHp <= 0.0f)
            curHp = 0.0f;

        if (Hpbar != null)
            Hpbar.fillAmount = curHp / maxHp;

        if (curHp <= 0.0f)
        {
            //보상
            if (type == MonsterType.Normal)
            {
                GameManager.inst.AddScore(20);
                MonsterGenerator.inst.killCount += 1;
            }                
            else if (type == MonsterType.Elite)
                GameManager.inst.AddScore(50);
            else if (type == MonsterType.Boss)
                GameManager.inst.AddScore(300);

            //몬스터 제거
            Destroy(gameObject);
        }

    }
}
