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
    float ClearLockTimer = 0.0f;     //데이터 초기화 대기 타이머

    public Text UserInfoText;

    [HideInInspector] public int m_MyRank = 0;
    public Button RestRk_Btn;  //Restore Ranking Button
    public Text   Ranking_Text;
    public Button RestPRk_Btn;

    public Text MessageText;    //메시지 내용을 표시할 UI
    float  ShowMsTimer = 0.0f;  //메시지를 몇 초동안 보이게 할 건지에 대한 타이머

    //[Header("--- ConfigBox ---")]
    public Button m_CfgBtn = null;
    //public GameObject Canvas_Dialog = null;
    //public GameObject m_ConfigBoxObj = null;    

    //--- 싱글턴 패턴
    public static LobbyMgr Inst = null;

    private void Awake()
    {
        Inst = this;
    }
    //--- 싱글턴 패턴

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f; //일시정지를 원래 속도로...
        //GlobalValue.LoadGameData();

        NetworkMgr.Inst.ReadyNetworkMgr(this);   //씬마다 추가

        if (m_Start_Btn != null)
            m_Start_Btn.onClick.AddListener(StartBtnClick);

        if (m_Store_Btn != null)
            m_Store_Btn.onClick.AddListener(() =>
            {
                MessageOnOff("해당 기능은 현재 미구현입니다. 게임을 즐겨주세요 :)", true);
                //SceneManager.LoadScene("StoreScene");
            });

        if (m_Logout_Btn != null)
            m_Logout_Btn.onClick.AddListener(() =>
            {
                GlobalValue.ClearGameData();
                SceneManager.LoadScene("TitleScene");
            });

        if (m_Clear_Save_Btn != null)
            m_Clear_Save_Btn.onClick.AddListener(Clear_Save_Click);

        RefreshUserInfo();

#if AutoRestore
        //--- 자동 랭킹 갱신인 경우
        if(RestRk_Btn != null)
            RestRk_Btn.gameObject.SetActive(false);
        //--- 자동 랭킹 갱신인 경우
#else
        //--- 수동 랭킹 갱신인 경우
        if (RestRk_Btn != null)
            RestRk_Btn.onClick.AddListener(RestoreRank);

        if (RestPRk_Btn != null)
            RestPRk_Btn.onClick.AddListener(RestorePointRank);
        //--- 수동 랭킹 갱신인 경우
