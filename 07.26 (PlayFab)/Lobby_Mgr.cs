//#define AutoRestore

using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ConfigBox;

public class Lobby_Mgr : MonoBehaviour
{
    public Button m_ClearSvDataBtn;
    float ClearLockTimer = 0.0f;    //������ �ʱ�ȭ ��� Ÿ�̸�

    public GameObject LoadingPanel;
    public Image loadingCharacter;

    public Button Store_Btn;
    public Button MyRoom_Btn;
    public Button Exit_Btn;
    public Button GameStart_Btn;

    public Text m_GoldText;
    public Text m_UserInfoText;
    public Text messageText;
    float messageTimer = 0.0f;
    

    //--- ȯ�漳�� Dlg ���� ����
    [Header("--- ConfigBox ---")]
    public Button m_CfgBtn = null;
    public GameObject Canvas_Dialog = null;
    GameObject m_ConfigBoxObj = null;
    //--- ȯ�漳�� Dlg ���� ����

    [Header("--- Ranking ---")]
    public Text rankingText;
    public Button restoreRankBtn;
    [HideInInspector] public int myRank = 0;
    float restoreTimer = 3.0f;  //��ŷ ���� Ÿ�̸�
    //float delayGetLB = 3.0f;    //�κ� ������ 3�� �ڿ� ��ŷ �ѹ� �� �ε��ϱ�
    

    public static Lobby_Mgr inst;

    private void Awake()
    {
        inst = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        GlobalValue.LoadGameData();

        if (m_ClearSvDataBtn != null)
            m_ClearSvDataBtn.onClick.AddListener(ClearSvData);

        if (Store_Btn != null)
            Store_Btn.onClick.AddListener(StoreBtnClick);

        if (MyRoom_Btn != null)
            MyRoom_Btn.onClick.AddListener(MyRoomBtnClick);

        if (Exit_Btn != null)
            Exit_Btn.onClick.AddListener(ExitBtnClick);

        if (GameStart_Btn != null)
            GameStart_Btn.onClick.AddListener(() =>
            {
                //SceneManager.LoadScene("GameScene");
                MyLoadScene("GameScene");
            });

        if(m_GoldText != null)
        {
            m_GoldText.text = GlobalValue.g_UserGold.ToString("N0");
            //"N0" �� ���� <-- �Ҽ��� �����δ� ���ܽ�Ű�� õ���� ���� ��ǥ �ٿ��ֱ�...
        }

        if(m_UserInfoText != null)
        {
            m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ") : ����(" + myRank +
                                  "��) : ����(" + GlobalValue.g_BestScore + "��)";
        }

        //--- ȯ�漳�� Dlg ���� ���� �κ�
        if (m_CfgBtn != null)
            m_CfgBtn.onClick.AddListener(() =>
            {
                if (m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject a_CfgBoxObj = Instantiate(m_ConfigBoxObj);
                a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);
                a_CfgBoxObj.GetComponent<ConfigBox>().DltMethod = CfgResponse;

                Time.timeScale = 0.0f;
            });
        //--- ȯ�漳�� Dlg ���� ���� �κ�

        Sound_Mgr.Inst.PlayBGM("sound_bgm_title_001", 0.5f);

        //Invoke("GetRankingList", delayGetLB);
        LobbyNetwork_Mgr.inst.PushPacket(LobbyNetwork_Mgr.PacketType.GetRankingList);

#if AutoRestore     //�� ���� ���� �ֱ⸶�� �ڵ����� ��ŷ ������Ʈ ����
        restoreTimer = 3.0f;        //�ڵ� Ÿ�̸� 3�� ����
        if (restoreRankBtn != null)
            restoreRankBtn.gameObject.SetActive(false);
#else               //�������� ������ ���� ����
        if (restoreRankBtn != null)
            restoreRankBtn.onClick.AddListener(RestoreRank);
#endif
    }



