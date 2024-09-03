using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class TitleManager : MonoBehaviour
{
    [Header("LoginPanel")]
    public GameObject loginPanel;
    public InputField idInputField;     //Email 로 받을 것임
    public InputField passInputField;
    public Button loginBtn;
    public Button createAccOpenBtn;
    public Toggle saveIdToggle;

    [Header("CreateAccountPanel")]
    public GameObject createAccPanel;
    public InputField newIdInputField;
    public InputField newPassInputField;
    public InputField newNickInputField;
    public Button createAccountBtn;
    public Button cancelBtn;

    string svIdStr = "";
    string svNewIdStr = "";
    string svNewPwStr = "";

    public static TitleManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (loginBtn != null)
            loginBtn.onClick.AddListener(GameStart);

        if (createAccOpenBtn != null)
            createAccOpenBtn.onClick.AddListener(OpenCreateAccBtn);

        //--- CreateAccountPanel
        if (cancelBtn != null)
            cancelBtn.onClick.AddListener(CreateCancelBtn);

        if (createAccountBtn != null)
            createAccountBtn.onClick.AddListener(CreateAccountBtn);


        //string a_strId = PlayerPrefs.GetString("MySave_Id", "");
        //if (PlayerPrefs.HasKey("MySave_Id") == false || a_strId == "")
        //{
        //    saveIdToggle.isOn = false;
        //}
        //else
        //{
        //    saveIdToggle.isOn = true;
        //    idInputField.text = a_strId;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GameStart()
    {
        NetworkManager.Inst.PushPacket(PacketType.Login, idInputField.text, passInputField.text);
    }

    private void OpenCreateAccBtn()
    {
        if (loginPanel != null)
            loginPanel.SetActive(false);

        if (createAccPanel != null)
            createAccPanel.SetActive(true);
    }

    private void CreateCancelBtn()
    {
        if (loginPanel != null)
            loginPanel.SetActive(true);

        if (createAccPanel != null)
            createAccPanel.SetActive(false);

        newIdInputField.text = "";
        newPassInputField.text = "";
        newNickInputField.text = "";
    }

    private void CreateAccountBtn()
    {
        NetworkManager.Inst.PushPacket(PacketType.CreateAccount, newIdInputField.text, 
            newPassInputField.text,
            newNickInputField.text);
    }
}
