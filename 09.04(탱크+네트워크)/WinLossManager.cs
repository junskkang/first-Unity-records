using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class WinLossManager : MonoBehaviour
{
    [HideInInspector] public double m_CheckWinTime = 2.0f;   //라운드 시작 후 승패 판정은 2초후부터 시작
    int IsRoomBuff_Team1Win = 0;    //정확히 한번만 ++시키기 위한 Room 기준의 버퍼 변수
    int IsRoomBuff_Team2Win = 0;
    int m_Team1Win = 0;
    int m_Team2Win = 0;

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
    }

    void CheckAliveTeam()
    {

    }
    void InitTeam1WinProps()
    {

    }

    void InitTeam2WinProps()
    {

    }
}