    void ClearSvData()
    {
        if (0.0 < ClearLockTimer)
        {
            MessageOnOff(ClearLockTimer.ToString("N0") + "�� �� �õ����ֽñ� �ٶ��ϴ�.");
            return;
        }
        //SceneManager.LoadScene("LobbyScene");  
        //��Ŷó���� ������ ��Ű�� ������ ���� ��� �ӹ�������
        //��ٸ��� ���� �߰� �ʿ�
        Time.timeScale = 0.0f;
        LoadingPanel.gameObject.SetActive(true);
        StartCoroutine(Loading());

        GlobalValue.g_BestScore = 0;
        GlobalValue.g_UserGold = 0;
        GlobalValue.g_Exp = 0;
        GlobalValue.g_Level = 0;
        Lobby_Mgr.inst.myRank = 0;
        for (int i = 0; i < GlobalValue.g_CurSkillCount.Count; i++)
        {
            GlobalValue.g_CurSkillCount[i] = 0;
        }

        PlayerPrefs.DeleteAll();    //���ÿ� ����Ǿ� �־��� ��� ������ �����ش�.

        LobbyNetwork_Mgr.inst.PushPacket(LobbyNetwork_Mgr.PacketType.ClearSave);
        LobbyNetwork_Mgr.inst.PushPacket(LobbyNetwork_Mgr.PacketType.ClearScore);
        LobbyNetwork_Mgr.inst.PushPacket(LobbyNetwork_Mgr.PacketType.ClearExp);

        //Network_Mgr.instance.PushPacket(PacketType.UserGold);
        //Network_Mgr.instance.PushPacket(PacketType.BestScore);
        //Network_Mgr.instance.PushPacket(PacketType.UpdateExp);
        //GlobalValue.g_CurSkillCount.Clear();
        //GlobalValue.LoadGameData();

        if (m_GoldText != null)
            m_GoldText.text = GlobalValue.g_UserGold.ToString("N0");

        if (m_UserInfoText != null)
            m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ") : ����(" + myRank +
                                  "��) : ����(" + GlobalValue.g_BestScore + "��)";

        LobbyNetwork_Mgr.inst.PushPacket(LobbyNetwork_Mgr.PacketType.GetRankingList);

        Sound_Mgr.Inst.PlayGUISound("Pop", 1.0f);

        ClearLockTimer = 10.0f;



    }
    IEnumerator Loading()
    {
        yield return new WaitForSecondsRealtime(0.7f);

        if (loadingCharacter != null)
            loadingCharacter.transform.position = new Vector3(loadingCharacter.transform.position.x + 95.0f, loadingCharacter.transform.position.y);

        yield return new WaitForSecondsRealtime(0.7f);

        if (loadingCharacter != null)
            loadingCharacter.transform.position = new Vector3(loadingCharacter.transform.position.x + 95.0f, loadingCharacter.transform.position.y);

        yield return new WaitForSecondsRealtime(0.7f);

        LoadingPanel.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
    private void StoreBtnClick()
    {
        //Debug.Log("�������� ���� ��ư Ŭ��");
        //SceneManager.LoadScene("StoreScene");
        MyLoadScene("StoreScene");
    }

    private void MyRoomBtnClick()
    {
        //Debug.Log("�ٹ̱� �� ���� ��ư Ŭ��");
        //SceneManager.LoadScene("MyRoomScene");
        MyLoadScene("MyRoomScene");
    }

    private void ExitBtnClick()
    {
        //�α׾ƿ� �� ��� �۷ι� ���� �ʱ�ȭ
        GlobalValue.g_Unique_ID = "";
        GlobalValue.g_NickName = "";
        GlobalValue.g_BestScore = 0;
        GlobalValue.g_UserGold = 0;
        GlobalValue.g_Exp = 0;
        GlobalValue.g_Level = 0;
        for (int i = 0; i < GlobalValue.g_CurSkillCount.Count; i++)
            GlobalValue.g_CurSkillCount[i] = 0;

        PlayFabClientAPI.ForgetAllCredentials();    //Playfab���� �α׾ƿ� ��Ű�� ��

        //Debug.Log("Ÿ��Ʋ ������ ������ ��ư Ŭ��");
        //SceneManager.LoadScene("TitleScene");
        MyLoadScene("TitleScene");
    }

    // Update is called once per frame
    void Update()
    {
#if AutoRestore
        restoreTimer -= Time.deltaTime;
        if (restoreTimer <= 0)
        {
            LobbyNetwork_Mgr.inst.PushPacket(LobbyNetwork_Mgr.PacketType.GetRankingList);
            restoreTimer = 5.0f;
        }

#else
        if (0.0f < restoreTimer)
            restoreTimer -= Time.deltaTime;
#endif

        if (0.0f < messageTimer)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0.0f)
            {
                MessageOnOff("", false);    //�޼��� ����
            }
        }

        if (0.0f < ClearLockTimer)
        {
            ClearLockTimer -= Time.deltaTime;
            if (ClearLockTimer <= 0.0f)
                ClearLockTimer = 0.0f;
        }

    }

    void MyLoadScene(string a_ScName)
    {
        bool IsFadeOk = false;
        if (Fade_Mgr.Inst != null)
            IsFadeOk = Fade_Mgr.Inst.SceneOutReserve(a_ScName);
        if (IsFadeOk == false)
            SceneManager.LoadScene(a_ScName);

        Sound_Mgr.Inst.PlayGUISound("Pop", 1.0f);
    }

    public void CfgResponse() //ȯ�漳�� �ڽ� Ok �� ȣ��ǰ� �ϱ� ���� �Լ�
    {
        if (m_UserInfoText != null)
            m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ") : ����(" + myRank +
                                  "��) : ����(" + GlobalValue.g_BestScore + "��)";
    }

    private void RestoreRank()  //���� �������� ��ư
    {
        if (0.0f < restoreTimer)
        {
            MessageOnOff("������ �ʹ� ����ϴ�. ��� �� ��ư�� �ٽ� �����ּ���.");
            return;
        }

        LobbyNetwork_Mgr.inst.PushPacket(LobbyNetwork_Mgr.PacketType.GetRankingList);

        restoreTimer = 5.0f;
    }

    void MessageOnOff(string a_Message = "", bool isOn = true)
    {
        if (isOn)
        {
            messageText.text = a_Message;
            messageText.gameObject.SetActive(true);
            messageTimer = 3.0f;
        }
        else
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
    }
    
}
