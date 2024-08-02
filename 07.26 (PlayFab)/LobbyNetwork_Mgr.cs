using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using SimpleJSON;

public class LobbyNetwork_Mgr : MonoBehaviour
{
    public enum PacketType
    {
        GetRankingList,     //��ŷ ����Ʈ ��������
        GetMyRanking        //�� ��� ��������
    }

    //������ ������ ��Ŷ ó���� ť ���� ����
    bool isNetworkLock = false;
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
        if (!isNetworkLock)     //���� ��Ŷó�� ���� ���°� �ƴϸ�
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

        isNetworkLock = true;

        PlayFabClientAPI.GetLeaderboard(
                request,
                (result) =>
                {
                    if (Lobby_Mgr.inst == null)
                    {
                        isNetworkLock = false;
                        return;
                    }

                    //��ŷ �޾ƿ��� ����
                    if (Lobby_Mgr.inst.rankingText == null)
                    {
                        isNetworkLock = false;
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
                    isNetworkLock = false;
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

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
                request,
                (result) =>
                {
                    if (Lobby_Mgr.inst == null)
                    {
                        isNetworkLock = false;
                        return;
                    }

                    if (0 < result.Leaderboard.Count)
                    {
                        var curBoard = result.Leaderboard[0];
                        Lobby_Mgr.inst.myRank = curBoard.Position + 1;     //�� ��� ��������
                        GlobalValue.g_BestScore = curBoard.StatValue; //�� �ְ� ���� ����

                        Lobby_Mgr.inst.CfgResponse();      //��� UI ����
                    }

                    isNetworkLock = false;
                },
                (error) =>
                {
                    Debug.Log("�� ��� �ҷ����� ����");
                    isNetworkLock = false;
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