#endif

        //--- 환경설정 Dlg 관련 구현 부분
        if (m_CfgBtn != null)
            m_CfgBtn.onClick.AddListener(() =>
            {
                MessageOnOff("해당 기능은 현재 미구현입니다. 게임을 즐겨주세요 :)", true);
                //닉네이 변경 요청 버튼
                //if (m_ConfigBoxObj == null)
                //    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                //GameObject a_CfgBoxObj = Instantiate(m_ConfigBoxObj);
                //a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);
                //Time.timeScale = 0.0f;
            });
        //--- 환경설정 Dlg 관련 구현 부분
    }//void Start()

    // Update is called once per frame
    void Update()
    {
        if(0.0f < ShowMsTimer)
        {
            ShowMsTimer -= Time.deltaTime;
            if(ShowMsTimer <= 0.0f)
            {
                MessageOnOff("", false);    //메시지 끄기
            }
        }//if(0.0f < ShowMsTimer)

        if (0.0f < ClearLockTimer)
            ClearLockTimer -= Time.deltaTime;
    }

    void StartBtnClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    void Clear_Save_Click()
    {
        MessageOnOff("해당 기능은 현재 미구현입니다. 게임을 즐겨주세요 :)", true);
        //if(0.0f < ClearLockTimer)
        //{
        //    MessageOnOff("저장정보초기화 중입니다.");
        //    return;
        //}

        //PlayerPrefs.DeleteAll();
        ////GlobalValue.LoadGameData();
        ////RefreshUserInfo();

        ////NetworkMgr.Inst.LobbyNetCom.DltMethod = Result_Clear_Save;
        //NetworkMgr.Inst.PushPacket(PacketType.ClearSave);

        //ClearLockTimer = 5.0f;  //5초 동안 다시 시도 못하게 막는 걸로 하려는 의도
        ////서버로부터 응답을 받은 후에 다시 시도할 수 있게 허용하는 것으로 변경함
    }

    //void Result_Clear_Save(bool IsOk)
    //{
    //    //--- 서버 초기화 성공 후 모든 변수 초기화 필요
    //    if(IsOk == true)
    //    {
    //        GlobalValue.g_BestScore = 0;    //게임점수
    //        GlobalValue.g_UserGold  = 0;    //게임머니
    //        GlobalValue.g_Exp = 0;          //경험치 Experience
    //        GlobalValue.g_Level = 0;        //레벨

    //        GlobalValue.g_BestFloor = 1;    //최종 도달 건물 층수
    //        GlobalValue.g_CurFloorNum = 1;  //현재 건물 층수

    //        for(int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
    //            GlobalValue.g_SkillCount[i] = 1;    //내 아이템 보유 정보

    //        RefreshUserInfo();  //점수, 골드값 UI 초기화
    //    }
    //    //--- 서버 초기화 성공 후 모든 변수 초기화 필요
    //}

    public void RefreshUserInfo()
    {
        UserInfoText.text = "내정보 : 별명(" + GlobalValue.g_NickName +
                            ") : 순위(" + m_MyRank + "등) : 최고기록(" +
                            GlobalValue.g_BestScore.ToString("N0") + "승 / " +
                            GlobalValue.g_MyPoint.ToString("N0") + "점)";
    }

    public void RefreshRankUI(RkRootInfo a_RkRootInfo)
    {
        Ranking_Text.text = "";

        for (int i = 0; i < a_RkRootInfo.RkList.Length; i++)
        {
            // 등수 안에 내가 있다면 색 표시
            if (a_RkRootInfo.RkList[i].user_id == GlobalValue.g_Unique_ID)
                Ranking_Text.text += "<color=#00ff00>";

            Ranking_Text.text += (i + 1).ToString() + "등 : " +
                                a_RkRootInfo.RkList[i].user_id +
                                " (" + a_RkRootInfo.RkList[i].nick_name + ") : " +
                                a_RkRootInfo.RkList[i].best_score + "승 / " + a_RkRootInfo.RkList[i].mypoint + "점 \n";

            if (a_RkRootInfo.RkList[i].user_id == GlobalValue.g_Unique_ID)
                Ranking_Text.text += "</color>";
        }//for (int i = 0; i < a_RkRootInfo.RkList.Length; i++)

        m_MyRank = a_RkRootInfo.my_rank;

        RefreshUserInfo();
    }//public void RefreshRankUI(RkRootInfo a_RkRootInfo)

    public void MessageOnOff(string Mess = "", bool isOn = true, float a_Time = 5.0f)
    {
        if(isOn == true)
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

    void RestoreRank()
    {
        if(0.0f < NetworkMgr.Inst.LobbyNetCom.RestoreTimer)
        {
            MessageOnOff($"{NetworkMgr.Inst.LobbyNetCom.RestoreTimer.ToString("N0")}초 후 재시도 해주세요.");
            return;
        }

        NetworkMgr.Inst.PushPacket(PacketType.GetRankingList);
        NetworkMgr.Inst.LobbyNetCom.RestoreTimer = 5.0f;

        RestRk_Btn.interactable = false;
        RestPRk_Btn.interactable = true;

        MessageOnOff($"랭킹이 갱신되었습니다!");
    }

    void RestorePointRank()
    {
        if (0.0f < NetworkMgr.Inst.LobbyNetCom.RestoreTimer)
        {
            MessageOnOff($"{NetworkMgr.Inst.LobbyNetCom.RestoreTimer.ToString("N0")}초 후 재시도 해주세요.");
            return;
        }

        NetworkMgr.Inst.PushPacket(PacketType.GetRankingPointList);
        NetworkMgr.Inst.LobbyNetCom.RestoreTimer = 5.0f;

        RestPRk_Btn.interactable = false;
        RestRk_Btn.interactable = true;

        MessageOnOff($"랭킹이 갱신되었습니다!");
    }
}
