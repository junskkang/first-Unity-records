using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class GameDirector : MonoBehaviour
{
    GameObject timerText;
    GameObject pointText;
    [HideInInspector] public float time = 30.0f;
    int point = 0;
    GameObject generator;

    public GameObject GameOverPanel;
    public Text PointText;
    public Button RestartBtn;
    public Button PauseBtn;
    public Image PausePanel;
    public Sprite[] PauseSprite;
    bool pauseOnOff = false;
    public Button ToLobbyBtn;

    // Start is called before the first frame update
    void Start()
    {
        time = 30.0f;
        Time.timeScale = 1.0f;

        this.timerText = GameObject.Find("Timer");
        this.pointText = GameObject.Find("Point");
        this.generator = GameObject.Find("ItemGenerator");

        if (RestartBtn != null)
            RestartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        if (PauseBtn != null)
            PauseBtn.onClick.AddListener(() =>
            {
                pauseOnOff = !pauseOnOff;
                if (PausePanel != null)
                    PausePanel.gameObject.SetActive(pauseOnOff);
                PauseBtnClick();
            });

        if (ToLobbyBtn != null)
            ToLobbyBtn.onClick.AddListener(() =>
            {
                GlobalData.gameGold += this.point;
                GlobalData.SaveGold();
                SceneManager.LoadScene("LobbyScene");
            });

        //ToLobbyBtn.GetComponent<Sprite>();
    }

    // Update is called once per frame
    void Update()
    {
        this.time -= Time.deltaTime;

        if (this.time < 0)
        {
            this.time = 0;
            this.generator.GetComponent<ItemGenerator>().SetParameter(10000.0f, 0, 0);

            GameOverPanel.SetActive(true);
            PauseBtn.gameObject.SetActive(false);
            PointText.text = "È¹µæÁ¡¼ö : " + this.point.ToString();
            Time.timeScale = 0.0f;

            GlobalData.gameGold += this.point;
            GlobalData.SaveGold();
        }
        else if (0 <= this.time && this.time < 5)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(0.9f, -0.04f, 3);
        }
        else if (5 <= this.time && this.time < 10)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(0.4f, -0.06f, 6);
        }
        else if (10 <= this.time && this.time < 20)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(0.7f, -0.04f, 4);
        }
        else if (20 <= this.time && this.time < 30)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(1.0f, -0.03f, 2);
        }

        this.timerText.GetComponent<Text>().text = this.time.ToString("F1");
        this.pointText.GetComponent<Text>().text = this.point.ToString() + " point";
    }

    public void GetApple()
    {
        this.point += 100;
    }

    public void GetBomb()
    {
        this.point /= 2;
    }

    void PauseBtnClick()
    {
        if (pauseOnOff == false)
        {
            Time.timeScale = 1.0f;
            PauseBtn.GetComponent<Image>().sprite = PauseSprite[0];

        }
        else
        {
            Time.timeScale = 0.0f;
            PauseBtn.GetComponent<Image>().sprite = PauseSprite[1];
        }
    }

    
}
