using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    GS_Ready = 0,
    GS_Playing,
    GS_End
}

public class GameManager : MonoBehaviourPunCallbacks
{
    //클라이언트 윈도우가 마우스 포커스를 가지고 있는지 확인하는 변수
    public static bool isFocus = true;
    public Text userCounttext;
    public Button exitBtn;

    //채팅창
    public InputField chatInput;
    [HideInInspector] public static bool isChat = false;
    //bool isWaitCtrl = false;
    public Text txtLogMsg;
    //RPC호출을 위한 포톤뷰
    PhotonView pv;

    //팀대전 관련 변수들
    public GameState oldState = GameState.GS_Ready;
    public static GameState gameState = GameState.GS_Ready;

    ExitGames.Client.Photon.Hashtable m_StateProps = 
        new ExitGames.Client.Photon.Hashtable();

    //Team Select 관련
    [Header("--- T1 UI ---")]
    public GameObject Team1Panel;
    public Button teamChange1;
    public Button team1ReadyBtn;
    public GameObject scrollTeam1;
    bool readyState = false;

    [Header("--- T2 UI ---")]
    public GameObject Team2Panel;
    public Button teamChange2;
    public Button team2ReadyBtn;
    public GameObject scrollTeam2;

    [Header("--- User Node ---")]
    public GameObject userNodeItem;

    //방장 스타트 버튼
    public Button startBtn;
    bool isStartOn = false;

    ExitGames.Client.Photon.Hashtable m_SelTeamProps =
        new ExitGames.Client.Photon.Hashtable();

    ExitGames.Client.Photon.Hashtable m_PlayerReadyState = 
        new ExitGames.Client.Photon.Hashtable();

    ExitGames.Client.Photon.Hashtable SitPosInxProps =
        new ExitGames.Client.Photon.Hashtable();
    //내부적으로 딕셔너리로 구현되어 있기 때문에 키값을 다르게 한다면
    //변수 하나에 여러가지 키값을 넣어서 사용할 수도 있음

    [HideInInspector] public static Vector3[] m_Team1Pos = new Vector3[4];
    [HideInInspector] public static Vector3[] m_Team2Pos = new Vector3[4];


    //Round 관련 변수
    [Header("--- StartTimer UI ---")]
    public Text waitTimerText;  //게임 시작 후 카운트 3, 2, 1, Go!
    [HideInInspector] public float countDown = 4.0f;
    int m_RoundCount = 0;   //총 5라운드로 진행 예정 5판 3선승

    [Header("--- WinLoss ---")]
    public Text countWinLossText;
    public Text winnerText; 

    public static GameManager inst;
    
