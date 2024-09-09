using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
       if (!PhotonNetwork.IsConnected)
        {
            //1. 포톤 클라우드에 접속 시도
            //포톤 서버에 접속 시도(지역 서버 접속) -> AppId 사용자 인증            
            PhotonNetwork.ConnectUsingSettings();

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        string str = PhotonNetwork.NetworkClientState.ToString();
        GUI.Label(new Rect(10, 1, 1500, 60), "<color=#00ff00><size=35>" + str + "</size></color>");
    }

    //2번 ConnetUsingSettings(); 함수 호출에 대한 서버 접속이 성공하면 호출되는 콜백 함수
    //PhotonNetwork.LeaveRoom(); 으로 방을 떠날 때도 로비로 나오면서 이 함수가 자동 호출
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 접속 성공");

        //3번 규모가 작은 게임에서는 서버로비가 하나
        //대형 게임인 경우 로비도 여러개로 나누어 관리

        //포톤에서 제공해주는 가상의 로비로 입장 진행
        PhotonNetwork.JoinLobby();
    }

    //4번 PhotonNetwork.JoinLobby() 성공시 호출되는 로비 접속 콜백함수
    public override void OnJoinedLobby()
    {
        Debug.Log("로비접속 성공");

        //무작위 방 입장 시도
        PhotonNetwork.JoinRandomRoom();
    }

   
}

