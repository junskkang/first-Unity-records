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
    string m_NickName = "����";

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



        if (a_RandomNum == 3) // ������ ����, ��ǻ�ʹ� ��    ���� ��
        {
            ComImage.sprite = PaperImg2;
            Result_Text.text = "User(����) : Com(��) �¸��ϼ̽��ϴ�.";
            m_WinCount++;
            m_Money += 100;

            CharacterImg.sprite = WinImg;
        }
        else if (a_RandomNum == 2) // ������ ����, ��ǻ�ʹ� �ָ�    ���� ��
        {
            ComImage.sprite = RockImg2;
            Result_Text.text = "User(����) : Com(�ָ�) �й��ϼ̽��ϴ�.";
            m_LostCount++;
            m_Money -= 200;

            CharacterImg.sprite = LostImg;

            if (m_Money <= 0)   // ���� ���� �Ӵϰ� ��� ������ ����
            {
                CharacterImg.gameObject.SetActive(false);
                GameOverImg.gameObject.SetActive(true);

                m_Money = 0;
                Result_Text.text = "Game Over";
            }
        }
        else  // ������ ��ǻ�� ��� ����   ���
        {
            ComImage.sprite = ScissorImg2;
            Result_Text.text = "User(����) : Com(����) �����ϴ�.";

            CharacterImg.sprite = WaitImg;
        }

        //������ ���� UI ����
        UserInfo_Text.text = m_NickName + "�� �����ݾ� : " + m_Money +
            " : ��(" + m_WinCount + ")" +
            " : ��(" + m_LostCount + ")";



    }//private void ScissorBtnClick()

    private void RockBtnClick()
    {
        if (m_Money <= 0)
            return;

        int a_UserSe = 2;
        int a_RandomNum = UnityEngine.Random.Range(1, 4);

        UserImg.sprite = RockImg;



        if (a_RandomNum == 1) // ������ ����, ��ǻ�ʹ� ����    ���� ��
        {
            ComImage.sprite = ScissorImg2;
            Result_Text.text = "User(����) : Com(����) �¸��ϼ̽��ϴ�.";
            m_WinCount++;
            m_Money += 100;

            CharacterImg.sprite = WinImg;
        }
        else if (a_RandomNum == 3) // ������ ����, ��ǻ�ʹ� ��    ���� ��
        {
            ComImage.sprite = PaperImg2;
            Result_Text.text = "User(����) : Com(��) �й��ϼ̽��ϴ�.";
            m_LostCount++;
            m_Money -= 200;

            CharacterImg.sprite = LostImg;

            if (m_Money <= 0)   // ���� ���� �Ӵϰ� ��� ������ ����
            {
                CharacterImg.gameObject.SetActive(false);
                GameOverImg.gameObject.SetActive(true);

                m_Money = 0;
                Result_Text.text = "Game Over";
            }
        }
        else  // ������ ��ǻ�� ��� ����   ���
        {
            ComImage.sprite = RockImg2;
            Result_Text.text = "User(����) : Com(����) �����ϴ�.";

            CharacterImg.sprite = WaitImg;
        }

        //������ ���� UI ����
        UserInfo_Text.text = m_NickName + "�� �����ݾ� : " + m_Money +
            " : ��(" + m_WinCount + ")" +
            " : ��(" + m_LostCount + ")";


    }//private void RockBtnClick()


    private void PaperBtnClick()
    {
        if (m_Money <= 0)
            return;

        int a_UserSe = 3;
        int a_RandomNum = UnityEngine.Random.Range(1, 4);

        UserImg.sprite = PaperImg;



        if (a_RandomNum == 2) // ������ ��, ��ǻ�ʹ� ����    ���� ��
        {
            ComImage.sprite = RockImg2;
            Result_Text.text = "User(��) : Com(����) �¸��ϼ̽��ϴ�.";
            m_WinCount++;
            m_Money += 100;

            CharacterImg.sprite = WinImg;
        }
        else if (a_RandomNum == 1) // ������ ��, ��ǻ�ʹ� ����    ���� ��
        {
            ComImage.sprite = PaperImg2;
            Result_Text.text = "User(��) : Com(����) �й��ϼ̽��ϴ�.";
            m_LostCount++;
            m_Money -= 200;

            CharacterImg.sprite = LostImg;

            if (m_Money <= 0)   // ���� ���� �Ӵϰ� ��� ������ ����
            {
                CharacterImg.gameObject.SetActive(false);
                GameOverImg.gameObject.SetActive(true);

                m_Money = 0;
                Result_Text.text = "Game Over";
            }
        }
        else  // ������ ��ǻ�� ��� ��   ���
        {
            ComImage.sprite = PaperImg2;
            Result_Text.text = "User(��) : Com(��) �����ϴ�.";

            CharacterImg.sprite = WaitImg;
        }

        //������ ���� UI ����
        UserInfo_Text.text = m_NickName + "�� �����ݾ� : " + m_Money +
            " : ��(" + m_WinCount + ")" +
            " : ��(" + m_LostCount + ")";


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
            m_NickName = "����";
        else
            m_NickName = a_Nick;

        //������ ���� UI ����
        UserInfo_Text.text = m_NickName + "�� �����ݾ� : " + m_Money +
            " : ��(" + m_WinCount + ")" +
            " : ��(" + m_LostCount + ")";



    }//private void UserNameSaveBtnClick()
}
