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
        if (GlobalValue.g_Unique_ID == "")  //정상적으로 로그인 되어 있지 않다면...
            yield break;        //코루틴 함수를 빠져 나가기...

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_score", GlobalValue.g_BestScore);

        //타임아웃 설정 (초 단위로 설정, 예: 3초)
        bool isTimeOut = false;
        float startTime = Time.unscaledTime;

        NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

        UnityWebRequest a_www = UnityWebRequest.Post(BestScoreUrl, form);
        a_www.SendWebRequest();    //응답이 올때까지 대기하기...

        //응답이 오거나 타임아웃이 발생할 때까지 대기
        while (!a_www.isDone && !isTimeOut)
        {
            if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
                isTimeOut = true;

            yield return null; //다음 프레임까지 대기
        }//while(!a_www.isDone && !isTimeOut)

        //타임아웃 처리
        if (isTimeOut == true)
        {
            a_www.Abort();  //요청을 중단
            NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
            yield break;
        }

        if (a_www.error == null) //에러가 나지 않았을 때 동작
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

        //타임아웃 설정 (초 단위로 설정, 예: 3초)
        bool isTimeOut = false;
        float startTime = Time.unscaledTime;

        NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

        //요청 보내기
        a_www.SendWebRequest();

        //응답이 오거나 타임아웃이 발생할 때까지 대기
        while (!a_www.isDone && !isTimeOut)
        {
            if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
                isTimeOut = true;

            yield return null; //다음 프레임까지 대기
        }//while(!a_www.isDone && !isTimeOut)

        //타임아웃 처리
        if (isTimeOut)
        {
            a_www.Abort();  //요청을 중단
            NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
            yield break;
        }

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(a_www.downloadHandler.data);

        bool a_isWait = false;
        string a_MsgStr = "";
        if (a_www.error == null)  //에러가 나지 않았을 때 동작
        {
            if (sz.Contains("Update Nick Success.") == true)
            {
                GlobalValue.g_NickName = a_NickStr;
                //if (m_RefLobbyMgr != null)
                //{
                //    m_RefLobbyMgr.RefreshUserInfo();
                //    //별명이 바뀌었으므로 랭킹도 다시 받아 온다.
                //    NetworkMgr.Inst.PushPacket(PacketType.GetRankingList);
                //    RestoreTimer = 10.0f;
                //    //별명이 바뀌었으므로 랭킹도 다시 받아 온다.
                //}
            }
            else if (sz.Contains("Nickname does exist.") == true)
            {
                a_isWait = true;
                a_MsgStr = "중복된 닉네임이 존재합니다.";
            }
            else
            {
                a_isWait = true;
                a_MsgStr = sz;
            }
        }//if(a_www.error == null)  //에러가 나지 않았을 때 동작
        else
        {
            a_isWait = true;
            a_MsgStr = sz + " : " + a_www.error;
        }

        a_www.Dispose();
        NetworkMgr.Inst.m_NetWaitTimer = 0.0f;

        if (m_RefGameMgr != null && a_isWait)   //띄울 메세지가 존재하면 인게임 메세지 띄우기
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

    //    //타임아웃 설정 (초 단위로 설정, 예: 3초)
    //    bool isTimeOut = false;
    //    float startTime = Time.unscaledTime;

    //    NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

    //    UnityWebRequest a_www = UnityWebRequest.Post(MyGoldUrl, form);

    //    a_www.SendWebRequest();    //응답이 올때까지 대기하기...

    //    //응답이 오거나 타임아웃이 발생할 때까지 대기
    //    while (!a_www.isDone && !isTimeOut)
    //    {
    //        if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
    //            isTimeOut = true;

    //        yield return null; //다음 프레임까지 대기
    //    }//while(!a_www.isDone && !isTimeOut)

    //    //타임아웃 처리
    //    if (isTimeOut == true)
    //    {
    //        a_www.Abort();  //요청을 중단
    //        NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
    //        yield break;
    //    }

    //    if (a_www.error == null) //에러가 나지 않았을 때 동작
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

    //    //--- Json 만들기...
    //    ItemList a_ItList = new ItemList();
    //    a_ItList.SkList = new int[GlobalValue.g_SkillCount.Length];
    //    for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
    //    {
    //        a_ItList.SkList[i] = GlobalValue.g_SkillCount[i];
    //    }
    //    // JSON 문자열로 변환 예) {"SkList":[1,1,0]} 
    //    string a_StrJson = JsonUtility.ToJson(a_ItList);
    //    //--- Json 만들기...

    //    WWWForm form = new WWWForm();
    //    form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
    //    form.AddField("Item_list", a_StrJson, System.Text.Encoding.UTF8);

    //    //타임아웃 설정 (초 단위로 설정, 예: 3초)
    //    bool isTimeOut = false;
    //    float startTime = Time.unscaledTime;

    //    NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

    //    UnityWebRequest a_www = UnityWebRequest.Post(InfoUpdateUrl, form);
    //    a_www.SendWebRequest();    //응답이 올때까지 대기하기...

    //    //응답이 오거나 타임아웃이 발생할 때까지 대기
    //    while (!a_www.isDone && !isTimeOut)
    //    {
    //        if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
    //            isTimeOut = true;

    //        yield return null; //다음 프레임까지 대기
    //    }//while(!a_www.isDone && !isTimeOut)

    //    //타임아웃 처리
    //    if (isTimeOut == true)
    //    {
    //        a_www.Abort();  //요청을 중단
    //        NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
    //        yield break;
    //    }

    //    if (a_www.error == null) //에러가 나지 않았다면...
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

    //    //--- JSON 만들기...
    //    FloorInfo a_FInfo = new FloorInfo();
    //    a_FInfo.CurFloor = GlobalValue.g_CurFloorNum;
    //    a_FInfo.BestFloor = GlobalValue.g_BestFloor;
    //    string a_StrJson = JsonUtility.ToJson(a_FInfo);
    //    //--- JSON 만들기...

    //    WWWForm form = new WWWForm();
    //    form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
    //    form.AddField("Input_floor", a_StrJson, System.Text.Encoding.UTF8);

    //    //타임아웃 설정 (초 단위로 설정, 예: 3초)
    //    bool isTimeOut = false;
    //    float startTime = Time.unscaledTime;

    //    NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

    //    UnityWebRequest a_www = UnityWebRequest.Post(UpdateFloorUrl, form);
    //    a_www.SendWebRequest(); //응답이 올때까지 대기하기...

    //    while (!a_www.isDone && !isTimeOut)
    //    {
    //        if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
    //            isTimeOut = true;

    //        yield return null; //다음 프레임까지 대기
    //    }//while(!a_www.isDone && !isTimeOut)

    //    //타임아웃 처리
    //    if (isTimeOut == true)
    //    {
    //        a_www.Abort();  //요청을 중단
    //        NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
    //        yield break;
    //    }

    //    if (a_www.error == null)  //에러가 나지 않았을 때 동작
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
