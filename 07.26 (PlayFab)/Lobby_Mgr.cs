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

    

    //--- 환경설정 Dlg 관련 변수
    [Header("--- ConfigBox ---")]
    public Button m_CfgBtn = null;
    public GameObject Canvas_Dialog = null;
    GameObject m_ConfigBoxObj = null;
    //--- 환경설정 Dlg 관련 변수

    [Header("--- Ranking ---")]
    public Text rankingText;
    public Button restoreRankBtn;
    int myRank = 0;
    float restoreTimer = 0.0f;  //랭킹 갱신 타이머
    float delayGetLB = 3.0f;    //로비 진입후 3초 뒤에 랭킹 한번 더 로딩하기
        

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

        Invoke("GetRankingList", delayGetLB);

        if (restoreRankBtn != null)
            restoreRankBtn.onClick.AddListener(RestoreRank);
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

    void CfgResponse() //환경설정 박스 Ok 후 호출되게 하기 위한 함수
    {
        if (m_UserInfoText != null)
            m_UserInfoText.text = "내정보 : 별명(" + GlobalValue.g_NickName + ") : 순위(" + myRank +
                                  "등) : 점수(" + GlobalValue.g_BestScore + "점)";
    }

    private void RestoreRank()  //순위 수동리셋 버튼
    {
        if (0.0f < restoreTimer)
        {
            Debug.Log("갱신이 너무 잦습니다. 잠시 후 버튼을 다시 눌러주세요.");
            return;
        }

        GetRankingList();

        restoreTimer = 5.0f;
    }

    void GetRankingList()
    {
        if (GlobalValue.g_Unique_ID == "") return;  //정상적인 로그인 상태에서만 진행되도록

        var request = new GetLeaderboardRequest()
        {
            StartPosition = 0,    //0번 인덱스부터 순서대로, 즉 1등부터
            StatisticName = "BestScore",
            //관리자 페이지의 순위표 변수 중 BestScore 기준
            MaxResultsCount = 10,  //10명까지
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,     //닉네임 요청
                ShowAvatarUrl = true,       //아바타 URL 요청               

            }
        };

        PlayFabClientAPI.GetLeaderboard(
                request,
                (result) =>
                {
                    //랭킹 받아오기 성공
                    if (rankingText == null) return;

                    string strBuff = "";

                    for (int i = 0; i < result.Leaderboard.Count; i++)
                    {
                        var curBoard = result.Leaderboard[i];

                        //등수 안에 내가 있다면 색 표시
                        if (curBoard.PlayFabId == GlobalValue.g_Unique_ID)
                            strBuff += "<color=#00ff00>";

                        strBuff += (i + 1).ToString() + "등 : " 
                                + curBoard.DisplayName + " : "
                                + curBoard.StatValue + "점 \n";

                        //등수 안에 내가 있다면 색 표시 마감
                        if (curBoard.PlayFabId == GlobalValue.g_Unique_ID)
                            strBuff += "</color>";
                    }

                    if (strBuff != "")
                        rankingText.text = strBuff;

                    //리더보드 등수를 불러온 직 후 내 등수 가져옴.
                    GetMyRanking();

                },
                (error) =>
                {
                    //랭킹 받아오기 실패
                    Debug.Log("리더보드불러오기 실패");
                }
                );
    }

    void GetMyRanking()
    {
        //GetLeaderboardAroundPlayer() : 특정 PlayFabID를 기준으로 주변으로 리스트를 불러오는 함수

        var request = new GetLeaderboardAroundPlayerRequest()
        {
            //PlayFabId = GlobalValue.g_Unique_ID,  //디폴트값으로 로그인 한 아이디로 설정이 됨
            StatisticName = "BestScore",
            MaxResultsCount = 1,    //한명의 정보만 가져온다는 뜻
            //ProfileConstraints = new PlayerProfileViewConstraints()
            //{
            //    ShowDisplayName = true,
            //}
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
                request,
                (result) =>
                {
                    if (0 < result.Leaderboard.Count)
                    {
                        var curBoard = result.Leaderboard[0];
                        myRank = curBoard.Position + 1;     //내 등수 가져오기
                        GlobalValue.g_BestScore = curBoard.StatValue; //내 최고 점수 갱신

                        CfgResponse();      //상단 UI 갱신
                    }
                },
                (error)=>
                {
                    Debug.Log("내 등수 불러오기 실패");
                }
                );
    }
}
