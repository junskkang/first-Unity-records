using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using JetBrains.Annotations;

public class WinLossManager : MonoBehaviour
{
    [HideInInspector] public double m_CheckWinTime = 2.0f;   //라운드 시작 후 승패 판정은 2초후부터 시작
    int IsRoomBuff_Team1Win = 0;    //정확히 한번만 ++시키기 위한 Room 기준의 버퍼 변수
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
        //CustomProperties 초기화
        InitTeam1WinProps();
        InitTeam2WinProps();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void WinLossObserver()  //승패 처리 함수
    {
        if (GameManager.inst == null) return;

        //승리 패배 체크

        if (GameManager.gameState == GameState.GS_Playing)
        {
            m_CheckWinTime -= Time.deltaTime;
            if (m_CheckWinTime <= 0.0f)
            {
                CheckAliveTeam();
            }
        }

        if (GameManager.inst.countWinLossText != null)
            GameManager.inst.countWinLossText.text = $"<color=Blue>Team1 : {m_Team1Win} 승</color> / <color=red>Team2 : {m_Team2Win} 승</color>";

        Debug.Log($"{m_Team1Win} : {m_Team2Win}");
        if (5 <= (m_Team1Win + m_Team2Win)) //5라운드까지 모두 진행된 상황이라면
        {
            //게임오버 처리
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

        //한 라운드가 끝날때마다 다음 라운드 시작시키기 위한 부분
        if (GameManager.inst.oldState != GameState.GS_Ready && GameManager.gameState == GameState.GS_Ready)
        {
            GameObject[] tanks = GameObject.FindGameObjectsWithTag("TANK");
            foreach (GameObject tank in tanks) 
            {
                TankDamage tankDamage = tank.GetComponent<TankDamage>();
                if (tankDamage != null)
                    tankDamage.ReadyStateTank();    //다음 라운드 준비

                Debug.Log("다음 라운드 준비");
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
                //탱크의 playerID가 포탄의 playerID와 동일한지 판단
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
                    rowTm1 = 1; //팀1 중에 한명이라도 살아있다는 의미
                a_Tm1Count++;   //이 방에 남아있는 팀1의 플레이어 수
            }
            else if (a_PlayerTeam == "red")
            {
                if (tankDamage != null && 0 < tankDamage.currHp)
                    rowTm2 = 1; //팀1 중에 한명이라도 살아있다는 의미
                a_Tm2Count++;   //이 방에 남아있는 팀1의 플레이어 수
            }
        }

        GameManager.inst.countDown = 4.0f;

        if (0 < rowTm1 && 0 < rowTm2) //양 팀이 모두 한명 이상 살아있다는 의미
            return;

        if (5 <= (m_Team1Win + m_Team2Win)) //5라운드까지 모두 진행된 상황
            return;

        if (!PhotonNetwork.IsMasterClient) return; //승리 패배의 중계는 마스터클라이언트만 가능하도록

        GameManager.inst.SendGState(GameState.GS_Ready);

        if (rowTm1 == 0) //팀1 전멸 상태
        {
            //m_CheckWinTime 변수 재활용
            if (-99999.0f < m_CheckWinTime)  //한번만 ++시키기 위한 용도
            {
                m_Team2Win++;
                //팀1이 모두 나가버린 경우 강제 승리 처리
                if (GameManager.gameState != GameState.GS_End && a_Tm1Count <= 0)
                    m_Team2Win = 5 - m_Team1Win;

                //여러번 발생하더라도 아직 업데이트가 안된 상황이기 때문에 이전 값에서 추가될 것
                IsRoomBuff_Team2Win = m_Team2Win;
                m_CheckWinTime = -150000.0f;
            }
            SendTeam2Win(IsRoomBuff_Team2Win);
        }
        else if (rowTm2 == 0) //팀2 전멸
        {
            if (-99999.0f < m_CheckWinTime)  //한번만 ++시키기 위한 용도
            {
                m_Team1Win++;
                //팀2이 모두 나가버린 경우 강제 승리 처리
                if (GameManager.gameState != GameState.GS_End && a_Tm2Count <= 0)
                    m_Team1Win = 5 - m_Team2Win;

                //여러번 발생하더라도 아직 업데이트가 안된 상황이기 때문에 이전 값에서 추가될 것
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

        //Debug.Log("Team1Win CustomProperties 초기화 완료");
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

        //Debug.Log("Team2Win CustomProperties 초기화 완료"); 
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