    private void Awake()
    {
        //팀대전 관련 변수 초기화
        gameState = GameState.GS_Ready;

        m_Team1Pos[0] = new Vector3(88.4f, 20.0f, 77.9f);
        m_Team1Pos[1] = new Vector3(61.1f, 20.0f, 88.6f);
        m_Team1Pos[2] = new Vector3(34.6f, 20.0f, 98.7f);
        m_Team1Pos[3] = new Vector3(7.7f, 20.0f, 108.9f);

        m_Team2Pos[0] = new Vector3(-19.3f, 20.0f, -134.1f);
        m_Team2Pos[1] = new Vector3(-43.1f, 20.0f, -125.6f);
        m_Team2Pos[2] = new Vector3(-66.7f, 20.0f, -117.0f);
        m_Team2Pos[3] = new Vector3(-91.4f, 20.0f, -108.6f);

        inst = this;

        isChat = false;

        CreateTank();
        //포톤 클라우드의 네트워크 메세지 수신을 다시 연결
        PhotonNetwork.IsMessageQueueRunning = true;
        pv = GetComponent<PhotonView>();

        //룸에 입장 후 기존 접속자 정보를 출력
        GetConnectPlayerCount();    

        //CustomProperties 초기화
        InitSelTeamProps();
        //내가 입장할 때 나를 포함한 다른 사람들에게 내가 우선 블루팀으로 입장한다고 알림

        InitReadyProps();
        InitGameStateProps();   //GameState == Ready 상태로 시작
    }
    // Start is called before the first frame update
    void Start()
    {
        //팀셋팅
        //팀1 버튼 처리
        if (teamChange1 != null)
            teamChange1.onClick.AddListener(() =>
            {
                SendSelTeam("red"); //2팀으로 이동 및 중계(나 포함)
            });

        if (team1ReadyBtn != null)
            team1ReadyBtn.onClick.AddListener(() =>
            {
                readyState = !readyState;

                if (readyState)
                    SendReady(1);
                else
                    SendReady(0);
            });

        //팀2 버튼 처리
        if (teamChange2 != null)
            teamChange2.onClick.AddListener(() =>
            {
                SendSelTeam("blue"); //1팀으로 이동 및 중계(나 포함)
                //RPC 옵션 중 AllViaServer와 유사함. 나를 포함한 모두에게 동일한 시간대에서 중계함
                //cf)AllBuffered : 나는 즉시, 나머지는 네트워크를 통해 
            });

        if (team2ReadyBtn != null)
            team2ReadyBtn.onClick.AddListener(() =>
            {
                readyState = !readyState;

                if (readyState)
                    SendReady(1);
                else
                    SendReady(0);
            });

        if (startBtn != null && PhotonNetwork.IsMasterClient)
        {
            //버튼의 초기 상태는 SetActive(false) && Interactable == false
            startBtn.gameObject.SetActive(true);
            //startBtn.interactable = false;

            startBtn.onClick.AddListener(() =>
            {
                isStartOn = true;
                startBtn.interactable = false;
            });
        }



        if (exitBtn != null)
            exitBtn.onClick.AddListener(ClickExitBtn);

        //접속 시 로그메세지에 출력할 문자열 생성
        string msg = $"\n<color=#00ff00>[{PhotonNetwork.LocalPlayer.NickName}] Connected </color>";
        //RPC함수 호출
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);
        //AllBuffered 특징
        //방에 남아있는 유저들의 메세지 내용만 남아있음
        //새로 방에 입장하는 사람들은 기존에 채팅을 하였지만 이미 방에서 나간 사람의
        //채팅 기록을 볼 수 없지만
        //여전히 방에 남아있는 사람들의 채팅은 볼 수 있음

    }

    // Update is called once per frame
    void Update()
    {
        //게임 Update()를 돌려도 되는 상태인지 확인한다
        if (!IsGamePossible())
            return;

        //채팅 함수 호출
        Chatting();
        //TeacherChat();

        if (GameManager.gameState == GameState.GS_Ready)
        {
            if (IsDifferentList())
            {
                RefreshPhotonTeam();
            }
        }

        AllReadyObserver();

        if (gameState == GameState.GS_Playing)
        {
            Team1Panel.gameObject.SetActive(false);
            Team2Panel.gameObject.SetActive(false); 

            waitTimerText.gameObject.SetActive(false);
        }

        if (isStartOn)
        {
            ClickStartBtn();
        }

        WinLossManager.Inst.WinLossObserver();  //한쪽팀 전멸 체크 및 승패 판정
    }

    void CreateTank()
    {
        float pos = Random.Range(-100.0f, 100.0f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 20.0f, pos), Quaternion.identity);
    }

    private void OnApplicationFocus(bool focus)
    {
        isFocus = focus;

        //Debug.Log(isFocus);

        //true : 이 창에 포커스를 가져왔다는 의미
        //false : 포커스를 잃었다는 의미 
    }

    //룸 접속자 정보를 조회하는 함수
    void GetConnectPlayerCount()
    {
        //현재 입장한 룸 정보를 받아옴
        Room currRoom = PhotonNetwork.CurrentRoom;

        //현재 룸의 접속자 수와 최대 접속 가능한 수를 문자열로 구성한 후 Text에 표시
        userCounttext.text = $"( {currRoom.PlayerCount} / {currRoom.MaxPlayers} )";
    }

    //네트워크 플레이어가 접속했을때 호출되는 함수
    public override void OnPlayerEnteredRoom(Player player)
    {
        GetConnectPlayerCount();
    }

    //네트워크 플레이어가 방을 나갈 때 호출되는 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetConnectPlayerCount();
    }

    public void ClickExitBtn()
    {
        //퇴장 시 로그메세지에 출력할 문자열 생성
        string msg = $"\n<color=#ff0000>[{PhotonNetwork.LocalPlayer.NickName}] Disconnected </color>";
        //RPC함수 호출
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);


        //마지막 사람이 방을 떠날 때 룸의 CustomProperties를 초기화해줘야 함
        if (PhotonNetwork.PlayerList != null && PhotonNetwork.PlayerList.Length <= 1)
        {
            if (PhotonNetwork.CurrentRoom != null)
                PhotonNetwork.CurrentRoom.CustomProperties.Clear();
        }

        //지금 나가려는 탱크를 찾아서 그 탱크의 모든 CustomProperties를 초기화 해주기
        if (PhotonNetwork.LocalPlayer != null)
            PhotonNetwork.LocalPlayer.CustomProperties.Clear();

        //현재 룸을 빠져나가며 생성했던 모든 네트워크 객체를 삭제
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //룸에서 접속종료 후 호출되는 콜백 함수
        //PhotonNetwork.LeaveRoom(); 이후 호출
        SceneManager.LoadScene("scLobby");

    }

    List<string> msgList = new List<string>();

    [PunRPC]
    void LogMsg(string msg) //bool isChatMsg = false, PhotonMessageInfo info = default
    {
        //로컬에서 내가 보낸 메세지인 경우만
        //if(info.Sender.IsLocal && msg.Contains("#ffffff) ==true)
        //if(isChatMsg == true &&
        //  senderActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        //if (info.Sender.IsLocal && isChatMsg == true)
        //    msg = msg.Replace("#ffffff", "#ffff00");

        msgList.Add(msg);
        if (20 < msgList.Count)
            msgList.RemoveAt(0);

        txtLogMsg.text = "";
        for (int i = 0; i < msgList.Count; i++)
        {
            txtLogMsg.text += msgList[i];   //줄바꿈은 메세지에 들어있음
        }
    }


    bool IsGamePossible() //게임이 가능한 상태인지 체크하는 함수
    {
        //나가는 타이밍에 포톤 정보들이 한프레임 먼저 사라지고
        //그 다음 LoadScene()이 실행됨
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.LocalPlayer == null)
            return false;

        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameState") ||
            !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Team1Win") ||
            !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Team2Win"))
            return false;

        //Debug.Log("리턴 안됨");

        gameState = ReceiveGState();

        //WinLossManager.Inst.m_Team1Win = (int)PhotonNetwork.CurrentRoom.CustomProperties["Team1Win"];
        //WinLossManager.Inst.m_Team2Win = (int)PhotonNetwork.CurrentRoom.CustomProperties["Team2Win"];


        //Debug.Log(gameState.ToString());

        return true;

    }
    void TeacherChat()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isChat = !isChat;
            if (isChat)
            {
                chatInput.gameObject.SetActive(true);
                chatInput.ActivateInputField();
            }
            else
            {
                chatInput.gameObject.SetActive(false);
                if (!string.IsNullOrEmpty(chatInput.text))
                {
                    BroadCastingChat();
                }
            }
        }
    }

    //채팅 내용을 중계하는 함수
    void BroadCastingChat()
    {
        string msg = $"\n[{PhotonNetwork.LocalPlayer.NickName}] : <color=#fffff>{chatInput.text}</color>";

        if (PhotonNetwork.IsMasterClient)
        {
            msg = $"\n<color=#ffff00>[{PhotonNetwork.LocalPlayer.NickName}] :</color> <color=#fffff>{chatInput.text}</color>";
        }

        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg, true);

        chatInput.text = "";
    }

    void Chatting()
    {
        //채팅
        if (Input.GetKeyUp(KeyCode.Return) && chatInput.gameObject.activeSelf)
        {            
            if (chatInput.text != "")
            {
                string chat = "";

                if (isFocus)
                {
                    chat = $"\n<color=#ffff00>[{PhotonNetwork.LocalPlayer.NickName}] :</color> <color=#fffff>{chatInput.text}</color>";
                    LogMsg(chat);
                }

                chat = $"\n[{PhotonNetwork.LocalPlayer.NickName}] : <color=#fffff>{chatInput.text}</color>";


                //RPC함수 호출
                pv.RPC("LogMsg", RpcTarget.OthersBuffered, chat);
                chatInput.text = "";
                chatInput.ActivateInputField();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && chatInput.text == "")
        {
            isChat = !isChat;
            chatInput.gameObject.SetActive(isChat);
            if (chatInput.gameObject.activeSelf)
            {
                chatInput.ActivateInputField();
            }
        }
    }

    private void OnGUI()
    {
        if (PhotonNetwork.CurrentRoom == null) return;

        //게임이 아직 시작되지 않은 경우
        //각 유저의 별명, 킬 수, 사망상태 표시하기

        //if (PhotonNetwork.CurrentRoom.IsOpen)
        //    return;

        //현재 입장한 룸에 접속한 모든 네트워크 플레이어 정보를 저장
        int a_CurHp = 0;
        int curKillCount = 0;
        string playerTeam = "blue";
        Player[] players = PhotonNetwork.PlayerList;    //using Photon.Realtime;                

        GameObject[] tanks = GameObject.FindGameObjectsWithTag("TANK");

        foreach (Player a_Player in players) 
        {
            curKillCount = 0;
            if (a_Player.CustomProperties.ContainsKey("KillCount"))
                curKillCount = (int)a_Player.CustomProperties["KillCount"];

            if (a_Player.CustomProperties.ContainsKey("MyTeam"))
                playerTeam = (string)a_Player.CustomProperties["MyTeam"];

            TankDamage tankDamage = null;
            foreach (GameObject a_Tank in tanks)
            {
                TankDamage a_TankDmg = a_Tank.GetComponent<TankDamage>();
                //탱크의 PlayerID가 방에 입장한 player.ActorNumber와 일치하는지 판단
                if (a_TankDmg == null) continue;

                if (a_TankDmg.PlayerId == a_Player.ActorNumber)
                {
                    tankDamage = a_TankDmg;
                    break;
                }
            }

            if (tankDamage != null)
            {               
                //모든 케릭터의 에너지바 동기화
                a_CurHp = tankDamage.currHp;
            }
            string printStr = "";
            string stringColor = "<color=Blue>";
            if (playerTeam == "red")
                stringColor = "<color=Red>";

            printStr = stringColor +"<size=25>" +
                "[" + a_Player.ActorNumber + "] " + a_Player.NickName + " "
                + curKillCount + " kill" + "</size></color>";
            if (a_CurHp <= 0)
                printStr += "<color=Black><size=25>" + " <Die>" + "</size></color>";

            GUILayout.Label(printStr);


            //if (a_CurHp <= 0)
            //{
            //    //죽어 있을 때
            //    GUILayout.Label("<color=Blue><size=25>" +
            //    "[" + a_Player.ActorNumber + "] " + a_Player.NickName + " "
            //    + curKillCount + " kill" + "</size></color>"
            //    + "<color=Black><size=25>" + " <Die>" + "</size></color>");
            //}
            //else
            //{
            //    //살아있을 때
            //    GUILayout.Label("<color=Blue><size=25>" +
            //    "[" + a_Player.ActorNumber + "] " + a_Player.NickName + " "
            //    + curKillCount + " kill" + "</size></color>");
            //}                            
        }
    }


    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

			List<RaycastResult> results = new List<RaycastResult>();
			for (int i = 0; i < Input.touchCount; ++i)
			{
				a_EDCurPos.position = Input.GetTouch(i).position;  
				results.Clear();
				EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
			}

			return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject() 

    #region ----- 게임 상태를 동기화 처리
    void InitGameStateProps()
    {
        //PhotonNetwork.CurrentRoom << 방장이 소유하고 있는 저장 공간
        if (PhotonNetwork.CurrentRoom == null) return; //방장이 아니면 접근 불가능한 함수

        m_StateProps.Clear();
        m_StateProps.Add("GameState", (int)GameState.GS_Ready);
        PhotonNetwork.CurrentRoom.SetCustomProperties(m_StateProps);
    }

    public void SendGState(GameState state)
    {
        if (m_StateProps == null)
        {
            m_StateProps = new ExitGames.Client.Photon.Hashtable();
            m_StateProps.Clear();
        }

        if (m_StateProps.ContainsKey("GameState"))
            m_StateProps["GameState"] = (int)state;
        else
            m_StateProps.Add("GameState", (int)state);

        //Debug.Log(m_StateProps["GameState"]); //1

        PhotonNetwork.CurrentRoom.SetCustomProperties(m_StateProps); // 여기가 제대로 동작을 안하는가본데..

        //Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["GameState"]); //0
    }

    GameState ReceiveGState()   //게임 상태를 받아서 처리하는 부분
    {
        GameState a_RoomValue = GameState.GS_Ready;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameState"))
            a_RoomValue = (GameState)PhotonNetwork.CurrentRoom.CustomProperties["GameState"];

        //Debug.Log(a_RoomValue.ToString());
        
        return a_RoomValue;
    }
    
    #endregion
    #region ------ 팀선택 동기화 처리 with CustomProperties
    //CustomProperties는 세가지를 한 세트라고 생각하는 것이 좋다.
    //1. 초기화
    //2. Send
    //3. Receive

    void InitSelTeamProps() //딕셔너리와 유사한 형태 초기화 함수
    {
        //속도를 위해 버퍼를 미리 만들어 놓는다는 의미
        m_SelTeamProps.Clear();
        m_SelTeamProps.Add("MyTeam", "blue");   //팀 선택시 기본적으로 1팀으로 시작
        PhotonNetwork.LocalPlayer.SetCustomProperties(m_SelTeamProps);
        //캐릭터 별로 동기화 시키고 싶은 경우
    }

    void SendSelTeam(string a_Team)
    {
        if (string.IsNullOrEmpty(a_Team)) return;

        if (m_SelTeamProps == null) //혹시나 만들어지지 않았으면 만들기
        {
            m_SelTeamProps = new ExitGames.Client.Photon.Hashtable();
            m_SelTeamProps.Clear ();
        }

        if (m_SelTeamProps.ContainsKey("MyTeam"))
            m_SelTeamProps["MyTeam"] = a_Team;
        else
            m_SelTeamProps.Add("MyTeam", a_Team);

        PhotonNetwork.LocalPlayer.SetCustomProperties(m_SelTeamProps);
        //캐릭터 별로 동기화 시키고 싶은 경우
        //PhotonNetwork.LocalPlayer의 의미는 PhotonView.IsMine && PhotonView.Owner
    }

    public string ReceiveSelTeam(Player a_Player)
    {
        string a_TeamKind = "blue";

        if (a_Player == null) return a_TeamKind;

        if (a_Player.CustomProperties.ContainsKey("MyTeam"))
            a_TeamKind = (string)a_Player.CustomProperties["MyTeam"];

        return a_TeamKind;
    }

    bool IsDifferentList()  //true == 변화가 생김, false == 이전과 동일
    {
        GameObject[] a_UserNodeItems = GameObject.FindGameObjectsWithTag("UserNode_Item");

        //Debug.Log(a_UserNodeItems.Length);

        if (a_UserNodeItems == null) return true;   //최소한 나는 들어와있어야 하는데 아무것도 없다면 갱신이 필요하다는 뜻

        if (PhotonNetwork.PlayerList.Length != a_UserNodeItems.Length) return true; 
        //포톤에 입장해있는 유저리스트와 노드의 갯수가 다르다면 갱신

        foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)    //포톤 네트워크에 입장해있는 유저 기준으로 for문
        {
            bool a_FindNode = false;
            UserNodeItem a_UserData = null;
            foreach (GameObject a_Node in a_UserNodeItems)
            {
                a_UserData = a_Node.GetComponent<UserNodeItem>();
                if (a_UserData == null)
                    continue;

                if (a_UserData.m_UniqID == a_RefPlayer.ActorNumber) //같은 아이디가 존재하는지 체크
                {
                    if (a_UserData.m_TeamKind != ReceiveSelTeam(a_RefPlayer))
                        return true;    //해당 유저의 팀이 변경되었다면...

                    if (a_UserData.m_BeReady != ReceiveReady(a_RefPlayer))
                        return true;

                    a_FindNode = true;
                    break;
                }
            }
            if (!a_FindNode) return true;   //해당 유저가 리스트에 존재하지 않는다면 갱신 필요
        }



        return false;   //변화 없음 갱신 불필요
    }

    void RefreshPhotonTeam() //각 팀의 리스트뷰 UI를 갱신해주는 함수
    {
        //일단 초기화
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("UserNode_Item"))
        {
            Destroy(obj);
        }


        string a_TeamKind = "blue";
        GameObject a_UserNode = null;
        foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)    //RefPlayer에는 입장한 사람들의 정보가 담겨있다.
        {
            a_TeamKind = ReceiveSelTeam(a_RefPlayer);   //해당 유저의 팀 여부를 받아올 수 있는 함수 blue or red
            a_UserNode = Instantiate(userNodeItem);

            //저장된 팀 스트링에 따라 스크롤뷰를 분류해주기
            if (a_TeamKind == "blue")
                a_UserNode.transform.SetParent(scrollTeam1.transform, false);
            else if (a_TeamKind == "red")
                a_UserNode.transform.SetParent(scrollTeam2.transform, false);

            //생성한 UserNodeItem에 표시하기 위한 텍스트정보 전달
            UserNodeItem a_UserData = a_UserNode.GetComponent<UserNodeItem>();
            if (a_UserData != null)
            {
                a_UserData.m_UniqID = a_RefPlayer.ActorNumber;
                a_UserData.m_TeamKind = a_TeamKind;
                a_UserData.m_BeReady = ReceiveReady(a_RefPlayer);

                bool isMine = (a_UserData.m_UniqID == PhotonNetwork.LocalPlayer.ActorNumber);   //색상 변경을 위한 두번째 변수
                a_UserData.DisplayPlayerData(a_RefPlayer.NickName, isMine);
            }            
        }

        //이름표 색깔 바꾸기
        //탱크 위 피아식별을 위해 사용할 용도
        DisplayUserID a_DpUserId = null;
        GameObject[] a_Tanks = GameObject.FindGameObjectsWithTag("TANK");
        foreach (GameObject tank in a_Tanks)
        {
            a_DpUserId = tank.GetComponent<DisplayUserID>();
            if (a_DpUserId == null) continue;

            a_DpUserId.ChangeTeamNameColor(this);
        }

        //나의 Ready 상태에 따라서 UI 변경해주기
        if (ReceiveReady(PhotonNetwork.LocalPlayer))
        {
            //내가 ready 상태라면 팀 못바꾸도록 만들기
            //team1ReadyBtn.gameObject.SetActive(false);
            //team2ReadyBtn.gameObject.SetActive(false);
            teamChange1.gameObject.SetActive(false);
            teamChange2.gameObject.SetActive(false);
        }
        else
        {
            //내가 레디 상태가 아니라면
            a_TeamKind = ReceiveSelTeam(PhotonNetwork.LocalPlayer);
            if (a_TeamKind == "blue")
            {
                team1ReadyBtn.gameObject.SetActive(true);
                team2ReadyBtn.gameObject.SetActive(false);
                teamChange1.gameObject.SetActive(true);
                teamChange2.gameObject.SetActive(false);
            }
            else if (a_TeamKind == "red")
            {
                team1ReadyBtn.gameObject.SetActive(false);
                team2ReadyBtn.gameObject.SetActive(true);
                teamChange1.gameObject.SetActive(false);
                teamChange2.gameObject.SetActive(true);
            }
        }

    }

    #endregion

    #region ------- ready 상태 동기화 처리
    void InitReadyProps()
    {
        //속도를 위해 버퍼를 미리 만들어놓기
        m_PlayerReadyState.Clear();
        m_PlayerReadyState.Add("BeReady", 0);   //0 : 준비 전
        PhotonNetwork.LocalPlayer.SetCustomProperties(m_PlayerReadyState);
    }

    void SendReady(int a_Ready = 1)
    {
        if (m_PlayerReadyState == null) //혹시나 만들어지지 않았으면 만들기
        {
            m_PlayerReadyState = new ExitGames.Client.Photon.Hashtable();
            m_PlayerReadyState.Clear();
        }

        if (m_PlayerReadyState.ContainsKey("BeReady"))
            m_PlayerReadyState["BeReady"] = a_Ready;
        else
            m_PlayerReadyState.Add("BeReady", a_Ready);

        PhotonNetwork.LocalPlayer.SetCustomProperties(m_PlayerReadyState);
    }

    bool ReceiveReady(Player a_Player)  //해당 플레이어에 대한 정보 받아오기
    {
        if (a_Player == null) return false;

        if (!a_Player.CustomProperties.ContainsKey("BeReady")) return false;

        if ((int)a_Player.CustomProperties["BeReady"] == 1) return true;

        return false;
    }
    #endregion

    #region ----- Observer Method 모음
    void AllReadyObserver()
    {
        //모든 참가자가 Ready버튼을 눌렀는지 감시하고 게임을 시작하게 처리하는 함수
        if (gameState != GameState.GS_Ready) return;    //레디 상태에서만 체크하도록

        int a_OldGoWait = (int)countDown;

        bool a_AllReady = true;

        foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)
        {
            if (!ReceiveReady(a_RefPlayer))
            {
                a_AllReady = false;
                break;
            }
        }

        if (a_AllReady) //모두가 레디 버튼을 누르고 대기하고 있는 상태
        {
            startBtn.interactable = true;
        }
        else
        {
            startBtn.interactable = false;
        }
    }

    void ClickStartBtn()
    {
        int a_OldGoWait = (int)countDown;

        //누가 발생시켰든 동기화 시키려고 하면?
        if (m_RoundCount == 0 && PhotonNetwork.CurrentRoom.IsOpen)
        {
            //게임이 시작되면 다른 유저들이 못하도록 막는 부분
            PhotonNetwork.CurrentRoom.IsOpen = false;
            //PhotonNetwork.CurrentRoom.IsVisible = false;  //로비에서 방의 목록까지 보이지 않게 함.
        }

        //각 플레이어 PC 별로 카운트다운 타이머 UI 표시를 위한 코드
        if (0.0f < countDown)
        {
            countDown -= Time.deltaTime;
            if (waitTimerText != null)
            {
                waitTimerText.gameObject.SetActive(true);
                waitTimerText.text = ((int)countDown).ToString();
            }
        }

        //마스터 클라이언트는 각 유저의 자리배치를 함
        //1초에 한번씩 총 3번
        if (PhotonNetwork.IsMasterClient)
            if (0.0f < countDown && a_OldGoWait != (int)countDown)
            {
                //자리배정
                //왜 세번이나 반복하느냐? 
                //혹시나 모종의 이유로 마스터 클라이언트가 바뀌는 경우를 대비하여.... 
                SitPosInxMasterCtrl();
            }

        if (countDown <= 0.0f)
        {
            //타임 아웃이 되었을때 한번만 발생
            m_RoundCount++;

            Team1Panel.SetActive(false);
            Team2Panel.SetActive(false);
            waitTimerText.gameObject.SetActive(false);
            startBtn.gameObject.SetActive(false);

            WinLossManager.Inst.m_CheckWinTime = 2.0f;
            countDown = 0.0f;
        }

        //게임이 시작되었어야 하는데 아직 시작되지 않았다면
        if (PhotonNetwork.IsMasterClient)
        if (countDown <= 0.0f) 
        {
            SendGState(GameState.GS_Playing);
            isStartOn = false;            
      
        }        
    }

    void SitPosInxMasterCtrl()  //팀을 변경할 때마다 마스터클라이언트에서 중계할 예정
    {
        int a_Tm1Count = 0;
        int a_Tm2Count = 0;
        string a_TeamKind = "blue";
        foreach (Player _player in PhotonNetwork.PlayerList)
        {
            if (_player.CustomProperties.ContainsKey("MyTeam"))
            {
                a_TeamKind = (string)_player.CustomProperties["MyTeam"];    //플레이어에 저장되어있는 팀값을 가져와서 분류
            }

            if (a_TeamKind == "blue")   //파랑팀일 때 순서대로 Tm1Count 인덱스 부여
            {
                SitPosInxProps.Clear();
                SitPosInxProps.Add("SitPosInx", a_Tm1Count);
                _player.SetCustomProperties(SitPosInxProps);
                a_Tm1Count++;
            }
            else if (a_TeamKind == "red") //빨강팀일 때 순서대로 Tm2Count 인덱스 부여
            {
                SitPosInxProps.Clear();
                SitPosInxProps.Add("SitPosInx", a_Tm2Count);
                _player.SetCustomProperties(SitPosInxProps);
                a_Tm2Count++;
            }
        }

        //부여된 인덱스에 따라 초기 좌표값에서 스폰되도록 하기
    }

    #endregion
}
