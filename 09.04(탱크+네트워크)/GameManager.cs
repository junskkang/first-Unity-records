using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public static GameManager inst;
    
    private void Awake()
    {
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
    void LogMsg(string msg)
    {
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

        pv.RPC("LogMsg", RpcTarget.AllBuffered, msg);

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
