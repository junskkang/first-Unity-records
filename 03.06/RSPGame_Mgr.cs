using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RSPGame_Mgr : MonoBehaviour
{
    [Header("--- Button ---")]
    public Button Scissor_Btn;
    public Button Rock_Btn;
    public Button Paper_Btn;

    public Button Restart_Btn;

    [Header("--- Text ---")]
    public Text UserInfo_Text;
    public Text Result_Text;

    [Header("--- Character Image ---")]
    public Image CharacterImg;
    public Sprite WinImg;
    public Sprite LostImg;
    public Sprite WaitImg;
    public Image GameOverImg;

    [Header("--- User Image ---")]
    public Image UserImg;
    public Sprite ScissorImg;
    public Sprite RockImg;
    public Sprite PaperImg;

    [Header("--- Com Image ---")]
    public Image ComImage;
    public Sprite ScissorImg2;
    public Sprite RockImg2;
    public Sprite PaperImg2;



    [Header("--- Borrow ---")]
    public InputField UserName_InputField;
    public Button UserNameSave_Btn;
    string m_NickName = "유저";

    int m_Money = 1000;
    int m_WinCount = 0;
    int m_LostCount = 0;


    void Start()
    {
        if (Scissor_Btn != null)
            Scissor_Btn.onClick.AddListener(ScissorBtnClick);

        if (Rock_Btn != null)
            Rock_Btn.onClick.AddListener(RockBtnClick);

        if (Paper_Btn != null)
            Paper_Btn.onClick.AddListener(PaperBtnClick);

        if (Restart_Btn != null)
            Restart_Btn.onClick.AddListener(RestartBtnClick);

        if (UserNameSave_Btn != null)
            UserNameSave_Btn.onClick.AddListener(UserNameSaveBtnClick);
    }//void Start()


    //void Update()
    //{

    //}

    private void ScissorBtnClick()
    {
        if(m_Money <= 0)
            return;

        int a_UserSe = 1;
        int a_RandomNum = UnityEngine.Random.Range(1, 4);

        UserImg.sprite = ScissorImg;



        if (a_RandomNum == 3) // 유저는 가위, 컴퓨터는 보    유저 승
        {
            ComImage.sprite = PaperImg2;
            Result_Text.text = "User(가위) : Com(보) 승리하셨습니다.";
            m_WinCount++;
            m_Money += 100;

            CharacterImg.sprite = WinImg;
        }
        else if (a_RandomNum == 2) // 유저는 가위, 컴퓨터는 주먹    유저 패
        {
            ComImage.sprite = RockImg2;
            Result_Text.text = "User(가위) : Com(주먹) 패배하셨습니다.";
            m_LostCount++;
            m_Money -= 200;

            CharacterImg.sprite = LostImg;

            if (m_Money <= 0)   // 보유 게임 머니가 모두 소진된 상태
            {
                CharacterImg.gameObject.SetActive(false);
                GameOverImg.gameObject.SetActive(true);

                m_Money = 0;
                Result_Text.text = "Game Over";
            }
        }
        else  // 유저와 컴퓨터 모두 가위   비김
        {
            ComImage.sprite = ScissorImg2;
            Result_Text.text = "User(가위) : Com(가위) 비겼습니다.";

            CharacterImg.sprite = WaitImg;
        }

        //유저의 정보 UI 갱신
        UserInfo_Text.text = m_NickName + "의 보유금액 : " + m_Money +
            " : 승(" + m_WinCount + ")" +
            " : 패(" + m_LostCount + ")";



    }//private void ScissorBtnClick()

    private void RockBtnClick()
    {
        if (m_Money <= 0)
            return;

        int a_UserSe = 2;
        int a_RandomNum = UnityEngine.Random.Range(1, 4);

        UserImg.sprite = RockImg;



        if (a_RandomNum == 1) // 유저는 바위, 컴퓨터는 가위    유저 승
        {
            ComImage.sprite = ScissorImg2;
            Result_Text.text = "User(바위) : Com(가위) 승리하셨습니다.";
            m_WinCount++;
            m_Money += 100;

            CharacterImg.sprite = WinImg;
        }
        else if (a_RandomNum == 3) // 유저는 바위, 컴퓨터는 보    유저 패
        {
            ComImage.sprite = PaperImg2;
            Result_Text.text = "User(바위) : Com(보) 패배하셨습니다.";
            m_LostCount++;
            m_Money -= 200;

            CharacterImg.sprite = LostImg;

            if (m_Money <= 0)   // 보유 게임 머니가 모두 소진된 상태
            {
                CharacterImg.gameObject.SetActive(false);
                GameOverImg.gameObject.SetActive(true);

                m_Money = 0;
                Result_Text.text = "Game Over";
            }
        }
        else  // 유저와 컴퓨터 모두 바위   비김
        {
            ComImage.sprite = RockImg2;
            Result_Text.text = "User(바위) : Com(바위) 비겼습니다.";

            CharacterImg.sprite = WaitImg;
        }

        //유저의 정보 UI 갱신
        UserInfo_Text.text = m_NickName + "의 보유금액 : " + m_Money +
            " : 승(" + m_WinCount + ")" +
            " : 패(" + m_LostCount + ")";


    }//private void RockBtnClick()


    private void PaperBtnClick()
    {
        if (m_Money <= 0)
            return;

        int a_UserSe = 3;
        int a_RandomNum = UnityEngine.Random.Range(1, 4);

        UserImg.sprite = PaperImg;



        if (a_RandomNum == 2) // 유저는 보, 컴퓨터는 바위    유저 승
        {
            ComImage.sprite = RockImg2;
            Result_Text.text = "User(보) : Com(바위) 승리하셨습니다.";
            m_WinCount++;
            m_Money += 100;

            CharacterImg.sprite = WinImg;
        }
        else if (a_RandomNum == 1) // 유저는 보, 컴퓨터는 가위    유저 패
        {
            ComImage.sprite = PaperImg2;
            Result_Text.text = "User(보) : Com(가위) 패배하셨습니다.";
            m_LostCount++;
            m_Money -= 200;

            CharacterImg.sprite = LostImg;

            if (m_Money <= 0)   // 보유 게임 머니가 모두 소진된 상태
            {
                CharacterImg.gameObject.SetActive(false);
                GameOverImg.gameObject.SetActive(true);

                m_Money = 0;
                Result_Text.text = "Game Over";
            }
        }
        else  // 유저와 컴퓨터 모두 보   비김
        {
            ComImage.sprite = PaperImg2;
            Result_Text.text = "User(보) : Com(보) 비겼습니다.";

            CharacterImg.sprite = WaitImg;
        }

        //유저의 정보 UI 갱신
        UserInfo_Text.text = m_NickName + "의 보유금액 : " + m_Money +
            " : 승(" + m_WinCount + ")" +
            " : 패(" + m_LostCount + ")";


    }//private void PaperBtnClick()

    private void RestartBtnClick()
    {
        SceneManager.LoadScene("RSPGame");
    }//private void RestartBtnClick()

    private void UserNameSaveBtnClick()
    {
        if (m_Money <= 0) 
            return;

        string a_Nick = UserName_InputField.text;
        if (a_Nick == "")
            m_NickName = "유저";
        else
            m_NickName = a_Nick;

        //유저의 정보 UI 갱신
        UserInfo_Text.text = m_NickName + "의 보유금액 : " + m_Money +
            " : 승(" + m_WinCount + ")" +
            " : 패(" + m_LostCount + ")";



    }//private void UserNameSaveBtnClick()
}
