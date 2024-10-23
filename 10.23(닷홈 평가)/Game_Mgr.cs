using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GawiBawiBo
{
    Gawi = 1,
    Bawi = 2,
    Bo = 3
}

public enum Record
{
    Draw = 0,  //비겼다.
    Win  = 1,  //이겼다.
    Lost = 2   //졌다.
}

public class Game_Mgr : MonoBehaviour
{
    public Button Gawi_Btn;     //가위 버튼 연결 변수
    public Button Bawi_Btn;     //바위 버튼 연결 변수
    public Button Bo_Btn;       //보 버튼 연결 변수
    public Button Replay_Btn;   //게임다시하기 버튼 연결 변수

    public Text UserInfo_Text;  //유저 정보 표시 텍스트
    public Text Result_Text;    //결과 표시 텍스트

    int m_Money = 1000;         //유저의 보유 금액
    int m_WinCount = 0;         //승리 카운트
    int m_LostCount = 0;        //패배 카운트

    int m_Point = 0;            //승리 시 100점, 연승카운트에 따른 추가 점수 부여
    int m_ContinuousCount = 0;  //연승 카운트
    bool isUpdate = false;

    [Header("--- ShowUserData ---")]
    public InputField NickNameIF;   //별명 입력 상자
    public Button NickInput_Btn;    //별명 입력 버튼
    string m_NickName = "";

    [Header("--- Direction Image ---")]
    public Image UserGBB_Img;   //UserSelPanel의 GBB Image
    public Image ComGBB_Img;    //ComSelPanel의 GBB Image

    public Sprite[] m_IconSprite;  //가위, 바위, 보 스프라이트를 연결할 배열 변수

    [Header("--- ShowBestWinCount ---")]
    public Text BestWinCountText;  //승리 최고 기록 표시 Text
    int m_BestWinCount = 0;        //최고 점수 변수
    public Button ClearSaveBtn;    //저장 정보 초기화
                                   
    float m_WaitTimer = 0.0f;

    public Text Message_Text;
    float showMessageTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (Gawi_Btn != null) //여기에 가위 버튼이 잘 연결 되어 있으면....
            Gawi_Btn.onClick.AddListener(()=>
            {
                BtnClickMethod(GawiBawiBo.Gawi);
            });

        if (Bawi_Btn != null)
            Bawi_Btn.onClick.AddListener(()=>
            {
                BtnClickMethod(GawiBawiBo.Bawi);
            });

        if (Bo_Btn != null)
            Bo_Btn.onClick.AddListener(()=>
            {
                BtnClickMethod(GawiBawiBo.Bo);
            });

