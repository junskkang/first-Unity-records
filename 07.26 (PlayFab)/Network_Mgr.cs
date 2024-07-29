using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public enum PacketType
{
    //패킷이란 서버에 보내는 데이터 단위
    //원래는 각 항목에 대한 각각의 클래스로 만들어 관리해줘야함
    //유저의 고유 아이디 등, 시간값, 보안관련 등등...
    BestScore,      //최고점수
    UserGold,       //보유골드
    NickUpdate,     //닉네임갱신
    UpdateExp,      //경험치갱신
}

public class Network_Mgr : MonoBehaviour
{
    //서버에 전송할 패킷 처리용 큐 관련 변수
    bool isNetworkLock = false;     //Network 대기 상태 여부 체크용
    List<PacketType> packetBuff = new List<PacketType>();
    //보낼 패킷 타입 대기 리스트 (큐 역할)

    //[HideInInspector] public string tempStrBuff = "";
    //public delegate void Net_Response(bool isOk, string message);   //델리게이트 데이터(옵션)형 하나 선언
    //public Net_Response dltMethod = null;           //델리게이트 변수 선언(소켓 역할)

    //싱글턴 패턴을 위한 인스턴스 변수 선언
    public static Network_Mgr instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isNetworkLock)     //지금 패킷 처리 중인 상태가 아니라면
        {
            if (0 < packetBuff.Count)       //대기 패킷이 존재한다면
            {
                Req_Network();      //패킷 처리 함수 실행
            }
        }
    }

    void Req_Network() //RequestNetWork
    {
        if (packetBuff[0] == PacketType.UserGold)
            UpdateGoldCo();     //Playfab 서버에 골드 갱신 요청 함수
        else if (packetBuff[0] == PacketType.BestScore)
            UpdateScoreCo();
        //else if (packetBuff[0] == PacketType.NickUpdate)  //ConfigBox에서 바로 처리하는 걸로 수정
        //    NickChangeCo(tempStrBuff);


        packetBuff.RemoveAt(0); //처리한 패킷 제거
    }

    void UpdateGoldCo()     //원래 게임플레이에 지장 없도록 코루틴 함수로 만들어야함
    {
        if (GlobalValue.g_Unique_ID == "") return;
        //var request = new UpdateUserDataRequest();          //객체 생성
        //request.Permission = UserDataPermission.Private;    //멤버 변수 설정
        //request.Data = new Dictionary<string, string>();    //데이터를 딕셔너리로 저장
        //request.Data.Add("UserGold", GlobalValue.g_UserGold.ToString());    //서버에 저장하고 싶은 값을 저장
        //request.Data.Add("Level", GlobalValue.g_Level.ToString());
        //request.Data.Add("UserStar", GlobalValue.g_UserStar.ToString());

        // 사이트 내 <플레이어 데이터(타이틀)> 값 활용 코드
        var request = new UpdateUserDataRequest()
        {
            //Permission = UserDataPermission.Private, //디폴트값
            //Permission = UserDataPermission.Public,
            //Public : 공개설정(다른 유저들이 볼 수도 있게 하는 옵션) 계급, 레벨 등
            //Private : 비공개설정 (나만 볼 수 있는 값의 속성을 변경) 골드값 등

            Data = new Dictionary<string, string>()
            {
                {"UserGold", GlobalValue.g_UserGold.ToString() },
                //{"Level", GlobalValue.g_Level.ToString()},
                //{"UserStar", GlobalValue.g_UserStar.ToString()}
            }            
        };

        isNetworkLock = true;       
        //요청을 보낼 때 true로 바꿔 update함수를 잠시 멈추게 함
        PlayFabClientAPI.UpdateUserData(request, 
        (result) =>
        {
            //응답을 받으면 다시 update함수 돌아가도록
            isNetworkLock = false;
            //데이터 저장 성공
        }, 
        (error) =>
        {
            isNetworkLock = false;
            Debug.Log(error.GenerateErrorReport());
        });
    }
    void UpdateScoreCo()
    {
        if (GlobalValue.g_Unique_ID == "") return; //정상적으로 로그인이 되었음을 확인하는 용도

        var request = new UpdatePlayerStatisticsRequest()
        {
            //best score, best level....
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate
                {
                    StatisticName = "BestScore",
                    Value = GlobalValue.g_BestScore
                },
                //new StatisticUpdate
                //{
                //    StatisticName = "BestLevel",
                //    Value = GlobalValue.g_BestLevel
                //}
            }
        };

        isNetworkLock = true;
        PlayFabClientAPI.UpdatePlayerStatistics(request,
            (result) =>
            {
                isNetworkLock = false;
            },
            (error) =>
            {
                isNetworkLock = false;
            }
        );
    }

    void NickChangeCo(string nickName)
    {
        if (GlobalValue.g_Unique_ID == "" || nickName == "") return;

        isNetworkLock = true;

        //PlayFabClientAPI.UpdateUserTitleDisplayName(
        //    new UpdateUserTitleDisplayNameRequest()
        //    {
        //        DisplayName = nickName
        //    },
        //    (result) =>
        //    { 
        //        GlobalValue.g_NickName = result.DisplayName;
                
        //        if (Game_Mgr.Inst != null)
        //        {
        //            Game_Mgr.Inst.m_UserInfoText.text = "내정보 : 별명(" +
        //                                                GlobalValue.g_NickName + ")";
        //        }

        //        isNetworkLock = false; 
        //    },
        //    (error) =>
        //    { 
        //        isNetworkLock = false;

        //        //동일한 닉네임이 이미 존재할 때는 메세지 출력 필요
        //        Debug.Log(error.GenerateErrorReport());
        //    });
    }

    //private void UpdateSuccess(UpdateUserDataResult result)
    //{

    //}

    //private void UpdateFailure(PlayFabError error)
    //{

    //}

    public void PushPacket(PacketType pType)
    {
        bool isExist = false;

        for (int i = 0; i < packetBuff.Count; i++)
        {
            if (packetBuff[i] == pType) //아직 처리되지 않은 패킷이 존재한다면
                isExist = true;     
            //리스트에 패킷을 새로 추가하지 않고 기본 버퍼의 패킷으로 업데이트 한다.
        }

        if (!isExist)   //대기중인 해당 타입의 패킷이 없으면 새로 추가한다.
            packetBuff.Add(pType);

    }
}
