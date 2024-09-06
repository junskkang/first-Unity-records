using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject[] prefabMonster;

    float timeSpawn = 0.0f; //���� �ֱ�
    float diffSpawn = 1.0f; //���̵��� ���� �����ֱ� ��ȭ��

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSpawn -= Time.deltaTime;
        if (timeSpawn <= 0.0f)
        {
            timeSpawn = diffSpawn;  //diffSpawn ����ŭ �ֱⰡ �����ǰ� ��
            int monIdx = Random.Range(0, 2); 
            GameObject monster = Instantiate(prefabMonster[monIdx]);

            //�ʱ���ġ ����ֱ�
            Vector3 spawnPos = Vector3.zero;
            int side = Random.Range(0, 4); //0���� ���ʴ�� �¿��ϻ�
            switch (side)
            {
                case 0: //����
                    spawnPos = new Vector3(
                        Random.Range(CameraResolution.minVtW.x - 2.0f, CameraResolution.minVtW.x - 0.5f),
                        Random.Range(CameraResolution.minVtW.y - 2.0f, CameraResolution.maxVtW.y + 2.0f), 0.0f
                        );
                    break;
                case 1: //������
                    spawnPos = new Vector3(
                        Random.Range(CameraResolution.maxVtW.x + 0.5f, CameraResolution.maxVtW.x + 2.0f),
                        Random.Range(CameraResolution.minVtW.y - 2.0f, CameraResolution.maxVtW.y + 2.0f), 0.0f
                        );
                    break;
                case 2: //�Ʒ���
                    spawnPos = new Vector3(
                        Random.Range(CameraResolution.minVtW.x - 2.0f, CameraResolution.maxVtW.x + 2.0f),
                        Random.Range(CameraResolution.minVtW.y - 2.0f, CameraResolution.minVtW.y - 0.5f), 0.0f
                        );
                    break;
                case 3: //����
                    spawnPos = new Vector3(
                        Random.Range(CameraResolution.minVtW.x - 2.0f, CameraResolution.maxVtW.x + 2.0f),
                        Random.Range(CameraResolution.maxVtW.y + 0.5f, CameraResolution.maxVtW.y + 2.0f), 0.0f
                        );
                    break;
            }

            monster.transform.position = spawnPos;
            //monster.transform.
        }
    }
}
