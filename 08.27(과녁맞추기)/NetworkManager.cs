using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PacketType
{
    CreateAccount,
    Login,
    BestScore,
    ClearSave,
    RankingUpdate
}

public class NetworkManager : G_Singleton<NetworkManager>
{
    [HideInInspector] public List<PacketType> packetBuff = new List<PacketType>();
    [HideInInspector] public float netWaitTime = 0.0f;

    string textID = "";
    string textPW = "";
    string textNick = "";
    protected override void Init()
    {
        base.Init();

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

        if (packetBuff[0] == PacketType.CreateAccount)
            CreateAccount(textID, textPW, textNick);
        else if (packetBuff[0] == PacketType.Login)
            LoginCheck(textID, textPW);
        else if (packetBuff[0] == PacketType.BestScore)
            UpdateScore();
        else if (packetBuff[0] == PacketType.ClearSave)
            ClearSaveDataCo();
        else if (packetBuff[0] == PacketType.RankingUpdate)
            RankingUp();

        packetBuff.RemoveAt(0);
    }

    public void LoginCheck(string idInput = "", string pwInput = "")
    {
        Debug.Log("로그인 시도");

        string a_IdStr = idInput.Trim();
        string a_PwStr = pwInput.Trim();

        if (string.IsNullOrEmpty(a_IdStr) == true ||
           string.IsNullOrEmpty(a_PwStr) == true)
        {
            //MessageOnOff("Id, Pw 빈칸 없이 입력해 주세요.");
        }

        if (!(6 <= a_IdStr.Length && a_IdStr.Length <= 20))  // 6 ~20
        {
            //MessageOnOff("Id는 6글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!(6 == a_PwStr.Length))
        {
            //MessageOnOff("비밀번호는 4자리의 숫자로만 작성해 주세요.");
            return;
        }

        netWaitTime = 0.5f;

        //--- 로그인 성공시 어떤 유저 정보를 가져올지를 설정하는 옵션 객체 생성
        var option = new GetPlayerCombinedInfoRequestParams()
        {
            //--- DisplayName(닉네임)을 가져오기 위한 옵션
            GetPlayerProfile = true,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,  //DisplayName(닉네임) 가져오기 위한 요청 옵션
                //ShowAvatarUrl = true     //아바 URL을 가져오는 옵션
            },
            //--- DisplayName(닉네임)을 가져오기 위한 옵션

            GetPlayerStatistics = true,
            //--- 이 옵션으로 통계값(순위표에 관여하는)을 불러올 수 있다.
            GetUserData = true
            //--- 이 옵션으로 < 플레이어 테이터(타이틀) > 값을 불러올 수 있다.
        };

        var request = new LoginWithEmailAddressRequest()
        {
            Email = a_IdStr,
            Password = a_PwStr,
            InfoRequestParameters = option
        };

        PlayFabClientAPI.LoginWithEmailAddress(request,
                                    OnLoginSuccess, (error) => { });

    }

    void OnLoginSuccess(LoginResult result)
    {
        GlobalUserData.Unique_ID = result.PlayFabId;

        if (result.InfoResultPayload != null)
        {
            //닉네임 가져오기
            GlobalUserData.Nickname = result.InfoResultPayload.PlayerProfile.DisplayName;


            //--- < 플레이어 데이터(타이틀) > 값 받아오기
            int a_GetValue = 0;
            foreach (var eachData in result.InfoResultPayload.UserData)
            {
                if (eachData.Key == "BestScore")
                {
                    if (int.TryParse(eachData.Value.Value, out a_GetValue) == true)
                        GlobalUserData.BestScore = a_GetValue;

                    Debug.Log(GlobalUserData.BestScore);
                }
            }//foreach( var eachData in result.InfoResultPayload.UserData)
             //--- < 플레이어 데이터(타이틀) >값 받아오기
        }


        //로그인 성공시에 ...
        if (TitleManager.instance.saveIdToggle.isOn == true)  //체크 버튼이 켜져 있으면
        {
            PlayerPrefs.SetString("MySave_Id", textID);
        }
        else  //체크 버튼이 꺼져 있으면
        {
            PlayerPrefs.DeleteKey("MySave_Id");
        }

        //Debug.Log("버튼을 클릭 했어요.");
        //bool IsFadeOk = false;
        //if (Fade_Mgr.Inst != null)
        //    IsFadeOk = Fade_Mgr.Inst.SceneOutReserve("LobbyScene");
        //if (IsFadeOk == false)
        SceneManager.LoadScene("GameScene");

        textID = "";
        textPW = "";
    }

