using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]   //�Ľ��� ���� �ڵ�
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


    [HideInInspector] public string m_NickStrBuff;
    [HideInInspector] public ConfigBox m_RefCfgBox;
    string UpdateNickUrl = "";
   

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
        UpdateNickUrl = "http://junskk.dothome.co.kr/practice/UpdateNickname.php";

        RestoreTimer = 3.0f;    //��ŷ Ÿ�̸� ����

        GetRankingList();   //��ŷ �ҷ����� �Լ�
    }

    // Update is called once per frame
    void Update()
    {
#if AutoRestore //�ڵ���ŷ ����
        RestoreTimer -= Time.deltaTime;
        if (RestoreTimer <= 0.0f)
        {
            GetRankingList();
            RestoreTimer = 7.0f;    //�ֱ�
        }
#else   //������ŷ ����
        if (0.0f < RestoreTimer)
        {
            RestoreTimer -= Time.deltaTime;
        }
#endif

        if (!isNetworkLock) //���� ó�� ���� ��Ŷ�� ���� ��
        {
            if (m_PacketBuff.Count > 0)
                Req_Network();
        }
    }

    void Req_Network()
    {
        if (m_PacketBuff[0] == PacketType.NickUpdate)
            StartCoroutine(NickChangeCo(m_NickStrBuff));

        m_PacketBuff.RemoveAt(0);
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
    {//Record Ranking List and My Rank
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

    IEnumerator NickChangeCo(string a_Nick)
    {
        if (GlobalValue.g_Unique_ID == "") yield break;

        if (a_Nick == "") yield break;
                

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_nick", GlobalValue.g_NickName, System.Text.Encoding.UTF8);

        UnityWebRequest www = UnityWebRequest.Post(UpdateNickUrl, form);

        //Ÿ�Ӿƿ� ����(�� ������ ����)
        float timeout = 3.0f;
        bool isTimeout = false;
        float startTime = Time.unscaledTime;

        isNetworkLock = true;

        //��û ������
        www.SendWebRequest();

        while (!www.isDone && !isTimeout)   //���� ������ ���� �ʾҰ� Ÿ�Ӿƿ����°� �ƴ϶��
        {
            if (Time.unscaledTime - startTime > timeout)    //�帥�ð� > timeout(3��)
            {
                isTimeout = true;
            }

            yield return null;  //���� �����ӱ��� ���
        }
        
        //Ÿ�Ӿƿ� ó��
        if (isTimeout)
        {
            www.Abort();    //��û�� �ߴ�
            isNetworkLock = false;
            if (m_RefCfgBox != null)
                m_RefCfgBox.ResultOkBtn(true, "Request timed out");
            yield break;
        }

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(www.downloadHandler.data);

        bool isWait = false;
        string msg = "";

        if (www.error == null)
        {
            if (sz.Contains("Update Nick Success."))
            {
                GlobalValue.g_NickName = a_Nick;
                LobbyMgr.Inst.RefreshUserInfo();

                //������ �ٲ�����Ƿ� ��ŷ�� �ٽ� �޾ƿ´�.
                GetRankingList();
                RestoreTimer = 7.0f;
            }
            else if (sz.Contains("Nickname does exist."))
            {
                isWait = true;
                msg = "�ߺ��� �г����� �����մϴ�.";
            }
            else
            {
                isWait = true;
                msg = sz;
            }
        }
        else
        {
            isWait = true;
            msg = sz + " : " + www.error;
        }

        www.Dispose();
        isNetworkLock = false;

        if (m_RefCfgBox != null)
            m_RefCfgBox.ResultOkBtn(isWait, msg);

    }

    public void PushPacket(PacketType a_PType)
    {
        bool a_isExist = false;
        for (int i = 0; i < m_PacketBuff.Count; i++)
        {
            if (m_PacketBuff[i] == a_PType) //���� ó�� ���� ���� ��Ŷ�� �����ϸ�
                a_isExist = true;
            //�� �߰����� �ʰ� ���� ������ ��Ŷ���� ������Ʈ �Ѵ�.
        }

        if (a_isExist == false)
            m_PacketBuff.Add(a_PType);
        //��� ���� �� Ÿ���� ��Ŷ�� ������ ���� �߰��Ѵ�.
    }
}
