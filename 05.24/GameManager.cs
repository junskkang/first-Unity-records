using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //UI���� ������
    public Text userInfoText;
    public Text bestScoreText;
    public Text curScoreText;
    public Text goldText;
    public Button lobbyBtn;

    public static int bestScore = 0;
    public int curScore = 0;
    public int curGold = 0;

    //������ �ؽ�Ʈ ���� ����
    public Transform damageCanvas = null;   //����Ƽ �����
    public GameObject damagePrefab = null;  //����Ƽ �����
    GameObject damageClone;                 //���纻
    DamageCtrl damageText;                  //������Ʈ �޾ƿ����

    //���� ���� ����
    public GameObject CoinPre = null;
    GameObject coinClone;

    public GameObject heartPrefab = null;
    GameObject heartClone;
    public float healValue;
    
    HeroCtrl refHero = null;

    Vector3 startPos = Vector3.zero;

    //�̱��� ����
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

            Debug.Log("���������� �ε� �Ϸ�");
        }

        if (heartPrefab == null)
        {
            heartPrefab = Resources.Load("BossHeart") as GameObject;

            Debug.Log("���� ���� �ε� �Ϸ�");
        }


        refHero = GameObject.FindObjectOfType<HeroCtrl>();

        if (lobbyBtn != null)
            lobbyBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (bestScoreText != null)
        {
            bestScoreText.text = $"�ְ����� : {bestScore.ToString("N0")}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        UIUpdate();
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

        //Debug.Log("�Լ� ȣ���� �ȴ�");


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
            goldText.text = $"������� : {curGold.ToString("N0")}";

        if (curScoreText != null)
            curScoreText.text = $"�������� : {curScore.ToString("N0")}";


        if (bestScore < curScore)
        {
            bestScore = curScore;
            if (bestScoreText != null)
            {
                bestScoreText.text = $"�ְ����� : {bestScore.ToString("N0")}";
            }

            PlayerPrefs.SetInt("BestScore", bestScore);
        }
    }
}
