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
    //서버에 전송할 패킷 처리용 큐 관련 변수
    bool isNetworkLock = false;
    List<PacketType> m_PacketBuff = new List<PacketType>();
    string GetRankListUrl = "";
    RkRootInfo m_RkList = new RkRootInfo();

    [HideInInspector] public float RestoreTimer = 0.0f;

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

        RestoreTimer = 3.0f;    //랭킹 타이머 갱신

        GetRankingList();   //랭킹 불러오는 함수
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
    {
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
}
