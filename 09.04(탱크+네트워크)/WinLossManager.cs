using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using JetBrains.Annotations;

public class WinLossManager : MonoBehaviour
{
    [HideInInspector] public double m_CheckWinTime = 2.0f;   //���� ���� �� ���� ������ 2���ĺ��� ����
    int IsRoomBuff_Team1Win = 0;    //��Ȯ�� �ѹ��� ++��Ű�� ���� Room ������ ���� ����
    int IsRoomBuff_Team2Win = 0;
    [HideInInspector] public int m_Team1Win = 0;
    [HideInInspector] public int m_Team2Win = 0;

    ExitGames.Client.Photon.Hashtable m_Team1WinProps = new ExitGames.Client.Photon.Hashtable();
    ExitGames.Client.Photon.Hashtable m_Team2WinProps = new ExitGames.Client.Photon.Hashtable();


    public static WinLossManager Inst;

    private void Awake()
    {
        Inst = this;


    }
    // Start is called before the first frame update
    void Start()
    {
        //CustomProperties �ʱ�ȭ
        InitTeam1WinProps();
        InitTeam2WinProps();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WinLossObserver()  //���� ó�� �Լ�
    {
        if (GameManager.inst == null) return;

        //�¸� �й� üũ

        if (GameManager.gameState == GameState.GS_Playing)
        {
            m_CheckWinTime -= Time.deltaTime;
            if (m_CheckWinTime <= 0.0f)
            {
                CheckAliveTeam();
            }
        }

        if (GameManager.inst.countWinLossText != null)
            GameManager.inst.countWinLossText.text = $"<color=Blue>Team1 : {m_Team1Win} ��</color> / <color=red>Team2 : {m_Team2Win} ��</color>";

        Debug.Log($"{m_Team1Win} : {m_Team2Win}");
        if (5 <= (m_Team1Win + m_Team2Win)) //5������� ��� ����� ��Ȳ�̶��
        {
            //���ӿ��� ó��
            if (PhotonNetwork.IsMasterClient)
                GameManager.inst.SendGState(GameState.GS_End);

            if (GameManager.inst.winnerText != null)
            {
                string winner = "";
                if (m_Team1Win > m_Team2Win)
                    winner = "Blue Team Win!";
                else
                    winner = "Red Team Win!";

                GameManager.inst.winnerText.gameObject.SetActive(true);
                GameManager.inst.winnerText.text = winner;  
            }

            if (GameManager.inst.waitTimerText != null)
                GameManager.inst.gameObject.SetActive(false);
            return;
        }

        //�� ���尡 ���������� ���� ���� ���۽�Ű�� ���� �κ�
        if (GameManager.inst.oldState != GameState.GS_Ready && GameManager.gameState == GameState.GS_Ready)
        {
            GameObject[] tanks = GameObject.FindGameObjectsWithTag("TANK");
            foreach (GameObject tank in tanks) 
            {
                TankDamage tankDamage = tank.GetComponent<TankDamage>();
                if (tankDamage != null)
                    tankDamage.ReadyStateTank();    //���� ���� �غ�

                Debug.Log("���� ���� �غ�");
            }
        }
        
        GameManager.inst.oldState = GameManager.gameState;
    }

    void CheckAliveTeam()
    {
        int a_Tm1Count = 0;
        int a_Tm2Count = 0;
        int rowTm1 = 0;
        int rowTm2 = 0;
        string a_PlayerTeam = "blue";
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("TANK");
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Player p in players)
        {
            if (p.CustomProperties.ContainsKey("MyTeam"))
                a_PlayerTeam = (string)p.CustomProperties["MyTeam"];

            TankDamage tankDamage = null;
            foreach (GameObject tank in tanks)
            {
                TankDamage a_TD = tank.GetComponent<TankDamage>();
                //��ũ�� playerID�� ��ź�� playerID�� �������� �Ǵ�
                if (a_TD == null) continue;

                if (a_TD.PlayerId == p.ActorNumber)
                {
                    tankDamage = a_TD;
                    break;
                }
            }

            if (a_PlayerTeam == "blue")
            {
                if (tankDamage != null && 0 < tankDamage.currHp)
                    rowTm1 = 1; //��1 �߿� �Ѹ��̶� ����ִٴ� �ǹ�
                a_Tm1Count++;   //�� �濡 �����ִ� ��1�� �÷��̾� ��
            }
            else if (a_PlayerTeam == "red")
            {
                if (tankDamage != null && 0 < tankDamage.currHp)
                    rowTm2 = 1; //��1 �߿� �Ѹ��̶� ����ִٴ� �ǹ�
                a_Tm2Count++;   //�� �濡 �����ִ� ��1�� �÷��̾� ��
            }
        }

        GameManager.inst.countDown = 4.0f;

        if (0 < rowTm1 && 0 < rowTm2) //�� ���� ��� �Ѹ� �̻� ����ִٴ� �ǹ�
            return;

        if (5 <= (m_Team1Win + m_Team2Win)) //5������� ��� ����� ��Ȳ
            return;

        if (!PhotonNetwork.IsMasterClient) return; //�¸� �й��� �߰�� ������Ŭ���̾�Ʈ�� �����ϵ���

        GameManager.inst.SendGState(GameState.GS_Ready);

        if (rowTm1 == 0) //��1 ���� ����
        {
            //m_CheckWinTime ���� ��Ȱ��
            if (-99999.0f < m_CheckWinTime)  //�ѹ��� ++��Ű�� ���� �뵵
            {
                m_Team2Win++;
                //��1�� ��� �������� ��� ���� �¸� ó��
                if (GameManager.gameState != GameState.GS_End && a_Tm1Count <= 0)
                    m_Team2Win = 5 - m_Team1Win;

                //������ �߻��ϴ��� ���� ������Ʈ�� �ȵ� ��Ȳ�̱� ������ ���� ������ �߰��� ��
                IsRoomBuff_Team2Win = m_Team2Win;
                m_CheckWinTime = -150000.0f;
            }
            SendTeam2Win(IsRoomBuff_Team2Win);
        }
        else if (rowTm2 == 0) //��2 ����
        {
            if (-99999.0f < m_CheckWinTime)  //�ѹ��� ++��Ű�� ���� �뵵
            {
                m_Team1Win++;
                //��2�� ��� �������� ��� ���� �¸� ó��
                if (GameManager.gameState != GameState.GS_End && a_Tm2Count <= 0)
                    m_Team1Win = 5 - m_Team2Win;

                //������ �߻��ϴ��� ���� ������Ʈ�� �ȵ� ��Ȳ�̱� ������ ���� ������ �߰��� ��
                IsRoomBuff_Team1Win = m_Team1Win;
                m_CheckWinTime = -150000.0f;
            }
            SendTeam1Win(IsRoomBuff_Team1Win);
        }
    }
    void InitTeam1WinProps()
    {

        if (PhotonNetwork.CurrentRoom == null) return;

        m_Team1WinProps.Clear();
        m_Team1WinProps.Add("Team1Win", 0);
        PhotonNetwork.CurrentRoom.SetCustomProperties(m_Team1WinProps);

        //Debug.Log("Team1Win CustomProperties �ʱ�ȭ �Ϸ�");
    }

