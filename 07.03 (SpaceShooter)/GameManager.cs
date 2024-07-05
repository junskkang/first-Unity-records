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

public enum PlayerCharacter
{
    Player1,
    Player2
}

public class GameManager : MonoBehaviour
{
    public static GameState GameState = GameState.GameIng;
    public static PlayerCharacter playerCharacter = PlayerCharacter.Player1;

    [Header("UI")]
    //Text UI �׸� ������ ���� ����
    public Text txtScore;
    //���� ������ ����ϱ� ���� ����
    private int totalScore = 0;

    //���� ��尪�� ����ϱ� ���� ����
    public Text txtGold;
    private int curGold = 0;

    //�Ӹ� ���� ���ؽ�Ʈ ǥ��
    [Header("HealText")]
    public Transform healCanvas = null;
    public GameObject healPrefab;

    //���ӿ��� �ǳ�
    public RectTransform gameOverPanel;
    public Text gameOverText;
    public Text nickText;
    public Text playerNickText;
    public Text scoreText;
    public Text playerScoreText;
    public Text goldText;
    public Text playerGoldText;
    public Button retryBtn;
    public Button lobbyBtn;

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

    //ĳ���� ����
    bool swapChar = true;
    public GameObject player1;
    public GameObject player2;

    public static GameManager inst;

    PlayerCtrl playerCtrl;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        GameState = GameState.GameIng;
        GlobalValue.LoadGameDate();
        RefreshGameUI();
        
        
        if (retryBtn != null)
            retryBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("scLevel01");
                SceneManager.LoadScene("scPlay", LoadSceneMode.Additive);
            });

        if (lobbyBtn != null)
            lobbyBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });

        if (backBtn != null)
            backBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });
        
        //������ġ �迭�� ��ƿ�
        SpawnPoints = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        playerCtrl = GameObject.FindObjectOfType<PlayerCtrl>();

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
        //ĳ���� ��� ���� �� ���� �Ұ���
        if (GameManager.GameState == GameState.GameEnd) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            swapChar = !swapChar;
            ChangeCharacter(swapChar);
        }
        else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill_Key(SkillType.Skill_0);    //��
        }
        else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseSkill_Key(SkillType.Skill_1);    //����ź
        }
    }

    void ChangeCharacter(bool swap)
    {
        switch (swap)
        {
            case true:
                {
                    playerCharacter = PlayerCharacter.Player1;

                    player1.transform.position = player2.transform.position;
                    player1.transform.rotation = player2.transform.rotation;
                    player1.GetComponent<PlayerCtrl>().CharacterChange();
                    player1.SetActive(true);
                    player2.SetActive(false);
                }
                break;
            case false:
                {
                    playerCharacter = PlayerCharacter.Player2;
                    player2.transform.position = player1.transform.position;
                    player2.transform.rotation = player1.transform.rotation;
                    player2.GetComponent<PlayerCtrl>().CharacterChange();
                    player2.SetActive(true);
                    player1.SetActive(false);                   
                }
                break;
        }                      
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

    public void DispGold(int gold = 10)
    {
        //�̹� ������������ ���� ��尪
        if (gold < 0)
        {
            curGold += gold;
            if (curGold < 0)
                curGold = 0;
        }
        else if (curGold <= int.MaxValue - gold)
            curGold += gold;
        else
            curGold = int.MaxValue;

        //���ÿ� ����Ǿ� �ִ� ���� ���� ��尪
        if (gold < 0)
        {
            GlobalValue.g_UserGold += gold;
            if (GlobalValue.g_UserGold < 0)
                GlobalValue.g_UserGold = 0;
        }
        else if (GlobalValue.g_UserGold <= int.MaxValue - gold)
            GlobalValue.g_UserGold += gold;
        else
            GlobalValue.g_UserGold = int.MaxValue;

        if (txtGold != null)
            txtGold.text = "gold <color=#ffff00>" + GlobalValue.g_UserGold.ToString("N0") + "</color>";

        PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);        
    }

    public void RefreshGameUI()
    {
        if (txtGold != null)
            txtGold.text = "gold <color=#ffff00>" + GlobalValue.g_UserGold.ToString("N0") + "</color>";
    }

    public void UseSkill_Key(SkillType type)
    {
        if (playerCtrl != null)
            playerCtrl.UseSkill_Item(type);
    }

    public void SpawnText(int cont, Vector3 spawnPos, Color color)
    {
        if (healCanvas == null && healPrefab == null) return;

        GameObject healObj = Instantiate(healPrefab);
        HealTextCtrl healTextCtrl = healObj.GetComponent<HealTextCtrl>();
        if (healTextCtrl != null)
            healTextCtrl.InitState(cont, spawnPos, healCanvas, color);
    }

    public IEnumerator GameOver()
    {
        gameOverPanel.gameObject.SetActive(true);

        char[] chars = { 'g', 'a', 'm', 'e', ' ', 'o', 'v', 'e', 'r' };
        for (int i = 0; i < chars.Length; i++)
        {
            if (gameOverText != null)
            {
                gameOverText.text += chars[i].ToString();

                yield return new WaitForSecondsRealtime(0.15f);
            }
        }

        yield return new WaitForSecondsRealtime(0.7f);

        nickText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        playerNickText.text = GlobalValue.g_NickName;

        yield return new WaitForSecondsRealtime(0.7f);

        scoreText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        playerScoreText.text = totalScore.ToString();

        yield return new WaitForSecondsRealtime(0.7f);

        goldText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        playerGoldText.text = curGold.ToString();


        yield return new WaitForSecondsRealtime(0.7f);

        retryBtn.gameObject.SetActive(true);
        lobbyBtn.gameObject.SetActive(true);


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
