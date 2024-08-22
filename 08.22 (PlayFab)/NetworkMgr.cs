using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PacketType
{
    BestScore,      //최고점수
    UserGold,       //유저골드
    UpdateItem,     //아이템 보유수 갱신
    NickUpdate,     //닉네임갱신
    UpdateExp,      //경헌치갱신
}
public class NetworkMgr : MonoBehaviour
{
    
    //--- 서버에 전송할 패킷 처리용 큐 관련 변수
    [HideInInspector] public List<PacketType> m_PacketBuff = new List<PacketType>();
    //보낼 패킷 타입 대기 리스트 (큐 역할)

    [HideInInspector] public float m_NetWaitTime = 0.0f;

    //싱글턴 패턴을 위한 인스턴스 변수 선언
    public static NetworkMgr Inst = null;

    void Awake()
    {
        //NetworkMgr 클래스를 인스턴에 대입
        Inst = this;
    }
    //싱글턴 패턴을 위한 인스턴스 변수 선언
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_NetWaitTime -= Time.unscaledDeltaTime;
        if (m_NetWaitTime < 0.0f)
            m_NetWaitTime = 0.0f;

        if (m_NetWaitTime <= 0.0f) //지금 패킷 처리 중인 상태가 아니면...
        {
            if (0 < m_PacketBuff.Count) //대기 패킷이 존재한다면...
            {
                Req_NetWork();
            }
            //else  //처리할 패킷이 하나도 없다면...
            //{
            //    //매번 처리할 패킷이 하나도 없을 때만 종료처리 해야 할지 확인한다.
            //    Exe_GameEnd();
            //}
        }//if(m_NetWaitTime <= 0.0f) //지금 패킷 처리 중인 상태가 아니면...

    }

    void Req_NetWork()  //RequestNetWork
    {
        if (m_PacketBuff[0] == PacketType.BestScore)
            UpdateScoreCo();
        else if (m_PacketBuff[0] == PacketType.UserGold)
            UpdateGoldCo(); //Playfab 서버에 골드갱신 요청 함수
        else if (m_PacketBuff[0] == PacketType.UpdateItem)
            UpdateItemCo(); //Playfab 서버에 아이템 보유수 갱신 요청 함수
        //else if (m_PacketBuff[0] == PacketType.UpdateExp)
        //    UpdateExpCo();

        m_PacketBuff.RemoveAt(0);
    }

    //void Exe_GameEnd()  //Execute //샐행하다.
    //{  //매번 처리할 패킷이 하나도 없을 때만 종료처리 해야 할지 판단하는 함수

    //    //if (Game_Mgr.Inst.m_GameState == GameState.GameExit)
    //    //{   //"로비로 이동" 버튼이 눌려진 상태라면...
    //    //    SceneManager.LoadScene("LobbyScene");
    //    //}
    //    //else if (Game_Mgr.Inst.m_GameState == GameState.GameReplay)
    //    //{   //"다시하기" 버튼이 눌려진 상태라면...
    //    //    SceneManager.LoadScene("GameScene");
    //    //}
    //}//void Exe_GameEnd()  //Execute //샐행하다.

    void UpdateScoreCo()
    {
        if (GlobalValue.g_Unique_ID == "")
            return;

        var request = new UpdatePlayerStatisticsRequest
        {
            //BestScore, BestLevel, ...
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate { StatisticName = "BestScore",
                                     Value = GlobalValue.g_BestScore },
                //new StatisticUpdate { StatisticName = "BestLevel", 
                //                     Value = GlobalValue.g_BestLevel }
            }
        };

        m_NetWaitTime = 0.5f;

        PlayFabClientAPI.UpdatePlayerStatistics(
                        request,

                        (result) =>
                        { //업데이트 성공시 응답 함수

                        },

                        (error) =>
                        { //업데이트 실패시 응답 함수

                        }
             );
    }

    void UpdateGoldCo() //Playfab 서버에 골드갱신 요청 함수
    {
        if (GlobalValue.g_Unique_ID == "")
            return;

        //var request = new UpdateUserDataRequest();
        //request.Permission = UserDataPermission.Private;
        //request.Data = new Dictionary<string, string>();
        //request.Data.Add("UserGold", GlobalValue.g_UserGold.ToString());
        //request.Data.Add("Level", GlobalValue.g_Level.ToString());
        //request.Data.Add("UserStar", GlobalValue.g_UserStar.ToString());

        // < 플레이어 데이터(타이틀) > 값 활용 코드
        var request = new UpdateUserDataRequest()
        {
            //Permission = UserDataPermission.Private, //디폴트값
            //Permission = UserDataPermission.Public,
            //Public 공개설정 : 다른 유저들이 볼 수도 있게 하는 옵션
            //Private 비공개 설정(기본설정임) : 나만 볼 수 있는 값의 속성을 변경

            Data = new Dictionary<string, string>()
            {
                { "UserGold", GlobalValue.g_UserGold.ToString() },
                //{ "Level", GlobalValue.g_Level.ToString() },
                //{ "UserStar", GlobalValue.g_UserStar.ToString() },
            }
        };

        m_NetWaitTime = 0.5f;

        PlayFabClientAPI.UpdateUserData(request,
            (result) =>
            {
                //Debug.Log("데이터 저장 성공");
            },
            (error) =>
            {
                //Debug.Log("데이터 저장 실패 " + error.GenerateErrorReport());
            });
    }


    void UpdateItemCo()
    {
        if (GlobalValue.g_Unique_ID == "")
            return;     //정상적으로 로그인이 되어 있는 상태일 때만...

        Dictionary<string, string> a_ItemList = new Dictionary<string, string>();
        for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
        {
            a_ItemList.Add($"SkItem_{i}", GlobalValue.g_SkillCount[i].ToString());
        }

        //< 플레이어 데이터(타이틀) > 값 활용 코드
        var request = new UpdateUserDataRequest()
        {
            Data = a_ItemList
        };

        m_NetWaitTime = 0.5f;

        PlayFabClientAPI.UpdateUserData(request,
                (result) =>
                {
                    //Debug.Log("데이터 저장 성공");
                },
                (error) =>
                {
                    //Debug.Log("데이터 저장 실패");
                }
            );
    }
    public void PushPacket(PacketType a_PType)
    {
        bool a_IsExist = false;
        for (int i = 0; i < m_PacketBuff.Count; i++)
        {
            if (m_PacketBuff[i] == a_PType)  //아직 처리 되지 않은 패킷이 존재하면.
                a_IsExist = true;
            //또 추가하지 않고 기본 버퍼의 패킷으로 업데이트 한다.
        }

        if (a_IsExist == false)
            m_PacketBuff.Add(a_PType);
        //대기 중인 이 타입의 패킷이 없으면 새로 추가한다.
    }

    public bool IsBackLobbyOk()
    {
        //대기시간과 패킷이 남아있는 동안에는 false 리턴
        if ((0.0f < m_NetWaitTime) || (0 < m_PacketBuff.Count))
        {
            return false;
        }


        //없으면 true를 리턴
        return true;
    }
}
