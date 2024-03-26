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
    public Button Gawi_Btn;      //���� ��ư ���� ���� ����
    public Button Bawi_Btn;      //���� ��ư ���� ���� ����
    public Button Bo_Btn;        //�� ��ư ���� ���� ����
    public Button Replay_Btn;    //���Ӵٽ��ϱ� ��ư ���� ���� ����

    public Text UserInfo_text;   //���� ���� ǥ�� �ؽ�Ʈ
    public Text Result_Text;     //��� ǥ�� �ؽ�Ʈ
    public Text Record_Text;

    int m_Money = 1000;          //������ ���� �ݾ�
    int m_WinCount = 0;          //�¸� ī��Ʈ
    int m_LostCount = 0;         //�й� ī��Ʈ
    float m_Timer = 3.0f;        //Ÿ�̸� ī��Ʈ
    int m_Record;            //�ְ��� ī��Ʈ


    [Header("--- ShowUserData ---")]
    public InputField NickNameIF;  //���� �Է� ����
    public Button NickInput_Btn;   //���� �Է� ��ư
    string m_NickName = "����";

    [Header("--- Direction Image ---")]
    public Image UserGBB_Img;     //UserSelPanel�� GBB Image
    public Image ComGBB_Img;      //ComSelPanel�� GBB Image
    public Sprite[] m_IconSprite; //��������Ʈ �̹����� ������ �迭 ����

    void Start()
    {
        if (Gawi_Btn != null)     //�ش� ������ ������Ʈ�� ����Ǿ� �ִٸ�(���� �־����ٸ�)
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

            Record_Text.text = $"{PlayerPrefs.GetString("UserName", "����")}�� " +
                               $"�ְ��� : ({PlayerPrefs.GetInt("BestRecord")})";
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
        string a_strUser = "����";
        if (a_UserSel == GawiBawiBo.Bawi)
            a_strUser = "����";
        else if (a_UserSel == GawiBawiBo.Bo)
            a_strUser = "��";

        string a_strCom = "����";
        if (a_ComSel == GawiBawiBo.Bawi)
            a_strCom = "����";
        else if (a_ComSel == GawiBawiBo.Bo)
            a_strCom = "��";

        Result_Text.text = "User(" + a_strUser + ") : " + "Com(" + a_strCom + ")";

        //Judge result = Judge.Draw;
        //����
        if (a_UserSel == a_ComSel)   // ���
        {
            Result_Text.text += " �����ϴ�.";
        }
        else if (a_UserSel == GawiBawiBo.Gawi && a_ComSel == GawiBawiBo.Bo
            || a_UserSel == GawiBawiBo.Bawi && a_ComSel == GawiBawiBo.Gawi
            || a_UserSel == GawiBawiBo.Bo && a_ComSel == GawiBawiBo.Bawi)
        {
            Result_Text.text += " �̰���ϴ�.";
            //result = Judge.Win;
            Win();
        }
        else  //��� �͵� �̱� �͵� �ƴϸ�?? ���� ���� ��� �� ��
        {
            Result_Text.text += " �����ϴ�.";
            //result = Judge.Lost;
            Lost();
        }

        //���� ���� UI ����
        if (UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "�� �����ݾ� : " + m_Money +
                " : ��(" + m_WinCount +
                ") : ��(" + m_LostCount + ")";
        }

        //���ÿ� ���� �̹��� ��ü
        UserGBB_Img.sprite = m_IconSprite[(int)a_UserSel -1];
                                        //enum���� ������������ 1,2,3�ε�
                                        //�迭 �ε����� 0,1,2�̴ϱ�
                                        // -1�� ���ذ�
        UserGBB_Img.gameObject.SetActive(true);

        ComGBB_Img.sprite = m_IconSprite[(int)a_ComSel-1];

        #region
        //UserGBB_Img.gameObject.SetActive(true);   //��Ȱ��ȭ �Ǿ� �ִ� ������Ʈ�� Ȱ��ȭ �ϴ� �ڵ�
        //UserGBB_Img.sprite = GawiIcon;

        //if (a_ComSel == 1)         //��ǻ���� ������ ����
        //    ComGBB_Img.sprite = GawiIcon;
        //else if (a_ComSel == 2)    //��ǻ���� ������ ����
        //    ComGBB_Img.sprite = BawiIcon;
        //else if (a_ComSel == 3)    //��ǻ���� ������ ��
        //    ComGBB_Img.sprite = BoIcon;

        //// ������ ���� ����
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
            m_NickName = "����";
        else
            m_NickName = a_Nick;



        if (UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "�� �����ݾ� : " + m_Money +
                " : ��(" + m_WinCount + ")" +
                " : ��(" + m_LostCount + ")";
        }

        PlayerPrefs.SetString("UserName", m_NickName);
        if (Record_Text != null)
        {
            Record_Text.text = $"{PlayerPrefs.GetString("UserName", "����")}�� " +
                               $"�ְ��� : ({PlayerPrefs.GetInt("BestRecord")})";
        }
    }
}
