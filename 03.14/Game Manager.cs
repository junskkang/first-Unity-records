using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button Equal_Btn;
    public Button Small_Btn;
    public Button Big_Btn;
    public Button Replay_Btn;

    public Text UserInfo_Text;
    public Text ComQuestion_Text;
    public Text Result_Text;

    int m_Count = 0;      //���� Ƚ��
    int m_CurNum = 0;     //���������� ���� �������� ���� ���� (���� ��)
    int m_Min = 1;        //���� ���� �ּڰ�
    int m_Max = 100;      //���� ���� �ִ�
    bool m_IsGameOver = false; //���� ���� ���� ����

    void Start()
    {
        m_CurNum = Random.Range(m_Min, m_Max + 1);   //1 ~ 100 ������ �߻�
        ComQuestion_Text.text = "����� ������ ���ڴ� " + m_CurNum + "�Դϱ�?";
        
        if (Equal_Btn != null)
            Equal_Btn.onClick.AddListener(EqualBtnClick);
        if (Small_Btn != null)
            Small_Btn.onClick.AddListener(SmallBtnClick);
        if (Big_Btn != null)
            Big_Btn.onClick.AddListener(BigBtnClick);
        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(ReplayBtnClick);
    }

    void Update()
    {
        
    }
    private void EqualBtnClick()
    {
        if (m_IsGameOver == true)
            return;

        Result_Text.text = "����� ������ ���ڴ� " + m_CurNum + "�Դϴ�.";
        UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

        m_IsGameOver = true;
    }

    private void SmallBtnClick()
    {
        if (m_IsGameOver == true)
            return;

        m_Max = m_CurNum - 1;
        m_Count++;

        if (m_Max < m_Min) // ������ �߸� ������ ���, ����ó��
            Result_Text.text = "��ư�� �߸� �����ϼ̽��ϴ�. (�ٽ� ����)";
        else if (m_Min == m_Max) // ������ �ϳ��� ������ ���, �ٷ� �ش� ���� ���ϱ�
        {   
            Result_Text.text = "����� ������ ���ڴ� " + m_Max + "�Դϴ�.";
            UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

            m_IsGameOver = true;
            return;
        }
        else // ���� max�� min���� ū ��� (������ �ƴ� ���), ������ �ٽ� �߻�
        {
            if(m_Count >= 20)
            {
                Result_Text.text = "����!";
                UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

                m_IsGameOver = true;
                return;
            }

            m_CurNum = Random.Range(m_Min, m_Max+1);
            ComQuestion_Text.text = "����� ������ ���ڴ� " + m_CurNum + "�Դϱ�?";
            UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";
        }
    }

    private void BigBtnClick()
    {
        if (m_IsGameOver == true)
            return;

        m_Min = m_CurNum + 1;
        m_Count++;

        if (m_Max < m_Min) // ������ �߸� ������ ���, ����ó��
            Result_Text.text = "��ư�� �߸� �����ϼ̽��ϴ�. (�ٽ� ����)";
        else if (m_Min == m_Max) // ������ �ϳ��� ������ ���, �ٷ� �ش� ���� ���ϱ�
        {
            Result_Text.text = "����� ������ ���ڴ� " + m_Max + "�Դϴ�.";
            UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

            m_IsGameOver = true;
            return;
        }
        else // ���� max�� min���� ū ��� (������ �ƴ� ���), ������ �ٽ� �߻�
        {
            if (m_Count >= 20)
            {
                Result_Text.text = "����!";
                UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

                m_IsGameOver = true;
                return;
            }

            m_CurNum = Random.Range(m_Min, m_Max + 1);
            ComQuestion_Text.text = "����� ������ ���ڴ� " + m_CurNum + "�Դϱ�?";
            UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";
        }
    }

    private void ReplayBtnClick()
    {
        SceneManager.LoadScene(0);
    }


}
