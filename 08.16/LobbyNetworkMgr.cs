using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]   //파싱을 위한 코드
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
    //서버에 전송할 패킷 처리용 큐 관련 변수
    bool isNetworkLock = false;
    List<PacketType> m_PacketBuff = new List<PacketType>();
    string GetRankListUrl = "";
    RkRootInfo m_RkList = new RkRootInfo();

    [HideInInspector] public float RestoreTimer = 0.0f;


    [HideInInspector] public string m_NickStrBuff;
    [HideInInspector] public ConfigBox m_RefCfgBox;
    string UpdateNickUrl = "";
   

    //싱글턴 패턴을 위한 인스턴스 변수 선언
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

        RestoreTimer = 3.0f;    //랭킹 타이머 갱신

        GetRankingList();   //랭킹 불러오는 함수
    }

    // Update is called once per frame
    void Update()
    {
#if AutoRestore //자동랭킹 갱신
        RestoreTimer -= Time.deltaTime;
        if (RestoreTimer <= 0.0f)
        {
            GetRankingList();
            RestoreTimer = 7.0f;    //주기
        }
#else   //수동랭킹 갱신
        if (0.0f < RestoreTimer)
        {
            RestoreTimer -= Time.deltaTime;
        }
#endif

        if (!isNetworkLock) //현재 처리 중인 패킷이 없을 때
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
        if (GlobalValue.g_Unique_ID == "") yield break; //정상 로그인 체크

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);

        UnityWebRequest www = UnityWebRequest.Post(GetRankListUrl, form);

        yield return www.SendWebRequest();  //응답이 올 때까지 대기하기

        if (www.error == null)   //오류가 없다면
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string a_ReStr = enc.GetString(www.downloadHandler.data);

            if (a_ReStr.Contains("Get_Rank_List_Success~"))
            {
                //제이슨 형식이 아닌 문자열이 붙어있을 경우에
                //파싱할 때 오류가 난다. 고로 제거해주자.
                a_ReStr = a_ReStr.Replace("\nGet_Rank_List_Success~", "");

                //Debug.Log(a_ReStr);
                RecRankList_MyRank(a_ReStr);    //점수를 표시하는 함수를 호출
            }
            else
            {
                LobbyMgr.Inst.MessageOnOff("랭킹 불러오기 실패 잠시 후 다시 시도해 주세요.");
            }
        }
        else
        {
            LobbyMgr.Inst.MessageOnOff("랭킹 불러오기 실패 잠시 후 다시 시도해 주세요.");
        }

        www.Dispose();
        
    }

    void RecRankList_MyRank(string strJson)
    {//Record Ranking List and My Rank
        if (!strJson.Contains("RkList")) return;

        //Json 파일 파싱
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

        //타임아웃 설정(초 단위로 설정)
        float timeout = 3.0f;
        bool isTimeout = false;
        float startTime = Time.unscaledTime;

        isNetworkLock = true;

        //요청 보내기
        www.SendWebRequest();

        while (!www.isDone && !isTimeout)   //아직 응답이 오지 않았고 타임아웃상태가 아니라면
        {
            if (Time.unscaledTime - startTime > timeout)    //흐른시간 > timeout(3초)
            {
                isTimeout = true;
            }

            yield return null;  //다음 프레임까지 대기
        }
        
        //타임아웃 처리
        if (isTimeout)
        {
            www.Abort();    //요청을 중단
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

                //별명이 바뀌었으므로 랭킹도 다시 받아온다.
                GetRankingList();
                RestoreTimer = 7.0f;
            }
            else if (sz.Contains("Nickname does exist."))
            {
                isWait = true;
                msg = "중복된 닉네임이 존재합니다.";
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
            if (m_PacketBuff[i] == a_PType) //아직 처리 되지 않은 패킷이 존재하면
                a_isExist = true;
            //또 추가하지 않고 기존 버퍼의 패킷으로 업데이트 한다.
        }

        if (a_isExist == false)
            m_PacketBuff.Add(a_PType);
        //대기 중인 이 타입의 패킷이 없으면 새로 추가한다.
    }
}
