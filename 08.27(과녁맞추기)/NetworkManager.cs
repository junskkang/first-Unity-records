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
        Debug.Log("�α��� �õ�");

        string a_IdStr = idInput.Trim();
        string a_PwStr = pwInput.Trim();

        if (string.IsNullOrEmpty(a_IdStr) == true ||
           string.IsNullOrEmpty(a_PwStr) == true)
        {
            //MessageOnOff("Id, Pw ��ĭ ���� �Է��� �ּ���.");
        }

        if (!(6 <= a_IdStr.Length && a_IdStr.Length <= 20))  // 6 ~20
        {
            //MessageOnOff("Id�� 6���ں��� 20���ڱ��� �ۼ��� �ּ���.");
            return;
        }

        if (!(6 == a_PwStr.Length))
        {
            //MessageOnOff("��й�ȣ�� 4�ڸ��� ���ڷθ� �ۼ��� �ּ���.");
            return;
        }

        netWaitTime = 0.5f;

        //--- �α��� ������ � ���� ������ ���������� �����ϴ� �ɼ� ��ü ����
        var option = new GetPlayerCombinedInfoRequestParams()
        {
            //--- DisplayName(�г���)�� �������� ���� �ɼ�
            GetPlayerProfile = true,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,  //DisplayName(�г���) �������� ���� ��û �ɼ�
                //ShowAvatarUrl = true     //�ƹ� URL�� �������� �ɼ�
            },
            //--- DisplayName(�г���)�� �������� ���� �ɼ�

            GetPlayerStatistics = true,
            //--- �� �ɼ����� ��谪(����ǥ�� �����ϴ�)�� �ҷ��� �� �ִ�.
            GetUserData = true
            //--- �� �ɼ����� < �÷��̾� ������(Ÿ��Ʋ) > ���� �ҷ��� �� �ִ�.
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
            //�г��� ��������
            GlobalUserData.Nickname = result.InfoResultPayload.PlayerProfile.DisplayName;


            //--- < �÷��̾� ������(Ÿ��Ʋ) > �� �޾ƿ���
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
             //--- < �÷��̾� ������(Ÿ��Ʋ) >�� �޾ƿ���
        }


        //�α��� �����ÿ� ...
        if (TitleManager.instance.saveIdToggle.isOn == true)  //üũ ��ư�� ���� ������
        {
            PlayerPrefs.SetString("MySave_Id", textID);
        }
        else  //üũ ��ư�� ���� ������
        {
            PlayerPrefs.DeleteKey("MySave_Id");
        }

        //Debug.Log("��ư�� Ŭ�� �߾��.");
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
        Debug.Log("���� �õ�");
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
            //MessageOnOff("Id, Pw, ������ ��ĭ ���� �Է��� �ּ���.");
            return;
        }

        if (!(6 <= a_IdStr.Length && a_IdStr.Length <= 20))  // 6~2-
        {
            //MessageOnOff("Id�� 6���ں��� 20���ڱ��� �ۼ��� �ּ���.");
            return;
        }

        if (!(6 == a_PwStr.Length))
        {
            //MessageOnOff("��й�ȣ�� 6���ں��� 20���ڱ��� �ۼ��� �ּ���.");
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

            RequireBothUsernameAndEmail = false //Email �� �⺻ Id�� ����ϰڴٴ� �ɼ�
        };

        //MessageOnOff("ȸ�� ���� ��... ��ø� ��ٷ� �ּ���.");
        //showMsTimer = 300.0f;

        PlayFabClientAPI.RegisterPlayFabUser(request,
            (result) =>
            {
                //MessageOnOff("���� ����! �ڷΰ��� ��ư�� ������ �α��� ���ּ���.");
                //idInputField.text = svNewIdStr;
                //passInputField.text = svNewPwStr;
                Debug.Log("���Լ���");
            },
            (error) =>
            {
                Debug.Log(error);
            });

        textID = "";
        textPW = "";
        textNick = "";

    }

    void UpdateScore() //Playfab ������ ��尻�� ��û �Լ�
    {       
        if (GlobalUserData.Unique_ID == "")
            return;

        // < �÷��̾� ������(Ÿ��Ʋ) > �� Ȱ�� �ڵ�
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
                Debug.Log("������ ���� ����");
            },
            (error) =>
            {
                //Debug.Log("������ ���� ���� " + error.GenerateErrorReport());
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
                Debug.Log("��ŷ ���ε� ����");
            },
            (error) =>
            {
                Debug.Log("��ŷ ���ε� ����" + error.ToString());
            });

    }


    void ClearSaveDataCo()
    {
        if (GlobalUserData.Unique_ID == "")
            return;

        // < �÷��̾� ������(Ÿ��Ʋ) > �� Ȱ�� �ڵ�
        var request = new UpdateUserDataRequest();
        //�ɹ����� KeysToRemove : Ư��Ű ���� ���� ������ �� �� �ִ�.
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
        if (GlobalUserData.Unique_ID == "")  //�α��� ���¿�����...
            return;

        var request = new GetLeaderboardRequest
        {
            StartPosition = 0,              //0�� �ε��� �� 1�����
            StatisticName = "BestScore",
            //������ �������� ����ǥ ���� �� "BestScore" ����
            MaxResultsCount = 10,           //10�����
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true, //�г��ӵ� ��û
                //ShowAvatarUrl = true  //���� ���� ����� �ּҵ� ��û(�̰� ����ġ�� ���)
            }
        };

        netWaitTime = 0.5f;

        PlayFabClientAPI.GetLeaderboard(
            request,

            (result) =>
            { //��ŷ ����Ʈ �޾ƿ��� ����

                string a_strBuff = "";

                for (int i = 0; i < result.Leaderboard.Count; i++)
                {
                    var curBoard = result.Leaderboard[i];
                    //int a_ULevel = LvMyJsonParser(curBoard.Profile.AvatarUrl);

                    //��� �ȿ� ���� �ִٸ� �� ǥ��
                    if (curBoard.PlayFabId == GlobalUserData.Unique_ID)
                        a_strBuff += "<color=#008800>";

                    a_strBuff += (i + 1).ToString() + "�� : " +
                                    curBoard.DisplayName + " : " +
                                    curBoard.StatValue + "��" + "\n";

                    //��� �ȿ� ���� �ִٸ� �� ǥ��
                    if (curBoard.PlayFabId == GlobalUserData.Unique_ID)
                        a_strBuff += "</color>";

                }//for(int i = 0; i < result.Leaderboard.Count; i++)

                if (a_strBuff != "")
                    GameMgr.inst.rankingText.text = a_strBuff;

                //�������� ����� �ҷ��� �� �� �� ����� �ҷ� �´�.
                //GetMyRanking();
            },

            (error) =>
            { //��ŷ ����Ʈ ����� ���� ���� �� 
                Debug.Log("�������� �ҷ����� ����");
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
