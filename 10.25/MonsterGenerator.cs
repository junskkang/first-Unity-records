using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject[] prefabMonster;
    public Transform startPos;

    float timeSpawn = 0.0f; //���� �ֱ�
    float diffSpawn = 30.0f; //���̵��� ���� �����ֱ� ��ȭ��
    public Text timerText;

    int monsterCount = 15;
    [HideInInspector] public int curMonCount = 0;
    bool createDone = false;

    public static MonsterGenerator inst;

    private void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (curMonCount == 0 && timeSpawn > 10.0f && createDone == true)
            timeSpawn = 10.0f;

        timeSpawn -= Time.deltaTime;

        if (timeSpawn <= 0.0f)
        {
            GameManager.Inst.round++;
            StartCoroutine(MonsterCreate());
            timeSpawn = diffSpawn;  //diffSpawn ����ŭ �ֱⰡ �����ǰ� ��            
        }

        timerText.text = timeSpawn.ToString("N2");
            ////�ʱ���ġ ����ֱ�
            //Vector3 spawnPos = Vector3.zero;
            //int side = Random.Range(0, 4); //0���� ���ʴ�� �¿��ϻ�
            //switch (side)
            //{
            //    case 0: //����
            //        spawnPos = new Vector3(
            //            Random.Range(CameraResolution.minVtW.x - 2.0f, CameraResolution.minVtW.x - 0.5f),
            //            Random.Range(CameraResolution.minVtW.y - 2.0f, CameraResolution.maxVtW.y + 2.0f), 0.0f
            //            );
            //        break;
            //    case 1: //������
            //        spawnPos = new Vector3(
            //            Random.Range(CameraResolution.maxVtW.x + 0.5f, CameraResolution.maxVtW.x + 2.0f),
            //            Random.Range(CameraResolution.minVtW.y - 2.0f, CameraResolution.maxVtW.y + 2.0f), 0.0f
            //            );
            //        break;
            //    case 2: //�Ʒ���
            //        spawnPos = new Vector3(
            //            Random.Range(CameraResolution.minVtW.x - 2.0f, CameraResolution.maxVtW.x + 2.0f),
            //            Random.Range(CameraResolution.minVtW.y - 2.0f, CameraResolution.minVtW.y - 0.5f), 0.0f
            //            );
            //        break;
            //    case 3: //����
            //        spawnPos = new Vector3(
            //            Random.Range(CameraResolution.minVtW.x - 2.0f, CameraResolution.maxVtW.x + 2.0f),
            //            Random.Range(CameraResolution.maxVtW.y + 0.5f, CameraResolution.maxVtW.y + 2.0f), 0.0f
            //            );
            //        break;
            //}

    }

    IEnumerator MonsterCreate()
    {
        createDone = false;

        if (timeSpawn <= 0.0f)
        {
            for (int i = 0; i < monsterCount; i++)
            {
                int monIdx = Random.Range(0, 2);
                GameObject monster = Instantiate(prefabMonster[monIdx]);

                monster.transform.position = startPos.position;
                monster.transform.SetParent(this.transform);
                curMonCount++;

                yield return new WaitForSeconds(1f);               
            }                    
        }

        createDone = true;
    }
}
