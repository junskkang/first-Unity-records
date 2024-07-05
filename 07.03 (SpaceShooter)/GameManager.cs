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
    //Text UI 항목 연결을 위한 변수
    public Text txtScore;
    //누적 점수를 기록하기 위한 변수
    private int totalScore = 0;

    //누적 골드값을 기록하기 위한 변수
    public Text txtGold;
    private int curGold = 0;

    //머리 위에 힐텍스트 표시
    [Header("HealText")]
    public Transform healCanvas = null;
    public GameObject healPrefab;

    //게임오버 판넬
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


    [Header("몬스터 스폰 관련")]
    //몬스터 스폰 위치 담을 배열
    public Transform[] SpawnPoints = null;
    //몬스터 프리팹 변수
    public GameObject monsterPrefab;
    //몬스터를 미리 생성해 저장할 리스트 자료형
    public List<GameObject> monsterPoolList = new List<GameObject>();
    public Transform monsterPool;

    public float createTime = 2.0f;     //몬스터 발생 주기
    public int maxMonster = 10;         //최대 몬스터 마리수


    public bool isGameOver = false;

    //캐릭터 스왑
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
        
        //스폰위치 배열로 담아옴
        SpawnPoints = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        playerCtrl = GameObject.FindObjectOfType<PlayerCtrl>();

        //몬스터를 생성해 오브젝트 풀에 저장
        for (int i = 0; i < maxMonster; i++)
        {
            //몬스터 프리팹을 생성
            GameObject monster = (GameObject)Instantiate(monsterPrefab);
            //생성한 몬스터의 이름 설정
            monster.name = "Monster_" + i.ToString();
            //생성한 몬스터 MonsterPool의 child로 저장
            monster.transform.SetParent(monsterPool);
            //생성한 몬스터 비활성화 저장
            monster.SetActive(false);
            //생성한 몬스터를 오브젝트 풀에 추가
            monsterPoolList.Add(monster);
        }



        if (SpawnPoints.Length > 0)
            StartCoroutine(this.CreateMonster());   //몬스터 생성 코루틴 함수 호출

        DispScore(0);
    }

    // Update is called once per frame
    void Update()
    {
        //캐릭터 사망 상태 시 조작 불가능
        if (GameManager.GameState == GameState.GameEnd) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            swapChar = !swapChar;
            ChangeCharacter(swapChar);
        }
        else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill_Key(SkillType.Skill_0);    //힐
        }
        else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseSkill_Key(SkillType.Skill_1);    //수류탄
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
        while (!isGameOver) //게임 종료시까지 무한 루프
        {
            //오브젝트풀링 X
            ////현재 생성된 몬스터 갯수 산출
            //int monsterCount = (int)GameObject.FindGameObjectsWithTag("MONSTER").Length;

            ////몬스터의 최대 생성 갯수보다 현재 몬스터 수가 작을 때만 몬스터 생성
            //if (monsterCount < maxMonster)
            //{
            //    //몬스터 생성 주기 시간만큼 대기
            //    yield return new WaitForSeconds(createTime);

            //    //불규칙적인 위치 산출
            //    int idx = Random.Range (1, SpawnPoints.Length);
            //    //몬스터 동적 생성
            //    Instantiate(monsterPrefab, SpawnPoints[idx].position, SpawnPoints[idx].rotation);
            //}
            //else
            //    yield return null;  //한프레임 동안 쉼

            //오브젝트풀링 O
            
            //몬스터 생성 주기 시간만큼 메인 루프에 양보
            yield return new WaitForSeconds(createTime);

            //플레이어 사망시 코루틴을 종료해 다음 루틴 실행x
            if (isGameOver) yield break;

            //오브젝트 풀 리스트의 처음부터 끝까지 순회
            foreach (GameObject monster in monsterPoolList)
            {
                //비활성화 여부로 사용 가능한 몬스터 판단
                if (monster.activeSelf == false)
                {
                    //몬스터 출현시킬 위치의 인덱스값 추출
                    int idx = Random.Range(1, SpawnPoints.Length);
                    //몬스터의 출현위치를 설정
                    monster.transform.position = SpawnPoints[idx].transform.position;
                    //오브젝트 풀링에 저장된 몬스터 활성화
                    monster.SetActive(true);
                    //오브젝트 풀에서 몬스터 프리팹 하나를 활성화한 후 for 루프를 빠져나감
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
        //이번 스테이지에서 얻은 골드값
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

        //로컬에 저장되어 있는 유저 보유 골드값
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
    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
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
