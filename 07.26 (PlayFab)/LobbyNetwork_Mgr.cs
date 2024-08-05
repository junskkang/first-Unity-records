using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using SimpleJSON;
using System;

public class LobbyNetwork_Mgr : MonoBehaviour
{
    public enum PacketType
    {
        GetRankingList,     //랭킹 리스트 가져오기
        GetMyRanking,        //내 등수 가져오기
        ClearSave,          //서버에 저장된 내용 초기화하기 <플레이어 데이터(타이틀)>
        ClearScore,         //서버에 저장된 등수 초기화하기 <통계>
        ClearExp            //서버에 저장된 경험치, 레벨 초기화하기
    }

    //서버에 전송할 패킷 처리용 큐 관련 변수
    //bool isNetworkLock = false;
    float netWaitTime = 0.0f;
    List<PacketType> packetBuff = new List<PacketType>();
    //단순히 어떤 패킷을 보낼 필요가 있다라는 버퍼의 의미 


    //싱글턴 패턴
    public static LobbyNetwork_Mgr inst;

    void Awake()
    {
        inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        netWaitTime -= Time.unscaledDeltaTime;

        if (netWaitTime <= 0.0f)     //지금 패킷처리 중인 상태가 아니면
        {
            if (0 < packetBuff.Count)   //대기 패킷이 존재한다면
            {
                Req_Network();          //패킷요청 함수 호출
            }
        }
    }

    void Req_Network()
    {
        if (packetBuff[0] == PacketType.GetRankingList)
            GetRankingList();
        else if (packetBuff[0] == PacketType.ClearSave)
            UpdateClearSaveCo();
        else if (packetBuff[0] == PacketType.ClearScore)
            UpdateClearScoreCo();
        else if (packetBuff[0] == PacketType.ClearExp)
            UpdateClearExpCo();

        packetBuff.RemoveAt(0);
    }


    void GetRankingList()
    {
        if (GlobalValue.g_Unique_ID == "") return;  //정상적인 로그인 상태에서만 진행되도록

        var request = new GetLeaderboardRequest()
        {
            StartPosition = 0,    //0번 인덱스부터 순서대로, 즉 1등부터
            StatisticName = "BestScore",
            //관리자 페이지의 순위표 변수 중 BestScore 기준
            MaxResultsCount = 10,  //10명까지
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,     //닉네임 요청
                ShowAvatarUrl = true,       //아바타 URL 요청               

            }
        };

        netWaitTime = 0.5f;

