using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text timerText;
    public Text pointText;
    float m_Timer = 30.0f;
    int m_Point = 0;
    GameObject itemGenerator;

    [Header("Game Over")]
    public GameObject GameOverParent;
    public Text scoreText;
    public Button replayBtn;

    public static GameManager Inst = null;

    private void Awake()
    {
        Inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        this.itemGenerator = GameObject.Find("ItemGenerator");
        Time.timeScale = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        this.m_Timer -= Time.deltaTime;
        if (this.m_Timer < 0)
        {
            this.m_Timer = 0;
            this.itemGenerator.GetComponent<ItemGenerator>().SetParameter(10000.0f, 0, 0);
            Time.timeScale = 0.0f;
            GameOver();
        }
        else if (0 <= this.m_Timer && this.m_Timer < 4)
        {
            this.itemGenerator.GetComponent<ItemGenerator>().SetParameter(0.3f, -0.06f, 0);
        }
        else if (4 <= this.m_Timer && this.m_Timer < 12)
        {
            this.itemGenerator.GetComponent<ItemGenerator>().SetParameter(0.5f, -0.05f, 6);
        }
        else if (12 <= this.m_Timer && this.m_Timer < 23)
        {
            this.itemGenerator.GetComponent<ItemGenerator>().SetParameter(0.8f, -0.04f, 4);
        }
        else if (23 <= this.m_Timer && this.m_Timer < 30)
        {
            this.itemGenerator.GetComponent<ItemGenerator>().SetParameter(1.0f, -0.03f, 2);
        }

        if (timerText != null)
            this.timerText.text = this.m_Timer.ToString("N1");

        if (pointText != null)
            this.pointText.text = this.m_Point.ToString() + " Point";
    }

    public void GetApple()
    {
        this.m_Point += 100;
    }
    
    public void GetBomb()
    {
        this.m_Point /= 2;
    }
    
    void GameOver()
    {
        GameOverParent.gameObject.SetActive(true);
        if (scoreText != null)
            scoreText.text = $"È¹µæÁ¡¼ö : {m_Point}";
        if (replayBtn != null)
            replayBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("GameScene");
            });
    }
}
