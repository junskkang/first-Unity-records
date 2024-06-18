using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    GameIng,
    GameEnd
}

public class GameManager : MonoBehaviour
{
    public GameState state;
    //게이지 관련
    public float curHp = 100.0f;
    public float maxHp = 100.0f;
    public float curMp = 50.0f;
    public float maxMp = 50.0f;
    public float recoveryMp = 0.5f;
    public int curMC = 30;
    [HideInInspector] public int maxMC = 30;
    public Image hpbar;
    public Text hpText;
    public Image mpbar;
    public Text mpText;
    public Image magazinebar;
    public Text magazineText;

    //점수 UI
    public Text scoreText;
    int curScore = 0;
    static int bestScore = 0;

    //게임오버 판넬
    public Image gameOverPanel;
    public Text gameOverText;
    public Text curScoreText;
    public Text BestScoreText;
    public Button RestartBtn;
    public Button ClearBtn;

    PlayerCtrl refPlayer = null;


    public static GameManager inst;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        state = GameState.GameIng;

        Time.timeScale = 1.0f;

        refPlayer = GameObject.FindObjectOfType<PlayerCtrl>();

        bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (scoreText != null)
            scoreText.text = $"Score : {curScore.ToString("N0")} \n BestScore : {bestScore.ToString("N0")}";

    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.GameEnd) return;

        UpdateUI();

        //마나 자연회복
        if ((curMp < maxMp) && refPlayer.isShield == false)
        {
            curMp += recoveryMp * Time.deltaTime;

            if (curMp >= maxMp)
                curMp = maxMp;
        }
    }

    public void AddScore(int score)
    {
        curScore += score;


        if (curScore > bestScore)
        {
            bestScore = curScore;

            PlayerPrefs.SetInt("BestScore", bestScore);
        }


        if (scoreText != null)
            scoreText.text = scoreText.text = $"Score : {curScore.ToString("N0")} \n BestScore : {bestScore.ToString("N0")}";

    }

    public void UpdateUI()
    {
        if (hpbar != null)
            hpbar.fillAmount = curHp / maxHp;

        if (hpText != null)
            hpText.text = $"{curHp.ToString("N0")} / {maxHp}";

        if (mpbar != null)
            mpbar.fillAmount = curMp / maxMp;

        if (mpText != null)
            mpText.text = $"{curMp.ToString("N0")} / {maxMp}";

        if (magazinebar != null)
            magazinebar.fillAmount = (float)curMC / (float)maxMC;

        if (magazineText != null)
            magazineText.text = $"{curMC} / {maxMC}";

    }

    public void TakeDamage(float damage)
    {
        if (curHp <= 0.0f) return;

        curHp -= damage;

        if (curHp <= 0.0f)
            curHp = 0.0f;

        if (curHp == 0.0f)
        {
            StartCoroutine(GameOver());

            Time.timeScale = 0.0f;
        }
    }

    IEnumerator GameOver()
    {
        state = GameState.GameEnd;

        if (gameOverPanel != null)
            gameOverPanel.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1.0f);

        if(gameOverText != null)
            gameOverText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1.0f);

        if (curScoreText != null)
        {
            curScoreText.gameObject.SetActive(true);
            curScoreText.text = $"SCORE : {curScore.ToString("N0")}";
        }

        if (BestScoreText != null)
        {
            BestScoreText.gameObject.SetActive(true);
            BestScoreText.text = $"BEST SCORE : {bestScore.ToString("N0")}";
        }


        yield return new WaitForSecondsRealtime(1.0f);
        
        if (RestartBtn != null)
        {
            RestartBtn.gameObject.SetActive(true);

            RestartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("GameScene");
            });
        }

        if (ClearBtn != null)
        {
            ClearBtn.gameObject.SetActive(true);

            ClearBtn.onClick.AddListener(() =>
            {
                PlayerPrefs.DeleteAll();

                bestScore = PlayerPrefs.GetInt("BestScore", 0);

                BestScoreText.text = $"BEST SCORE : {bestScore.ToString("N0")}";
            });
        }
    }
}