        if (ClearSaveBtn != null)
            ClearSaveBtn.onClick.AddListener(() =>
            {
                //PlayerPrefs.DeleteAll();
                //GlobalValue.ClearGameData();
                SceneManager.LoadScene("LobbyScene"); 
            });

        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("GameScene");
            });

        //--- User Data Loading        
        if (NickNameIF != null)
            NickNameIF.text = GlobalValue.g_NickName;

        //m_BestWinCount = PlayerPrefs.GetInt("BestWinCount", 0);

        ////--- 유저 정보 UI 갱신
        //RefreshUserUI();
        ////--- 유저 정보 UI 갱신
        //--- User Data Loading

        if (NickInput_Btn != null)
            NickInput_Btn.onClick.AddListener(NickNameBtnClick);

        isUpdate = false;

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        if(0.0f < m_WaitTimer)
        {
            m_WaitTimer -= Time.deltaTime;
            if(m_WaitTimer <= 0.0f)
            {
                UserGBB_Img.gameObject.SetActive(false);
            }
        }//if(0.0f < m_WaitTimer)

        if (0.0f < showMessageTimer)
        {
            showMessageTimer -= Time.deltaTime;
            if (showMessageTimer <= 0.0f)
            {
                MessageOnOff("", false);    //메시지 끄기
            }
        }

        RefreshUI();

    }//void Update()


    void BtnClickMethod(GawiBawiBo a_UserSel)
    {
        if (m_Money <= 0)
            return;

        GawiBawiBo a_ComSel = (GawiBawiBo)Random.Range((int)GawiBawiBo.Gawi,
                                                       (int)GawiBawiBo.Bo + 1);

        string a_strUser = "가위";
        if (a_UserSel == GawiBawiBo.Bawi)
            a_strUser = "바위";
        else if (a_UserSel == GawiBawiBo.Bo)
            a_strUser = "보";

        string a_strCom = "가위";
        if (a_ComSel == GawiBawiBo.Bawi)
            a_strCom = "바위";
        else if (a_ComSel == GawiBawiBo.Bo)
            a_strCom = "보";

        Result_Text.text = "User(" + a_strUser + ") : Com(" + a_strCom + ")";

        //--- 판정
        Record IsWin = Record.Draw;
        if (a_UserSel == a_ComSel)
        {
            Result_Text.text += " 비겼습니다.";
            IsWin = Record.Draw;
        }
        else if ((a_UserSel == GawiBawiBo.Gawi && a_ComSel == GawiBawiBo.Bo) ||
                 (a_UserSel == GawiBawiBo.Bawi && a_ComSel == GawiBawiBo.Gawi) ||
                 (a_UserSel == GawiBawiBo.Bo && a_ComSel == GawiBawiBo.Bawi))
        {
            Result_Text.text += " 이겼습니다.";
            IsWin = Record.Win;
        }
        else
        {
            Result_Text.text += " 졌습니다.";
            IsWin = Record.Lost;
        }
        //--- 판정

        //--- 보상
        if (IsWin == Record.Win) //이겼을 때 
        {
            m_WinCount++;
            m_Money += 100;
            m_Point += 100 * (1+ m_ContinuousCount);
            m_ContinuousCount++;
        }
        else if(IsWin == Record.Lost) //졌을 때 
        {
            m_LostCount++;
            m_Money -= 200;
            m_ContinuousCount = 0;
            if(m_Money <= 0)
            {
                m_Money = 0;
                Result_Text.text = "Game Over ";
                if (isUpdate == true)
                    Result_Text.text += "기록이 갱신 되었습니다. 랭킹을 확인해보세요!";

            }
        }
        //--- 보상

        //--- 최고 승리 수 갱신
        if (GlobalValue.g_BestScore < m_WinCount)
        {
            GlobalValue.g_BestScore = m_WinCount;
            NetworkMgr.Inst.PushPacket(PacketType.BestScore);
            //PlayerPrefs.SetInt("BestWinCount", m_BestWinCount);
            isUpdate = true;
        }
        //--- 최고 승리 수 갱신

        if (GlobalValue.g_MyPoint < m_Point)
        {
            GlobalValue.g_MyPoint = m_Point;
            NetworkMgr.Inst.PushPacket(PacketType.MyPoint);
            isUpdate = true;
        }

        ////--- 유저 정보 UI 갱신
        //RefreshUserUI();
        ////--- 유저 정보 UI 갱신

        //--- 선택 상태에 따른 이미지 교체 코드
        UserGBB_Img.sprite = m_IconSprite[(int)a_UserSel - 1];
        UserGBB_Img.gameObject.SetActive(true);

        ComGBB_Img.sprite = m_IconSprite[(int)a_ComSel - 1];
        ////--- 선택 상태에 따른 이미지 교체 코드
        
        m_WaitTimer = 3.0f;  //<-- 타이머 설정
    }
 
    private void NickNameBtnClick()
    {
        if (m_Money <= 0)
            return;

        
        string a_Nick = NickNameIF.text.Trim();

        if (a_Nick == "" || a_Nick.Length < 3)
        {
            MessageOnOff("닉네임은 공백없이 3글자 이상 입력해주세요.", true);
            return;
        }

        if (!string.IsNullOrEmpty(a_Nick) && a_Nick != GlobalValue.g_NickName)
        { 
            NetworkMgr.Inst.m_NickCgBuff = a_Nick;
            NetworkMgr.Inst.PushPacket(PacketType.NickUpdate);
        }      
                

        ////--- 유저 정보 UI 갱신
        //RefreshUserUI();
        ////--- 유저 정보 UI 갱신
    }

    void RefreshUI()
    {
        //--- 유저 정보 UI 갱신
        if (UserInfo_Text != null)
            UserInfo_Text.text = GlobalValue.g_NickName + "의 보유금액 : " + m_Money +
                                " : 승(" + m_WinCount + ")" +
                                " : 패(" + m_LostCount + ")";
        //--- 유저 정보 UI 갱신

        //--- 이번 판 점수 UI 갱신
        if (BestWinCountText != null)
            BestWinCountText.text = "이긴횟수 : (" + m_WinCount + ")승 \n"
                                    +"점수 : (" + m_Point + ")점";
                                    
        //--- 최고 점수 UI 갱신
    }

    public void MessageOnOff(string Msg = "", bool isOn = true)
    {
        if (isOn == true)
        {
            Message_Text.text = Msg;
            Message_Text.gameObject.SetActive(true);
            showMessageTimer = 5.0f;
        }
        else
        {
            Message_Text.text = "";
            Message_Text.gameObject.SetActive(false);
        }
    }

}
