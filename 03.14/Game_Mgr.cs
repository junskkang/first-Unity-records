using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Mgr : MonoBehaviour
{
    public Button Gawi_Btn;      //가위 버튼 연결 변수 선언
    public Button Bawi_Btn;      //바위 버튼 연결 변수 선언
    public Button Bo_Btn;        //보 버튼 연결 변수 선언
    public Button Replay_Btn;    //게임다시하기 버튼 연결 변수 선언

    public Text UserInfo_text;   //유저 정보 표시 텍스트
    public Text Result_Text;     //결과 표시 텍스트

    int m_Money = 1000;          //유저의 보유 금액
    int m_WinCount = 0;          //승리 카운트
    int m_LostCount = 0;         //패배 카운트
    float m_Timer = 3.0f;         //타이머 카운트


    [Header("--- ShowUserData ---")]
    public InputField NickNameIF;  //별명 입력 상자
    public Button NickInput_Btn;   //별명 입력 버튼
    string m_NickName = "유저";

    [Header("--- Direction Image ---")]
    public Image UserGBB_Img;     //UserSelPanel의 GBB Image
    public Image ComGBB_Img;      //ComSelPanel의 GBB Image

    public Sprite GawiIcon;       //가위 텍스쳐
    public Sprite BawiIcon;       //바위 텍스쳐
    public Sprite BoIcon;         //보 텍스쳐

    void Start()
    {
        if (Gawi_Btn != null)     //해당 변수에 오브젝트가 연결되어 있다면(값이 주어졌다면)
            Gawi_Btn.onClick.AddListener(GawiBtnClick);
        if (Bawi_Btn != null)
            Bawi_Btn.onClick.AddListener(BawiBtnClick);
        if (Bo_Btn != null)
            Bo_Btn.onClick.AddListener(BoBtnClick);
        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() =>
            {
                    SceneManager.LoadScene("SampleScene");
            });

        // User Data Loading
        //if (NickNameIF != null)
        //    NickNameIF.text = ;

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
        
    }

    
    private void GawiBtnClick()
    {

        if (m_Money <= 0)  //게임머니가 소진되었을시 게임이 작동하지 않도록 하는 조건
            return;

        m_Timer = 3.0f;


        // 1, 가위     2, 바위     3, 보
        int a_ComSel = UnityEngine.Random.Range(1, 4);     // Random.Range(최솟값이상, 최댓값미만)

        string a_strCom = "가위";
        if (a_ComSel == 2)
            a_strCom = "바위";
        else if (a_ComSel == 3)
            a_strCom = "보";

        Result_Text.text = "User(가위) : " + "Com(" + a_strCom + ") ";


        //판정   유저는 가위 상태로 고정
        if(1 == a_ComSel)  // 비긴 경우
        {
            Result_Text.text += "비겼습니다.";
        }
        else if (3 == a_ComSel)   // 유저가 이긴 경우
        {
            Result_Text.text += "이겼습니다.";

            m_WinCount++;
            m_Money += 100;
        }
        else   // 진 경우
        {
            Result_Text.text += "졌습니다.";

            m_LostCount++;
            m_Money -= 200;

            //Game Over
            if (m_Money <= 0 )  //게임머니가 모두 소진 됐다면
            {
                m_Money = 0;
                Result_Text.text = "Game Over";
            }
               
        }

        //유저 정보 UI 갱신
        if (UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "의 보유금액 : " + m_Money +
                " : 승(" + m_WinCount +
                ") : 패(" + m_LostCount + ")";
        }

        //선택에 따른 이미지 교체
        UserGBB_Img.gameObject.SetActive(true);   //비활성화 되어 있던 오브젝트를 활성화 하는 코드
        UserGBB_Img.sprite = GawiIcon;

        if (a_ComSel == 1)         //컴퓨터의 선택이 가위
            ComGBB_Img.sprite = GawiIcon;
        else if (a_ComSel == 2)    //컴퓨터의 선택이 바위
            ComGBB_Img.sprite = BawiIcon;
        else if (a_ComSel == 3)    //컴퓨터의 선택이 보
            ComGBB_Img.sprite = BoIcon;
        
        
    }
    private void BawiBtnClick()
    {
        if (m_Money <= 0)
            return;

        m_Timer = 3.0f;

        // 1, 가위     2, 바위     3, 보
        int a_ComSel = UnityEngine.Random.Range(1, 4);     // Random.Range(최솟값이상, 최댓값미만)

        string a_strCom = "가위";
        if (a_ComSel == 2)
            a_strCom = "바위";
        else if (a_ComSel == 3)
            a_strCom = "보";

        Result_Text.text = "User(바위) : " + "Com(" + a_strCom + ") ";


        //판정   유저는 바위 상태로 고정
        if (2 == a_ComSel)  // 비긴 경우
        {
            Result_Text.text += "비겼습니다.";
        }
        else if (1 == a_ComSel)   // 유저가 이긴 경우
        {
            Result_Text.text += "이겼습니다.";

            m_WinCount++;
            m_Money += 100;
        }
        else   // 진 경우
        {
            Result_Text.text += "졌습니다.";

            m_LostCount++;
            m_Money -= 200;

            //Game Over
            if (m_Money <= 0)  //게임머니가 모두 소진 됐다면
            {
                m_Money = 0;
                Result_Text.text = "Game Over";
            }

        }

        //유저 정보 UI 갱신
        if (UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "의 보유금액 : " + m_Money +
                " : 승(" + m_WinCount +
                ") : 패(" + m_LostCount + ")";
        }

        //선택에 따른 이미지 교체
        UserGBB_Img.gameObject.SetActive(true);   //비활성화 되어 있던 오브젝트를 활성화 하는 코드
        UserGBB_Img.sprite = BawiIcon;

        if (a_ComSel == 1)         //컴퓨터의 선택이 가위
            ComGBB_Img.sprite = GawiIcon;
        else if (a_ComSel == 2)    //컴퓨터의 선택이 바위
            ComGBB_Img.sprite = BawiIcon;
        else if (a_ComSel == 3)    //컴퓨터의 선택이 보
            ComGBB_Img.sprite = BoIcon;

    }
    private void BoBtnClick()
    {
        if (m_Money <= 0)
            return;

        m_Timer = 3.0f;

        // 1, 가위     2, 바위     3, 보
        int a_ComSel = UnityEngine.Random.Range(1, 4);     // Random.Range(최솟값이상, 최댓값미만)

        string a_strCom = "가위";
        if (a_ComSel == 2)
            a_strCom = "바위";
        else if (a_ComSel == 3)
            a_strCom = "보";

        Result_Text.text = "User(보) : " + "Com(" + a_strCom + ") ";


        //판정   유저는 보 상태로 고정
        if (3 == a_ComSel)  // 비긴 경우
        {
            Result_Text.text += "비겼습니다.";
        }
        else if (2 == a_ComSel)   // 유저가 이긴 경우
        {
            Result_Text.text += "이겼습니다.";

            m_WinCount++;
            m_Money += 100;
        }
        else   // 진 경우
        {
            Result_Text.text += "졌습니다.";

            m_LostCount++;
            m_Money -= 200;

            //Game Over
            if (m_Money <= 0)  //게임머니가 모두 소진 됐다면
            {
                m_Money = 0;
                Result_Text.text = "Game Over";
            }

        }

        //유저 정보 UI 갱신
        if (UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "의 보유금액 : " + m_Money +
                " : 승(" + m_WinCount +
                ") : 패(" + m_LostCount + ")";
        }

        //선택에 따른 이미지 교체
        UserGBB_Img.gameObject.SetActive(true);   //비활성화 되어 있던 오브젝트를 활성화 하는 코드
        UserGBB_Img.sprite = BoIcon;

        if (a_ComSel == 1)         //컴퓨터의 선택이 가위
            ComGBB_Img.sprite = GawiIcon;
        else if (a_ComSel == 2)    //컴퓨터의 선택이 바위
            ComGBB_Img.sprite = BawiIcon;
        else if (a_ComSel == 3)    //컴퓨터의 선택이 보
            ComGBB_Img.sprite = BoIcon;

    }
    //private void ReplayBtnClick()
    //{
    //    SceneManager.LoadScene("SampleScene");
    //}

    private void NickBtnClick()
    {
        if (m_Money <= 0)
            return;

        string a_Nick = NickNameIF.text;
        if (a_Nick == "")
            m_NickName = "유저";
        else
            m_NickName = a_Nick;

        if(UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "의 보유금액 : " + m_Money +
                " : 승(" + m_WinCount + ")" +
                " : 패(" + m_LostCount + ")";
        }
    }
}
