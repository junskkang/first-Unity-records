using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject[] MonPrefab;

    float spawnDelta = 0.0f;
    float diffSpawn = 1.0f;         //난이도에 따른 몬스터 스폰 주기 변수

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
            go = Instantiate(MonPrefab[0]);
            float posY = Random.Range(-7.5f, 7.5f);
            go.transform.position = new Vector3(20.0f, posY, go.transform.position.z);
        }

    }
}
