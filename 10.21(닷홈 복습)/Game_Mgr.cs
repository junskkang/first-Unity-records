using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Mgr : MonoBehaviour
{
    public Button Equal_Btn;        //��ġ�Ѵ� ��ư
    public Button Small_Btn;        //~���� �۴� ��ư
    public Button Big_Btn;          //~���� ũ�� ��ư
    public Button Replay_Btn;       //���� �ٽ��ϱ� ��ư

    public Text UserInfo_Text;
    public Text ComQuestion_Text;
    public Text Result_Text;

    int m_Count  = 0;       //���� Ƚ��
    int m_CurNum = 0;       //���������� ���� �������� ���� ���� (���� ��)
    int m_Min    = 1;       //�ּҰ�
    int m_Max    = 100;     //�ִ밪
    bool m_IsGameOver = false;  //���� ���� ���� ����

    public Button Back_Btn; //�κ�� ���ư��� ��ư
    public Button NickChange_Btn;  //�г��� ���� ��ư
    public InputField Name_InputField;
    public Text BestScore_Text;
    public Text Message_Text;
    float showMessageTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        NetworkMgr.Inst.ReadyNetworkMgr(this);

        m_CurNum = Random.Range(m_Min, (m_Max + 1));  // 1 ~ 100 ������ �߻�
        ComQuestion_Text.text = "����� ������ ���ڴ� " + m_CurNum + "�Դϱ�?";

        if (Equal_Btn != null)
            Equal_Btn.onClick.AddListener(EqualBtnClick);

        if (Small_Btn != null)
            Small_Btn.onClick.AddListener(SmallBtnClick);

        if (Big_Btn != null)
            Big_Btn.onClick.AddListener(BigBtnClick);

        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(ReplayBtnClick);

        if (Back_Btn != null)
            Back_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        if (Name_InputField != null)
            Name_InputField.text = GlobalValue.g_NickName;

        if (NickChange_Btn != null)
            NickChange_Btn.onClick.AddListener(NickChangeClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < showMessageTimer)
        {
            showMessageTimer -= Time.deltaTime;
            if (showMessageTimer <= 0.0f)
            {
                MessageOnOff("", false);    //�޽��� ����
            }
        }

        if (BestScore_Text != null)
            BestScore_Text.text = $"�ְ��� : {GlobalValue.g_BestScore}��";
    }

    private void EqualBtnClick()
    {
        if (m_IsGameOver == true)
            return;

        Result_Text.text = "����� ������ ���ڴ� " + m_CurNum + "�Դϴ�.";
        UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

        m_IsGameOver = true;

        if (GlobalValue.g_BestScore < int.MaxValue - 1)
        {
            GlobalValue.g_BestScore++;
            NetworkMgr.Inst.PushPacket(PacketType.BestScore);
        }
    }

    private void SmallBtnClick()
    {
        if(m_IsGameOver == true)
            return; 

        m_Max = m_CurNum - 1;
        m_Count++;

        if(m_Max < m_Min) //������ �� �� ������ ���, ����ó��
        {
            Result_Text.text = "��ư�� �� �� �����ϼ̽��ϴ�.(�ٽ� ����)";
        }
        else if(m_Min == m_Max) //��ġ�ϴ� ���
        {
            Result_Text.text = "����� ������ ���ڴ� " + m_Max + "�Դϴ�.";
            UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

            m_IsGameOver = true;
            return;
        }
        else //���������� max�� min���� ū ���
        {
            if(20 <= m_Count)
            {
                Result_Text.text = "����~~";
                UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

                m_IsGameOver = true;
                return;
            }

            m_CurNum = Random.Range(m_Min, (m_Max + 1));
            ComQuestion_Text.text = "����� ������ ���ڴ� " + m_CurNum + "�Դϱ�?";
            UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";
        }////���������� max�� min���� ū ���

        if (GlobalValue.g_BestScore < int.MaxValue - 1)
        {
            GlobalValue.g_BestScore++;
            NetworkMgr.Inst.PushPacket(PacketType.BestScore);
        }

    }//private void SmallBtnClick()

    private void BigBtnClick()
    {
        if(m_IsGameOver == true)
            return;

        m_Min = m_CurNum + 1;
        m_Count++;

        if(m_Max < m_Min)
        {
            Result_Text.text = "��ư�� �� �� �����ϼ̽��ϴ�.(�ٽ� ����)";
        }
        else if(m_Min == m_Max) //��ġ�ϴ� ���
        {
            Result_Text.text = "����� ������ ���ڴ� " + m_Min + "�Դϴ�.";
            UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

            m_IsGameOver = true;
            return;
        }
        else  //���������� max�� min���� ū ���
        {
            if (20 <= m_Count)
            {
                Result_Text.text = "����~~";
                UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

                m_IsGameOver = true;
                return;
            }

            m_CurNum = Random.Range(m_Min, (m_Max + 1));
            ComQuestion_Text.text = "����� ������ ���ڴ� " + m_CurNum + "�Դϱ�?";
            UserInfo_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";
        }

        if (GlobalValue.g_BestScore < int.MaxValue - 1)
        {
            GlobalValue.g_BestScore++;
            NetworkMgr.Inst.PushPacket(PacketType.BestScore);
        }

    }//private void BigBtnClick()

    private void ReplayBtnClick()
    {
        SceneManager.LoadScene("GameScene");
    }

    void NickChangeClick()
    {
        if (Name_InputField.text == GlobalValue.g_Unique_ID) return;

        string a_NickStr = Name_InputField.text.Trim();

        if (a_NickStr == "" || a_NickStr.Length < 3)
        {
            MessageOnOff("�г����� ������� 3���� �̻� �Է����ּ���.", true);
            return;
        }

        NetworkMgr.Inst.m_NickCgBuff = a_NickStr;
        NetworkMgr.Inst.PushPacket(PacketType.NickUpdate);
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

}//public class Game_Mgr : MonoBehaviour
