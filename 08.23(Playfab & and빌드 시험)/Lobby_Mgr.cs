using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;

public class Lobby_Mgr : MonoBehaviour
{
    public Button Store_Btn;
    public Button MyRoom_Btn;
    public Button Exit_Btn;
    public Button m_GameStartBtn;
    public Button m_ClearSvDataBtn;
    float clearLockTimer = 0.0f;

    public Text m_GoldText;
    public Text m_MyInfoText;

    //--- 환경설정 Dlg 관련 변수
    [Header("--- DialogBox ---")]
    public Button m_CfgBtn = null;
    public GameObject Canvas_Dialog = null;
    GameObject m_ConfigBoxObj = null;
    //--- 환경설정 Dlg 관련 변수

    //--- 구글 버튼 관련 변수
    [Header("--- Google Button ---")]
    public Button Google_Btn = null;
    bool m_Google_ScOnOff = false;
    public Transform m_ScrollTr = null;
    float m_ScSpeed = 800.0f;
    Vector3 m_ScOnPos = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 m_ScOffPos = new Vector3(122.0f, 0.0f, 0.0f);
    //--- 구글 버튼 관련 변수

    // Start is called before the first frame update
    void Start()
    {
        GlobalUserData.LoadGameInfo();

        if(m_GoldText != null)
        {
            if (GlobalUserData.g_UserGold <= 0)
                m_GoldText.text = "x 00";
            else
                m_GoldText.text = "x " + GlobalUserData.g_UserGold.ToString("N0");
        }

        if (Store_Btn != null)
            Store_Btn.onClick.AddListener(StoreBtnClick);

        if (MyRoom_Btn != null)
            MyRoom_Btn.onClick.AddListener(MyRoomBtnClick);

        if (Exit_Btn != null)
            Exit_Btn.onClick.AddListener(ExitBtnClick);

        if (m_GameStartBtn != null)
            m_GameStartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("GameScene");
                SoundMgr.Inst.PlayGUISound("Pop", 0.4f);
            });

        if (m_ClearSvDataBtn != null)
            m_ClearSvDataBtn.onClick.AddListener(ClearSvData);

        if (Google_Btn != null)
            Google_Btn.onClick.AddListener(() =>
            {
                m_Google_ScOnOff = !m_Google_ScOnOff;
            });

        //--- 환경설정 Dlg 관련 구현 부분
        if (m_CfgBtn != null)
            m_CfgBtn.onClick.AddListener(() =>
            {
                if(m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject a_CfgBoxObj = Instantiate(m_ConfigBoxObj);
                a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);

                Time.timeScale = 0.0f;
            });
        //--- 환경설정 Dlg 관련 구현 부분

        SoundMgr.Inst.PlayBGM("sound_bgm_village_001", 0.2f);
    }

    private void StoreBtnClick()
    {
        //Debug.Log("상점으로 가기 버튼 클릭");
        SceneManager.LoadScene("StoreScene");
        SoundMgr.Inst.PlayGUISound("Pop", 0.4f);
    }

    private void MyRoomBtnClick()
    {
        //Debug.Log("꾸미기 방 가기 버튼 클릭");
        SceneManager.LoadScene("MyRoomScene");
        SoundMgr.Inst.PlayGUISound("Pop", 0.4f);
    }

    private void ExitBtnClick()
    {
        GlobalUserData.g_Unique_ID = "";
        GlobalUserData.g_NickName = "";
        GlobalUserData.g_UserGold = 0;
        GlobalUserData.g_BombCount = 0;

        PlayFabClientAPI.ForgetAllCredentials();

        SceneManager.LoadScene("TitleScene");
        SoundMgr.Inst.PlayGUISound("Pop", 0.4f);
    }

    void ClearSvData()
    {
        if (0.0f < clearLockTimer) return;

        PlayerPrefs.DeleteAll();

        GlobalUserData.g_UserGold = 0;
        GlobalUserData.g_BombCount = 0;

        Network_Mgr.Inst.PushPacket(PacketType.ClearSave);

        if (m_GoldText != null)
        {
            if (GlobalUserData.g_UserGold <= 0)
                m_GoldText.text = "x 00";
            else
                m_GoldText.text = "x " + GlobalUserData.g_UserGold.ToString("N0");
        }

        clearLockTimer = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MyInfoText != null)
            m_MyInfoText.text = "내정보 : 별명(" + GlobalUserData.g_NickName + ") : 점수(0)";

        if (0.0f < clearLockTimer)
            clearLockTimer -= Time.deltaTime;

        GoogleMenuUpdate();
    }

    void GoogleMenuUpdate()
    {
        if (m_ScrollTr == null)
            return;

        if(m_Google_ScOnOff == false)
        {
            if(m_ScrollTr.localPosition.x < m_ScOffPos.x)
            {
                m_ScrollTr.localPosition = 
                    Vector3.MoveTowards(m_ScrollTr.localPosition,
                                    m_ScOffPos, m_ScSpeed * Time.deltaTime);
            }
        }
        else
        {
            if(m_ScOnPos.x < m_ScrollTr.localPosition.x)
            {
                m_ScrollTr.localPosition = 
                    Vector3.MoveTowards(m_ScrollTr.localPosition,
                                    m_ScOnPos, m_ScSpeed * Time.deltaTime);
            }
        }

    }// void GoogleMenuUpdate()
}
