using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine.SceneManagement;
using SimpleJSON;

public enum PacketType
{
    //��Ŷ�̶� ������ ������ ������ ����
    //������ �� �׸� ���� ������ Ŭ������ ����� �����������
    //������ ���� ���̵� ��, �ð���, ���Ȱ��� ���...
    BestScore,      //�ְ�����
    UserGold,       //�������
    UpdateItem,     //������ ���� �� ����
    NickUpdate,     //�г��Ӱ���
    UpdateExp,      //����ġ����
}

public class Network_Mgr : MonoBehaviour
{
    //������ ������ ��Ŷ ó���� ť ���� ����
    bool isNetworkLock = false;     //Network ��� ���� ���� üũ��
    List<PacketType> packetBuff = new List<PacketType>();
    //���� ��Ŷ Ÿ�� ��� ����Ʈ (ť ����)

    //[HideInInspector] public string tempStrBuff = "";
    //public delegate void Net_Response(bool isOk, string message);   //��������Ʈ ������(�ɼ�)�� �ϳ� ����
    //public Net_Response dltMethod = null;           //��������Ʈ ���� ����(���� ����)

    //�̱��� ������ ���� �ν��Ͻ� ���� ����
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
        if (!isNetworkLock)     //���� ��Ŷ ó�� ���� ���°� �ƴ϶��
        {
            if (0 < packetBuff.Count)       //��� ��Ŷ�� �����Ѵٸ�
            {
                Req_Network();      //��Ŷ ó�� �Լ� ����
            }
            else    //ó���� ��Ŷ�� �ϳ��� ���ٸ�    packetBuff.Count < 0
            {
                //�Ź� ó���� ��Ŷ�� �ϳ��� ���� ���� ����ó�� �ǵ���
                if (Game_Mgr.Inst.State == GameState.GameExit || Game_Mgr.Inst.State == GameState.GameReplay)
                    Exe_GameEnd();
            }
        }
    }

    void Req_Network() //RequestNetWork
    {
        if (packetBuff[0] == PacketType.UserGold)
            UpdateGoldCo();     //Playfab ������ ��� ���� ��û �Լ�
        else if (packetBuff[0] == PacketType.BestScore)
            UpdateScoreCo();    //Playfab ������ �ְ� ��� ���� ��û
        else if (packetBuff[0] == PacketType.UpdateItem)
            UpdateItemCo();     //Playfab ������ ������ ���� �� ���� ��û
        else if (packetBuff[0] == PacketType.UpdateExp)
            UpdateExpCo();
        //else if (packetBuff[0] == PacketType.NickUpdate)  //ConfigBox���� �ٷ� ó���ϴ� �ɷ� ����
        //    NickChangeCo(tempStrBuff);


        packetBuff.RemoveAt(0); //ó���� ��Ŷ ����
    }

    private float exitTimer = 0.3f; 
    //State�� �ٲ�� �� ������ �ڵ����� ī��Ʈ�� ���� �ǰ�
    //State�� �Ѿ�ٴ� �� �ݵ�� ���̵��� �ִٴ� ���̹Ƿ� 
    //������ ������ �ʿ� ���� �̴�θ� �ֵ� ���ۿ� �̻� ����

    //public void SetExitTime(float value = 0.3f)
    //{
    //    exitTimer = value;
    //}


    void Exe_GameEnd()      //Execute �����ϴ��� ����
    {
        //���� ó������ ���� ��Ŷ�� ������ ó������ ����ä ��ũ��Ʈ�� ������� ��츦 ���
        if (isNetworkLock) return;

        if (Game_Mgr.Inst.State == GameState.GameExit || Game_Mgr.Inst.State == GameState.GameReplay)
        {
            if (exitTimer > 0.0f)
            {
                exitTimer -= Time.unscaledDeltaTime;    //timeScale�� ������ ���� �ʴ� ��ŸŸ��

                if(exitTimer <= 0.0f)
                    Exit_Game();
            }
        }            
    }

    void Exit_Game()
    {
        Debug.Log("Exit_Game ȣ�� �Ϸ�");
        //ó���� ��Ŷ�� �ϳ��� ���� ���(packetBuff.Count == 0)���� ���� ó��
        if (Game_Mgr.Inst.State == GameState.GameExit)
        {
            //�κ�� �̵� ��ư�� ������ ���¶��
            SceneManager.LoadScene("LobbyScene");
        }
        else if (Game_Mgr.Inst.State == GameState.GameReplay)
        {
            //�ٽ� �ϱ� ��ư�� ������ ���¶��
            SceneManager.LoadScene("GameScene");
        }
    }

    void UpdateGoldCo()     //���� �����÷��̿� ���� ������ �ڷ�ƾ �Լ��� ��������
    {
        if (GlobalValue.g_Unique_ID == "") return;
        //var request = new UpdateUserDataRequest();          //��ü ����
        //request.Permission = UserDataPermission.Private;    //��� ���� ����
        //request.Data = new Dictionary<string, string>();    //�����͸� ��ųʸ��� ����
        //request.Data.Add("UserGold", GlobalValue.g_UserGold.ToString());    //������ �����ϰ� ���� ���� ����
        //request.Data.Add("Level", GlobalValue.g_Level.ToString());
        //request.Data.Add("UserStar", GlobalValue.g_UserStar.ToString());

        // ����Ʈ �� <�÷��̾� ������(Ÿ��Ʋ)> �� Ȱ�� �ڵ�
        var request = new UpdateUserDataRequest()
        {
            //Permission = UserDataPermission.Private, //����Ʈ��
            //Permission = UserDataPermission.Public,
            //Public : ��������(�ٸ� �������� �� ���� �ְ� �ϴ� �ɼ�) ���, ���� ��
            //Private : ��������� (���� �� �� �ִ� ���� �Ӽ��� ����) ��尪 ��

            Data = new Dictionary<string, string>()
            {
                {"UserGold", GlobalValue.g_UserGold.ToString() },
                //{"Level", GlobalValue.g_Level.ToString()},
                //{"UserStar", GlobalValue.g_UserStar.ToString()}
            }            
        };

        isNetworkLock = true;       
        //��û�� ���� �� true�� �ٲ� update�Լ��� ��� ���߰� ��
        PlayFabClientAPI.UpdateUserData(request, 
        (result) =>
        {
            //������ ������ �ٽ� update�Լ� ���ư�����
            isNetworkLock = false;
            //������ ���� ����
        }, 
        (error) =>
        {
            isNetworkLock = false;
            Debug.Log(error.GenerateErrorReport() + " : isNetworkLock : " + isNetworkLock);
        });
    }
    void UpdateScoreCo()
    {
        if (GlobalValue.g_Unique_ID == "") return; //���������� �α����� �Ǿ����� Ȯ���ϴ� �뵵

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

    void UpdateItemCo()
    {
        if (GlobalValue.g_Unique_ID == "") return; //���������� �α����� �Ǿ����� Ȯ���ϴ� �뵵

        Dictionary<string, string> itemList = new Dictionary<string, string>();
        for (int i = 0; i < GlobalValue.g_CurSkillCount.Count; i++)
        {
            itemList.Add($"Skill_Item_{i}", GlobalValue.g_CurSkillCount[i].ToString());
        }

        //<�÷��̾� ������(Ÿ��Ʋ)> �� Ȱ�� �ڵ�
        var request = new UpdateUserDataRequest()
        {
            Data = itemList
        };

        isNetworkLock = true;

        PlayFabClientAPI.UpdateUserData(request,
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

    void UpdateExpCo()
    {
        if (GlobalValue.g_Unique_ID == "") return; //���������� �α����� �Ǿ����� Ȯ���ϴ� �뵵

        //AvatarURL�� �̿��ؼ� �����ϸ�
        //����ǥ ����Ʈ ���� �� AvatarURL�� ���� �޾ƿ� �� �ִ�. == ������ ����

        //AvatarURL�� �̿��ؼ� ������ Level�� �����ϵ��� Ȱ��
        //Json �������� ����
        JSONObject makeJson = new JSONObject();
        makeJson["UserExp"] = GlobalValue.g_Exp;
        makeJson["UserLevel"] = GlobalValue.g_Level;
        string strJson = makeJson.ToString();

        var request = new UpdateAvatarUrlRequest()
        {
            ImageUrl = strJson
        };

        isNetworkLock = true;
        PlayFabClientAPI.UpdateAvatarUrl(request,
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
        //            Game_Mgr.Inst.m_UserInfoText.text = "������ : ����(" +
        //                                                GlobalValue.g_NickName + ")";
        //        }

        //        isNetworkLock = false; 
        //    },
        //    (error) =>
        //    { 
        //        isNetworkLock = false;

        //        //������ �г����� �̹� ������ ���� �޼��� ��� �ʿ�
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
            if (packetBuff[i] == pType) //���� ó������ ���� ��Ŷ�� �����Ѵٸ�
                isExist = true;     
            //����Ʈ�� ��Ŷ�� ���� �߰����� �ʰ� �⺻ ������ ��Ŷ���� ������Ʈ �Ѵ�.
        }

        if (!isExist)   //������� �ش� Ÿ���� ��Ŷ�� ������ ���� �߰��Ѵ�.
            packetBuff.Add(pType);

    }
}
