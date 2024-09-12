using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomIcon : MonoBehaviour
{
    [HideInInspector] public string roomName = "";
    [HideInInspector] public int connectPlayer = 0;
    [HideInInspector] public int maxPlayers = 0;

    public Text textRoomName;
    public Text textConnectInfo;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DispRoomData(bool isOpen)
    {
        if (isOpen) //룸에 들어갈 수 있는지 여부에 따라 글자색 변경
        {
            textRoomName.color = new Color32(0, 0, 0, 255);
            textConnectInfo.color = new Color32(0, 0, 0, 255);
        }
        else
        {
            textRoomName.color = new Color32(0, 0, 255, 255);
            textConnectInfo.color = new Color32(0, 0, 255, 255);
        }
        textRoomName.text = roomName;
        textConnectInfo.text = $"({connectPlayer.ToString()} / {maxPlayers.ToString()}";
    }
}
