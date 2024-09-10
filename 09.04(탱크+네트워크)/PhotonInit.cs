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
            //1. ���� Ŭ���忡 ���� �õ�
            //���� ������ ���� �õ�(���� ���� ����) -> AppId ����� ����            
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
        PhotonNetwork.JoinRandomRoom();
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
        CreateTank();
    }

    void CreateTank()
    {
        float pos = Random.Range(-100.0f, 100.0f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 20.0f, pos), Quaternion.identity, 0);
    }
}