    public void CreateAccount(string idInput = "", string pwInput = "", string nickInput = "")
    {
        Debug.Log("가입 시도");
        string a_IdStr = idInput;
        string a_PwStr = pwInput;
        string a_NickStr = nickInput;

        a_IdStr = a_IdStr.Trim();
        a_PwStr = a_PwStr.Trim();
        a_NickStr = a_NickStr.Trim();

        Debug.Log(a_IdStr);
        Debug.Log(a_PwStr);
        Debug.Log(a_NickStr);

        if (string.IsNullOrEmpty(a_IdStr) == true ||
           string.IsNullOrEmpty(a_PwStr) == true ||
           string.IsNullOrEmpty(a_NickStr))
        {
            //MessageOnOff("Id, Pw, 별명은 빈칸 없이 입력해 주세요.");
            return;
        }

        if (!(6 <= a_IdStr.Length && a_IdStr.Length <= 20))  // 6~2-
        {
            //MessageOnOff("Id는 6글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!(6 == a_PwStr.Length))
        {
            //MessageOnOff("비밀번호는 6글자부터 20글자까지 작성해 주세요.");
            return;
        }

        netWaitTime = 0.5f;
        //svNewIdStr = a_IdStr;
        //svNewPwStr = a_PwStr;

        var request = new RegisterPlayFabUserRequest()
        {
            Email = a_IdStr,
            Password = a_PwStr,
            DisplayName = a_NickStr,

            RequireBothUsernameAndEmail = false //Email 을 기본 Id로 사용하겠다는 옵션
        };

        //MessageOnOff("회원 가입 중... 잠시만 기다려 주세요.");
        //showMsTimer = 300.0f;

        PlayFabClientAPI.RegisterPlayFabUser(request,
            (result) =>
            {
                //MessageOnOff("가입 성공! 뒤로가기 버튼을 누르고 로그인 해주세요.");
                //idInputField.text = svNewIdStr;
                //passInputField.text = svNewPwStr;
                Debug.Log("가입성공");
            },
            (error) =>
            {
                Debug.Log(error);
            });

        textID = "";
        textPW = "";
        textNick = "";

    }

    void UpdateScore() //Playfab 서버에 골드갱신 요청 함수
    {       
        if (GlobalUserData.Unique_ID == "")
            return;

        // < 플레이어 데이터(타이틀) > 값 활용 코드
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"BestScore", GlobalUserData.BestScore.ToString()},
            }
        };

        netWaitTime = 0.5f;

        PlayFabClientAPI.UpdateUserData(request,
            (result) =>
            {
                UpdateRank();
                Debug.Log("데이터 저장 성공");
            },
            (error) =>
            {
                //Debug.Log("데이터 저장 실패 " + error.GenerateErrorReport());
            });
    }

    void UpdateRank()
    {
        if (GlobalUserData.Unique_ID == "")
            return;

        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate()
                {
                    StatisticName = "BestScore",
                    Value = GlobalUserData.BestScore
                }
            }
        };

        netWaitTime = 0.5f;
        PlayFabClientAPI.UpdatePlayerStatistics(request,
            (result) =>
            {
                Debug.Log("랭킹 업로드 성공");
            },
            (error) =>
            {
                Debug.Log("랭킹 업로드 실패" + error.ToString());
            });

    }


    void ClearSaveDataCo()
    {
        if (GlobalUserData.Unique_ID == "")
            return;

        // < 플레이어 데이터(타이틀) > 값 활용 코드
        var request = new UpdateUserDataRequest();
        //맴버변수 KeysToRemove : 특정키 값을 삭제 까지는 할 수 있다.
        request.KeysToRemove = new List<string>();
        request.KeysToRemove.Add("BestScore");

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

    public void RankingUp()
    {
        if (GlobalUserData.Unique_ID == "")  //로그인 상태에서만...
            return;

        var request = new GetLeaderboardRequest
        {
            StartPosition = 0,              //0번 인덱스 즉 1등부터
            StatisticName = "BestScore",
            //관리자 페이지의 순위표 변수 중 "BestScore" 기준
            MaxResultsCount = 10,           //10명까지
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true, //닉네임도 요청
                //ShowAvatarUrl = true  //유저 사진 썸네일 주소도 요청(이건 경험치로 사용)
            }
        };

        netWaitTime = 0.5f;

        PlayFabClientAPI.GetLeaderboard(
            request,

            (result) =>
            { //랭킹 리스트 받아오기 성공

                string a_strBuff = "";

                for (int i = 0; i < result.Leaderboard.Count; i++)
                {
                    var curBoard = result.Leaderboard[i];
                    //int a_ULevel = LvMyJsonParser(curBoard.Profile.AvatarUrl);

                    //등수 안에 내가 있다면 색 표시
                    if (curBoard.PlayFabId == GlobalUserData.Unique_ID)
                        a_strBuff += "<color=#008800>";

                    a_strBuff += (i + 1).ToString() + "등 : " +
                                    curBoard.DisplayName + " : " +
                                    curBoard.StatValue + "점" + "\n";

                    //등수 안에 내가 있다면 색 표시
                    if (curBoard.PlayFabId == GlobalUserData.Unique_ID)
                        a_strBuff += "</color>";

                }//for(int i = 0; i < result.Leaderboard.Count; i++)

                if (a_strBuff != "")
                    GameMgr.inst.rankingText.text = a_strBuff;

                //리더보드 등수를 불러온 직 후 내 등수를 불러 온다.
                //GetMyRanking();
            },

            (error) =>
            { //랭킹 리스트 방오기 실패 했을 때 
                Debug.Log("리더보드 불러오기 실패");
                //isNetworkLock = false;
            });
    }

    public void PushPacket(PacketType a_PType, string id = "", string pw = "", string nick = "")
    {
        bool a_IsExist = false;
        for (int i = 0; i < packetBuff.Count; i++)
        {
            if (packetBuff[i] == a_PType)
                a_IsExist = true;
        }

        if (a_IsExist == false)
            packetBuff.Add(a_PType);

        if (id != "" && pw != "")
        {
            textID = id;
            textPW = pw;

            if (nick != "")
                textNick = nick;
        }
    }
}
