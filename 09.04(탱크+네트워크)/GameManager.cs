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

    public static GameManager inst;
    
    private void Awake()
    {
        inst = this;

        isChat = false;

        CreateTank();
        //포톤 클라우드의 네트워크 메세지 수신을 다시 연결
        PhotonNetwork.IsMessageQueueRunning = true;
        pv = GetComponent<PhotonView>();

        GetConnectPlayerCount();
    }
    // Start is called before the first frame update
    void Start()
    {
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
    void LogMsg(string msg)
    {
        msgList.Add(msg);
        if (20 < msgList.Count)
            msgList.RemoveAt(0);

        txtLogMsg.text = "";
        for (int i = 0; i < msgList.Count; i++)
        {
            txtLogMsg.text += msgList[i];   //줄바꿈은 메세지에 들어있음
        }
    }

    public static bool IsGamePossible() //게임이 가능한 상태인지 체크하는 함수
    {
        //나가는 타이밍에 포톤 정보들이 한프레임 먼저 사라지고
        //그 다음 LoadScene()이 실행됨
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

    //채팅 내용을 중계하는 함수
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
}
