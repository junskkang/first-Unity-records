using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public Button GameStartBtn;
    public Button ToTitleBtn;
    public Button ToShopBtn;
    public Button ToMyRoomBtn;
    public Button SettingBtn;
    
    public Button MyInfoBtn;
    public Text HelpText;
    public Text GoldText;

    //HelpText Ÿ�̸�
    float textTimer = 0.0f;
    float showTextTime = 2.0f;


    //�Ҽȹ�ư ��ũ�Ѻ�
    public Button SocialBtn;
    public Transform SocialTr;
    bool socialOnOff = false;
    float scSpeed = 2000.0f;
    Vector3 scOnPos = new Vector3(-205.0f, 0.0f, 0.0f);
    Vector3 scOffPos = new Vector3(320.0f, 0.0f, 0.0f);
    public Button MessageBtn;
    public Button FBBtn;
    public Button TwitterBtn;
    public Button AchieveBtn;


    // Start is called before the first frame update
    void Start()
    {
        GlobalData.LoadGold();
        
        if (GoldText != null)
            GoldText.text = GlobalData.gameGold.ToString("N0");


        if (ToTitleBtn != null)
            ToTitleBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TitleScene");
            });

        if (GameStartBtn != null)
            GameStartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("GameScene");
            });

        if (ToShopBtn != null)
            ToShopBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("ShopScene");
            });

        if (ToMyRoomBtn != null)
            ToMyRoomBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MyRoomScene");
            });

        if (SettingBtn != null)
            SettingBtn.onClick.AddListener(() =>
            {
                textTimer = showTextTime;
                if (HelpText != null)
                    HelpText.text = "���� ����� ���� �̱����Դϴ�.";
            });

        if (MyInfoBtn != null)
            MyInfoBtn.onClick.AddListener(() =>
            {
                textTimer = showTextTime;
                if (HelpText != null)
                    HelpText.text = "�� ���� ����� ���� �̱����Դϴ�.";
            });

        if (SocialBtn != null)
            SocialBtn.onClick.AddListener(() =>
            {
                socialOnOff = !socialOnOff;

            });

        if (MessageBtn != null)
            MessageBtn.onClick.AddListener(() =>
            {
                textTimer = showTextTime;
                if (HelpText != null)
                    HelpText.text = "������ ����� ���� �̱����Դϴ�.";
            });

        if (FBBtn != null)
            FBBtn.onClick.AddListener(() =>
            {
                textTimer = showTextTime;
                if (HelpText != null)
                    HelpText.text = "���̽��� ����� ���� �̱����Դϴ�.";
            });

        if (TwitterBtn != null)
            TwitterBtn.onClick.AddListener(() =>
            {
                textTimer = showTextTime;
                if (HelpText != null)
                    HelpText.text = "Ʈ���� ����� ���� �̱����Դϴ�.";
            });

        if (AchieveBtn != null)
            AchieveBtn.onClick.AddListener(() =>
            {
                textTimer = showTextTime;
                if (HelpText != null)
                    HelpText.text = "���� ����� ���� �̱����Դϴ�.";
            });
    }



    // Update is called once per frame
    void Update()
    {
        SocialScOnOff();

        TextTimer();
    }
    private void SocialScOnOff()
    {
        if (SocialTr == null) return;

        if (socialOnOff == false)
        {
            if (SocialTr.localPosition.x < scOffPos.x)
            {
                SocialTr.localPosition =
                    Vector3.MoveTowards(SocialTr.localPosition, scOffPos, scSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (scOnPos.x < SocialTr.localPosition.x)
            {
                SocialTr.localPosition = 
                    Vector3.MoveTowards(SocialTr.localPosition, scOnPos, scSpeed * Time.deltaTime);
            }
        }
        
    }

    void TextTimer()
    {
        if (textTimer >= showTextTime)
            HelpText.gameObject.SetActive(true);

        textTimer -= Time.deltaTime;

        if (textTimer < 0.0f)
            HelpText.gameObject.SetActive(false);

        if (textTimer <= 0)
            textTimer = 0.0f;
    }
}
