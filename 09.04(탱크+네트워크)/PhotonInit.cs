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

   
}

