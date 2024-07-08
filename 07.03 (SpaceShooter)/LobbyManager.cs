using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("UI")]
    public Button startBtn;
    Vector3 enterScale = new Vector3(1.1f, 1.1f, 1.0f);
    public Button optionBtn;
    public Button storeBtn;
    public Button logoutBtn;
    public Button configBtn;
    public Button clearBtn;

    public Text userInfoText;

    [HideInInspector] public int myRank = 0;
    
    void Start()
    {
        Time.timeScale = 1.0f;
        GlobalValue.LoadGameDate(); //������ Ÿ��Ʋ���� �α��� ������ �� �������� ���;� ��


        if (startBtn != null)
            startBtn.onClick.AddListener(StartBtnClick);

        if (logoutBtn != null)
            logoutBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TitleScene");
            });

        if (clearBtn != null)
            clearBtn.onClick.AddListener(() =>
            {
                PlayerPrefs.DeleteAll();
                GlobalValue.LoadGameDate();
                RefreshUserInfo();
            });

        RefreshUserInfo();
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    void StartBtnClick()
    {
        SceneManager.LoadScene("scLevel01");
        SceneManager.LoadScene("scPlay", LoadSceneMode.Additive);
    }

    public void BtnEnter(Button button)
    {
        button.GetComponentInChildren<Outline>().enabled = true;
        button.GetComponent<RectTransform>().localScale = enterScale;
        button.GetComponentInChildren<Text>().fontSize += 3;
    }

    public void BtnExit(Button button)
    {
        button.GetComponentInChildren<Outline>().enabled = false;
        button.GetComponent<RectTransform>().localScale = Vector3.one;
        button.GetComponentInChildren<Text>().fontSize -= 3;
    }

    public void RefreshUserInfo()
    {
        userInfoText.text = $"������ : ����({GlobalValue.g_NickName}) : ����({myRank}) : " +
                            $"����({GlobalValue.g_BestScore.ToString("N0")}) : " +
                            $"���({GlobalValue.g_UserGold.ToString("N0")})";
    }
}
