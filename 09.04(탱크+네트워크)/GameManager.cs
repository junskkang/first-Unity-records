using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    GS_Ready,
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
    GameState oldState = GameState.GS_Ready;
    public static GameState gameState = GameState.GS_Ready;

    ExitGames.Client.Photon.Hashtable m_StateProps = 
        new ExitGames.Client.Photon.Hashtable();

    //Team Select ����
    [Header("--- T1 UI ---")]
    public GameObject Team1Panel;
    public Button teamChange1;
    public Button team1ReadyBtn;
    public GameObject scrollTeam1;

    [Header("--- T2 UI ---")]
    public GameObject Team2Panel;
    public Button teamChange2;
    public Button team2ReadyBtn;
    public GameObject scrollTeam2;

    [Header("--- User Node ---")]
    public GameObject userNodeItem;



    public static GameManager inst;
    
    private void Awake()
    {
        //������ ���� ���� �ʱ�ȭ
        gameState = GameState.GS_Ready;

        inst = this;

        isChat = false;

        CreateTank();
        //���� Ŭ������ ��Ʈ��ũ �޼��� ������ �ٽ� ����
        PhotonNetwork.IsMessageQueueRunning = true;
        pv = GetComponent<PhotonView>();

        GetConnectPlayerCount();
    }
    // Start is called before the first frame update
    void Start()
    {
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


    public static bool IsGamePossible() //������ ������ �������� üũ�ϴ� �Լ�
    {
        //������ Ÿ�ֿ̹� ���� �������� �������� ���� �������
        //�� ���� LoadScene()�� �����
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.LocalPlayer == null)
            return false;

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
        Player[] players = PhotonNetwork.PlayerList;    //using Photon.Realtime;                

        GameObject[] tanks = GameObject.FindGameObjectsWithTag("TANK");

        foreach (Player a_Player in players) 
        {
            curKillCount = 0;
            if (a_Player.CustomProperties.ContainsKey("KillCount"))
                curKillCount = (int)a_Player.CustomProperties["KillCount"];

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

            if (a_CurHp <= 0)
            {
                //�׾� ���� ��
                GUILayout.Label("<color=Blue><size=25>" +
                "[" + a_Player.ActorNumber + "] " + a_Player.NickName + " "
                + curKillCount + " kill" + "</size></color>"
                + "<color=Red><size=25>" + " <Die>" + "</size></color>");
            }
            else
            {
                //������� ��
                GUILayout.Label("<color=Blue><size=25>" +
                "[" + a_Player.ActorNumber + "] " + a_Player.NickName + " "
                + curKillCount + " kill" + "</size></color>");
            }                            
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
}
