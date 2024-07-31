using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//�̸��� ������ �´��� Ȯ���ϴ� ����� ���� ���ӽ����̽� �߰�
using System.Globalization; 
using System.Text.RegularExpressions;
using PlayFab.ClientModels;
using PlayFab;

public class Title_Mgr : MonoBehaviour
{
    public Button StartBtn;

    [Header("LoginPanel")]
    public GameObject loginPanel;
    public InputField inputID;  //E-mail
    public InputField inputPw;
    public Button loginBtn;
    public Button createAccOpenBtn;

    [Header("CreateAccountPanel")]
    public GameObject createAccPanel;
    public InputField inputNewID;
    public InputField inputNewPw;
    public InputField inputNewNick;
    public Button createAccountBtn;
    public Button cancleBtn;

    [Header("Normal")]
    public Text messageText;
    float showMsgTimer = 0.0f;

    bool invalidEmailType = false; // �̸��� ������ �ùٸ��� üũ
    bool isValidFormat = false; // �ùٸ� �������� �ƴ��� üũ


    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameData();
        StartBtn.onClick.AddListener(StartClick);

        //Login Account
        if (loginBtn != null)
            loginBtn.onClick.AddListener(LoginClick);

        if (createAccOpenBtn != null)
            createAccOpenBtn.onClick.AddListener(OpenCreateAccount);          
        
        //Create Account 
        if (cancleBtn != null)
            cancleBtn.onClick.AddListener(CancelClick);

        if (createAccountBtn != null)
            createAccountBtn.onClick.AddListener(CreateAccount);

