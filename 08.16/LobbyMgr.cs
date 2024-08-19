using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMgr : MonoBehaviour
{
    public Button m_Start_Btn;
    public Button m_Store_Btn;
    public Button m_Logout_Btn;
    public Button m_Clear_Save_Btn;

    public Text UserInfoText;

    [HideInInspector] public int m_MyRank = 0;

    public Button RestRk_Btn;
    public Text Rank_Text;
    public Text MessageText;
    float ShowMsTimer = 0.0f;

    public static LobbyMgr Inst = null;

    void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f; //일시정지를 원래 속도로...
        GlobalValue.LoadGameData();

        if (m_Start_Btn != null)
            m_Start_Btn.onClick.AddListener(StartBtnClick);

        if (m_Store_Btn != null)
            m_Store_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("StoreScene");
            });

        if (m_Logout_Btn != null)
            m_Logout_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TitleScene");
            });

        if (m_Clear_Save_Btn != null)
            m_Clear_Save_Btn.onClick.AddListener(Clear_Save_Click);

        //if (RestRk_Btn != null)
        //    RestRk_Btn.onClick.AddListener();

        RefreshUserInfo();

#if AutoRestore
        //자동 랭킹 갱신인 경우
        if (RestRk_Btn != null)
            RestRk_Btn.gameObject.SetActive(false);
#else
        //수동 랭킹 갱신인 경우
        if (RestRk_Btn != null)
            RestRk_Btn.onClick.AddListener(RestoreRank);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < ShowMsTimer)
        {
            ShowMsTimer -= Time.deltaTime;
            if (ShowMsTimer <= 0.0f)
            {
                MessageOnOff("", false);    //메세지 끄기
            }
        }
    }

    void StartBtnClick()
    {
        if(100 <= GlobalValue.g_CurFloorNum)
        {
            //마지막 층에 도달한 상태에서 게임을 시작 했다면...
            //바로 직전 층(99층)에서 시작하게 하기...
            GlobalValue.g_CurFloorNum = 99;
            PlayerPrefs.SetInt("CurFloorNum", GlobalValue.g_CurFloorNum);
        }

        SceneManager.LoadScene("scLevel01");
        SceneManager.LoadScene("scPlay", LoadSceneMode.Additive);
    }

    void Clear_Save_Click()
    {
        PlayerPrefs.DeleteAll();
        GlobalValue.LoadGameData();
        RefreshUserInfo();
    }

    public void RefreshUserInfo()
    {
        UserInfoText.text = "내정보 : 별명(" + GlobalValue.g_NickName +
                            ") : 순위(" + m_MyRank + "등) : 점수(" +
                            GlobalValue.g_BestScore.ToString("N0") + "점) : 골드(" +
                            GlobalValue.g_UserGold.ToString("N0") + ")";
    }

    public void RefreshRankUI(RkRootInfo rootInfo)
    {
        Rank_Text.text = "";

        for (int i = 0; i < rootInfo.RkList.Length; i++)
        {
            //등수 안에 내가 있다면 색 표시
            if (rootInfo.RkList[i].user_id == GlobalValue.g_Unique_ID)
                Rank_Text.text += "<color=#00ff00>";

            Rank_Text.text += (i + 1).ToString() + "등 : " +
                rootInfo.RkList[i].user_id + " (" + rootInfo.RkList[i].nick_name + ") :" +
                rootInfo.RkList[i].best_score +"점 \n";

            if (rootInfo.RkList[i].user_id == GlobalValue.g_Unique_ID)
                Rank_Text.text += "</color>";
        }

        m_MyRank = rootInfo.my_rank;

        RefreshUserInfo();
    }
    void RestoreRank()
    {
        if (0.0f < LobbyNetworkMgr.Inst.RestoreTimer)
        {
            MessageOnOff("최소 7초 주기로만 갱신됩니다.");
            return;
        }

        LobbyNetworkMgr.Inst.GetRankingList();
        LobbyNetworkMgr.Inst.RestoreTimer = 7.0f;
    }
    public void MessageOnOff(string Mess = "", bool isOn = true, float a_Time = 5.0f)
    {
        if (isOn)
        {
            MessageText.text = Mess;
            MessageText.gameObject.SetActive(true);
            ShowMsTimer = a_Time;
        }
        else
        {
            MessageText.text = "";
            MessageText.gameObject.SetActive(false);
        }
    }
}
