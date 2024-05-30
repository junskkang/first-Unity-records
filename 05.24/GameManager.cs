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

    public int curScore = 0;
    public int curGold = 0;

    //데미지 텍스트 관련 변수
    public Transform damageCanvas = null;   //유니티 연결용
    public GameObject damagePrefab = null;  //유니티 연결용
    GameObject damageClone;                 //복사본
    DamageCtrl damageText;                  //컴포넌트 받아오기용

    //코인 관련 변수
    public GameObject CoinPre = null;
    GameObject coinClone;
    
    HeroCtrl refHero = null;

    Vector3 startPos = Vector3.zero;

    //싱글턴 패턴
    public static GameManager Inst = null;

    void Awake()
    {
        Inst = this;
    }
    void Start()
    {
        Time.timeScale = 1.0f;
        if (CoinPre == null)
        {
            CoinPre = Resources.Load("CoinPrefab") as GameObject;

            Debug.Log("코인프리팹 로드 완료");
        }
            

        refHero = GameObject.FindObjectOfType<HeroCtrl>();

        if (lobbyBtn != null)
            lobbyBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
