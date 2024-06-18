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
    public Material[] textures;
    public GameObject bamsongiPrefab;
    public PlayerCtrl playerTarget;

    public GameObject[] eliteMonster;

    public Image Hpbar;
    float curHp;
    float maxHp;

    //노말카드 회전 관련
    Quaternion firstRot = Quaternion.identity;
    Quaternion lastRot = new Quaternion(0, -180, 0, 0);
    float rotTimer = 0.0f;

    Vector3 spawnPos;
    //엘리트카드 움직임 관련
    Vector3 eliteMoveY = Vector3.zero;
    bool isUp = true;
    float eliteMoveSpeed = 2.0f;
    int attackCount = 0;

    //보스카드 움직임 관련
    Vector3 bossMaxX = Vector3.zero;
    Vector3 bossMinX = Vector3.zero;
    bool isBossMove = true;
    float bossMoveSpeed = 3.0f;


    //공격관련 변수
    float attackTime = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        playerTarget = GameObject.FindObjectOfType<PlayerCtrl>();

        if (type == MonsterType.Normal)
        {
            Destroy(gameObject, 7.0f);
            int idx = Random.Range(0, textures.Length);
            GetComponentInChildren<MeshRenderer>().material = textures[idx];

            maxHp = 100.0f;
            curHp = maxHp;
            attackTime = 3.5f;
        }
        else if (type == MonsterType.Elite)
        {
            spawnPos = transform.position;
            eliteMoveY = new Vector3(transform.position.x, 12.5f, transform.position.z);
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

    private void OnEnable()
    {

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

        Attack();
    }

    void Attack()
    {
        attackTime -= Time.deltaTime;
        if (attackTime < 0)
        {
            if (type == MonsterType.Normal)
            {

                FireBam();
                attackTime = 2.0f;
            }            
            else if (type == MonsterType.Elite)
            {
                FireBam();
                attackCount++;
                attackTime = 0.5f;

                if (attackCount == 2)
                {
                    attackTime = 6.0f;
                    attackCount = 0;
                }
            }
            else if (type == MonsterType.Boss)
            {               
                FireBam();
                attackTime = 5.0f;

                int ran = Random.Range(0, 4);
                if (ran < 2)
                {
                    for (int i = 0; i < eliteMonster.Length; i++)
                    {
                        if (eliteMonster[i].activeSelf == true) continue;

                        if (eliteMonster[i] != null)
                            eliteMonster[i].SetActive(true);
                    }                       
                }
            }
        } 
    }

    IEnumerator NormalAI()
    {
        

        yield return new WaitForSeconds(1.5f);


        rotTimer += Time.deltaTime;

        transform.rotation = Quaternion.Lerp(firstRot, lastRot, rotTimer / 10);

        if (rotTimer >= 10.0f)
        {
            rotTimer = 10.0f;
        }


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

    void FireBam()
    {        
        if (type == MonsterType.Boss)
        {
            
            Vector3 pos = new Vector3 (0.0f, transform.position.y + 5.0f, transform.position.z -1.0f);
            for (float xx = -10; xx <= 10; xx += 5)
            {                
                pos.x = xx;
                
                GameObject bamsongi = Instantiate (bamsongiPrefab);
                bamsongi.transform.position = pos;

                Vector3 toPlayerDir = playerTarget.transform.position - bamsongi.transform.position;
                toPlayerDir.y += 5.0f;
                toPlayerDir.Normalize();
                bamsongi.GetComponent<BamsongiController>().Shoot(toPlayerDir * 2000);
            }
        }
        else
        {
            GameObject bamsongi = Instantiate(bamsongiPrefab);
            bamsongi.transform.position = new Vector3(transform.position.x, transform.position.y + 3.5f, transform.position.z - 1.0f);
            Vector3 toPlayerDir = playerTarget.transform.position - transform.position;
            toPlayerDir.y += 5.0f;
            toPlayerDir.Normalize();

            if (type == MonsterType.Normal)
            {
                bamsongi.GetComponent<BamsongiController>().Shoot(toPlayerDir * 1000);
            }
            else if (type == MonsterType.Elite)
            {
                bamsongi.GetComponent<BamsongiController>().Shoot(toPlayerDir * 1500);
            }
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
                Destroy(gameObject);
            }                
            else if (type == MonsterType.Elite)
            {
                GameManager.inst.AddScore(50);
                
                gameObject.SetActive(false);
                
            }
            else if (type == MonsterType.Boss)
            {
                GameManager.inst.AddScore(300);
                
                gameObject.SetActive(false);
                
            }             
            
        }

    }

    private void OnDisable()
    {
        if (type == MonsterType.Elite || type == MonsterType.Boss)
        {
            curHp = maxHp;

            if (Hpbar != null)
                Hpbar.fillAmount = curHp / maxHp;

            attackTime = 2.0f;
        }
    }
}
