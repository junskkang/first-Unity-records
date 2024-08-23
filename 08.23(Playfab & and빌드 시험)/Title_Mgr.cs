using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class Title_Mgr : MonoBehaviour
{
    public Button StartBtn;

    //8.23 �ɷ´��� �� ���� ���� �߰�
    [Header("LoginPanel")]
    public GameObject loginPanel;
    public InputField idInputField;     //Email �� ���� ����
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

    [Header("Normal")]
    public Text messageText;
    float showMsTimer = 0.0f;

    [Header("FacebookLogin")]
    public Button faceboookLoginBtn;


    bool invalidEmailType = false;       // �̸��� ������ �ùٸ��� üũ
    bool isValidFormat = false;          // �ùٸ� �������� �ƴ��� üũ

    string svIdStr = "";
    string svNewIdStr = "";
    string svNewPwStr = "";

    // Start is called before the first frame update
    void Start()
    {
        //GlobalUserData.LoadGameInfo();

        SoundMgr.Inst.PlayBGM("sound_bgm_title_001", 0.2f);


        //--- LoginPanel
        if (loginBtn != null)
            loginBtn.onClick.AddListener(LoginBtn);

        if (createAccOpenBtn != null)
            createAccOpenBtn.onClick.AddListener(OpenCreateAccBtn);

        //--- CreateAccountPanel
        if (cancelBtn != null)
            cancelBtn.onClick.AddListener(CreateCancelBtn);

        if (createAccountBtn != null)
            createAccountBtn.onClick.AddListener(CreateAccountBtn);


        string a_strId = PlayerPrefs.GetString("MySave_Id", "");
        if (PlayerPrefs.HasKey("MySave_Id") == false || a_strId == "")
        {
            saveIdToggle.isOn = false;
        }
        else
        {
            saveIdToggle.isOn = true;
            idInputField.text = a_strId;
        }

#if FacebookLogin
        if (faceboookLoginBtn != null)
            faceboookLoginBtn.onClick.AddListener(LoginBtn);
#else
        if (faceboookLoginBtn != null)
            faceboookLoginBtn.gameObject.SetActive(false);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < showMsTimer)
        {
            showMsTimer -= Time.deltaTime;
            if (showMsTimer <= 0.0f)
            {
                MessageOnOff("", false);
            }
        }
    }

    void StartClick()
    {
        //Debug.Log("��ư�� Ŭ�� �߾��.");
        SceneManager.LoadScene("LobbyScene");

        SoundMgr.Inst.PlayGUISound("Pop", 0.4f);
    }

    void LoginBtn()
    {
        SoundMgr.Inst.PlayGUISound("Pop", 0.4f);

        string a_IdStr = idInputField.text.Trim();
        string a_PwStr = passInputField.text.Trim();

        if (string.IsNullOrEmpty(a_IdStr) == true ||
           string.IsNullOrEmpty(a_PwStr) == true)
        {
            MessageOnOff("Id, Pw ��ĭ ���� �Է��� �ּ���.");
        }

        if (!(6 <= a_IdStr.Length && a_IdStr.Length <= 20))  // 6 ~ 20
        {
            MessageOnOff("Id�� 6���ں��� 20���ڱ��� �ۼ��� �ּ���.");
            return;
        }

        if (!(6 <= a_PwStr.Length && a_PwStr.Length <= 20))
        {
            MessageOnOff("��й�ȣ�� 6���ں��� 20���ڱ��� �ۼ��� �ּ���.");
            return;
        }

        if (!CheckEmailAddress(a_IdStr))
        {
            MessageOnOff("Email ������ ���� �ʽ��ϴ�.");
            return;
        }

        //--- �α��� ������ � ���� ������ ���������� �����ϴ� �ɼ� ��ü ����
        var option = new GetPlayerCombinedInfoRequestParams()
        {
            //--- DisplayName(�г���)�� �������� ���� �ɼ�
            GetPlayerProfile = true,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,  //DisplayName(�г���) �������� ���� ��û �ɼ�
                ShowAvatarUrl = true     //�ƹ� URL�� �������� �ɼ�
            },
            //--- DisplayName(�г���)�� �������� ���� �ɼ�

            GetPlayerStatistics = true,
            //--- �� �ɼ����� ��谪(����ǥ�� �����ϴ�)�� �ҷ��� �� �ִ�.
            GetUserData = true
            //--- �� �ɼ����� < �÷��̾� ������(Ÿ��Ʋ) > ���� �ҷ��� �� �ִ�.
        };

        svIdStr = a_IdStr;    //�α��� ���̵� ����

        var request = new LoginWithEmailAddressRequest()
        {
            Email = a_IdStr,
            Password = a_PwStr,
            InfoRequestParameters = option
        };

        PlayFabClientAPI.LoginWithEmailAddress(request,
                                    OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        MessageOnOff("�α��� ����");

        GlobalUserData.g_Unique_ID = result.PlayFabId;

        if (result.InfoResultPayload != null)
        {
            //�г��� ��������
            GlobalUserData.g_NickName = result.InfoResultPayload.PlayerProfile.DisplayName;
            

            //--- < �÷��̾� ������(Ÿ��Ʋ) > �� �޾ƿ���
            int a_GetValue = 0;
            foreach (var eachData in result.InfoResultPayload.UserData)
            {
                if (eachData.Key == "UserGold")
                {
                    if (int.TryParse(eachData.Value.Value, out a_GetValue) == true)
                        GlobalUserData.g_UserGold = a_GetValue;
                }
                else if (eachData.Key == "BombCount")
                {
                    if (int.TryParse(eachData.Value.Value, out a_GetValue) == true)
                        GlobalUserData.g_BombCount = a_GetValue;
                }
            }//foreach( var eachData in result.InfoResultPayload.UserData)
            //--- < �÷��̾� ������(Ÿ��Ʋ) >�� �޾ƿ���

        }//if (result.InfoResultPayload != null)


        //�α��� �����ÿ� ...
        if (saveIdToggle.isOn == true)  //üũ ��ư�� ���� ������
        {
            PlayerPrefs.SetString("MySave_Id", svIdStr);
        }
        else  //üũ ��ư�� ���� ������
        {
            PlayerPrefs.DeleteKey("MySave_Id");
        }

        //Debug.Log("��ư�� Ŭ�� �߾��.");
        //bool IsFadeOk = false;
        //if (Fade_Mgr.Inst != null)
        //    IsFadeOk = Fade_Mgr.Inst.SceneOutReserve("LobbyScene");
        //if (IsFadeOk == false)
        SceneManager.LoadScene("LobbyScene");
    }

    void OnLoginFailure(PlayFabError error)
    {
        if (error.GenerateErrorReport().Contains("User not found") == true)
        {
            MessageOnOff("�α��� ���� : �ش� Id�� �������� �ʽ��ϴ�.");
        }
        else if (error.GenerateErrorReport().Contains("Invalid email address or password") == true)
        {
            MessageOnOff("�α��� ���� : �н����尡 ��ġ���� �ʽ��ϴ�.");
        }
        else
        {
            MessageOnOff("�α��� ���� : " + error.GenerateErrorReport());
        }
    }

    void OpenCreateAccBtn()
    {
        SoundMgr.Inst.PlayGUISound("Pop", 0.4f);

        if (loginPanel != null)
            loginPanel.SetActive(false);

        if (createAccPanel != null)
            createAccPanel.SetActive(true);
    }

    void CreateCancelBtn()
    {
        SoundMgr.Inst.PlayGUISound("Pop", 0.4f);

        if (loginPanel != null)
            loginPanel.SetActive(true);

        if (createAccPanel != null)
            createAccPanel.SetActive(false);

        newIdInputField.text = "";
        newPassInputField.text = "";
        newNickInputField.text = "";
    }

    void CreateAccountBtn() //���� ���� ��û �Լ�
    {
        SoundMgr.Inst.PlayGUISound("Pop", 0.4f);

        string a_IdStr = newIdInputField.text;
        string a_PwStr = newPassInputField.text;
        string a_NickStr = newNickInputField.text;

        a_IdStr = a_IdStr.Trim();
        a_PwStr = a_PwStr.Trim();
        a_NickStr = a_NickStr.Trim();

        if (string.IsNullOrEmpty(a_IdStr) == true ||
           string.IsNullOrEmpty(a_PwStr) == true ||
           string.IsNullOrEmpty(a_NickStr) == true)
        {
            MessageOnOff("Id, Pw, ������ ��ĭ ���� �Է��� �ּ���.");
            return;
        }

        if (!(6 <= a_IdStr.Length && a_IdStr.Length <= 20))  // 6 ~ 20
        {
            MessageOnOff("Id�� 6���ں��� 20���ڱ��� �ۼ��� �ּ���.");
            return;
        }

        if (!(6 <= a_PwStr.Length && a_PwStr.Length <= 20))
        {
            MessageOnOff("��й�ȣ�� 6���ں��� 20���ڱ��� �ۼ��� �ּ���.");
            return;
        }

        if (!(3 <= a_NickStr.Length && a_NickStr.Length <= 20))
        {
            MessageOnOff("������ 3���ں��� 20���ڱ��� �ۼ��� �ּ���.");
            return;
        }

        if (!CheckEmailAddress(a_IdStr))
        {
            MessageOnOff("Email ������ ���� �ʽ��ϴ�.");
            return;
        }

        svNewIdStr = a_IdStr;
        svNewPwStr = a_PwStr;

        var request = new RegisterPlayFabUserRequest()
        {
            Email = a_IdStr,
            Password = a_PwStr,
            DisplayName = a_NickStr,

            RequireBothUsernameAndEmail = false //Email �� �⺻ Id�� ����ϰڴٴ� �ɼ�
        };

        MessageOnOff("ȸ�� ���� ��... ��ø� ��ٷ� �ּ���.");
        showMsTimer = 300.0f;

        PlayFabClientAPI.RegisterPlayFabUser(request,
            (result)=>
            {
                MessageOnOff("���� ����! �ڷΰ��� ��ư�� ������ �α��� ���ּ���.");
                idInputField.text = svNewIdStr;
                passInputField.text = svNewPwStr;
            }, 
            RegisterFailure);
    }

    void RegisterFailure(PlayFabError error)
    {
        if (error.GenerateErrorReport().Contains("Email address already exists") == true)
        {
            MessageOnOff("���� ���� : " + "�̹� �����ϴ� Id �Դϴ�.");
        }
        else if (error.GenerateErrorReport().Contains("The display name entered is not available") == true)
        {
            MessageOnOff("���� ���� : " + "�̹� �����ϴ� ���� �Դϴ�.");
        }
        else
        {
            MessageOnOff("���� ���� : " + error.GenerateErrorReport());
        }
    }

    void MessageOnOff(string Mess = "", bool isOn = true)
    {
        if (isOn == true)
        {
            messageText.text = Mess;
            messageText.gameObject.SetActive(true);
            showMsTimer = 7.0f;
        }
        else
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
    }

    //----------------- �̸��������� �´��� Ȯ���ϴ� ��� ��ũ��Ʈ
    //https://blog.naver.com/rlawndks4204/221591566567
    // <summary>
    /// �ùٸ� �̸������� üũ.
    /// </summary>
    private bool CheckEmailAddress(string EmailStr)
    {
        if (string.IsNullOrEmpty(EmailStr)) isValidFormat = false;

        EmailStr = Regex.Replace(EmailStr, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
        if (invalidEmailType) isValidFormat = false;

        // true �� ��ȯ�� ��, �ùٸ� �̸��� ������.
        isValidFormat = Regex.IsMatch(EmailStr,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase);
        return isValidFormat;
    }

    /// <summary>
    /// ���������� ��������.
    /// </summary>
    /// <param name="match"></param>
    /// <returns></returns>
    private string DomainMapper(Match match)
    {
        // IdnMapping class with default property values.
        IdnMapping idn = new IdnMapping();

        string domainName = match.Groups[2].Value;
        try
        {
            domainName = idn.GetAscii(domainName);
        }
        catch (ArgumentException)
        {
            invalidEmailType = true;
        }
        return match.Groups[1].Value + domainName;
    }
    //----------------- �̸��������� �´��� Ȯ���ϴ� ��� ��ũ��Ʈ
}
