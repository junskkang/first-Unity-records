using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public InputField chatInput = null;
    public Text chatLogText = null;
    [HideInInspector] public static bool isChat = false;
    List<string> messages = new List<string>();

    float chatCleanerTimer = 0.0f;
    float resetTimer = 15.0f;
    PhotonView pv = null;

    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();

        isChat = false;
        chatCleanerTimer = resetTimer;

        //접속 메세지
        string msg = $"\n<color=#D94B4E>[{PhotonNetwork.LocalPlayer.NickName}]님이 입장했습니다. 환영해주세요!</color>";
        pv.RPC("LogMessages", RpcTarget.AllBuffered, msg, -1);

        Debug.Log(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    // Update is called once per frame
    void Update()
    {
        Chatting();

        if (messages.Count > 0)
        {
            AutoChatCleaner();
        }
    }

    void Chatting()
    {
        if (Input.GetKeyDown(KeyCode.Return) && chatInput.gameObject.activeSelf)
        {
            if (chatInput.text != "")
            {
                string msg = "";

                msg = $"\n<color=#00FF7C>[{PhotonNetwork.LocalPlayer.NickName}] :</color> <color=#ffffff>{chatInput.text}</color>";

                pv.RPC("LogMessages", RpcTarget.AllBuffered, msg, PhotonNetwork.LocalPlayer.ActorNumber);
                chatInput.text = "";
                chatInput.ActivateInputField();
                return;
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

    [PunRPC]
    void LogMessages(string msg, int sender = -1)
    {
        //sender == -1 : 공지라는 의미 
        if (sender != -1 && sender != PhotonNetwork.LocalPlayer.ActorNumber)
            msg = msg.Replace("#00FF7C", "#ffffff");

        messages.Add(msg);
        if (15 <  messages.Count)
            messages.RemoveAt(0);

        chatLogText.text = "";
        for (int i = 0; i < messages.Count; i++)
        {
            chatLogText.text += messages[i];
        }
    }

    void AutoChatCleaner()
    {
        if (chatCleanerTimer > 0.0f)
        {
            chatCleanerTimer -= Time.deltaTime;

            if (chatCleanerTimer <= 0.0f)
            {
                chatCleanerTimer = 0.0f;

                messages.RemoveAt(0);

                chatLogText.text = "";
                for (int i = 0; i < messages.Count; i++)
                {
                    chatLogText.text += messages[i];
                }

                chatCleanerTimer = resetTimer;
            }            
        }
    }
}
