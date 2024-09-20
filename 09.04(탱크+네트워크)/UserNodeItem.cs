using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserNodeItem : MonoBehaviour
{

    [HideInInspector] public int m_UniqID = -1;         //유저의 고유번호
    [HideInInspector] public string m_TeamKind = "";    //유저의 팀
    [HideInInspector] public bool m_BeReady = false;    //ready상태

    //Tank 이름을 표시할 Text UI 
    public Text textTankName;
    public Text textReadyState;

    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void DisplayPlayerData(string a_TankName, bool isMine = false)
    {
        if (isMine)
        {
            textTankName.color = Color.magenta;
            textTankName.text = a_TankName;
        }
        else
            textTankName.text = a_TankName;

        if (m_BeReady)
            textReadyState.text = "Ready!";
        else
            textReadyState.text = "";
    }
}
