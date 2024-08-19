using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class UserInfo
{
    public string user_id;
    public string nick_name;
    public int best_score;
}
[System.Serializable]
public class RkRootInfo
{
    public UserInfo[] RkList;
    public int my_rank;
}

public class LobbyNetworkMgr : MonoBehaviour
{
    //������ ������ ��Ŷ ó���� ť ���� ����
    bool isNetworkLock = false;
    List<PacketType> m_PacketBuff = new List<PacketType>();
    string GetRankListUrl = "";
    RkRootInfo m_RkList = new RkRootInfo();

    [HideInInspector] public float RestoreTimer = 0.0f;

    //�̱��� ������ ���� �ν��Ͻ� ���� ����
    public static LobbyNetworkMgr Inst = null;

    void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetRankListUrl = "http://junskk.dothome.co.kr/practice/Get_ID_Rank.php";

        RestoreTimer = 3.0f;    //��ŷ Ÿ�̸� ����

        GetRankingList();   //��ŷ �ҷ����� �Լ�
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < RestoreTimer)
        {
            RestoreTimer -= Time.deltaTime;
        }
    }

    public void GetRankingList()
    {
        StartCoroutine(GetRankListCo());
    }

    IEnumerator GetRankListCo()
    {
        if (GlobalValue.g_Unique_ID == "") yield break; //���� �α��� üũ

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);

        UnityWebRequest www = UnityWebRequest.Post(GetRankListUrl, form);

        yield return www.SendWebRequest();  //������ �� ������ ����ϱ�

        if (www.error == null)   //������ ���ٸ�
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string a_ReStr = enc.GetString(www.downloadHandler.data);

            if (a_ReStr.Contains("Get_Rank_List_Success~"))
            {
                //���̽� ������ �ƴ� ���ڿ��� �پ����� ��쿡
                //�Ľ��� �� ������ ����. ��� ����������.
                a_ReStr = a_ReStr.Replace("\nGet_Rank_List_Success~", "");

                //Debug.Log(a_ReStr);
                RecRankList_MyRank(a_ReStr);    //������ ǥ���ϴ� �Լ��� ȣ��
            }
            else
            {
                LobbyMgr.Inst.MessageOnOff("��ŷ �ҷ����� ���� ��� �� �ٽ� �õ��� �ּ���.");
            }
        }
        else
        {
            LobbyMgr.Inst.MessageOnOff("��ŷ �ҷ����� ���� ��� �� �ٽ� �õ��� �ּ���.");
        }

        www.Dispose();
        
    }

    void RecRankList_MyRank(string strJson)
    {
        if (!strJson.Contains("RkList")) return;

        //Json ���� �Ľ�
        m_RkList = JsonUtility.FromJson<RkRootInfo>(strJson);

        if(m_RkList == null) return;

        //for (int i = 0; i < m_RkList.RkList.Length; i++) 
        //{
        //    Debug.Log(i + " : User_ID" + m_RkList.RkList[i].user_id);
        //    Debug.Log("User_Name" + m_RkList.RkList[i].nick_name);
        //    Debug.Log("Best_Score" + m_RkList.RkList[i].best_score);
        //}

        //Debug.Log("MyRank : " + m_RkList.my_rank);

        LobbyMgr.Inst.RefreshRankUI(m_RkList);
    }
}
