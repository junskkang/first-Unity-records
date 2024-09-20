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

        string a_TeamKind = mgr.ReceiveSelTeam(pv.Owner);   //�ش� ������ �� ���θ� �޾ƿ� �� �ִ� �Լ� blue or red

        //����� �� ��Ʈ���� ���� ��ũ�Ѻ並 �з����ֱ�
        if (a_TeamKind == "blue")
            userId.color = Color.blue;
        else if (a_TeamKind == "red")
            userId.color = Color.red;
    }
}
