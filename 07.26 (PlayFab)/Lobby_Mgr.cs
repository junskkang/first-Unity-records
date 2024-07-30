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

    public Button Store_Btn;
    public Button MyRoom_Btn;
    public Button Exit_Btn;
    public Button GameStart_Btn;

    public Text m_GoldText;
    public Text m_UserInfoText;

    

    //--- ȯ�漳�� Dlg ���� ����
    [Header("--- ConfigBox ---")]
    public Button m_CfgBtn = null;
    public GameObject Canvas_Dialog = null;
    GameObject m_ConfigBoxObj = null;
    //--- ȯ�漳�� Dlg ���� ����

    [Header("--- Ranking ---")]
    public Text rankingText;
    public Button restoreRankBtn;
    int myRank = 0;
    float restoreTimer = 0.0f;  //��ŷ ���� Ÿ�̸�
    float delayGetLB = 3.0f;    //�κ� ������ 3�� �ڿ� ��ŷ �ѹ� �� �ε��ϱ�
        

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

        Invoke("GetRankingList", delayGetLB);

        if (restoreRankBtn != null)
            restoreRankBtn.onClick.AddListener(RestoreRank);
    }



    void ClearSvData()
    {
        PlayerPrefs.DeleteAll();    //���ÿ� ����Ǿ� �־��� ��� ������ �����ش�.

        GlobalValue.g_CurSkillCount.Clear();
        GlobalValue.LoadGameData();

        if (m_GoldText != null)
            m_GoldText.text = GlobalValue.g_UserGold.ToString("N0");

        if (m_UserInfoText != null)
            m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ") : ����(" + myRank +
                                  "��) : ����(" + GlobalValue.g_BestScore + "��)";

        Sound_Mgr.Inst.PlayGUISound("Pop", 1.0f);
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
        //Debug.Log("Ÿ��Ʋ ������ ������ ��ư Ŭ��");
        //SceneManager.LoadScene("TitleScene");
        MyLoadScene("TitleScene");
    }

    // Update is called once per frame
    void Update()
    {
        
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

    void CfgResponse() //ȯ�漳�� �ڽ� Ok �� ȣ��ǰ� �ϱ� ���� �Լ�
    {
        if (m_UserInfoText != null)
            m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ") : ����(" + myRank +
                                  "��) : ����(" + GlobalValue.g_BestScore + "��)";
    }

    private void RestoreRank()  //���� �������� ��ư
    {
        if (0.0f < restoreTimer)
        {
            Debug.Log("������ �ʹ� ����ϴ�. ��� �� ��ư�� �ٽ� �����ּ���.");
            return;
        }

        GetRankingList();

        restoreTimer = 5.0f;
    }

    void GetRankingList()
    {
        if (GlobalValue.g_Unique_ID == "") return;  //�������� �α��� ���¿����� ����ǵ���

        var request = new GetLeaderboardRequest()
        {
            StartPosition = 0,    //0�� �ε������� �������, �� 1�����
            StatisticName = "BestScore",
            //������ �������� ����ǥ ���� �� BestScore ����
            MaxResultsCount = 10,  //10�����
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,     //�г��� ��û
                ShowAvatarUrl = true,       //�ƹ�Ÿ URL ��û               

            }
        };

        PlayFabClientAPI.GetLeaderboard(
                request,
                (result) =>
                {
                    //��ŷ �޾ƿ��� ����
                    if (rankingText == null) return;

                    string strBuff = "";

                    for (int i = 0; i < result.Leaderboard.Count; i++)
                    {
                        var curBoard = result.Leaderboard[i];

                        //��� �ȿ� ���� �ִٸ� �� ǥ��
                        if (curBoard.PlayFabId == GlobalValue.g_Unique_ID)
                            strBuff += "<color=#00ff00>";

                        strBuff += (i + 1).ToString() + "�� : " 
                                + curBoard.DisplayName + " : "
                                + curBoard.StatValue + "�� \n";

                        //��� �ȿ� ���� �ִٸ� �� ǥ�� ����
                        if (curBoard.PlayFabId == GlobalValue.g_Unique_ID)
                            strBuff += "</color>";
                    }

                    if (strBuff != "")
                        rankingText.text = strBuff;

                    //�������� ����� �ҷ��� �� �� �� ��� ������.
                    GetMyRanking();

                },
                (error) =>
                {
                    //��ŷ �޾ƿ��� ����
                    Debug.Log("��������ҷ����� ����");
                }
                );
    }

    void GetMyRanking()
    {
        //GetLeaderboardAroundPlayer() : Ư�� PlayFabID�� �������� �ֺ����� ����Ʈ�� �ҷ����� �Լ�

        var request = new GetLeaderboardAroundPlayerRequest()
        {
            //PlayFabId = GlobalValue.g_Unique_ID,  //����Ʈ������ �α��� �� ���̵�� ������ ��
            StatisticName = "BestScore",
            MaxResultsCount = 1,    //�Ѹ��� ������ �����´ٴ� ��
            //ProfileConstraints = new PlayerProfileViewConstraints()
            //{
            //    ShowDisplayName = true,
            //}
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
                request,
                (result) =>
                {
                    if (0 < result.Leaderboard.Count)
                    {
                        var curBoard = result.Leaderboard[0];
                        myRank = curBoard.Position + 1;     //�� ��� ��������
                        GlobalValue.g_BestScore = curBoard.StatValue; //�� �ְ� ���� ����

                        CfgResponse();      //��� UI ����
                    }
                },
                (error)=>
                {
                    Debug.Log("�� ��� �ҷ����� ����");
                }
                );
    }
}
