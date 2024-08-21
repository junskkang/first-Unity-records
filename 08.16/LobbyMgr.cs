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

    [Header("ConfigBox")]
    public Button ConfigBtn;
    public GameObject Canvas_Dialog;
    public GameObject configBoxObj;

    //저장정보 초기화 관련 변수
    float ClearLockTimer = 0.0f;

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

        //환경설정 추가
        if (ConfigBtn != null)
            ConfigBtn.onClick.AddListener(() =>
            {
                if (configBoxObj == null)
                    configBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject ob = Instantiate(configBoxObj) as GameObject;
                ob.transform.SetParent(Canvas_Dialog.transform, false);
                Time.timeScale = 0.0f;
            });
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

        if (0.0f < ClearLockTimer)
        {
            ClearLockTimer -= Time.deltaTime;   
        }
    }

    void StartBtnClick()
    {
        if(100 <= GlobalValue.g_CurFloorNum)
        {
            //마지막 층에 도달한 상태에서 게임을 시작 했다면...
            //바로 직전 층(99층)에서 시작하게 하기...
            GlobalValue.g_CurFloorNum = 99;
            
            //로컬에 층 로딩, 저장 부분 서버로 옮기기 위해 주석처리 (08/20)
            //PlayerPrefs.SetInt("CurFloorNum", GlobalValue.g_CurFloorNum);
        }

        SceneManager.LoadScene("scLevel01");
        SceneManager.LoadScene("scPlay", LoadSceneMode.Additive);
    }

    void Clear_Save_Click()
    {
        if (0.0f < ClearLockTimer)
        {
            MessageOnOff("저장정보 초기화 중입니다.");
            return;
        }

        PlayerPrefs.DeleteAll();
        GlobalValue.LoadGameData();
        //RefreshUserInfo();

        LobbyNetworkMgr.Inst.DltMethod = Result_Clear_Save;
        LobbyNetworkMgr.Inst.PushPacket(PacketType.ClearSave);

        ClearLockTimer = 5.0f;
        //5초간 재시도 제한
    }

    void Result_Clear_Save(bool isOk)
    {
        //서버 초기화 성공 후 모든 변수 초기화 필요
        if (isOk)
        {
            GlobalValue.g_BestScore = 0;    //최고 기록
            GlobalValue.g_UserGold = 0;     //게임머니
            GlobalValue.g_Exp = 0;          //경험치
            GlobalValue.g_Level = 0;        //레벨 초기화

            GlobalValue.g_BestFloor = 1;    //최종 도달 층
            GlobalValue.g_CurFloorNum = 1;  //현재 층

            for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
            {
                GlobalValue.g_SkillCount[i] = 1;    //아이템 보유 초기화
            }
            RefreshUserInfo();  //점수, 골드값 UI 초기화
        }
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
