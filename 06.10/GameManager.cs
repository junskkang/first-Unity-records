using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    GameIng,
    GameEnd
}

public class GameManager : MonoBehaviour
{
    public static GameState GameState = GameState.GameIng;

    [Header("UI")]
    //Text UI �׸� ������ ���� ����
    public Text txtScore;
    //���� ������ ����ϱ� ���� ����
    private int totalScore = 0;

    public Button backBtn;


    [Header("���� ���� ����")]
    //���� ���� ��ġ ���� �迭
    public Transform[] SpawnPoints = null;
    //���� ������ ����
    public GameObject monsterPrefab;
    //���͸� �̸� ������ ������ ����Ʈ �ڷ���
    public List<GameObject> monsterPoolList = new List<GameObject>();
    public Transform monsterPool;

    public float createTime = 2.0f;     //���� �߻� �ֱ�
    public int maxMonster = 10;         //�ִ� ���� ������


    public bool isGameOver = false;

    public static GameManager inst;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameState = GameState.GameIng;

        if (backBtn != null)
            backBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });
        
        //������ġ �迭�� ��ƿ�
        SpawnPoints = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        //���͸� ������ ������Ʈ Ǯ�� ����
        for (int i = 0; i < maxMonster; i++)
        {
            //���� �������� ����
            GameObject monster = (GameObject)Instantiate(monsterPrefab);
            //������ ������ �̸� ����
            monster.name = "Monster_" + i.ToString();
            //������ ���� MonsterPool�� child�� ����
            monster.transform.SetParent(monsterPool);
            //������ ���� ��Ȱ��ȭ ����
            monster.SetActive(false);
            //������ ���͸� ������Ʈ Ǯ�� �߰�
            monsterPoolList.Add(monster);
        }


        if (SpawnPoints.Length > 0)
            StartCoroutine(this.CreateMonster());   //���� ���� �ڷ�ƾ �Լ� ȣ��

        DispScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator CreateMonster()
    {
        while (!isGameOver) //���� ����ñ��� ���� ����
        {
            //������ƮǮ�� X
            ////���� ������ ���� ���� ����
            //int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;

            ////������ �ִ� ���� �������� ���� ���� ���� ���� ���� ���� ����
            //if (monsterCount < maxMonster)
            //{
            //    //���� ���� �ֱ� �ð���ŭ ���
            //    yield return new WaitForSeconds(createTime);

            //    //�ұ�Ģ���� ��ġ ����
            //    int idx = Random.Range (1, SpawnPoints.Length);
            //    //���� ���� ����
            //    Instantiate(monsterPrefab, SpawnPoints[idx].position, SpawnPoints[idx].rotation);
            //}
            //else
            //    yield return null;  //�������� ���� ��

            //������ƮǮ�� O
            
            //���� ���� �ֱ� �ð���ŭ ���� ������ �纸
            yield return new WaitForSeconds(createTime);

            //�÷��̾� ����� �ڷ�ƾ�� ������ ���� ��ƾ ����x
            if (isGameOver) yield break;

            //������Ʈ Ǯ ����Ʈ�� ó������ ������ ��ȸ
            foreach (GameObject monster in monsterPoolList)
            {
                //��Ȱ��ȭ ���η� ��� ������ ���� �Ǵ�
                if (monster.activeSelf == false)
                {
                    //���� ������ų ��ġ�� �ε����� ����
                    int idx = Random.Range(1, SpawnPoints.Length);
                    //������ ������ġ�� ����
                    monster.transform.position = SpawnPoints[idx].transform.position;
                    //������Ʈ Ǯ���� ����� ���� Ȱ��ȭ
                    monster.SetActive(true);
                    //������Ʈ Ǯ���� ���� ������ �ϳ��� Ȱ��ȭ�� �� for ������ ��������
                    break;
                }
            }
        }
    }

    public void DispScore(int score) 
    {
        totalScore += score;
        txtScore.text = "score <color=#ff0000>" + totalScore.ToString() + "</color>";
    }

    public static bool IsPointerOverUIObject() //UGUI�� UI���� ���� ��ŷ�Ǵ��� Ȯ���ϴ� �Լ�
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

			List<RaycastResult> results = new List<RaycastResult>();
			for (int i = 0; i < Input.touchCount; ++i)
			{
				a_EDCurPos.position = Input.GetTouch(i).position;  
				results.Clear();
				EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
			}

			return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject() 


}
