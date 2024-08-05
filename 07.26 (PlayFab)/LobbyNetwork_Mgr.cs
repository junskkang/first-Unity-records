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
        GetRankingList,     //��ŷ ����Ʈ ��������
        GetMyRanking,        //�� ��� ��������
        ClearSave,          //������ ����� ���� �ʱ�ȭ�ϱ� <�÷��̾� ������(Ÿ��Ʋ)>
        ClearScore,         //������ ����� ��� �ʱ�ȭ�ϱ� <���>
        ClearExp            //������ ����� ����ġ, ���� �ʱ�ȭ�ϱ�
    }

    //������ ������ ��Ŷ ó���� ť ���� ����
    //bool isNetworkLock = false;
    float netWaitTime = 0.0f;
    List<PacketType> packetBuff = new List<PacketType>();
    //�ܼ��� � ��Ŷ�� ���� �ʿ䰡 �ִٶ�� ������ �ǹ� 


    //�̱��� ����
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

        if (netWaitTime <= 0.0f)     //���� ��Ŷó�� ���� ���°� �ƴϸ�
        {
            if (0 < packetBuff.Count)   //��� ��Ŷ�� �����Ѵٸ�
            {
                Req_Network();          //��Ŷ��û �Լ� ȣ��
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
        if (GlobalValue.g_Unique_ID == "") return;  //�������� �α��� ���¿����� ����ǵ���

        var request = new GetLeaderboardRequest()
        {
            StartPosition = 0,    //0�� �ε������� �������, �� 1�����
            StatisticName = "BestScore",
            //������ �������� ����ǥ ���� �� BestScore ����
            MaxResultsCount = 10,  //10�����
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,     //�г��� ��û
                ShowAvatarUrl = true,       //�ƹ�Ÿ URL ��û               

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

                    //��ŷ �޾ƿ��� ����
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

                        //��� �ȿ� ���� �ִٸ� �� ǥ��
                        if (curBoard.PlayFabId == GlobalValue.g_Unique_ID)
                            strBuff += "<color=#00ff00>";

                        strBuff += (i + 1).ToString() + "�� : "
                                + curBoard.DisplayName + " : "
                                + curBoard.StatValue + "�� : " 
                                + (userLv+1) + "���� \n";

                        //��� �ȿ� ���� �ִٸ� �� ǥ�� ����
                        if (curBoard.PlayFabId == GlobalValue.g_Unique_ID)
                            strBuff += "</color>";
                    }

                    if (strBuff != "")
                        Lobby_Mgr.inst.rankingText.text = strBuff;

                    //�������� ����� �ҷ��� �� �� �� ��� ������.
                    GetMyRanking();

                },
                (error) =>
                {
                    //��ŷ �޾ƿ��� ����
                    Debug.Log("��������ҷ����� ����");
                    //isNetworkLock = false;
                }
                );
    }
    int LVMyJsonParse(string json)
    {
        string a_AvatarURL = json;
        int result = 0;
        //Json�Ľ�
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
        //GetLeaderboardAroundPlayer() : Ư�� PlayFabID�� �������� �ֺ����� ����Ʈ�� �ҷ����� �Լ�

        var request = new GetLeaderboardAroundPlayerRequest()
        {
            //PlayFabId = GlobalValue.g_Unique_ID,  //����Ʈ������ �α��� �� ���̵�� ������ ��
            StatisticName = "BestScore",
            MaxResultsCount = 1,    //�Ѹ��� ������ �����´ٴ� ��
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
                        Lobby_Mgr.inst.myRank = curBoard.Position + 1;     //�� ��� ��������
                        GlobalValue.g_BestScore = curBoard.StatValue; //�� �ְ� ���� ����

                        Lobby_Mgr.inst.CfgResponse();      //��� UI ����
                    }

                    //isNetworkLock = false;
                },
                (error) =>
                {
                    Debug.Log("�� ��� �ҷ����� ����");
                    //isNetworkLock = false;
                }
                );
    }

    private void UpdateClearSaveCo()    //<�÷��̾� ������(Ÿ��Ʋ)> �ʱ�ȭ
    {
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdateUserDataRequest();
        //KeysToRemove //Ư��Ű ���� ������û�ϴ� ����Ʈ
        request.KeysToRemove = new List<string>();
        //����Ʈ�� ��������־ �����ϰ���� Ű �̸���  Add�ϸ� �ȴ�.
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
                    GlobalValue.g_CurSkillCount[i] = 1; //ü�轺ų �ٽ� ����
                }
            },
            (error) =>
            {

            }
            );
    }

    private void UpdateClearScoreCo()   //<����ǥ> �ʱ�ȭ
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

    private void UpdateClearExpCo()     //����ġ(AvatarURL) �ʱ�ȭ
    {
        if (GlobalValue.g_Unique_ID == "") return;

        //AvatarUrl�� �̿��ؼ� �����ϴ� ������
        //����ǥ ����Ʈ�� ���� �� ���� �޾ƿ� �� �ִٴ� ��

        //AvatarUrl�� �̿��ؼ� ������ ������ �����ϴ� ���
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
            //���� ó������ ���� ��Ŷ�� �����Ѵٸ�
            if (packetBuff[i] == packetType)
                isExist = true;

            //�� �߰����� �ʵ��� ���� ������ ��Ŷ�� ������Ʈ 
        }

        //������� Ÿ���� ��Ŷ�� ������ ���� �߰���.
        if (!isExist)
            packetBuff.Add(packetType);
    }
}
