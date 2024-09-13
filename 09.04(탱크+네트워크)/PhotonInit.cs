using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    //�÷��̾��� �̸��� �Է��ϴ� UI
    public InputField userIdInput;
    public Button joinButton;

    //�� �̸��� �Է¹��� UI
    public InputField roomName;
    public Button createRoomBtn;

    //�� ��� ������ ���� ������
    public GameObject roomNodePrefab; //�� ��ϸ�ŭ ������ Room Node Item
    public Transform content;         //��带 ���� �θ� ��ü
    RoomIcon[] roomIconList;          //Content ������ ���ϵ� ����� ã�� ���� ����

    
    

    private void Awake()
    {
       if (!PhotonNetwork.IsConnected)
        {
            //1. ���� Ŭ���忡 ���� �õ�
            //���� ������ ���� �õ�(���� ���� ����) -> AppId ����� ����            
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

    //���ÿ� ����� �÷��̾� �̸��� ��ȯ�ϰų� �����ϴ� �Լ�
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
        //���� �÷��̾��� �̸��� ����
        PhotonNetwork.LocalPlayer.NickName = userIdInput.text;

        //�÷��̾� �̸��� ����
        PlayerPrefs.SetString("USER_ID", userIdInput.text);

        //������ �� ���� �õ�
        PhotonNetwork.JoinRandomRoom();
    }

    private void OnGUI()
    {
        string str = PhotonNetwork.NetworkClientState.ToString();
        GUI.Label(new Rect(10, 1, 1500, 60), "<color=#00ff00><size=35>" + str + "</size></color>");
    }

    //2�� ConnetUsingSettings(); �Լ� ȣ�⿡ ���� ���� ������ �����ϸ� ȣ��Ǵ� �ݹ� �Լ�
    //PhotonNetwork.LeaveRoom(); ���� ���� ���� ���� �κ�� �����鼭 �� �Լ��� �ڵ� ȣ��
    public override void OnConnectedToMaster()
    {
        Debug.Log("���� ���� ����");

        //3�� �Ը� ���� ���ӿ����� �����κ� �ϳ�
        //���� ������ ��� �κ� �������� ������ ����

        //���濡�� �������ִ� ������ �κ�� ���� ����
        PhotonNetwork.JoinLobby();
    }

    //4�� PhotonNetwork.JoinLobby() ������ ȣ��Ǵ� �κ� ���� �ݹ��Լ�
    public override void OnJoinedLobby()
    {
        Debug.Log("�κ����� ����");

        //������ �� ���� �õ�
        //PhotonNetwork.JoinRandomRoom();

        userIdInput.text = GetUserId();
    }

    //PhotonNetwork.JoinRandomRoom();�Լ��� �����ϸ� ȣ��Ǵ� �Լ�
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("������ ���� �������� �ʽ��ϴ�.");

        //������ ���� ���ٸ� ���� �����ϸ鼭 ��
        //������ ���� ���Ǻ��� ����
        RoomOptions roomOptions = new RoomOptions();    //using Photon.Realtime;
        roomOptions.IsVisible = true;   //�κ񿡼� ���� ���� ����
        roomOptions.MaxPlayers = 8;     //�뿡 ������ �� �ִ� �ִ� ������ ��

        //������ ������ ������ ���� ����� �Լ�
        PhotonNetwork.CreateRoom("MyRoom", roomOptions);
    }


    //PhotonNetwork.CreateRoom();
    //PhotonNetwork.JoinRoom();
    //PhotonNetwork.JoinRandomRoom();
    //���� ������ ��츦 ���ؼ� �� ���忡 �����ϸ� ȣ�� �Ǵ� �Լ�
    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���忡 �����Ͽ����ϴ�.");

        //��ũ�� �����ϴ� �Լ� ȣ��
        //�÷��̾�� �ǽð����� �����ؾ��� ������Ʈ��
        //���濡�� �������ִ� �Լ��� ���ؼ� ���� �����ؾ����� 
        //�ش�뿡 ���� �ִ� ��� �÷��̾�� ���ÿ� ����ȴ�.
        //CreateTank();

        StartCoroutine(this.LoadBattleField());
    }

    //��Ʋ������ �̵��ϴ� �ڷ�ƾ �Լ�
    IEnumerator LoadBattleField()
    {
        //���� �̿��ϴ� ���� ���� Ŭ���� �����κ��� ��Ʈ��ũ �޼��� ���� �ߴ�
        PhotonNetwork.IsMessageQueueRunning = false;

        Time.timeScale = 1.0f;  //���ӿ� �� �� ���� �ӵ��� ��������

        //��׶���� �� �ε�
        //Async �񵿱�� �ε� (���Ľ� �ε�)
        //�ڿ��� �ε��ϴ� ���� �ε��ϴ� ������ �����ְų� �ϴ� �뵵�� ���
        AsyncOperation ao = SceneManager.LoadSceneAsync("scBattleField");
        //while (!ao.isDone)
        //{
        //    Debug.Log(ao.progress); //�ε� ���� ���� ���
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
        //�� �̸��� ���ų� Null�� ��� �� �̸� ����
        if (string.IsNullOrEmpty(roomName.text))
        {
            _roomName = "Room_" + Random.Range(0, 999).ToString("000");
        }

        //���� �÷��̾��� �̸��� ����
        PhotonNetwork.LocalPlayer.NickName = userIdInput.text;
        //�÷��̾� �̸��� ���ÿ� ����
        PlayerPrefs.SetString("USER_ID", userIdInput.text);

        //������ �� ���� ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 8;

        //������ ���ǿ� �´� �� ����
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
        //TypedLobby.Default : ��� �κ� ���� �������

        //MakeRoom();
    }

    //�� ���� ���н� ȣ��Ǵ� �ݹ� �Լ�
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("�� ����� ����");
        //�ַ� ���� �̸��� ���� ������ �� �����ϰ� ��.
        Debug.Log(returnCode.ToString());
        Debug.Log(message);
    }

    //������ �� ����� ���� �Ǿ����� ȣ��Ǵ� �������̵� �Լ�
    //�� ����Ʈ ������ ���� Ŭ���� �κ񿡼��� �����ϴ�.
    //<�� �Լ��� ȣ��Ǵ� ��Ȳ��>
    //1. ���� �κ�� ������ ��
    //2. ������ ���� ���� ����ų� ���� �ı��� ��(�ش� �濡�� ������������� ���� ������ ��)
    //3. ���� �����Ǵ� ������ �κ� ������� �ٸ� ����鿡�� ȣ��
    //4. ���� ����Ʈ�� ������ �� �� roomList[i].RemovedFromList = false;
    //   ���� ����Ʈ���� ����� �� roomList[i].RemovedFromList = true;
    //   (���� �����, ����, ������)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {        
        roomIconList = content.transform.GetComponentsInChildren<RoomIcon>(true);
        //roomIconList : ��ũ�Ѻ信 ���� �޸� �� ����
        //roomList : ���濡 ��ϵ� ��ȭ�� �ִ� �� ����Ʈ = ��������� �ϴ� ��

        int roomCount = roomList.Count;
        int arrIdx = 0;
        for (int i = 0; i < roomCount; i++)
        {
            arrIdx = MyFindIndex(roomIconList, roomList[i]);

            if (!roomList[i].RemovedFromList) //���� ���� �����ϰų�, �������� ��������� �ϴ� ��Ȳ
            {
                
                if (arrIdx < 0) //arrIdx == -1 : ��ġ�ϴ� ���� �����Ƿ� ���� ����
                {
                    //��ũ�� �信 �ٿ��� ���ο� �� ������Ʈ�� �����������
                    GameObject room = Instantiate(roomNodePrefab);
                    room.transform.SetParent(content, false);
                    //������ �����տ� �ؽ�Ʈ ���� ����
                    RoomIcon roomData = room.GetComponent<RoomIcon>();
                    roomData.roomName = roomList[i].Name;
                    roomData.connectPlayer = roomList[i].PlayerCount;
                    roomData.maxPlayers = roomList[i].MaxPlayers;

                    //�ؽ�Ʈ ������ UI�� ǥ��
                    roomData.DispRoomData(roomList[i].IsOpen);
                }
                else
                {
                    //�ش� ���� ����Ʈ �信 �����ϸ� ���� �濡 �������� ����
                    roomIconList[arrIdx].roomName = roomList[i].Name;
                    roomIconList[arrIdx].connectPlayer = roomList[i].PlayerCount;
                    roomIconList[arrIdx].maxPlayers = roomList[i].MaxPlayers;

                    //�ؽ�Ʈ ���� ����
                    roomIconList[arrIdx].DispRoomData(roomList[i].IsOpen);
                }
            }
            else //roomList[i].RemovedFromList == true (���� �����, ����, ������)
            {
                if (0 <= arrIdx)
                {
                    //�� �� ������ ���� �ִ� ����Ʈ�� ����� ��� ����
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
        //���� �÷��̾��� �̸��� ����
        PhotonNetwork.LocalPlayer.NickName = userIdInput.text;
        //�÷��̾� �̸��� ����
        PlayerPrefs.SetString("USER_ID", userIdInput.text);

        //�Ű������� ���޵� �̸��� �ش��ϴ� ������ ����
        PhotonNetwork.JoinRoom(roomName);
    }


}