        Sound_Mgr.Inst.PlayBGM("sound_bgm_title_001", 1.0f);
        Sound_Mgr.Inst.m_AudioSrc.clip = null;  //����� �÷��� ����
    }


    // Update is called once per frame
    void Update()
    {
        if (showMsgTimer  > 0.0f)
            showMsgTimer -= Time.deltaTime;

        if (showMsgTimer <= 0.0f)
        {
            showMsgTimer = 0.0f;

            messageText.gameObject.SetActive(false);
        }
    }

    void StartClick()
    {
        //Debug.Log("��ư�� Ŭ�� �߾��.");
        bool IsFadeOk = false;
        if (Fade_Mgr.Inst != null)
            IsFadeOk = Fade_Mgr.Inst.SceneOutReserve("LobbyScene");
        if(IsFadeOk == false)
            SceneManager.LoadScene("LobbyScene");

        Sound_Mgr.Inst.PlayGUISound("Pop", 1.0f);
    }

    private void LoginClick()
    {
        string strID = (inputID.text).Trim();
        string strPw = (inputPw.text).Trim();

        if (string.IsNullOrEmpty(strID) ||
           string.IsNullOrEmpty(strPw))
        {
            ShowMessage("id, pw�� ��ĭ ���� �Է��� �ּ���.");
            return;
        }

        if (!(6 <= strID.Length && strID.Length <= 20))   //ID 6 ~ 20 ������ �ƴ� ���
        {
            ShowMessage("id�� 6���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }
        if (!(8 <= strPw.Length && strPw.Length <= 14))   //Pw 8 ~ 14 ������ �ƴ� ���
        {
            ShowMessage("��й�ȣ�� 8���� �̻� 14���� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }

        if (!CheckEmailAddress(strID))
        {
            ShowMessage("ID�� �̸����������� �ۼ��� �ּ���.");
            return;
        }

        //�α��� ������ � ���� ������ ���������� �����ϴ� �ɼ� ��ü ����
        //��û�� �͸� �Ѱ���
        var option = new GetPlayerCombinedInfoRequestParams()
        {
            //DisplayName(�г���)�� �������� ���� �ɼ�
            GetPlayerProfile = true,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,     //DisplayName(�г���)
            },

            GetUserData = true,  //�÷��̾� ������(Ÿ��Ʋ)�� ������ �ɼ��� �������� ���� ����

            GetPlayerStatistics = true  //�÷��̾� ��迡 ������ �ɼ��� �������� ���� ����
        };

        var request = new LoginWithEmailAddressRequest()
        {
            Email = strID,
            Password = strPw,
            InfoRequestParameters = option
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        ShowMessage("�α��� ����");

        GlobalValue.g_Unique_ID = result.PlayFabId;

        Debug.Log(GlobalValue.g_Unique_ID);

        if (result.InfoResultPayload != null)
        {
            GlobalValue.g_NickName = result.InfoResultPayload.PlayerProfile.DisplayName;
            //�г��� ��������

            //�÷��̾� ������(Ÿ��Ʋ) �� �޾ƿ���
            int getValue = 0;
            int Idx = 0;
            foreach(var eachData in result.InfoResultPayload.UserData)
            {
                if (eachData.Key == "UserGold")
                {
                    if (int.TryParse(eachData.Value.Value, out getValue))
                        GlobalValue.g_UserGold = getValue;
                }
                else if (eachData.Key.Contains("Skill_Item_") == true)
                {
                    bool a_IsDifferent = false;
                    Idx = 0;
                    string[] strArr = eachData.Key.Split('_');      // _�� �������� ������ ����
                    if (3 <= strArr.Length)         //Skill_Item_1 �̷������� �Ǿ��ֱ⿡ 3����� ���ð�
                    {
                        //2��° �ε����� �ִ� ���� ������ �ѹ��ε� Ʈ�����ļ��� �����ߴ�? ������          
                        if (int.TryParse(strArr[2], out Idx) == false)
                            a_IsDifferent = true;
                    }
                    else
                        a_IsDifferent = true;

                    if (GlobalValue.g_CurSkillCount.Count <= Idx)
                        a_IsDifferent = true;

                    if (int.TryParse(eachData.Value.Value, out getValue) == false)
                        a_IsDifferent = true;

                    if (a_IsDifferent == true)
                    {
                        ShowMessage("������ ���� �Ľ� ����");
                        continue;
                    }

                    Debug.Log("idx : " + Idx + ", getValue : " + getValue);
                    GlobalValue.g_CurSkillCount[Idx] = getValue;

                    //GlobalValue.g_CurSkillCount[Idx] = getValue;
                }
            }

            //��谪 ��������
            //�ɼ� ������ ���� LoginWithEmailAdress()�����ε� ���� ��谪(����)�� �ҷ��� �� �ִ�.
            foreach (var eachStat in result.InfoResultPayload.PlayerStatistics)
            {
                if (eachStat.StatisticName == "BestScore")
                {
                    GlobalValue.g_BestScore = eachStat.Value;
                }
            }
        }

        //Debug.Log("��ư�� Ŭ�� �߾��.");
        bool IsFadeOk = false;
        if (Fade_Mgr.Inst != null)
            IsFadeOk = Fade_Mgr.Inst.SceneOutReserve("LobbyScene");
        if (IsFadeOk == false)
            SceneManager.LoadScene("LobbyScene");

    }

    private void OnLoginFailure(PlayFabError error)
    {
        if (error.GenerateErrorReport().Contains("User not found"))
        {
            ShowMessage("�α��� ���� : �ش� ID�� �������� �ʽ��ϴ�.");
        }
        else if (error.GenerateErrorReport().Contains("Invalid email address or password"))
        {
            ShowMessage("�α��� ���� : �н����尡 ��ġ���� �ʽ��ϴ�.");
        }
        else
        {
            ShowMessage("�α��� ���� :" + error.GenerateErrorReport());
        }
    }

    private void OpenCreateAccount()
    {
        if (loginPanel != null)
            loginPanel.SetActive(false);

        if (createAccPanel != null)
            createAccPanel.SetActive(true);
    }

    private void CancelClick()
    {
        if (createAccPanel != null)
            createAccPanel.SetActive(false);

        if (loginPanel != null)
            loginPanel.SetActive(true);
    }

    private void CreateAccount()
    {
        string strID = (inputNewID.text).Trim();
        string strPw = (inputNewPw.text).Trim();
        string strNick = (inputNewNick.text).Trim();

        if (string.IsNullOrEmpty(strID) ||
            string.IsNullOrEmpty(strPw) ||
            string.IsNullOrEmpty(strNick))
        {
            ShowMessage("id, pw, nick ��ĭ ���� �Է��� �ּ���.");
            return;
        }

        if (!(6 <= strID.Length && strID.Length <= 20))   //ID 6 ~ 20 ������ �ƴ� ���
        {
            ShowMessage("id�� 6���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }
        if (!(8 <= strPw.Length && strPw.Length <= 14))   //Pw 8 ~ 14 ������ �ƴ� ���
        {
            ShowMessage("��й�ȣ�� 8���� �̻� 14���� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }
        if (!(3 <= strNick.Length && strNick.Length <= 8))   //Nick 2 ~ 8 ������ �ƴ� ���
        {
            ShowMessage("�г����� 3���� �̻� 8���� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }

        if (!CheckEmailAddress(strID))
        {
            ShowMessage("ID�� �̸����������� �ۼ��� �ּ���.");
            return;
        }

        //if (!isPassCheck(strPw))
        //{
        //    Debug.Log("��й�ȣ�� ����1��, ����1��, Ư������1�ڸ� �ݵ�� ������ �ּ���.");
        //    return;
        //}

        var request = new RegisterPlayFabUserRequest()
        {
            Email = strID,
            Password = strPw,
            DisplayName = strNick,

            RequireBothUsernameAndEmail = false //�̸����� �⺻ ID�� ����ϰڴٴ� �ɼ�
        };

        //���� ����� Ǯ� ���� �Ʒ��� ����.
        //var test = new RegisterPlayFabUserRequest();
        //test.Email = strID;
        //test.Password = strPw;
        //test.DisplayName = strNick;
        //test.RequireBothUsernameAndEmail = false;

        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFailure);
    }

    private void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        //Debug.Log("���Լ���");
        ShowMessage("���Լ���");
    }

    private void RegisterFailure(PlayFabError error)
    {
        if (error.GenerateErrorReport().Contains("Email address already exists"))
        {
            ShowMessage("���Խ��� :" + "�̹� �����ϴ� �̸����Դϴ�.");
            //���� ����Ʈ ��Ʈ���� �Ʒ��� ����׸� ���� ��� �������� �ߴ��� üũ�ϰ�
            //�׿� �°� �б�ó���� ���־� �ڼ��ϰ� �������ֵ��� ����            
        }
        else
        {
            Debug.Log(error.GenerateErrorReport());
        }        
    }

    void ShowMessage(string message = "")
    {
        showMsgTimer = 3.0f;

        if (messageText != null && message != null)
        {
            messageText.text = message;
        }
        
        messageText.gameObject.SetActive(true);
    }



    //���� 10�� �̻�, ����1�̻�, ����1�̻�, Ư������1�̻�
    public bool isPassCheck(string pass)
    {
        //10�ڸ� �̻�
        if (pass != null && pass.Length < 10) return false;

        //����1�̻�, ����1�̻�, Ư������1�̻�
        Regex regexPass = new Regex(@"^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{9,}$", RegexOptions.IgnorePatternWhitespace);
        return regexPass.IsMatch(pass);
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
