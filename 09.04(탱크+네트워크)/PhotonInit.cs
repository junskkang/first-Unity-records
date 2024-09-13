using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    //플레이어의 이름을 입력하는 UI
    public InputField userIdInput;
    public Button joinButton;

    //룸 이름을 입력받을 UI
    public InputField roomName;
    public Button createRoomBtn;

    //룸 목록 갱신을 위한 변수들
    public GameObject roomNodePrefab; //룸 목록만큼 생성될 Room Node Item
    public Transform content;         //노드를 붙일 부모 객체
    RoomIcon[] roomIconList;          //Content 하위의 차일드 목록을 찾기 위한 변수

    
    

    private void Awake()
    {
       if (!PhotonNetwork.IsConnected)
        {
            //1. 포톤 클라우드에 접속 시도
            //포톤 서버에 접속 시도(지역 서버 접속) -> AppId 사용자 인증            
            PhotonNetwork.ConnectUsingSettings();

        }

        userIdInput.text = GetUserId();
        roomName.text = "Room_" + Random.Range(0, 999).ToString("000");
        roomNodePrefab = Resources.Load("RoomIcon") as GameObject;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (joinButton != null)
            joinButton.onClick.AddListener(ClickJoninButton);

        if (createRoomBtn != null)
            createRoomBtn.onClick.AddListener(OnClickCreateRoom);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //로컬에 저장된 플레이어 이름을 반환하거나 생성하는 함수
    string GetUserId()
    {
        string userId = PlayerPrefs.GetString("USER_ID");
        if (string.IsNullOrEmpty(userId))
        {
            userId = "USER_" + Random.Range(0, 999).ToString("000");
        }

        return userId;
    }

    public void ClickJoninButton()
    {
        //로컬 플레이어의 이름을 설정
        PhotonNetwork.LocalPlayer.NickName = userIdInput.text;

        //플레이어 이름을 저장
        PlayerPrefs.SetString("USER_ID", userIdInput.text);

        //무작위 방 입장 시도
        PhotonNetwork.JoinRandomRoom();
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
        //PhotonNetwork.JoinRandomRoom();

        userIdInput.text = GetUserId();
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
        //CreateTank();

        StartCoroutine(this.LoadBattleField());
    }

    //배틀씬으로 이동하는 코루틴 함수
    IEnumerator LoadBattleField()
    {
        //씬을 이용하는 동안 포톤 클라우드 서버로부터 네트워크 메세지 수신 중단
        PhotonNetwork.IsMessageQueueRunning = false;

        Time.timeScale = 1.0f;  //게임에 들어갈 때 원래 속도로 돌려놓기

        //백그라운드로 씬 로딩
        //Async 비동기식 로딩 (병렬식 로딩)
        //뒤에서 로딩하는 동안 로딩하는 연출을 보여주거나 하는 용도로 사용
        AsyncOperation ao = SceneManager.LoadSceneAsync("scBattleField");
        //while (!ao.isDone)
        //{
        //    Debug.Log(ao.progress); //로딩 진행 상태 출력
        //}
        


        yield return ao;
    }

    //void CreateTank()
    //{
    //    float pos = Random.Range(-100.0f, 100.0f);
    //    PhotonNetwork.Instantiate("Tank", new Vector3(pos, 20.0f, pos), Quaternion.identity, 0);
    //}

    public void OnClickCreateRoom()
    {
        string _roomName = roomName.text;
        //룸 이름이 없거나 Null일 경우 룸 이름 지정
        if (string.IsNullOrEmpty(roomName.text))
        {
            _roomName = "Room_" + Random.Range(0, 999).ToString("000");
        }

        //로컬 플레이어의 이름을 설정
        PhotonNetwork.LocalPlayer.NickName = userIdInput.text;
        //플레이어 이름을 로컬에 저장
        PlayerPrefs.SetString("USER_ID", userIdInput.text);

        //생성할 룸 조건 설정
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 8;

        //지정한 조건에 맞는 룸 생성
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
        //TypedLobby.Default : 어느 로비에 방을 만들건지

        //MakeRoom();
    }

    //룸 생성 실패시 호출되는 콜백 함수
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 만들기 실패");
        //주로 같은 이름의 방이 존재할 때 실패하게 됨.
        Debug.Log(returnCode.ToString());
        Debug.Log(message);
    }

    //생성된 룸 목록이 변경 되었을때 호출되는 오버라이드 함수
    //방 리스트 갱신은 포톤 클라우드 로비에서만 가능하다.
    //<이 함수가 호출되는 상황들>
    //1. 내가 로비로 진입할 때
    //2. 누군가 방을 새로 만들거나 방이 파괴될 때(해당 방에서 마지막사람까지 방을 나갔을 때)
    //3. 방이 생성되는 시점에 로비에 대기중인 다른 사람들에게 호출
    //4. 방이 리스트에 노출이 될 때 roomList[i].RemovedFromList = false;
    //   방이 리스트에서 사라질 때 roomList[i].RemovedFromList = true;
    //   (방이 사라짐, 꽉참, 숨겨짐)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {        
        roomIconList = content.transform.GetComponentsInChildren<RoomIcon>(true);
        //roomIconList : 스크롤뷰에 노드로 달린 방 갯수
        //roomList : 포톤에 등록된 변화가 있는 방 리스트 = 갱신해줘야 하는 방

        int roomCount = roomList.Count;
        int arrIdx = 0;
        for (int i = 0; i < roomCount; i++)
        {
            arrIdx = MyFindIndex(roomIconList, roomList[i]);

            if (!roomList[i].RemovedFromList) //방을 새로 생성하거나, 방정보를 갱신해줘야 하는 상황
            {
                
                if (arrIdx < 0) //arrIdx == -1 : 일치하는 방이 없으므로 새로 생성
                {
                    //스크롤 뷰에 붙여줄 새로운 방 오브젝트를 생성해줘야함
                    GameObject room = Instantiate(roomNodePrefab);
                    room.transform.SetParent(content, false);
                    //생성한 프리팹에 텍스트 정보 전달
                    RoomIcon roomData = room.GetComponent<RoomIcon>();
                    roomData.roomName = roomList[i].Name;
                    roomData.connectPlayer = roomList[i].PlayerCount;
                    roomData.maxPlayers = roomList[i].MaxPlayers;

                    //텍스트 정보를 UI에 표시
                    roomData.DispRoomData(roomList[i].IsOpen);
                }
                else
                {
                    //해당 방이 리스트 뷰에 존재하면 기존 방에 방정보만 갱신
                    roomIconList[arrIdx].roomName = roomList[i].Name;
                    roomIconList[arrIdx].connectPlayer = roomList[i].PlayerCount;
                    roomIconList[arrIdx].maxPlayers = roomList[i].MaxPlayers;

                    //텍스트 정보 갱신
                    roomIconList[arrIdx].DispRoomData(roomList[i].IsOpen);
                }
            }
            else //roomList[i].RemovedFromList == true (방이 사라짐, 꽉참, 숨겨짐)
            {
                if (0 <= arrIdx)
                {
                    //이 방 정보를 갖고 있는 리스트뷰 목록을 모두 제거
                    MyDestroy(roomIconList, roomList[i]);
                }
            }
        }
    }

    int MyFindIndex(RoomIcon[] roomIconList, RoomInfo roomInfo)
    {
        if (roomIconList == null) return -1;

        if (roomIconList.Length <= 0) return -1;

        for (int i = 0; i < roomIconList.Length; i++)
        {
            if (roomIconList[i].roomName == roomInfo.Name)
            {
                return i;
            }
        }

        return -1;
    }

    void MyDestroy(RoomIcon[] roomIconList, RoomInfo roomInfo)
    {
        if (roomIconList == null) return;
        if (roomIconList.Length <= 0) return;

        for (int i = 0; i < roomIconList.Length; i++)
        {
            if (roomIconList[i].roomName == roomInfo.Name)
            {
                Destroy(roomIconList[i].gameObject);

            }
        }
    }

    public void OnClickRoomIcon(string roomName)
    {
        //로컬 플레이어의 이름을 설정
        PhotonNetwork.LocalPlayer.NickName = userIdInput.text;
        //플레이어 이름을 저장
        PlayerPrefs.SetString("USER_ID", userIdInput.text);

        //매개변수로 전달된 이름에 해당하는 룸으로 입장
        PhotonNetwork.JoinRoom(roomName);
    }


}

