using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InGameNetCoroutine : MonoBehaviour
{
    string BestScoreUrl = "";
    string MyGoldUrl = "";
    string InfoUpdateUrl = "";
    string UpdateFloorUrl = "";
    string UpdateNickUrl = "";

    Game_Mgr m_RefGameMgr = null;

    void Start()
    {
        BestScoreUrl   = "http://junskk.dothome.co.kr/NumberGame/UpdateBScore.php";
        UpdateNickUrl = "http://junskk.dothome.co.kr/NumberGame/UpdateNickname.php";


        //MyGoldUrl      = "http://xxxxxx.dothome.co.kr/xxxxxx/UpdateMyGold.php";
        //InfoUpdateUrl  = "http://xxxxxx.dothome.co.kr/xxxxxx/InfoUpdate.php";
        //UpdateFloorUrl = "http://xxxxxx.dothome.co.kr/xxxxxx/UpdateFloor.php";
    }

    public void GameStart(Game_Mgr a_RefGMgr)
    {
        m_RefGameMgr = a_RefGMgr;
    }

    public IEnumerator UpdateScoreCo()
    {
        if (GlobalValue.g_Unique_ID == "")  //���������� �α��� �Ǿ� ���� �ʴٸ�...
            yield break;        //�ڷ�ƾ �Լ��� ���� ������...

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_score", GlobalValue.g_BestScore);

        //Ÿ�Ӿƿ� ���� (�� ������ ����, ��: 3��)
        bool isTimeOut = false;
        float startTime = Time.unscaledTime;

        NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

        UnityWebRequest a_www = UnityWebRequest.Post(BestScoreUrl, form);
        a_www.SendWebRequest();    //������ �ö����� ����ϱ�...

        //������ ���ų� Ÿ�Ӿƿ��� �߻��� ������ ���
        while (!a_www.isDone && !isTimeOut)
        {
            if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
                isTimeOut = true;

            yield return null; //���� �����ӱ��� ���
        }//while(!a_www.isDone && !isTimeOut)

        //Ÿ�Ӿƿ� ó��
        if (isTimeOut == true)
        {
            a_www.Abort();  //��û�� �ߴ�
            NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
            yield break;
        }

        if (a_www.error == null) //������ ���� �ʾ��� �� ����
        {
            //Debug.Log("UpdateSuccess~~");
        }
        else
        {
            Debug.Log(a_www.error);
        }

        a_www.Dispose();
        NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
    }

    public IEnumerator NickChangeCo(string a_NickStr)
    {
        if (GlobalValue.g_Unique_ID == "")
            yield break;

        if (a_NickStr == "")
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_nick", a_NickStr, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(UpdateNickUrl, form);

        //Ÿ�Ӿƿ� ���� (�� ������ ����, ��: 3��)
        bool isTimeOut = false;
        float startTime = Time.unscaledTime;

        NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

        //��û ������
        a_www.SendWebRequest();

        //������ ���ų� Ÿ�Ӿƿ��� �߻��� ������ ���
        while (!a_www.isDone && !isTimeOut)
        {
            if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
                isTimeOut = true;

            yield return null; //���� �����ӱ��� ���
        }//while(!a_www.isDone && !isTimeOut)

        //Ÿ�Ӿƿ� ó��
        if (isTimeOut)
        {
            a_www.Abort();  //��û�� �ߴ�
            NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
            yield break;
        }

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(a_www.downloadHandler.data);

        bool a_isWait = false;
        string a_MsgStr = "";
        if (a_www.error == null)  //������ ���� �ʾ��� �� ����
        {
            if (sz.Contains("Update Nick Success.") == true)
            {
                GlobalValue.g_NickName = a_NickStr;
                //if (m_RefLobbyMgr != null)
                //{
                //    m_RefLobbyMgr.RefreshUserInfo();
                //    //������ �ٲ�����Ƿ� ��ŷ�� �ٽ� �޾� �´�.
                //    NetworkMgr.Inst.PushPacket(PacketType.GetRankingList);
                //    RestoreTimer = 10.0f;
                //    //������ �ٲ�����Ƿ� ��ŷ�� �ٽ� �޾� �´�.
                //}
            }
            else if (sz.Contains("Nickname does exist.") == true)
            {
                a_isWait = true;
                a_MsgStr = "�ߺ��� �г����� �����մϴ�.";
            }
            else
            {
                a_isWait = true;
                a_MsgStr = sz;
            }
        }//if(a_www.error == null)  //������ ���� �ʾ��� �� ����
        else
        {
            a_isWait = true;
            a_MsgStr = sz + " : " + a_www.error;
        }

        a_www.Dispose();
        NetworkMgr.Inst.m_NetWaitTimer = 0.0f;

        if (m_RefGameMgr != null && a_isWait)   //��� �޼����� �����ϸ� �ΰ��� �޼��� ����
        {
            m_RefGameMgr.MessageOnOff(a_MsgStr);
        }
    }

    //public IEnumerator UpdateGoldCo()
    //{
    //    if (GlobalValue.g_Unique_ID == "")
    //        yield break;

    //    WWWForm form = new WWWForm();
    //    form.AddField("Input_user", GlobalValue.g_Unique_ID,
    //                                System.Text.Encoding.UTF8);
    //    form.AddField("Input_gold", GlobalValue.g_UserGold);

    //    //Ÿ�Ӿƿ� ���� (�� ������ ����, ��: 3��)
    //    bool isTimeOut = false;
    //    float startTime = Time.unscaledTime;

    //    NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

    //    UnityWebRequest a_www = UnityWebRequest.Post(MyGoldUrl, form);

    //    a_www.SendWebRequest();    //������ �ö����� ����ϱ�...

    //    //������ ���ų� Ÿ�Ӿƿ��� �߻��� ������ ���
    //    while (!a_www.isDone && !isTimeOut)
    //    {
    //        if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
    //            isTimeOut = true;

    //        yield return null; //���� �����ӱ��� ���
    //    }//while(!a_www.isDone && !isTimeOut)

    //    //Ÿ�Ӿƿ� ó��
    //    if (isTimeOut == true)
    //    {
    //        a_www.Abort();  //��û�� �ߴ�
    //        NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
    //        yield break;
    //    }

    //    if (a_www.error == null) //������ ���� �ʾ��� �� ����
    //    {
    //        //Debug.Log("UpdateGoldSuccess");
    //    }
    //    else
    //    {
    //        Debug.Log(a_www.error);
    //    }

    //    a_www.Dispose();
    //    NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
    //}

    //public IEnumerator UpdateInfoCo()
    //{
    //    if (GlobalValue.g_Unique_ID == "")
    //        yield break;

    //    //--- Json �����...
    //    ItemList a_ItList = new ItemList();
    //    a_ItList.SkList = new int[GlobalValue.g_SkillCount.Length];
    //    for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
    //    {
    //        a_ItList.SkList[i] = GlobalValue.g_SkillCount[i];
    //    }
    //    // JSON ���ڿ��� ��ȯ ��) {"SkList":[1,1,0]} 
    //    string a_StrJson = JsonUtility.ToJson(a_ItList);
    //    //--- Json �����...

    //    WWWForm form = new WWWForm();
    //    form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
    //    form.AddField("Item_list", a_StrJson, System.Text.Encoding.UTF8);

    //    //Ÿ�Ӿƿ� ���� (�� ������ ����, ��: 3��)
    //    bool isTimeOut = false;
    //    float startTime = Time.unscaledTime;

    //    NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

    //    UnityWebRequest a_www = UnityWebRequest.Post(InfoUpdateUrl, form);
    //    a_www.SendWebRequest();    //������ �ö����� ����ϱ�...

    //    //������ ���ų� Ÿ�Ӿƿ��� �߻��� ������ ���
    //    while (!a_www.isDone && !isTimeOut)
    //    {
    //        if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
    //            isTimeOut = true;

    //        yield return null; //���� �����ӱ��� ���
    //    }//while(!a_www.isDone && !isTimeOut)

    //    //Ÿ�Ӿƿ� ó��
    //    if (isTimeOut == true)
    //    {
    //        a_www.Abort();  //��û�� �ߴ�
    //        NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
    //        yield break;
    //    }

    //    if (a_www.error == null) //������ ���� �ʾҴٸ�...
    //    {
    //        //Debug.Log("Update Success~");
    //    }
    //    else
    //    {
    //        Debug.Log(a_www.error);
    //    }

    //    a_www.Dispose();
    //    NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
    //}

    //public IEnumerator UpdateFloorCo()
    //{
    //    if (GlobalValue.g_Unique_ID == "")
    //        yield break;

    //    //--- JSON �����...
    //    FloorInfo a_FInfo = new FloorInfo();
    //    a_FInfo.CurFloor = GlobalValue.g_CurFloorNum;
    //    a_FInfo.BestFloor = GlobalValue.g_BestFloor;
    //    string a_StrJson = JsonUtility.ToJson(a_FInfo);
    //    //--- JSON �����...

    //    WWWForm form = new WWWForm();
    //    form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
    //    form.AddField("Input_floor", a_StrJson, System.Text.Encoding.UTF8);

    //    //Ÿ�Ӿƿ� ���� (�� ������ ����, ��: 3��)
    //    bool isTimeOut = false;
    //    float startTime = Time.unscaledTime;

    //    NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

    //    UnityWebRequest a_www = UnityWebRequest.Post(UpdateFloorUrl, form);
    //    a_www.SendWebRequest(); //������ �ö����� ����ϱ�...

    //    while (!a_www.isDone && !isTimeOut)
    //    {
    //        if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
    //            isTimeOut = true;

    //        yield return null; //���� �����ӱ��� ���
    //    }//while(!a_www.isDone && !isTimeOut)

    //    //Ÿ�Ӿƿ� ó��
    //    if (isTimeOut == true)
    //    {
    //        a_www.Abort();  //��û�� �ߴ�
    //        NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
    //        yield break;
    //    }

    //    if (a_www.error == null)  //������ ���� �ʾ��� �� ����
    //    {
    //        //Debug.Log("UpdateSuccess~");
    //    }
    //    else
    //    {
    //        Debug.Log(a_www.error);
    //    }

    //    a_www.Dispose();

    //    NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
    //}

}//public class InGameNetCoroutine : MonoBehaviour
