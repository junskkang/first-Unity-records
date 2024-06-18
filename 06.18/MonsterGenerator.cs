using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    public Transform[] SpawnPos;
    public GameObject monsterPrefab;
    public GameObject bombPrefab;
    public GameObject allyPrefab;
    public GameObject bossMonster;

    float spawnTime = 9.0f;

    public int killCount = 0;

    public static MonsterGenerator inst;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTime <= 9.0f)
        {
            spawnTime -= Time.deltaTime;
            if (spawnTime <= 0)
            {
                spawnTime = 0.0f;

                CardSpawn();

                spawnTime = 9.0f;
            }
        }

        if (killCount == 10)
        {
            BossSpawn();

            killCount = 0;
        }
    }

    public void CardSpawn()
    {
        if (monsterPrefab == null) monsterPrefab = Resources.Load("EnemyCardParent") as GameObject;
        if (bombPrefab == null ) bombPrefab = Resources.Load("BombCardParent") as GameObject;
        if (allyPrefab == null) allyPrefab = Resources.Load("AllyCardParent") as GameObject;

        for (int i = 0; i < SpawnPos.Length; i++)
        {
            int ran = Random.Range(0, 10);
            if (ran < 2)
            {
                GameObject go = Instantiate(bombPrefab, SpawnPos[i]);
                go.transform.position = SpawnPos[i].transform.position;
            }
            else if (ran < 6)
            {
                GameObject go = Instantiate(monsterPrefab, SpawnPos[i]);
                go.transform.position = SpawnPos[i].transform.position;
            }
            else
            {
                GameObject go = Instantiate(allyPrefab, SpawnPos[i]);
                go.transform.position = SpawnPos[i].transform.position;
            }
        }
    }

    void BossSpawn()
    {
        if (bossMonster.activeSelf == true) return;

        if (bossMonster != null)
            bossMonster.SetActive(true);

        float ranX = Random.Range(-20.0f, 20.0f);
        bossMonster.transform.position = new Vector3(ranX, 0, 15);
    }

}
