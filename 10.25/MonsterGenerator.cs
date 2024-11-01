using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterGenerator : MonoBehaviour
{
    public GameObject[] prefabMonster;
    public Transform startPos;
    Animator anim;
    public GameObject warningEff;

    float timeSpawn = 0.0f; //스폰 주기
    float diffSpawn = 30.0f; //난이도에 따른 스폰주기 변화용
    public Text timerText;

    int monsterCount = 15;
    [HideInInspector] public int curMonCount = 0;
    bool createDone = false;

    public static MonsterGenerator inst;

    private void Awake()
    {
        inst = this;
        anim = GetComponentInChildren<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        timeSpawn = 5.0f;
        createDone = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (curMonCount == 0 && timeSpawn > 10.0f && createDone == true)
            timeSpawn = 5.0f;

        timeSpawn -= Time.deltaTime;

        if (timeSpawn <= 3.0f && !warningEff.gameObject.activeSelf)
            warningEff.gameObject.SetActive(true);

        if (timeSpawn <= 0.0f && createDone)
        {
            GameManager.Inst.round++;
            createDone = false;
            anim.SetTrigger("doorOpen");
            //StartCoroutine(MonsterCreate());
            timeSpawn = 0.0f;  //diffSpawn 값만큼 주기가 생성되게 됨
            
        }

        if (timeSpawn > 0.0f)
            timerText.text = timeSpawn.ToString("N2");
        else
            timerText.text = $"{GameManager.Inst.round}라운드 몬스터 등장!!";

            ////초기위치 잡아주기
            //Vector3 spawnPos = Vector3.zero;
            //int side = Random.Range(0, 4); //0부터 차례대로 좌우하상
            //switch (side)
            //{
            //    case 0: //왼쪽
            //        spawnPos = new Vector3(
            //            Random.Range(CameraResolution.minVtW.x - 2.0f, CameraResolution.minVtW.x - 0.5f),
            //            Random.Range(CameraResolution.minVtW.y - 2.0f, CameraResolution.maxVtW.y + 2.0f), 0.0f
            //            );
            //        break;
            //    case 1: //오른쪽
            //        spawnPos = new Vector3(
            //            Random.Range(CameraResolution.maxVtW.x + 0.5f, CameraResolution.maxVtW.x + 2.0f),
            //            Random.Range(CameraResolution.minVtW.y - 2.0f, CameraResolution.maxVtW.y + 2.0f), 0.0f
            //            );
            //        break;
            //    case 2: //아래쪽
            //        spawnPos = new Vector3(
            //            Random.Range(CameraResolution.minVtW.x - 2.0f, CameraResolution.maxVtW.x + 2.0f),
            //            Random.Range(CameraResolution.minVtW.y - 2.0f, CameraResolution.minVtW.y - 0.5f), 0.0f
            //            );
            //        break;
            //    case 3: //위쪽
            //        spawnPos = new Vector3(
            //            Random.Range(CameraResolution.minVtW.x - 2.0f, CameraResolution.maxVtW.x + 2.0f),
            //            Random.Range(CameraResolution.maxVtW.y + 0.5f, CameraResolution.maxVtW.y + 2.0f), 0.0f
            //            );
            //        break;
            //}

    }

    public void CoOpenDoor()
    {
        StartCoroutine(MonsterCreate());
    }

    IEnumerator MonsterCreate()
    {
        

        if (timeSpawn <= 0.0f)
        {
            for (int i = 0; i < monsterCount; i++)
            {
                int monIdx = Random.Range(0, 2);
                GameObject monster = Instantiate(prefabMonster[monIdx]);

                monster.transform.position = startPos.position;
                monster.transform.SetParent(this.transform);
                curMonCount++;

                yield return new WaitForSeconds(0.8f);               
            }                    
        }

        timeSpawn = diffSpawn;
        createDone = true;
        anim.SetTrigger("doorClose");
        warningEff.gameObject.SetActive(false);
    }
}
