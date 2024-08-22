using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby_Mgr : MonoBehaviour
{
    public Button m_ClearSvDataBtn;
    float ClearLockTimer = 0;

    public Button m_StoreBtn;
    public Button m_GameStartBtn;
    public Button m_LogoutBtn;

    public Text m_GoldText;
    public Text m_UserInfoText;

    public Text MessageText;
    float ShowMsTimer = 0;


    public Button m_RestoreRankBtn;
    public Text m_RankingText;
    float RestoreTimer = 3.0f;  //��ŷ ���� Ÿ�̸�

    [HideInInspector] public int m_My_Rank = 0;

    //--- �̱��� ����
    public static Lobby_Mgr Inst = null;

    void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;  //���� �ӵ���...

        GlobalValue.LoadGameData();

        if (m_ClearSvDataBtn != null)
            m_ClearSvDataBtn.onClick.AddListener(ClearSvData);

        if (m_StoreBtn != null)
            m_StoreBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("StoreScene");
            });

        if (m_GameStartBtn != null)
            m_GameStartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("GameScene");
            });

        if (m_LogoutBtn != null)
            m_LogoutBtn.onClick.AddListener(() =>
            {
                GlobalValue.g_Unique_ID = "";
                GlobalValue.g_NickName = "";
                GlobalValue.g_BestScore = 0;
                GlobalValue.g_UserGold = 0;
                GlobalValue.g_Level = 0;
                GlobalValue.g_Exp = 0;
                for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
                {
                    GlobalValue.g_SkillCount[i] = 0;
                }
                PlayFabClientAPI.ForgetAllCredentials();    //Playfab �α׾ƿ��ϴ� �Լ�

                SceneManager.LoadScene("TitleScene");
            });

        if(m_GoldText != null)
        {
            m_GoldText.text = GlobalValue.g_UserGold.ToString("N0");
            // "N0" <-- �Ҽ��� �����δ� ���ܽ�Ű�� õ���� ���� ��ǥ �ٿ��ֱ�...
        }

        if(m_UserInfoText != null)
        {
            m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ") ����("
                + GlobalValue.g_BestScore + "��) ����(" + m_My_Rank + ")";
        }


        LobbyNetworkMgr.Inst.PushPacket(LobbyNetworkMgr.PacketType.GetRankingList);

#if AutoRestore
        //--- �ڵ� ��������� ���
        if (m_RestoreRankBtn != null)
            m_RestoreRankBtn.gameObject.SetActive(false);
        //--- �ڵ� ��������� ���
#else
        //--- ���� ��������� ���
        if (m_RestoreRankBtn != null)
            m_RestoreRankBtn.onClick.AddListener(RestoreRank);
        //--- ���� ��������� ���
#endif

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        if (0.0f < ClearLockTimer)
            ClearLockTimer -= Time.deltaTime;
#if AutoRestore
        //-- �ڵ� ��������� ���
        RestoreTimer -= Time.deltaTime;
        if(RestoreTimer <= 0.0f)
        {
            LobbyNetworkMgr.Inst.PushPacket(LobbyNetworkMgr.PacketType.GetRankingList);
            RestoreTimer = 7.0f;
        }
        //-- �ڵ� ��������� ���
#else
        //<-- ���� ��������� ���
        if (0.0f < RestoreTimer)
            RestoreTimer -= Time.deltaTime;
        //<-- ���� ��������� ���
#endif
        if (0.0f < ShowMsTimer)
        {
            ShowMsTimer -= Time.deltaTime;
            if (ShowMsTimer <= 0.0f)
            {
                MessageOnOff("", false); //�޽��� ����
            }
        }//if(0.0f < ShowMsTimer)
    }

    void ClearSvData()
    {
        if (0.0f < ClearLockTimer) return;

        PlayerPrefs.DeleteAll();    //���ÿ� ����Ǿ� �ִ� ��� ������ �����ش�.        
        GlobalValue.LoadGameData();

        GlobalValue.g_BestScore = 0;
        GlobalValue.g_UserGold = 0;
        GlobalValue.g_Level = 0;
        GlobalValue.g_Exp   = 0;
        for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
        {
            GlobalValue.g_SkillCount[i] = 0;
        }
        m_My_Rank = 0;


        LobbyNetworkMgr.Inst.PushPacket(LobbyNetworkMgr.PacketType.ClearSave);
        LobbyNetworkMgr.Inst.PushPacket(LobbyNetworkMgr.PacketType.ClearScore);
        LobbyNetworkMgr.Inst.PushPacket(LobbyNetworkMgr.PacketType.ClearExp);

        if (m_GoldText != null)
            m_GoldText.text = GlobalValue.g_UserGold.ToString("N0");

        if (m_UserInfoText != null)
            m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ") ����("
                + GlobalValue.g_BestScore + "��) ����(" +m_My_Rank+")";

        ClearLockTimer = 5.0f;
    }

    void RestoreRank() //���� ������ ���
    {
        if (0.0f < RestoreTimer)
        {
            MessageOnOff("�ּ� 7�� �ֱ�θ� ���ŵ˴ϴ�.");
            return;
        }

        LobbyNetworkMgr.Inst.PushPacket(LobbyNetworkMgr.PacketType.GetRankingList);

        RestoreTimer = 7.0f;
    }

    public void CfgResponse() //ȯ�漳�� �ڽ� Ok �� ȣ��ǰ� �ϱ� ���� �Լ�
    {
        if (m_UserInfoText != null)
        {
            m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + " : ����(" + m_My_Rank +
                                  "��) : ����(" + GlobalValue.g_BestScore + "��)";
        }
    }

    void MessageOnOff(string Mess = "", bool isOn = true)
    {
        if (isOn == true)
        {
            MessageText.text = Mess;
            MessageText.gameObject.SetActive(true);
            ShowMsTimer = 5.0f;
        }
        else
        {
            MessageText.text = "";
            MessageText.gameObject.SetActive(false);
        }
    }
}
