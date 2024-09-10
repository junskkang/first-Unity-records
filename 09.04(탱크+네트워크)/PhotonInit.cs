using Photon.Pun;
using Photon.Realtime;
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

    //PhotonNetwork.JoinRandomRoom();함수가 실패하면 호출되는 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("접속할 방이 존재하지 않습니다.");

        //접속할 방이 없다면 방을 생성하면서 들어감
        //생성할 룸의 조건부터 설정
        RoomOptions roomOptions = new RoomOptions();    //using Photon.Realtime;
        roomOptions.IsVisible = true;   //로비에서 룸의 노출 여부
        roomOptions.MaxPlayers = 8;     //룸에 입장할 수 있는 최대 접속자 수

        //위에서 지정한 조건을 갖는 룸생성 함수
        PhotonNetwork.CreateRoom("MyRoom", roomOptions);
    }


    //PhotonNetwork.CreateRoom();
    //PhotonNetwork.JoinRoom();
    //PhotonNetwork.JoinRandomRoom();
    //위의 세가지 경우를 통해서 방 입장에 성공하면 호출 되는 함수
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장에 성공하였습니다.");

        //탱크를 생성하는 함수 호출
        //플레이어끼리 실시간으로 공유해야할 오브젝트는
        //포톤에서 제공해주는 함수를 통해서 동적 생성해야지만 
        //해당룸에 같이 있는 모든 플레이어에게 동시에 적용된다.
        CreateTank();
    }

    void CreateTank()
    {
        float pos = Random.Range(-100.0f, 100.0f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 20.0f, pos), Quaternion.identity, 0);
    }
}

