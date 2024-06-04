using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //UI관련 변수들
    public Text userInfoText;
    public Text bestScoreText;
    public Text curScoreText;
    public Text goldText;
    public Button lobbyBtn;

    //public static int bestScore = 0;
    [HideInInspector] public int curScore = 0;
    [HideInInspector] public int curGold = 0;

    //데미지 텍스트 관련 변수
    public Transform damageCanvas = null;   //유니티 연결용
    public GameObject damagePrefab = null;  //유니티 연결용
    GameObject damageClone;                 //복사본
    DamageCtrl damageText;                  //컴포넌트 받아오기용

    //코인 관련 변수
    [HideInInspector] public GameObject CoinPre = null;
    GameObject coinClone;

    [HideInInspector] public GameObject heartPrefab = null;
    GameObject heartClone;
    [HideInInspector] public float healValue;

    [HideInInspector] public HeroCtrl refHero = null;

    Vector3 startPos = Vector3.zero;

    [Header("Inventory Show On/Off")]
    public Button invenBtn;
    public Transform invenRoot = null;
    Transform arrowIcon;
    bool invenScOnOff = true;
    float scrollSpeed = 1000.0f;
    Vector3 scOnPos = new Vector3(-197.0f, 0.0f, 0.0f);
    Vector3 scOffPos = new Vector3(-520.0f, 0.0f, 0.0f);
    //싱글턴 패턴
    public static GameManager Inst = null;

    void Awake()
    {
        Inst = this;
    }
    void Start()
    {
        Time.timeScale = 1.0f;

        GlobalValue.LoadGameData();

        if (CoinPre == null)
        {
            CoinPre = Resources.Load("CoinPrefab") as GameObject;

            //Debug.Log("코인프리팹 로드 완료");
        }

        if (heartPrefab == null)
        {
            heartPrefab = Resources.Load("BossHeart") as GameObject;

            //Debug.Log("보스 보상 로드 완료");
        }


        refHero = GameObject.FindObjectOfType<HeroCtrl>();

        arrowIcon = invenBtn.transform.Find("ArrowIcon");

        if (lobbyBtn != null)
            lobbyBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        //bestScore = PlayerPrefs.GetInt("BestScore", 0);

        //if (bestScoreText != null)
        //{
        //    bestScoreText.text = $"최고점수 : {bestScore.ToString("N0")}";
        //}

        if (invenBtn != null)
            invenBtn.onClick.AddListener(() =>
            {              
                invenScOnOff = !invenScOnOff;
            });
    }

    // Update is called once per frame
    void Update()
    {
        //UIUpdate();

        InvenScrollUpdate();
    }

    public void DamageText(int Value, Vector3 ownerPos, Color ownerColor)
    {
        damageClone = Instantiate(damagePrefab);
        if (damageClone != null && damageCanvas != null) 
        {
            startPos = new Vector3(ownerPos.x, ownerPos.y + 2.0f, ownerPos.z);
            damageClone.transform.SetParent(damageCanvas);
            damageText = damageClone.GetComponent<DamageCtrl>();
            damageText.DamageSpawn(Value, ownerColor);
            damageClone.transform.position = startPos;
        }

    }

    public void GoldDrop(Vector3 spawnPos, float value)
    {

        //Debug.Log("함수 호출은 된다");


        coinClone = Instantiate(CoinPre);

        if (coinClone != null)
        {
            coinClone.transform.position = spawnPos;
            CoinCtrl coinCtrl = coinClone.GetComponent<CoinCtrl>();
            coinCtrl.refHero = refHero;
        }

    }

    public void HeartDrop(Vector3 spawnPos, float value)
    {
        heartClone = Instantiate(heartPrefab);

        if (heartClone != null)
        {
            heartClone.transform.position = spawnPos;
            healValue = value;           
        }
    }

    void UIUpdate()
    {
        if (goldText != null)
            goldText.text = $"보유골드 : {curGold.ToString("N0")}";




        //if (bestScore < curScore)
        //{
        //    bestScore = curScore;
        //    if (bestScoreText != null)
        //    {
        //        bestScoreText.text = $"최고점수 : {bestScore.ToString("N0")}";
        //    }

        //    PlayerPrefs.SetInt("BestScore", bestScore);
        //}
    }

    void InvenScrollUpdate()
    {
        if (invenRoot == null) return;
        if (Input.GetKeyDown(KeyCode.R) == true)
        {
            invenScOnOff = !invenScOnOff;            
        }

        if (!invenScOnOff)
        {
            if (invenRoot.localPosition.x > scOffPos.x)
                invenRoot.localPosition =
                Vector3.MoveTowards(invenRoot.localPosition, scOffPos, scrollSpeed * Time.deltaTime);

            if (scOffPos.x <= invenRoot.localPosition.x)
                arrowIcon.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }
        else
        {
            if (invenRoot.localPosition.x < scOnPos.x)
                invenRoot.localPosition =
                          Vector3.MoveTowards(invenRoot.localPosition, scOnPos, scrollSpeed * Time.deltaTime);

            if (scOnPos.x >= invenRoot.localPosition.x)
                arrowIcon.transform.eulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
        }
    }

    public void AddScore(int value = 10)
    {
        if(curScore <= int.MaxValue - value)
            curScore += value;
        else 
            curScore = int.MaxValue;

        if(curScore < 0) curScore = 0;

        if (curScoreText != null)
            curScoreText.text = $"현재점수 : {curScore.ToString("N0")}";

        if (GlobalValue.bestScore < curScore)
        {
            GlobalValue.bestScore = curScore;
            if (bestScoreText != null)
            {
                bestScoreText.text = $"최고점수 : {GlobalValue.bestScore.ToString("N0")}";
                PlayerPrefs.SetInt("BestScore", GlobalValue.bestScore);
            }
        }
    }

    public void AddGold(int value = 10) 
    {
    
    }
}
