using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public GameObject MarkObjPrefab;
    float minTimer = 0.5f;
    float maxTimer = 3.0f;
    float timer = 0.0f;

    public GameObject particle;

    int curScore = 0;
    int bestScore = 0;

    int comboCount = 0;
    int bestCombo = 0;

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


    public GameObject hitTextPrefab;
    public Transform canvas;

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
                Time.timeScale = 0.0f;
            });
    }

    // Update is called once per frame
    void Update()
    {
        if (180.0f <= playTime)
        {
            Time.timeScale = 0.0f;
        }

        playTime = Time.time;
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

    }

    void AddScore(int point)
    {
        curScore += point;

        if (curScore <= bestScore)
            bestScore = curScore;

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

        if (comboCount <= bestCombo)
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
    }

    public void Judge(string point, Vector3 position)
    {           

        if (point.Contains("Miss"))
            MissMark();

        AddCombo(point);

        int addScore = 0;
        Color setColor = Color.white;
        switch(point)
        {
            case "Miss":
                {
                    addScore = 0;
                    setColor = Color.black;
                }
                break;
            case "Good":
                {
                    addScore = 1;
                    setColor = Color.cyan;
                }
                break;
            case "Great":
                {
                    addScore = 3;
                    setColor = Color.green;
                }
                break;
            case "Nice":
                {
                    addScore = 2;
                    setColor = Color.yellow;
                }
                break;
            case "Excellent":
                {
                    addScore = 5;
                    addScore += comboCount * 1; //콤보 수에 따른 가산점
                    setColor = Color.red;
                }
                break;
        }

        if (particle != null)
        {
            GameObject go = Instantiate(particle);
            go.transform.position = position;

            ParticleSystem[] glow = go.GetComponentsInChildren<ParticleSystem>();
            var forColor = glow[1].GetComponent<ParticleSystem>().main;
            forColor.startColor = setColor;

            Destroy(go, 1.0f);
        }


        if (hitTextPrefab != null)
        {
            GameObject go = Instantiate(hitTextPrefab);
            go.transform.SetParent(canvas, false);
            go.transform.position = position;
            go.GetComponent<HitText>().SetText(point);
        }


        AddScore(addScore);
        
    }

}
