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

    //ȯ�漳�� ����
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
            UserInfo.text = $"������ : ����({PlayerPrefs.GetString("UserNick", "�� ����")}) : ����(100)";

        if (ConfigBtn != null)
            ConfigBtn.onClick.AddListener(() =>
            {
                if (m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject a_CfgBox = Instantiate(m_ConfigBoxObj);
                a_CfgBox.transform.SetParent(Canvas_Dialog.transform, false);
                //false : ���� �����տ� ����� ��ǥ �� �������� �״�� ����

                Time.timeScale = 0.0f;  //ȯ�漳�� â ������ �� �Ͻ����� ȿ��
            });

        Sound_Mgr.Inst.PlayBGM("sound_bgm_village_001", 0.2f);
    }

    private void StoreBtnClick()
    {
        //Debug.Log("�������� ���� ��ư Ŭ��");
        SceneManager.LoadScene("StoreScene");
        Sound_Mgr.Inst.PlayGUISound("Pop", 0.4f);
    }

    private void MyRoomBtnClick()
    {
        //Debug.Log("�ٹ̱� �� ���� ��ư Ŭ��");
        SceneManager.LoadScene("MyRoomScene");
        Sound_Mgr.Inst.PlayGUISound("Pop", 0.4f);
    }

    private void ExitBtnClick()
    {
        //Debug.Log("Ÿ��Ʋ ������ ������ ��ư Ŭ��");
        SceneManager.LoadScene("TitleScene");
        Sound_Mgr.Inst.PlayGUISound("Pop", 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
