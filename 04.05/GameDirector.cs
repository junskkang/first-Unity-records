using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    GameIng,
    GameEnd
}

public class GameDirector : MonoBehaviour
{
    GameObject hpGauge;
    [HideInInspector] public int m_Gold = 0;
    public Text Gold_Text;
    public static GameState m_State = GameState.GameIng;

    public Text Result_Text;
    public Button Replay_Btn;
    public RawImage GameOver_Img;

    // Start is called before the first frame update
    void Start()
    {
        m_State = GameState.GameIng;
        this.hpGauge = GameObject.Find("hpGauge");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetApple()
    {
        m_Gold += 10;
        Gold_Text.text = $"Gold : {m_Gold}";
    }
    public void DecreaseHp()
    {
        this.hpGauge.GetComponent<Image>().fillAmount -= 0.1f;

        if (this.hpGauge.GetComponent<Image>().fillAmount <= 0)
        {
            m_State = GameState.GameEnd;
            if(Result_Text != null)
            {
                Result_Text.gameObject.SetActive(true);
                Result_Text.text += $"<size=50>  \n Gold : {m_Gold} </size>";
                Replay_Btn.gameObject.SetActive(true);
            }

            if(Replay_Btn != null)
            {
                Replay_Btn.onClick.AddListener(() =>
                {
                    SceneManager.LoadScene("GameScene");
                });
            }

            if(GameOver_Img != null)
            {
                GameOver_Img.gameObject.SetActive(true);
            }

        }

    }
}
