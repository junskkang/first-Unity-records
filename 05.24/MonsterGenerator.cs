using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject[] MonPrefab;

    float spawnDelta = 0.0f;
    float diffSpawn = 1.5f;         //난이도에 따른 몬스터 스폰 주기 변수

    //싱글턴 패턴
    public static MonsterGenerator Inst = null;

    void Awake()
    {
        Inst = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MonPrefab == null) return;

        spawnDelta -= Time.deltaTime;
        if (spawnDelta <= 0.0f)
        {
            spawnDelta = diffSpawn;
            GameObject go = null;
            int ran = Random.Range(0, 10);
            if (ran < 6)
            {
                go = Instantiate(MonPrefab[0]);
                go.transform.SetParent(transform);
                float posY = Random.Range(-7.5f, 7.5f);
                go.transform.position = new Vector3(20.0f, posY, go.transform.position.z);
            }
            else if (ran < 9)
            {
                go = Instantiate(MonPrefab[1]);
                go.transform.SetParent(transform);
                float posY = Random.Range(-7.5f, 7.5f);
                go.transform.position = new Vector3(20.0f, posY, go.transform.position.z);
            }
            else if (ran == 9)
            {
                MonsterCtrl[] existBoss = this.gameObject.GetComponentsInChildren<MonsterCtrl>();
                for (int i = 0; i < existBoss.Length; i++)
                {
                    if (existBoss[i].name.Contains("Boss") == true)
                    {
                        spawnDelta = 0.0f;
                        return;
                    }                      
                }

                go = Instantiate(MonPrefab[2]);
                go.transform.SetParent(transform);
                go.transform.position = new Vector3(13.0f, 5.0f, transform.position.z);
            }
        }
    }
}