        PlayFabClientAPI.GetLeaderboard(
                request,
                (result) =>
                {
                    if (Lobby_Mgr.inst == null)
                    {
                        //isNetworkLock = false;
                        return;
                    }

                    //랭킹 받아오기 성공
                    if (Lobby_Mgr.inst.rankingText == null)
                    {
                        //isNetworkLock = false;
                        return;
                    }

                    string strBuff = "";

                    for (int i = 0; i < result.Leaderboard.Count; i++)
                    {
                        var curBoard = result.Leaderboard[i];
                        int userLv = LVMyJsonParse(curBoard.Profile.AvatarUrl);

                        //등수 안에 내가 있다면 색 표시
                        if (curBoard.PlayFabId == GlobalValue.g_Unique_ID)
                            strBuff += "<color=#00ff00>";

                        strBuff += (i + 1).ToString() + "등 : "
                                + curBoard.DisplayName + " : "
                                + curBoard.StatValue + "점 : " 
                                + (userLv+1) + "레벨 \n";

                        //등수 안에 내가 있다면 색 표시 마감
                        if (curBoard.PlayFabId == GlobalValue.g_Unique_ID)
                            strBuff += "</color>";
                    }

                    if (strBuff != "")
                        Lobby_Mgr.inst.rankingText.text = strBuff;

                    //리더보드 등수를 불러온 직 후 내 등수 가져옴.
                    GetMyRanking();

                },
                (error) =>
                {
                    //랭킹 받아오기 실패
                    Debug.Log("리더보드불러오기 실패");
                    //isNetworkLock = false;
                }
                );
    }
    int LVMyJsonParse(string json)
    {
        string a_AvatarURL = json;
        int result = 0;
        //Json파싱
        if (string.IsNullOrEmpty(a_AvatarURL) == false &&
            a_AvatarURL.Contains("{\"") == true)
        {
            JSONNode parseJson = JSON.Parse(a_AvatarURL);
            
            if (parseJson["UserLevel"] != null)
            {
                result = parseJson["UserLevel"].AsInt;

                return result;
            }
        }
        return result;
    }
    void GetMyRanking()
    {
        //GetLeaderboardAroundPlayer() : 특정 PlayFabID를 기준으로 주변으로 리스트를 불러오는 함수

        var request = new GetLeaderboardAroundPlayerRequest()
        {
            //PlayFabId = GlobalValue.g_Unique_ID,  //디폴트값으로 로그인 한 아이디로 설정이 됨
            StatisticName = "BestScore",
            MaxResultsCount = 1,    //한명의 정보만 가져온다는 뜻
            //ProfileConstraints = new PlayerProfileViewConstraints()
            //{
            //    ShowDisplayName = true,
            //}
        };

        //netWaitTime = 0.5f; 

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
                request,
                (result) =>
                {
                    if (Lobby_Mgr.inst == null)
                    {
                        //isNetworkLock = false;
                        return;
                    }

                    if (0 < result.Leaderboard.Count)
                    {
                        var curBoard = result.Leaderboard[0];
                        Lobby_Mgr.inst.myRank = curBoard.Position + 1;     //내 등수 가져오기
                        GlobalValue.g_BestScore = curBoard.StatValue; //내 최고 점수 갱신

                        Lobby_Mgr.inst.CfgResponse();      //상단 UI 갱신
                    }

                    //isNetworkLock = false;
                },
                (error) =>
                {
                    Debug.Log("내 등수 불러오기 실패");
                    //isNetworkLock = false;
                }
                );
    }

    private void UpdateClearSaveCo()    //<플레이어 데이터(타이틀)> 초기화
    {
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdateUserDataRequest();
        //KeysToRemove //특정키 값을 삭제요청하는 리스트
        request.KeysToRemove = new List<string>();
        //리스트로 만들어져있어서 삭제하고싶은 키 이름을  Add하면 된다.
        //for (int i = 0; GlobalValue.g_CurSkillCount.Count > i; i++)
        //{
        //    request.KeysToRemove.Add($"Skill_Item_{i}");
        //}
        request.KeysToRemove.Add("UserGold");

        netWaitTime = 0.5f;

        PlayFabClientAPI.UpdateUserData(request,
            (result) =>
            {
                for (int i = 0; i < GlobalValue.g_CurSkillCount.Count; i++)
                {
                    GlobalValue.g_CurSkillCount[i] = 1; //체험스킬 다시 제공
                }
            },
            (error) =>
            {

            }
            );
    }

    private void UpdateClearScoreCo()   //<순위표> 초기화
    {
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate {StatisticName = "BestScore", Value = 0 }
            }
        };

        netWaitTime = 0.5f;

        PlayFabClientAPI.UpdatePlayerStatistics(request,
                       (result) =>
                       {

                       },
                       (error) =>
                       {

                       }
                       );
    }

    private void UpdateClearExpCo()     //경험치(AvatarURL) 초기화
    {
        if (GlobalValue.g_Unique_ID == "") return;

        //AvatarUrl를 이용해서 저장하는 장점은
        //순위표 리스트를 받을 때 같이 받아올 수 있다는 점

        //AvatarUrl을 이용해서 유저의 레벨을 저장하는 편법
        var request = new UpdateAvatarUrlRequest()
        {
            ImageUrl = ""
        };

        netWaitTime = 0.5f;

        PlayFabClientAPI.UpdateAvatarUrl(request,
                       (result) =>
                       {

                       },
                       (error) =>
                       {

                       }
                       );
    }

    public void PushPacket(PacketType packetType)        
    {
        bool isExist = false;
        for (int i = 0; i < packetBuff.Count; i++)
        {
            //아직 처리되지 않은 패킷이 존재한다면
            if (packetBuff[i] == packetType)
                isExist = true;

            //또 추가하지 않도록 기존 버퍼의 패킷에 업데이트 
        }

        //대기중인 타입의 패킷이 없으면 새로 추가함.
        if (!isExist)
            packetBuff.Add(packetType);
    }
}
