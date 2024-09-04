using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public GameObject MarkObjPrefab;
    float timer = 0.0f;

    public GameObject[] effects;

    int curScore = 0;
    int bestScore = 0;

    int comboCount = 0;
    int bestCombo = 0;

    bool isStop = false;

    public int hpCount = 10;
    int maxHp = 10;

    float playTime;

    public Text scoreText;
    public Text comboText;
    public Text bestComboText;
    public Text timeText;
    public Button pauseBtn;
    public Image hpImage;
    public Text hpText;
    public Image gameoverImg;
    public GameObject overParent;
    public Button rankingBtn;
    public Button exitBtn;
    public Text recordText;
    public GameObject rankingBoard;
    public Text rankingText;
    public Button closeBtn;



    public GameObject hitTextPrefab;
    public Transform canvas;

    //레벨링을 위한 변수
    public float downscaleSpeed = 1.3f;
    float minTimer = 2.0f;
    float maxTimer = 3.0f;
    int level = 1;

    public static GameMgr inst;

    private void Awake()
    {
        inst = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;

        if (MarkObjPrefab == null)
            MarkObjPrefab = Resources.Load("Mark") as GameObject;

        timer = Random.Range(minTimer, maxTimer);

        if (pauseBtn != null)
            pauseBtn.onClick.AddListener(() =>
            {
                PauseClick();
            });

        if (closeBtn != null)
            closeBtn.onClick.AddListener(() =>
            {
                if (rankingBoard.gameObject.activeSelf)
                {
                    rankingBoard.gameObject.SetActive(false);   
                }
            });
    }

    // Update is called once per frame
    void Update()
    {
        if (140.0f <= playTime)
        {            
            GameOver();
        }

        playTime += Time.deltaTime;
        timeText.text = playTime.ToString("N2");

        timer -= Time.deltaTime;

        if (timer <= 0.0f)
        {
            GameObject mark = Instantiate(MarkObjPrefab);
            //mark.GetComponent<Transform>();
            float ranX = Random.Range(0.1f, 0.9f);
            float ranY = Random.Range(0.1f, 0.8f);
            Vector3 createPoint = new Vector3(ranX, ranY, 0);
            
            mark.transform.position = Camera.main.ViewportToWorldPoint(createPoint);
            mark.transform.position = new Vector3(mark.transform.position.x, mark.transform.position.y, 0);

            timer = Random.Range(minTimer, maxTimer);
        }  
        
        if (level <= 12)
        {
            if (playTime >= level * 7)
            {
                level++;

                downscaleSpeed = downscaleSpeed * ((100.0f + (2.0f * level)) / 100.0f);

                minTimer -= 0.15f;

                if (level % 2 == 0)
                {
                    maxTimer -= 0.25f;
                }

                Debug.Log("Level :" + level + ", min : "+minTimer + ", max : " + maxTimer);
            }
        }
    }

    void AddScore(int point)
    {
        curScore += point;

        if (curScore >= GlobalUserData.BestScore)
            GlobalUserData.BestScore = curScore;

        if (scoreText != null)
            scoreText.text = $"SCORE : {curScore}";
    }

    void AddCombo(string combo)
    {
        if (combo == "Excellent")
        {
            comboCount++;
        }
        else
        {
            comboCount = 0;
        }        

        if (comboCount >= bestCombo)
            bestCombo = comboCount;

        if (comboText != null)
            comboText.text = $"COMBO : {comboCount}";        
    }

    void MissMark()
    {
        hpCount--;

        if (hpImage != null)
            hpImage.fillAmount = ((float)hpCount / (float)maxHp);

        if (hpText != null)
            hpText.text = $"HP : {hpCount} / {maxHp}";

        if (hpCount <= 0)
        {
            
            GameOver();
        }
    }

    public void Judge(string point, Vector3 position)
    {           

        if (point.Contains("Miss"))
            MissMark();

        AddCombo(point);

        int addScore = 0;
        int effectIdx = 0;
        Color setColor = Color.white;
        switch(point)
        {
            case "Miss":
                {
                    addScore = 0;
                    effectIdx = 0;
                    setColor = Color.black;
                }
                break;
            case "Good":
                {
                    addScore = 1;
                    effectIdx = 1;
                    setColor = Color.cyan;
                }
                break;
            case "Great":
                {
                    addScore = 3;
                    effectIdx = 2;
                    setColor = Color.green;
                }
                break;
            case "Nice":
                {
                    addScore = 2;
                    effectIdx = 3;
                    setColor = Color.yellow;
                }
                break;
            case "Excellent":
                {
                    addScore = 5;
                    effectIdx = 4;
                    addScore += comboCount * 1; //콤보 수에 따른 가산점
                    setColor = Color.red;
                }
                break;
        }

        if (effects != null)
        {
            GameObject go = Instantiate(effects[effectIdx]);
            go.transform.position = position;

            //ParticleSystem[] glow = go.GetComponentsInChildren<ParticleSystem>();
            //var forColor = glow[1].GetComponent<ParticleSystem>().main;
            //forColor.startColor = setColor;

            Destroy(go, 0.5f);
        }


        if (hitTextPrefab != null)
        {
            GameObject go = Instantiate(hitTextPrefab);
            go.transform.SetParent(canvas);
            go.transform.position = position;
            go.GetComponent<HitText>().SetText(point);
        }


        AddScore(addScore);
        
    }

    void PauseClick()
    {
        isStop = !isStop;

        if (isStop)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }

    public void GameOver()
    {
        if (!gameoverImg.gameObject.activeSelf)
        {
            Time.timeScale = 0.0f;
            gameoverImg.gameObject.SetActive(true);

            NetworkManager.Inst.PushPacket(PacketType.BestScore);

            StartCoroutine(GameoverMenu());
        }
    }

    IEnumerator GameoverMenu()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        overParent.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(0.5f);

        if (recordText != null)
        {
            string text = $"이번 기록 : {curScore}\n\n최고 기록 : {GlobalUserData.BestScore}\n\n" +
                         $"최대 콤보 : {bestCombo}";
            char[] split = text.ToCharArray();
            recordText.text = "";

            for (int i = 0; i < split.Length; i++)
            {
                recordText.text += split[i];
                yield return new WaitForSecondsRealtime(0.15f);
            }

        }

        if (exitBtn != null)
        {
            exitBtn.onClick.AddListener(() =>
            {
                Time.timeScale = 1.0f;
                SceneManager.LoadScene("TitleScene");
            });
        }

        if (rankingBtn != null)
        {
            rankingBtn.onClick.AddListener(() => 
            {
                rankingBoard.gameObject.SetActive(true);

                NetworkManager.Inst.PushPacket(PacketType.RankingUpdate);
            });
        }

        yield break;
    }

}
