using System;
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

public class Game_Mgr : MonoBehaviour
{
    public Button Gawi_Btn;      //가위 버튼 연결 변수 선언
    public Button Bawi_Btn;      //바위 버튼 연결 변수 선언
    public Button Bo_Btn;        //보 버튼 연결 변수 선언
    public Button Replay_Btn;    //게임다시하기 버튼 연결 변수 선언

    public Text UserInfo_text;   //유저 정보 표시 텍스트
    public Text Result_Text;     //결과 표시 텍스트
    public Text Record_Text;

    int m_Money = 1000;          //유저의 보유 금액
    int m_WinCount = 0;          //승리 카운트
    int m_LostCount = 0;         //패배 카운트
    float m_Timer = 3.0f;        //타이머 카운트
    int m_Record;            //최고기록 카운트


    [Header("--- ShowUserData ---")]
    public InputField NickNameIF;  //별명 입력 상자
    public Button NickInput_Btn;   //별명 입력 버튼
    string m_NickName = "유저";

    [Header("--- Direction Image ---")]
    public Image UserGBB_Img;     //UserSelPanel의 GBB Image
    public Image ComGBB_Img;      //ComSelPanel의 GBB Image
    public Sprite[] m_IconSprite; //스프라이트 이미지를 연결할 배열 선언

    void Start()
    {
        if (Gawi_Btn != null)     //해당 변수에 오브젝트가 연결되어 있다면(값이 주어졌다면)
            Gawi_Btn.onClick.AddListener(() =>
            {
                BtnClickMethod(GawiBawiBo.Gawi);
            });
        if (Bawi_Btn != null)
            Bawi_Btn.onClick.AddListener(() =>
            {
                BtnClickMethod(GawiBawiBo.Bawi);
            });
        if (Bo_Btn != null)
            Bo_Btn.onClick.AddListener(() => BtnClickMethod(GawiBawiBo.Bo));
        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() =>
            {
                    SceneManager.LoadScene("SampleScene");
            });
        if (NickInput_Btn != null)
            NickInput_Btn.onClick.AddListener(NickBtnClick);
    }

    void Update()
    {
        if (m_Money <= 0)
            return;

        if (m_Timer > 0.0f)
        {
            m_Timer -= Time.deltaTime;
            if (m_Timer <= 0.0f)
                UserGBB_Img.gameObject.SetActive(false);
        }   

        if (m_Record < m_WinCount)
        {
            m_Record = m_WinCount;
            PlayerPrefs.SetInt("BestRecord", m_Record);

            Record_Text.text = $"{PlayerPrefs.GetString("UserName", "유저")}의 " +
                               $"최고기록 : ({PlayerPrefs.GetInt("BestRecord")})";
        }

        if (Input.GetKeyDown(KeyCode.C) == true)
        {
            PlayerPrefs.DeleteAll();
        }
    }

    void BtnClickMethod(GawiBawiBo a_UserSel)
    {
        if (m_Money <= 0)
            return;

        m_Timer = 3.0f;

        GawiBawiBo a_ComSel = (GawiBawiBo)UnityEngine.Random.Range((int)GawiBawiBo.Gawi,
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

        Result_Text.text = "User(" + a_strUser + ") : " + "Com(" + a_strCom + ")";

        //Judge result = Judge.Draw;
        //판정
        if (a_UserSel == a_ComSel)   // 비김
        {
            Result_Text.text += " 비겼습니다.";
        }
        else if (a_UserSel == GawiBawiBo.Gawi && a_ComSel == GawiBawiBo.Bo
            || a_UserSel == GawiBawiBo.Bawi && a_ComSel == GawiBawiBo.Gawi
            || a_UserSel == GawiBawiBo.Bo && a_ComSel == GawiBawiBo.Bawi)
        {
            Result_Text.text += " 이겼습니다.";
            //result = Judge.Win;
            Win();
        }
        else  //비긴 것도 이긴 것도 아니면?? 남은 경우는 모두 진 것
        {
            Result_Text.text += " 졌습니다.";
            //result = Judge.Lost;
            Lost();
        }

        //유저 정보 UI 갱신
        if (UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "의 보유금액 : " + m_Money +
                " : 승(" + m_WinCount +
                ") : 패(" + m_LostCount + ")";
        }

        //선택에 따른 이미지 교체
        UserGBB_Img.sprite = m_IconSprite[(int)a_UserSel -1];
                                        //enum형에 가위바위보는 1,2,3인데
                                        //배열 인덱스는 0,1,2이니까
                                        // -1을 해준거
        UserGBB_Img.gameObject.SetActive(true);

        ComGBB_Img.sprite = m_IconSprite[(int)a_ComSel-1];

        #region
        //UserGBB_Img.gameObject.SetActive(true);   //비활성화 되어 있던 오브젝트를 활성화 하는 코드
        //UserGBB_Img.sprite = GawiIcon;

        //if (a_ComSel == 1)         //컴퓨터의 선택이 가위
        //    ComGBB_Img.sprite = GawiIcon;
        //else if (a_ComSel == 2)    //컴퓨터의 선택이 바위
        //    ComGBB_Img.sprite = BawiIcon;
        //else if (a_ComSel == 3)    //컴퓨터의 선택이 보
        //    ComGBB_Img.sprite = BoIcon;

        //// 판정에 따라 보상
        //if (result == Judge.Win)
        //{
        //    m_WinCount++;
        //    m_Money += 100;
        //}
        //else if (result == Judge.Lost)
        //{
        //    m_LostCount++;
        //    m_Money -= 200;
        //    if (m_Money <= 0)
        //    {
        //        m_Money = 0;
        //        Result_Text.text = "Game Over";
        //    }
        //}
        #endregion
    }

    void Win()
    {
        m_WinCount++;
        m_Money += 100;
    }

    void Lost()
    {
        m_LostCount++;
        m_Money -= 200;
        if (m_Money <= 0)
        {
            m_Money = 0;
            Result_Text.text = "Game Over";
        }
    }
    
    private void NickBtnClick()
    {
        if (m_Money <= 0)
            return;

        string a_Nick = NickNameIF.text;
        if (a_Nick == "")
            m_NickName = "유저";
        else
            m_NickName = a_Nick;



        if (UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "의 보유금액 : " + m_Money +
                " : 승(" + m_WinCount + ")" +
                " : 패(" + m_LostCount + ")";
        }

        PlayerPrefs.SetString("UserName", m_NickName);
        if (Record_Text != null)
        {
            Record_Text.text = $"{PlayerPrefs.GetString("UserName", "유저")}의 " +
                               $"최고기록 : ({PlayerPrefs.GetInt("BestRecord")})";
        }
    }
}
