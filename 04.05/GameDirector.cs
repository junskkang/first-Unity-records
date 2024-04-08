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
    Image m_HpImg;
    [HideInInspector] public int m_Gold = 0;
    public Text Gold_Text;
    public static GameState m_State;

    [Header("Game Over")]
    public Text Result_Text;
    public Button Replay_Btn;
    public RawImage GameOver_Img;

    public GameObject UI_Mask;

    private void Awake()
    {
        if(UI_Mask != null)
            UI_Mask.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f; //일시정지를 해제. 스태틱변수이기 때문에 마지막 값이 저장됨.
        m_State = GameState.GameIng;
        this.hpGauge = GameObject.Find("hpGauge");
        m_HpImg = hpGauge.GetComponent<Image>();
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
        m_HpImg.fillAmount -= 0.1f;

        if (m_HpImg.fillAmount <= 0)
        {
            Time.timeScale = 0.0f; // 델타타임 값을 사용하는 애들 일시정지 효과
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
