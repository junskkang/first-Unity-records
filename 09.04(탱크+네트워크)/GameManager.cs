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
    //Ŭ���̾�Ʈ �����찡 ���콺 ��Ŀ���� ������ �ִ��� Ȯ���ϴ� ����
    public static bool isFocus = true;
    public Text userCounttext;
    public Button exitBtn;

    //ä��â
    public InputField chatInput;
    [HideInInspector] public static bool isChat = false;
    //bool isWaitCtrl = false;
    public Text txtLogMsg;
    //RPCȣ���� ���� �����
    PhotonView pv;

    //������ ���� ������
    public GameState oldState = GameState.GS_Ready;
    public static GameState gameState = GameState.GS_Ready;

    ExitGames.Client.Photon.Hashtable m_StateProps = 
        new ExitGames.Client.Photon.Hashtable();

    //Team Select ����
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

    //���� ��ŸƮ ��ư
    public Button startBtn;
    bool isStartOn = false;

    ExitGames.Client.Photon.Hashtable m_SelTeamProps =
        new ExitGames.Client.Photon.Hashtable();

    ExitGames.Client.Photon.Hashtable m_PlayerReadyState = 
        new ExitGames.Client.Photon.Hashtable();

    ExitGames.Client.Photon.Hashtable SitPosInxProps =
        new ExitGames.Client.Photon.Hashtable();
    //���������� ��ųʸ��� �����Ǿ� �ֱ� ������ Ű���� �ٸ��� �Ѵٸ�
    //���� �ϳ��� �������� Ű���� �־ ����� ���� ����

    [HideInInspector] public static Vector3[] m_Team1Pos = new Vector3[4];
    [HideInInspector] public static Vector3[] m_Team2Pos = new Vector3[4];


    //Round ���� ����
    [Header("--- StartTimer UI ---")]
    public Text waitTimerText;  //���� ���� �� ī��Ʈ 3, 2, 1, Go!
    [HideInInspector] public float countDown = 4.0f;
    int m_RoundCount = 0;   //�� 5����� ���� ���� 5�� 3����

    [Header("--- WinLoss ---")]
    public Text countWinLossText;
    public Text winnerText; 

    public static GameManager inst;
    
    private void Awake()
    {
        //������ ���� ���� �ʱ�ȭ
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
        //���� Ŭ������ ��Ʈ��ũ �޼��� ������ �ٽ� ����
        PhotonNetwork.IsMessageQueueRunning = true;
        pv = GetComponent<PhotonView>();

        //�뿡 ���� �� ���� ������ ������ ���
        GetConnectPlayerCount();    

        //CustomProperties �ʱ�ȭ
        InitSelTeamProps();
        //���� ������ �� ���� ������ �ٸ� ����鿡�� ���� �켱 ��������� �����Ѵٰ� �˸�

        InitReadyProps();
        InitGameStateProps();   //GameState == Ready ���·� ����
    }
    // Start is called before the first frame update
    void Start()
    {
        //������
        //��1 ��ư ó��
        if (teamChange1 != null)
            teamChange1.onClick.AddListener(() =>
            {
                SendSelTeam("red"); //2������ �̵� �� �߰�(�� ����)
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

        //��2 ��ư ó��
        if (teamChange2 != null)
            teamChange2.onClick.AddListener(() =>
            {
                SendSelTeam("blue"); //1������ �̵� �� �߰�(�� ����)
                //RPC �ɼ� �� AllViaServer�� ������. ���� ������ ��ο��� ������ �ð��뿡�� �߰���
                //cf)AllBuffered : ���� ���, �������� ��Ʈ��ũ�� ���� 
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
            //��ư�� �ʱ� ���´� SetActive(false) && Interactable == false
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

        //���� �� �α׸޼����� ����� ���ڿ� ����
        string msg = $"\n<color=#00ff00>[{PhotonNetwork.LocalPlayer.NickName}] Connected </color>";
        //RPC�Լ� ȣ��
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);
        //AllBuffered Ư¡
        //�濡 �����ִ� �������� �޼��� ���븸 ��������
        //���� �濡 �����ϴ� ������� ������ ä���� �Ͽ����� �̹� �濡�� ���� �����
        //ä�� ����� �� �� ������
        //������ �濡 �����ִ� ������� ä���� �� �� ����

    }

    // Update is called once per frame
    void Update()
    {
        //���� Update()�� ������ �Ǵ� �������� Ȯ���Ѵ�
        if (!IsGamePossible())
            return;

        //ä�� �Լ� ȣ��
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

        WinLossManager.Inst.WinLossObserver();  //������ ���� üũ �� ���� ����
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

        //true : �� â�� ��Ŀ���� �����Դٴ� �ǹ�
        //false : ��Ŀ���� �Ҿ��ٴ� �ǹ� 
    }

    //�� ������ ������ ��ȸ�ϴ� �Լ�
    void GetConnectPlayerCount()
    {
        //���� ������ �� ������ �޾ƿ�
        Room currRoom = PhotonNetwork.CurrentRoom;

        //���� ���� ������ ���� �ִ� ���� ������ ���� ���ڿ��� ������ �� Text�� ǥ��
        userCounttext.text = $"( {currRoom.PlayerCount} / {currRoom.MaxPlayers} )";
    }

    //��Ʈ��ũ �÷��̾ ���������� ȣ��Ǵ� �Լ�
    public override void OnPlayerEnteredRoom(Player player)
    {
        GetConnectPlayerCount();
    }

    //��Ʈ��ũ �÷��̾ ���� ���� �� ȣ��Ǵ� �Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetConnectPlayerCount();
    }

    public void ClickExitBtn()
    {
        //���� �� �α׸޼����� ����� ���ڿ� ����
        string msg = $"\n<color=#ff0000>[{PhotonNetwork.LocalPlayer.NickName}] Disconnected </color>";
        //RPC�Լ� ȣ��
        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);


        //������ ����� ���� ���� �� ���� CustomProperties�� �ʱ�ȭ����� ��
        if (PhotonNetwork.PlayerList != null && PhotonNetwork.PlayerList.Length <= 1)
        {
            if (PhotonNetwork.CurrentRoom != null)
                PhotonNetwork.CurrentRoom.CustomProperties.Clear();
        }

        //���� �������� ��ũ�� ã�Ƽ� �� ��ũ�� ��� CustomProperties�� �ʱ�ȭ ���ֱ�
        if (PhotonNetwork.LocalPlayer != null)
            PhotonNetwork.LocalPlayer.CustomProperties.Clear();

        //���� ���� ���������� �����ߴ� ��� ��Ʈ��ũ ��ü�� ����
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        //�뿡�� �������� �� ȣ��Ǵ� �ݹ� �Լ�
        //PhotonNetwork.LeaveRoom(); ���� ȣ��
        SceneManager.LoadScene("scLobby");

    }

    List<string> msgList = new List<string>();

    [PunRPC]
    void LogMsg(string msg) //bool isChatMsg = false, PhotonMessageInfo info = default
    {
        //���ÿ��� ���� ���� �޼����� ��츸
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
            txtLogMsg.text += msgList[i];   //�ٹٲ��� �޼����� �������
        }
    }


    bool IsGamePossible() //������ ������ �������� üũ�ϴ� �Լ�
    {
        //������ Ÿ�ֿ̹� ���� �������� �������� ���� �������
        //�� ���� LoadScene()�� �����
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.LocalPlayer == null)
            return false;

        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameState") ||
            !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Team1Win") ||
            !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("Team2Win"))
            return false;

        //Debug.Log("���� �ȵ�");

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

    //ä�� ������ �߰��ϴ� �Լ�
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
        //ä��
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


                //RPC�Լ� ȣ��
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

        //������ ���� ���۵��� ���� ���
        //�� ������ ����, ų ��, ������� ǥ���ϱ�

        //if (PhotonNetwork.CurrentRoom.IsOpen)
        //    return;

        //���� ������ �뿡 ������ ��� ��Ʈ��ũ �÷��̾� ������ ����
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
                //��ũ�� PlayerID�� �濡 ������ player.ActorNumber�� ��ġ�ϴ��� �Ǵ�
                if (a_TankDmg == null) continue;

                if (a_TankDmg.PlayerId == a_Player.ActorNumber)
                {
                    tankDamage = a_TankDmg;
                    break;
                }
            }

            if (tankDamage != null)
            {               
                //��� �ɸ����� �������� ����ȭ
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
            //    //�׾� ���� ��
            //    GUILayout.Label("<color=Blue><size=25>" +
            //    "[" + a_Player.ActorNumber + "] " + a_Player.NickName + " "
            //    + curKillCount + " kill" + "</size></color>"
            //    + "<color=Black><size=25>" + " <Die>" + "</size></color>");
            //}
            //else
            //{
            //    //������� ��
            //    GUILayout.Label("<color=Blue><size=25>" +
            //    "[" + a_Player.ActorNumber + "] " + a_Player.NickName + " "
            //    + curKillCount + " kill" + "</size></color>");
            //}                            
        }
    }


    public static bool IsPointerOverUIObject() //UGUI�� UI���� ���� ��ŷ�Ǵ��� Ȯ���ϴ� �Լ�
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

    #region ----- ���� ���¸� ����ȭ ó��
    void InitGameStateProps()
    {
        //PhotonNetwork.CurrentRoom << ������ �����ϰ� �ִ� ���� ����
        if (PhotonNetwork.CurrentRoom == null) return; //������ �ƴϸ� ���� �Ұ����� �Լ�

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

        PhotonNetwork.CurrentRoom.SetCustomProperties(m_StateProps); // ���Ⱑ ����� ������ ���ϴ°�����..

        //Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["GameState"]); //0
    }

    GameState ReceiveGState()   //���� ���¸� �޾Ƽ� ó���ϴ� �κ�
    {
        GameState a_RoomValue = GameState.GS_Ready;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameState"))
            a_RoomValue = (GameState)PhotonNetwork.CurrentRoom.CustomProperties["GameState"];

        //Debug.Log(a_RoomValue.ToString());
        
        return a_RoomValue;
    }
    
    #endregion
    #region ------ ������ ����ȭ ó�� with CustomProperties
    //CustomProperties�� �������� �� ��Ʈ��� �����ϴ� ���� ����.
    //1. �ʱ�ȭ
    //2. Send
    //3. Receive

    void InitSelTeamProps() //��ųʸ��� ������ ���� �ʱ�ȭ �Լ�
    {
        //�ӵ��� ���� ���۸� �̸� ����� ���´ٴ� �ǹ�
        m_SelTeamProps.Clear();
        m_SelTeamProps.Add("MyTeam", "blue");   //�� ���ý� �⺻������ 1������ ����
        PhotonNetwork.LocalPlayer.SetCustomProperties(m_SelTeamProps);
        //ĳ���� ���� ����ȭ ��Ű�� ���� ���
    }

    void SendSelTeam(string a_Team)
    {
        if (string.IsNullOrEmpty(a_Team)) return;

        if (m_SelTeamProps == null) //Ȥ�ó� ��������� �ʾ����� �����
        {
            m_SelTeamProps = new ExitGames.Client.Photon.Hashtable();
            m_SelTeamProps.Clear ();
        }

        if (m_SelTeamProps.ContainsKey("MyTeam"))
            m_SelTeamProps["MyTeam"] = a_Team;
        else
            m_SelTeamProps.Add("MyTeam", a_Team);

        PhotonNetwork.LocalPlayer.SetCustomProperties(m_SelTeamProps);
        //ĳ���� ���� ����ȭ ��Ű�� ���� ���
        //PhotonNetwork.LocalPlayer�� �ǹ̴� PhotonView.IsMine && PhotonView.Owner
    }

    public string ReceiveSelTeam(Player a_Player)
    {
        string a_TeamKind = "blue";

        if (a_Player == null) return a_TeamKind;

        if (a_Player.CustomProperties.ContainsKey("MyTeam"))
            a_TeamKind = (string)a_Player.CustomProperties["MyTeam"];

        return a_TeamKind;
    }

    bool IsDifferentList()  //true == ��ȭ�� ����, false == ������ ����
    {
        GameObject[] a_UserNodeItems = GameObject.FindGameObjectsWithTag("UserNode_Item");

        //Debug.Log(a_UserNodeItems.Length);

        if (a_UserNodeItems == null) return true;   //�ּ��� ���� �����־�� �ϴµ� �ƹ��͵� ���ٸ� ������ �ʿ��ϴٴ� ��

        if (PhotonNetwork.PlayerList.Length != a_UserNodeItems.Length) return true; 
        //���濡 �������ִ� ��������Ʈ�� ����� ������ �ٸ��ٸ� ����

        foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)    //���� ��Ʈ��ũ�� �������ִ� ���� �������� for��
        {
            bool a_FindNode = false;
            UserNodeItem a_UserData = null;
            foreach (GameObject a_Node in a_UserNodeItems)
            {
                a_UserData = a_Node.GetComponent<UserNodeItem>();
                if (a_UserData == null)
                    continue;

                if (a_UserData.m_UniqID == a_RefPlayer.ActorNumber) //���� ���̵� �����ϴ��� üũ
                {
                    if (a_UserData.m_TeamKind != ReceiveSelTeam(a_RefPlayer))
                        return true;    //�ش� ������ ���� ����Ǿ��ٸ�...

                    if (a_UserData.m_BeReady != ReceiveReady(a_RefPlayer))
                        return true;

                    a_FindNode = true;
                    break;
                }
            }
            if (!a_FindNode) return true;   //�ش� ������ ����Ʈ�� �������� �ʴ´ٸ� ���� �ʿ�
        }



        return false;   //��ȭ ���� ���� ���ʿ�
    }

    void RefreshPhotonTeam() //�� ���� ����Ʈ�� UI�� �������ִ� �Լ�
    {
        //�ϴ� �ʱ�ȭ
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("UserNode_Item"))
        {
            Destroy(obj);
        }


        string a_TeamKind = "blue";
        GameObject a_UserNode = null;
        foreach (Player a_RefPlayer in PhotonNetwork.PlayerList)    //RefPlayer���� ������ ������� ������ ����ִ�.
        {
            a_TeamKind = ReceiveSelTeam(a_RefPlayer);   //�ش� ������ �� ���θ� �޾ƿ� �� �ִ� �Լ� blue or red
            a_UserNode = Instantiate(userNodeItem);

            //����� �� ��Ʈ���� ���� ��ũ�Ѻ並 �з����ֱ�
            if (a_TeamKind == "blue")
                a_UserNode.transform.SetParent(scrollTeam1.transform, false);
            else if (a_TeamKind == "red")
                a_UserNode.transform.SetParent(scrollTeam2.transform, false);

            //������ UserNodeItem�� ǥ���ϱ� ���� �ؽ�Ʈ���� ����
            UserNodeItem a_UserData = a_UserNode.GetComponent<UserNodeItem>();
            if (a_UserData != null)
            {
                a_UserData.m_UniqID = a_RefPlayer.ActorNumber;
                a_UserData.m_TeamKind = a_TeamKind;
                a_UserData.m_BeReady = ReceiveReady(a_RefPlayer);

                bool isMine = (a_UserData.m_UniqID == PhotonNetwork.LocalPlayer.ActorNumber);   //���� ������ ���� �ι�° ����
                a_UserData.DisplayPlayerData(a_RefPlayer.NickName, isMine);
            }            
        }

        //�̸�ǥ ���� �ٲٱ�
        //��ũ �� �Ǿƽĺ��� ���� ����� �뵵
        DisplayUserID a_DpUserId = null;
        GameObject[] a_Tanks = GameObject.FindGameObjectsWithTag("TANK");
        foreach (GameObject tank in a_Tanks)
        {
            a_DpUserId = tank.GetComponent<DisplayUserID>();
            if (a_DpUserId == null) continue;

            a_DpUserId.ChangeTeamNameColor(this);
        }

        //���� Ready ���¿� ���� UI �������ֱ�
        if (ReceiveReady(PhotonNetwork.LocalPlayer))
        {
            //���� ready ���¶�� �� ���ٲٵ��� �����
            //team1ReadyBtn.gameObject.SetActive(false);
            //team2ReadyBtn.gameObject.SetActive(false);
            teamChange1.gameObject.SetActive(false);
            teamChange2.gameObject.SetActive(false);
        }
        else
        {
            //���� ���� ���°� �ƴ϶��
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

    #region ------- ready ���� ����ȭ ó��
    void InitReadyProps()
    {
        //�ӵ��� ���� ���۸� �̸� ��������
        m_PlayerReadyState.Clear();
        m_PlayerReadyState.Add("BeReady", 0);   //0 : �غ� ��
        PhotonNetwork.LocalPlayer.SetCustomProperties(m_PlayerReadyState);
    }

    void SendReady(int a_Ready = 1)
    {
        if (m_PlayerReadyState == null) //Ȥ�ó� ��������� �ʾ����� �����
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

    bool ReceiveReady(Player a_Player)  //�ش� �÷��̾ ���� ���� �޾ƿ���
    {
        if (a_Player == null) return false;

        if (!a_Player.CustomProperties.ContainsKey("BeReady")) return false;

        if ((int)a_Player.CustomProperties["BeReady"] == 1) return true;

        return false;
    }
    #endregion

    #region ----- Observer Method ����
    void AllReadyObserver()
    {
        //��� �����ڰ� Ready��ư�� �������� �����ϰ� ������ �����ϰ� ó���ϴ� �Լ�
        if (gameState != GameState.GS_Ready) return;    //���� ���¿����� üũ�ϵ���

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

        if (a_AllReady) //��ΰ� ���� ��ư�� ������ ����ϰ� �ִ� ����
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

        //���� �߻����׵� ����ȭ ��Ű���� �ϸ�?
        if (m_RoundCount == 0 && PhotonNetwork.CurrentRoom.IsOpen)
        {
            //������ ���۵Ǹ� �ٸ� �������� ���ϵ��� ���� �κ�
            PhotonNetwork.CurrentRoom.IsOpen = false;
            //PhotonNetwork.CurrentRoom.IsVisible = false;  //�κ񿡼� ���� ��ϱ��� ������ �ʰ� ��.
        }

        //�� �÷��̾� PC ���� ī��Ʈ�ٿ� Ÿ�̸� UI ǥ�ø� ���� �ڵ�
        if (0.0f < countDown)
        {
            countDown -= Time.deltaTime;
            if (waitTimerText != null)
            {
                waitTimerText.gameObject.SetActive(true);
                waitTimerText.text = ((int)countDown).ToString();
            }
        }

        //������ Ŭ���̾�Ʈ�� �� ������ �ڸ���ġ�� ��
        //1�ʿ� �ѹ��� �� 3��
        if (PhotonNetwork.IsMasterClient)
            if (0.0f < countDown && a_OldGoWait != (int)countDown)
            {
                //�ڸ�����
                //�� �����̳� �ݺ��ϴ���? 
                //Ȥ�ó� ������ ������ ������ Ŭ���̾�Ʈ�� �ٲ�� ��츦 ����Ͽ�.... 
                SitPosInxMasterCtrl();
            }

        if (countDown <= 0.0f)
        {
            //Ÿ�� �ƿ��� �Ǿ����� �ѹ��� �߻�
            m_RoundCount++;

            Team1Panel.SetActive(false);
            Team2Panel.SetActive(false);
            waitTimerText.gameObject.SetActive(false);
            startBtn.gameObject.SetActive(false);

            WinLossManager.Inst.m_CheckWinTime = 2.0f;
            countDown = 0.0f;
        }

        //������ ���۵Ǿ���� �ϴµ� ���� ���۵��� �ʾҴٸ�
        if (PhotonNetwork.IsMasterClient)
        if (countDown <= 0.0f) 
        {
            SendGState(GameState.GS_Playing);
            isStartOn = false;            
      
        }        
    }

    void SitPosInxMasterCtrl()  //���� ������ ������ ������Ŭ���̾�Ʈ���� �߰��� ����
    {
        int a_Tm1Count = 0;
        int a_Tm2Count = 0;
        string a_TeamKind = "blue";
        foreach (Player _player in PhotonNetwork.PlayerList)
        {
            if (_player.CustomProperties.ContainsKey("MyTeam"))
            {
                a_TeamKind = (string)_player.CustomProperties["MyTeam"];    //�÷��̾ ����Ǿ��ִ� ������ �����ͼ� �з�
            }

            if (a_TeamKind == "blue")   //�Ķ����� �� ������� Tm1Count �ε��� �ο�
            {
                SitPosInxProps.Clear();
                SitPosInxProps.Add("SitPosInx", a_Tm1Count);
                _player.SetCustomProperties(SitPosInxProps);
                a_Tm1Count++;
            }
            else if (a_TeamKind == "red") //�������� �� ������� Tm2Count �ε��� �ο�
            {
                SitPosInxProps.Clear();
                SitPosInxProps.Add("SitPosInx", a_Tm2Count);
                _player.SetCustomProperties(SitPosInxProps);
                a_Tm2Count++;
            }
        }

        //�ο��� �ε����� ���� �ʱ� ��ǥ������ �����ǵ��� �ϱ�
    }

    #endregion
}