    void SendTeam1Win(int a_WinCount)
    {
        if (m_Team1WinProps == null)
        {
            m_Team1WinProps = new ExitGames.Client.Photon.Hashtable();
            m_Team1WinProps.Clear();
        }

        if(m_Team1WinProps.ContainsKey("Team1Win"))
            m_Team1WinProps["Team1Win"] = a_WinCount;
        else
            m_Team1WinProps.Add("Team1Win", a_WinCount);

        PhotonNetwork.CurrentRoom.SetCustomProperties(m_Team1WinProps);
    }

    void InitTeam2WinProps()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        m_Team2WinProps.Clear();
        m_Team2WinProps.Add("Team2Win", 0);
        PhotonNetwork.CurrentRoom.SetCustomProperties(m_Team2WinProps);

        //Debug.Log("Team2Win CustomProperties �ʱ�ȭ �Ϸ�"); 
    }

    void SendTeam2Win(int a_WinCount)
    {
        if (m_Team2WinProps == null)
        {
            m_Team2WinProps = new ExitGames.Client.Photon.Hashtable();
            m_Team2WinProps.Clear();
        }

        if (m_Team2WinProps.ContainsKey("Team2Win"))
            m_Team2WinProps["Team2Win"] = a_WinCount;
        else
            m_Team2WinProps.Add("Team2Win", a_WinCount);

        PhotonNetwork.CurrentRoom.SetCustomProperties(m_Team2WinProps);
    }
}
