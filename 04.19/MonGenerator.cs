using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonGenerator : MonoBehaviour
{
    public GameObject MonsterPrefab;

    Transform monGenerator = null;

    int monMaxCount = 50;
    int monCurCount = 50;

    
    // Start is called before the first frame update
    void Start()
    {
        //monGenerator = FindObjectOfType<MonGenerator>();
        monGenerator = gameObject.GetComponent<Transform>();


        if (MonsterPrefab != null)
        {
            for (int i = 0; i < monMaxCount; i++)
            {
                
                GameObject monster = Instantiate(MonsterPrefab);
                monster.transform.SetParent(monGenerator, false);
                int ranX = Random.Range(-95, 95);
                int ranZ = Random.Range(-95, 95);
                monster.transform.position = new Vector3((int)ranX, 1, (int)ranZ);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //MonsterRespawn();
    }

    void MonsterRespawn()
    {
        if (monGenerator == null) return;

        MonsterCtrl[] monsterCount = monGenerator.GetComponentsInChildren<MonsterCtrl>();
        //Debug.Log(monsterCount.Length);
        monCurCount = monsterCount.Length;

        if (monsterCount.Length == monMaxCount) return;
        
        if (monCurCount <= monMaxCount)
        {
            GameObject monster = Instantiate(MonsterPrefab);
            monster.transform.SetParent(monGenerator, false);
            int ranX = Random.Range(-95, 95);
            int ranZ = Random.Range(-95, 95);
            monster.transform.position = new Vector3((int)ranX, 1, (int)ranZ);
        }
    }
}
