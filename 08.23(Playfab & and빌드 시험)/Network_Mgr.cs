using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PacketType
{
    UserGold,   //골드
    BombCount,  //폭탄 스킬 카운트 
    NickUpdate, //닉네임 업데이트  
    ClearSave   //저장정보 초기화
}

public class Network_Mgr : MonoBehaviour
{
    [HideInInspector] public List<PacketType> packetBuff = new List<PacketType>();
    [HideInInspector] public float netWaitTime = 0.0f;

    string newNick = "";
    
    HeroCtrl refHero = null;
    ConfigBox refConfig = null;

    private static Network_Mgr instance = null;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static Network_Mgr Inst
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {        
        
    }

    // Update is called once per frame
    void Update()
    {
        netWaitTime -= Time.unscaledDeltaTime;
        if (netWaitTime < 0.0f)
            netWaitTime = 0.0f;

        if (netWaitTime <= 0.0f) 
        {
            if (0 < packetBuff.Count)
            {
                Req_NetWork();
            }
        }
    }
    void Req_NetWork()  //RequestNetWork
    {
        //Debug.Log(packetBuff[0]);

        if (packetBuff[0] == PacketType.UserGold)
            UpdateGoldCo();
        else if (packetBuff[0] == PacketType.BombCount)
            UpdateBombCountCo();
        else if (packetBuff[0] == PacketType.NickUpdate)
            UpdateNickCo();
        else if (packetBuff[0] == PacketType.ClearSave)
            ClearSaveDataCo();

        packetBuff.RemoveAt(0);
    }

    void UpdateGoldCo() //Playfab 서버에 골드갱신 요청 함수
    {
        if (GlobalUserData.g_Unique_ID == "")
            return;

        // < 플레이어 데이터(타이틀) > 값 활용 코드
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                { "UserGold", GlobalUserData.g_UserGold.ToString() },
            }
        };

        netWaitTime = 0.5f;

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

    void UpdateBombCountCo() //Playfab 서버에 골드갱신 요청 함수
    {
        //Debug.Log("폭탄스킬 함수 요청 성공");

        if (GlobalUserData.g_Unique_ID == "")
            return;

        // < 플레이어 데이터(타이틀) > 값 활용 코드
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"BombCount", GlobalUserData.g_BombCount.ToString()},
            }
        };

        netWaitTime = 0.5f;

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

    void UpdateNickCo()
    {
        newNick = PlayerPrefs.GetString("NewNick");

        netWaitTime = 0.5f;

        PlayFabClientAPI.UpdateUserTitleDisplayName(

                new UpdateUserTitleDisplayNameRequest()
                {
                    DisplayName = newNick
                },
                (result) =>
                {
                    GlobalUserData.g_NickName = result.DisplayName;

                    refHero = FindObjectOfType<HeroCtrl>();
                    if (refHero != null)
                        refHero.ChangeNickName(result.DisplayName);

                    Destroy(refConfig.gameObject);
                    Time.timeScale = 1.0f;
                },
                (error) =>
                {
                    //동일한 닉네임이 이미 존재할 때는 메세지 출력 필요
                    //Debug.Log(error.GenerateErrorReport());
                    string strError = error.GenerateErrorReport();
                    if (strError.Contains("Name not available"))
                    {
                        refConfig.MessageOnOff("동일한 닉네임이 존재합니다");
                    }
                    else
                    {
                        refConfig.MessageOnOff(strError);
                    }
                });

        PlayerPrefs.SetString("NewNick", "");
    }

    void ClearSaveDataCo()
    {
        if (GlobalUserData.g_Unique_ID == "")
            return;

        // < 플레이어 데이터(타이틀) > 값 활용 코드
        var request = new UpdateUserDataRequest();
        //맴버변수 KeysToRemove : 특정키 값을 삭제 까지는 할 수 있다.
        request.KeysToRemove = new List<string>();
        request.KeysToRemove.Add("UserGold");
        request.KeysToRemove.Add("BombCount");

        netWaitTime = 0.5f;

        PlayFabClientAPI.UpdateUserData(request,
                        (result) =>
                        {

                        },
                        (error) =>
                        {

                        }
                 );
    }

    public void PushPacket(PacketType a_PType)
    {
        bool a_IsExist = false;
        for (int i = 0; i < packetBuff.Count; i++)
        {
            if (packetBuff[i] == a_PType)
                a_IsExist = true;
        }

        if (a_IsExist == false)
            packetBuff.Add(a_PType);
    }

    public void TakeConfigBox(ConfigBox configBox)
    {
        refConfig = configBox;
    }
}
