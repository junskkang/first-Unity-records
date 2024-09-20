using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUserID : MonoBehaviour
{
    public Text userId;
    PhotonView pv = null;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        userId.text = pv.Owner.NickName;
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void ChangeTeamNameColor(GameManager mgr)
    {
        if (pv == null) return;

        if (mgr == null) return;

        string a_TeamKind = mgr.ReceiveSelTeam(pv.Owner);   //해당 유저의 팀 여부를 받아올 수 있는 함수 blue or red

        //저장된 팀 스트링에 따라 스크롤뷰를 분류해주기
        if (a_TeamKind == "blue")
            userId.color = Color.blue;
        else if (a_TeamKind == "red")
            userId.color = Color.red;
    }
}
