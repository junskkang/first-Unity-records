using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PacketType
{
    UserGold,   //���
    BombCount,  //��ź ��ų ī��Ʈ 
    NickUpdate, //�г��� ������Ʈ  
    ClearSave   //�������� �ʱ�ȭ
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

    void UpdateGoldCo() //Playfab ������ ��尻�� ��û �Լ�
    {
        if (GlobalUserData.g_Unique_ID == "")
            return;

        // < �÷��̾� ������(Ÿ��Ʋ) > �� Ȱ�� �ڵ�
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
                //Debug.Log("������ ���� ����");
            },
            (error) =>
            {
                //Debug.Log("������ ���� ���� " + error.GenerateErrorReport());
            });
    }

    void UpdateBombCountCo() //Playfab ������ ��尻�� ��û �Լ�
    {
        //Debug.Log("��ź��ų �Լ� ��û ����");

        if (GlobalUserData.g_Unique_ID == "")
            return;

        // < �÷��̾� ������(Ÿ��Ʋ) > �� Ȱ�� �ڵ�
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
                //Debug.Log("������ ���� ����");
            },
            (error) =>
            {
                //Debug.Log("������ ���� ���� " + error.GenerateErrorReport());
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
                    //������ �г����� �̹� ������ ���� �޼��� ��� �ʿ�
                    //Debug.Log(error.GenerateErrorReport());
                    string strError = error.GenerateErrorReport();
                    if (strError.Contains("Name not available"))
                    {
                        refConfig.MessageOnOff("������ �г����� �����մϴ�");
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

        // < �÷��̾� ������(Ÿ��Ʋ) > �� Ȱ�� �ڵ�
        var request = new UpdateUserDataRequest();
        //�ɹ����� KeysToRemove : Ư��Ű ���� ���� ������ �� �� �ִ�.
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
