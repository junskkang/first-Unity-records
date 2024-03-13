using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Mgr : MonoBehaviour
{
    public Button Gawi_Btn;      //���� ��ư ���� ���� ����
    public Button Bawi_Btn;      //���� ��ư ���� ���� ����
    public Button Bo_Btn;        //�� ��ư ���� ���� ����
    public Button Replay_Btn;    //���Ӵٽ��ϱ� ��ư ���� ���� ����

    public Text UserInfo_text;   //���� ���� ǥ�� �ؽ�Ʈ
    public Text Result_Text;     //��� ǥ�� �ؽ�Ʈ

    int m_Money = 1000;          //������ ���� �ݾ�
    int m_WinCount = 0;          //�¸� ī��Ʈ
    int m_LostCount = 0;         //�й� ī��Ʈ

    void Start()
    {
        if (Gawi_Btn != null)     //�ش� ������ ������Ʈ�� ����Ǿ� �ִٸ�(���� �־����ٸ�)
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
    }

    void Update()
    {
        
    }

    
    private void GawiBtnClick()
    {

        if (m_Money <= 0)  //���ӸӴϰ� �����Ǿ����� ������ �۵����� �ʵ��� �ϴ� ����
            return;


        // 1, ����     2, ����     3, ��
        int a_ComSel = UnityEngine.Random.Range(1, 4);     // Random.Range(�ּڰ��̻�, �ִ񰪹̸�)

        string a_strCom = "����";
        if (a_ComSel == 2)
            a_strCom = "����";
        else if (a_ComSel == 3)
            a_strCom = "��";

        Result_Text.text = "User(����) : " + "Com(" + a_strCom + ") ";


        //����   ������ ���� ���·� ����
        if(1 == a_ComSel)  // ��� ���
        {
            Result_Text.text += "�����ϴ�.";
        }
        else if (3 == a_ComSel)   // ������ �̱� ���
        {
            Result_Text.text += "�̰���ϴ�.";

            m_WinCount++;
            m_Money += 100;
        }
        else   // �� ���
        {
            Result_Text.text += "�����ϴ�.";

            m_LostCount++;
            m_Money -= 200;

            //Game Over
            if (m_Money <= 0 )  //���ӸӴϰ� ��� ���� �ƴٸ�
            {
                m_Money = 0;
                Result_Text.text = "Game Over";
            }
               
        }

        //���� ���� UI ����
        if (UserInfo_text != null)
        {
            UserInfo_text.text = "������ �����ݾ� : " + m_Money +
                " : ��(" + m_WinCount +
                ") : ��(" + m_LostCount + ")";
        }

        
    }
    private void BawiBtnClick()
    {
        if (m_Money <= 0)
            return;

        // 1, ����     2, ����     3, ��
        int a_ComSel = UnityEngine.Random.Range(1, 4);     // Random.Range(�ּڰ��̻�, �ִ񰪹̸�)

        string a_strCom = "����";
        if (a_ComSel == 2)
            a_strCom = "����";
        else if (a_ComSel == 3)
            a_strCom = "��";

        Result_Text.text = "User(����) : " + "Com(" + a_strCom + ") ";


        //����   ������ ���� ���·� ����
        if (2 == a_ComSel)  // ��� ���
        {
            Result_Text.text += "�����ϴ�.";
        }
        else if (1 == a_ComSel)   // ������ �̱� ���
        {
            Result_Text.text += "�̰���ϴ�.";

            m_WinCount++;
            m_Money += 100;
        }
        else   // �� ���
        {
            Result_Text.text += "�����ϴ�.";

            m_LostCount++;
            m_Money -= 200;

            //Game Over
            if (m_Money <= 0)  //���ӸӴϰ� ��� ���� �ƴٸ�
            {
                m_Money = 0;
                Result_Text.text = "Game Over";
            }

        }

        //���� ���� UI ����
        if (UserInfo_text != null)
        {
            UserInfo_text.text = "������ �����ݾ� : " + m_Money +
                " : ��(" + m_WinCount +
                ") : ��(" + m_LostCount + ")";
        }
    }
    private void BoBtnClick()
    {
        if (m_Money <= 0)
            return;

        // 1, ����     2, ����     3, ��
        int a_ComSel = UnityEngine.Random.Range(1, 4);     // Random.Range(�ּڰ��̻�, �ִ񰪹̸�)

        string a_strCom = "����";
        if (a_ComSel == 2)
            a_strCom = "����";
        else if (a_ComSel == 3)
            a_strCom = "��";

        Result_Text.text = "User(��) : " + "Com(" + a_strCom + ") ";


        //����   ������ �� ���·� ����
        if (3 == a_ComSel)  // ��� ���
        {
            Result_Text.text += "�����ϴ�.";
        }
        else if (2 == a_ComSel)   // ������ �̱� ���
        {
            Result_Text.text += "�̰���ϴ�.";

            m_WinCount++;
            m_Money += 100;
        }
        else   // �� ���
        {
            Result_Text.text += "�����ϴ�.";

            m_LostCount++;
            m_Money -= 200;

            //Game Over
            if (m_Money <= 0)  //���ӸӴϰ� ��� ���� �ƴٸ�
            {
                m_Money = 0;
                Result_Text.text = "Game Over";
            }

        }

        //���� ���� UI ����
        if (UserInfo_text != null)
        {
            UserInfo_text.text = "������ �����ݾ� : " + m_Money +
                " : ��(" + m_WinCount +
                ") : ��(" + m_LostCount + ")";
        }
    }
    //private void ReplayBtnClick()
    //{
    //    SceneManager.LoadScene("SampleScene");
    //}

}
