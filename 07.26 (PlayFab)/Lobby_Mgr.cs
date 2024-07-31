//#define AutoRestore

using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ConfigBox;

public class Lobby_Mgr : MonoBehaviour
{
    public Button m_ClearSvDataBtn;

    public Button Store_Btn;
    public Button MyRoom_Btn;
    public Button Exit_Btn;
    public Button GameStart_Btn;

    public Text m_GoldText;
    public Text m_UserInfoText;
    public Text messageText;
    float messageTimer = 0.0f;
    

    //--- 환경설정 Dlg 관련 변수
    [Header("--- ConfigBox ---")]
    public Button m_CfgBtn = null;
    public GameObject Canvas_Dialog = null;
    GameObject m_ConfigBoxObj = null;
    //--- 환경설정 Dlg 관련 변수

    [Header("--- Ranking ---")]
    public Text rankingText;
    public Button restoreRankBtn;
    [HideInInspector] public int myRank = 0;
    float restoreTimer = 3.0f;  //랭킹 갱신 타이머
    //float delayGetLB = 3.0f;    //로비 진입후 3초 뒤에 랭킹 한번 더 로딩하기

    public static Lobby_Mgr inst;

    private void Awake()
    {
        inst = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        GlobalValue.LoadGameData();

        if (m_ClearSvDataBtn != null)
            m_ClearSvDataBtn.onClick.AddListener(ClearSvData);

        if (Store_Btn != null)
            Store_Btn.onClick.AddListener(StoreBtnClick);

        if (MyRoom_Btn != null)
            MyRoom_Btn.onClick.AddListener(MyRoomBtnClick);

        if (Exit_Btn != null)
            Exit_Btn.onClick.AddListener(ExitBtnClick);

        if (GameStart_Btn != null)
            GameStart_Btn.onClick.AddListener(() =>
            {
                //SceneManager.LoadScene("GameScene");
                MyLoadScene("GameScene");
            });

        if(m_GoldText != null)
        {
            m_GoldText.text = GlobalValue.g_UserGold.ToString("N0");
            //"N0" 엔 제로 <-- 소수점 밑으로는 제외시키고 천단위 마다 쉼표 붙여주기...
        }

        if(m_UserInfoText != null)
        {
            m_UserInfoText.text = "내정보 : 별명(" + GlobalValue.g_NickName + ") : 순위(" + myRank +
                                  "등) : 점수(" + GlobalValue.g_BestScore + "점)";
        }

        //--- 환경설정 Dlg 관련 구현 부분
        if (m_CfgBtn != null)
            m_CfgBtn.onClick.AddListener(() =>
            {
                if (m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject a_CfgBoxObj = Instantiate(m_ConfigBoxObj);
                a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);
                a_CfgBoxObj.GetComponent<ConfigBox>().DltMethod = CfgResponse;

                Time.timeScale = 0.0f;
            });
        //--- 환경설정 Dlg 관련 구현 부분

        Sound_Mgr.Inst.PlayBGM("sound_bgm_title_001", 0.5f);

        //Invoke("GetRankingList", delayGetLB);
        LobbyNetwork_Mgr.inst.PushPacket(LobbyNetwork_Mgr.PacketType.GetRankingList);

#if AutoRestore     //이 때는 일정 주기마다 자동으로 랭킹 업데이트 해줌
        restoreTimer = 3.0f;        //자동 타이머 3초 충전
        if (restoreRankBtn != null)
            restoreRankBtn.gameObject.SetActive(false);
#else               //수동으로 눌러야 갱신 해줌
        if (restoreRankBtn != null)
            restoreRankBtn.onClick.AddListener(RestoreRank);
#endif
    }



    void ClearSvData()
    {
        PlayerPrefs.DeleteAll();    //로컬에 저장되어 있었던 모든 정보를 지워준다.

        GlobalValue.g_CurSkillCount.Clear();
        GlobalValue.LoadGameData();

        if (m_GoldText != null)
            m_GoldText.text = GlobalValue.g_UserGold.ToString("N0");

        if (m_UserInfoText != null)
            m_UserInfoText.text = "내정보 : 별명(" + GlobalValue.g_NickName + ") : 순위(" + myRank +
                                  "등) : 점수(" + GlobalValue.g_BestScore + "점)";

        Sound_Mgr.Inst.PlayGUISound("Pop", 1.0f);
    }

    private void StoreBtnClick()
    {
        //Debug.Log("상점으로 가기 버튼 클릭");
        //SceneManager.LoadScene("StoreScene");
        MyLoadScene("StoreScene");
    }

    private void MyRoomBtnClick()
    {
        //Debug.Log("꾸미기 방 가기 버튼 클릭");
        //SceneManager.LoadScene("MyRoomScene");
        MyLoadScene("MyRoomScene");
    }

    private void ExitBtnClick()
    {
        //Debug.Log("타이틀 씬으로 나가기 버튼 클릭");
        //SceneManager.LoadScene("TitleScene");
        MyLoadScene("TitleScene");
    }

    // Update is called once per frame
    void Update()
    {
#if AutoRestore
        restoreTimer -= Time.deltaTime;
        if (restoreTimer <= 0)
        {
            LobbyNetwork_Mgr.inst.PushPacket(LobbyNetwork_Mgr.PacketType.GetRankingList);
            restoreTimer = 5.0f;
        }

#else
        if (0.0f < restoreTimer)
            restoreTimer -= Time.deltaTime;
#endif

        if (0.0f < messageTimer)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0.0f)
            {
                MessageOnOff("", false);    //메세지 끄기
            }
        }
    }

    void MyLoadScene(string a_ScName)
    {
        bool IsFadeOk = false;
        if (Fade_Mgr.Inst != null)
            IsFadeOk = Fade_Mgr.Inst.SceneOutReserve(a_ScName);
        if (IsFadeOk == false)
            SceneManager.LoadScene(a_ScName);

        Sound_Mgr.Inst.PlayGUISound("Pop", 1.0f);
    }

    public void CfgResponse() //환경설정 박스 Ok 후 호출되게 하기 위한 함수
    {
        if (m_UserInfoText != null)
            m_UserInfoText.text = "내정보 : 별명(" + GlobalValue.g_NickName + ") : 순위(" + myRank +
                                  "등) : 점수(" + GlobalValue.g_BestScore + "점)";
    }

    private void RestoreRank()  //순위 수동리셋 버튼
    {
        if (0.0f < restoreTimer)
        {
            MessageOnOff("갱신이 너무 잦습니다. 잠시 후 버튼을 다시 눌러주세요.");
            return;
        }

        LobbyNetwork_Mgr.inst.PushPacket(LobbyNetwork_Mgr.PacketType.GetRankingList);

        restoreTimer = 5.0f;
    }

    void MessageOnOff(string a_Message = "", bool isOn = true)
    {
        if (isOn)
        {
            messageText.text = a_Message;
            messageText.gameObject.SetActive(true);
            messageTimer = 3.0f;
        }
        else
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
    }
    
}
