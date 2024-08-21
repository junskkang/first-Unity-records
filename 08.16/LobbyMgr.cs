using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMgr : MonoBehaviour
{
    public Button m_Start_Btn;
    public Button m_Store_Btn;
    public Button m_Logout_Btn;
    public Button m_Clear_Save_Btn;
    

    public Text UserInfoText;

    [HideInInspector] public int m_MyRank = 0;

    public Button RestRk_Btn;
    public Text Rank_Text;
    public Text MessageText;
    float ShowMsTimer = 0.0f;

    [Header("ConfigBox")]
    public Button ConfigBtn;
    public GameObject Canvas_Dialog;
    public GameObject configBoxObj;

    //�������� �ʱ�ȭ ���� ����
    float ClearLockTimer = 0.0f;

    public static LobbyMgr Inst = null;

    void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f; //�Ͻ������� ���� �ӵ���...
        GlobalValue.LoadGameData();

        if (m_Start_Btn != null)
            m_Start_Btn.onClick.AddListener(StartBtnClick);

        if (m_Store_Btn != null)
            m_Store_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("StoreScene");
            });

        if (m_Logout_Btn != null)
            m_Logout_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TitleScene");
            });

        if (m_Clear_Save_Btn != null)
            m_Clear_Save_Btn.onClick.AddListener(Clear_Save_Click);



        //if (RestRk_Btn != null)
        //    RestRk_Btn.onClick.AddListener();

        RefreshUserInfo();

#if AutoRestore
        //�ڵ� ��ŷ ������ ���
        if (RestRk_Btn != null)
            RestRk_Btn.gameObject.SetActive(false);
#else
        //���� ��ŷ ������ ���
        if (RestRk_Btn != null)
            RestRk_Btn.onClick.AddListener(RestoreRank);
#endif

        //ȯ�漳�� �߰�
        if (ConfigBtn != null)
            ConfigBtn.onClick.AddListener(() =>
            {
                if (configBoxObj == null)
                    configBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject ob = Instantiate(configBoxObj) as GameObject;
                ob.transform.SetParent(Canvas_Dialog.transform, false);
                Time.timeScale = 0.0f;
            });
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < ShowMsTimer)
        {
            ShowMsTimer -= Time.deltaTime;
            if (ShowMsTimer <= 0.0f)
            {
                MessageOnOff("", false);    //�޼��� ����
            }
        }

        if (0.0f < ClearLockTimer)
        {
            ClearLockTimer -= Time.deltaTime;   
        }
    }

    void StartBtnClick()
    {
        if(100 <= GlobalValue.g_CurFloorNum)
        {
            //������ ���� ������ ���¿��� ������ ���� �ߴٸ�...
            //�ٷ� ���� ��(99��)���� �����ϰ� �ϱ�...
            GlobalValue.g_CurFloorNum = 99;
            
            //���ÿ� �� �ε�, ���� �κ� ������ �ű�� ���� �ּ�ó�� (08/20)
            //PlayerPrefs.SetInt("CurFloorNum", GlobalValue.g_CurFloorNum);
        }

        SceneManager.LoadScene("scLevel01");
        SceneManager.LoadScene("scPlay", LoadSceneMode.Additive);
    }

    void Clear_Save_Click()
    {
        if (0.0f < ClearLockTimer)
        {
            MessageOnOff("�������� �ʱ�ȭ ���Դϴ�.");
            return;
        }

        PlayerPrefs.DeleteAll();
        GlobalValue.LoadGameData();
        //RefreshUserInfo();

        LobbyNetworkMgr.Inst.DltMethod = Result_Clear_Save;
        LobbyNetworkMgr.Inst.PushPacket(PacketType.ClearSave);

        ClearLockTimer = 5.0f;
        //5�ʰ� ��õ� ����
    }

    void Result_Clear_Save(bool isOk)
    {
        //���� �ʱ�ȭ ���� �� ��� ���� �ʱ�ȭ �ʿ�
        if (isOk)
        {
            GlobalValue.g_BestScore = 0;    //�ְ� ���
            GlobalValue.g_UserGold = 0;     //���ӸӴ�
            GlobalValue.g_Exp = 0;          //����ġ
            GlobalValue.g_Level = 0;        //���� �ʱ�ȭ

            GlobalValue.g_BestFloor = 1;    //���� ���� ��
            GlobalValue.g_CurFloorNum = 1;  //���� ��

            for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
            {
                GlobalValue.g_SkillCount[i] = 1;    //������ ���� �ʱ�ȭ
            }
            RefreshUserInfo();  //����, ��尪 UI �ʱ�ȭ
        }
    }

    public void RefreshUserInfo()
    {
        UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName +
                            ") : ����(" + m_MyRank + "��) : ����(" +
                            GlobalValue.g_BestScore.ToString("N0") + "��) : ���(" +
                            GlobalValue.g_UserGold.ToString("N0") + ")";
    }

    public void RefreshRankUI(RkRootInfo rootInfo)
    {
        Rank_Text.text = "";

        for (int i = 0; i < rootInfo.RkList.Length; i++)
        {
            //��� �ȿ� ���� �ִٸ� �� ǥ��
            if (rootInfo.RkList[i].user_id == GlobalValue.g_Unique_ID)
                Rank_Text.text += "<color=#00ff00>";

            Rank_Text.text += (i + 1).ToString() + "�� : " +
                rootInfo.RkList[i].user_id + " (" + rootInfo.RkList[i].nick_name + ") :" +
                rootInfo.RkList[i].best_score +"�� \n";

            if (rootInfo.RkList[i].user_id == GlobalValue.g_Unique_ID)
                Rank_Text.text += "</color>";
        }

        m_MyRank = rootInfo.my_rank;

        RefreshUserInfo();
    }
    void RestoreRank()
    {
        if (0.0f < LobbyNetworkMgr.Inst.RestoreTimer)
        {
            MessageOnOff("�ּ� 7�� �ֱ�θ� ���ŵ˴ϴ�.");
            return;
        }

        LobbyNetworkMgr.Inst.GetRankingList();
        LobbyNetworkMgr.Inst.RestoreTimer = 7.0f;
    }
    public void MessageOnOff(string Mess = "", bool isOn = true, float a_Time = 5.0f)
    {
        if (isOn)
        {
            MessageText.text = Mess;
            MessageText.gameObject.SetActive(true);
            ShowMsTimer = a_Time;
        }
        else
        {
            MessageText.text = "";
            MessageText.gameObject.SetActive(false);
        }
    }
}
