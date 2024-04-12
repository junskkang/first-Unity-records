using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public Text highScoreText;
    public Text currentScoreText;
    public Button restartBtn;
    public Button clearDataBtn;
    void Start()
    {
        if (GameManager.m_BestHeight < GameManager.m_CurBHeight)
        {
            GameManager.m_BestHeight = GameManager.m_CurBHeight;
            GameManager.Save();
        }

        if (highScoreText != null)
        {
            highScoreText.text = "최고기록 : " + GameManager.m_BestHeight.ToString("N2");
        }

        if (currentScoreText != null)
            currentScoreText.text = "이번기록 : " + GameManager.m_CurBHeight.ToString("N2");

        if (restartBtn != null)
            restartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("GameScene");
            });

        if (clearDataBtn != null)
            clearDataBtn.onClick.AddListener(() =>
            {
                CD_BtnClick();
            });


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    void CD_BtnClick()
    {
        PlayerPrefs.DeleteAll();
        GameManager.m_CurBHeight = 0.0f;
        
        GameManager.Load();

        if (highScoreText != null)
        {
            highScoreText.text = "최고기록 : " + GameManager.m_BestHeight.ToString("N2");
        }

        if (currentScoreText != null)
            currentScoreText.text = "이번기록 : " + GameManager.m_CurBHeight.ToString("N2");

    }
}
