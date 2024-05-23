using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby_Mgr : MonoBehaviour
{
    public Button Store_Btn;
    public Button MyRoom_Btn;
    public Button Exit_Btn;
    public Button GameStartBtn;
    public Text GoldText;
    public Text UserInfo;

    //환경설정 변수
    public Button ConfigBtn;
    public GameObject Canvas_Dialog;
    GameObject m_ConfigBoxObj;

    // Start is called before the first frame update
    void Start()
    {
        if (Store_Btn != null)
            Store_Btn.onClick.AddListener(StoreBtnClick);

        if (MyRoom_Btn != null)
            MyRoom_Btn.onClick.AddListener(MyRoomBtnClick);

        if (Exit_Btn != null)
            Exit_Btn.onClick.AddListener(ExitBtnClick);

        if (GameStartBtn != null)
            GameStartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("GameScene");
                Sound_Mgr.Inst.PlayGUISound("Pop", 0.4f);
            });

        if(GoldText != null)
            GoldText.text = (PlayerPrefs.GetInt("GoldCount", 0)).ToString("N0");

        if (UserInfo != null)
            UserInfo.text = $"내정보 : 별명({PlayerPrefs.GetString("UserNick", "나 강림")}) : 점수(100)";

        if (ConfigBtn != null)
            ConfigBtn.onClick.AddListener(() =>
            {
                if (m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject a_CfgBox = Instantiate(m_ConfigBoxObj);
                a_CfgBox.transform.SetParent(Canvas_Dialog.transform, false);
                //false : 로컬 프리팹에 저장된 좌표 및 스케일을 그대로 유지

                Time.timeScale = 0.0f;  //환경설정 창 열였을 때 일시정지 효과
            });

        Sound_Mgr.Inst.PlayBGM("sound_bgm_village_001", 0.2f);
    }

    private void StoreBtnClick()
    {
        //Debug.Log("상점으로 가기 버튼 클릭");
        SceneManager.LoadScene("StoreScene");
        Sound_Mgr.Inst.PlayGUISound("Pop", 0.4f);
    }

    private void MyRoomBtnClick()
    {
        //Debug.Log("꾸미기 방 가기 버튼 클릭");
        SceneManager.LoadScene("MyRoomScene");
        Sound_Mgr.Inst.PlayGUISound("Pop", 0.4f);
    }

    private void ExitBtnClick()
    {
        //Debug.Log("타이틀 씬으로 나가기 버튼 클릭");
        SceneManager.LoadScene("TitleScene");
        Sound_Mgr.Inst.PlayGUISound("Pop", 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
