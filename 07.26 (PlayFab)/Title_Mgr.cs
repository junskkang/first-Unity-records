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
using UnityEngine.EventSystems;
using SimpleJSON;

public class Title_Mgr : MonoBehaviour
{
    public Button StartBtn;

    [Header("LoginPanel")]
    public GameObject loginPanel;
    public InputField inputID;  //E-mail
    public InputField inputPw;
    public Button loginBtn;
    public Button createAccOpenBtn;
    public Toggle saveIDToggle;

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

    bool isNetworkLock = false; 


    string saveID;
    string savePw;
    string saveNick;

    //������ �Է��ʵ� �̵�
    EventSystem system;
    public Selectable firstInput;
    public Button submitButton;

    bool updown = false;


    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameData();
        StartBtn.onClick.AddListener(StartClick);

        //Login Account
        if (loginBtn != null)
            loginBtn.onClick.AddListener(LoginClick);

        if (saveIDToggle != null)
            saveIDToggle.onValueChanged.AddListener(SaveIDInput);

        int saveIDOnOff = PlayerPrefs.GetInt("SaveIdCheck", 0);
        saveIDToggle.isOn = (saveIDOnOff == 1)? true : false;

        if (saveIDToggle.isOn == true && PlayerPrefs.GetString("LoginSaveID") != null)
        {
            inputID.text = PlayerPrefs.GetString("LoginSaveID", "");
        }
        else if(!saveIDToggle.isOn && PlayerPrefs.GetString("LoginSaveID") != null)
            PlayerPrefs.DeleteKey("LoginSaveID");

        if (createAccOpenBtn != null)
            createAccOpenBtn.onClick.AddListener(OpenCreateAccount);          
        
        //Create Account 
        if (cancleBtn != null)
            cancleBtn.onClick.AddListener(CancelClick);

        if (createAccountBtn != null)
            createAccountBtn.onClick.AddListener(CreateAccount);

        Sound_Mgr.Inst.PlayBGM("sound_bgm_title_001", 1.0f);
        Sound_Mgr.Inst.m_AudioSrc.clip = null;  //����� �÷��� ����

        system = EventSystem.current;

        firstInput.Select();
    }

    private void SaveIDInput(bool isOn)
    {
        //isSaveId = isOn;
        int saveBool = (isOn == true) ? 1: 0;
        PlayerPrefs.SetInt("SaveIdCheck", saveBool);

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

        if (Input.GetKeyDown(KeyCode.Tab) && loginPanel.gameObject.activeSelf)
        {
            updown = !updown;
            ChangeInput(updown);
        }
            
    }

    void ChangeInput(bool value)
    {
        if (value)
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (next != null)
            {
                next.Select();
            }
        }
        else if (!value)
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
            if (next != null)
            {
                next.Select();
            }
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
                ShowAvatarUrl = true,       //AvatarURL�� �������� ���� ����
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

        PlayFabClientAPI.LoginWithEmailAddress(request, 
            (result) => 
            {
                OnLoginSuccess(result);
                PlayerPrefs.SetString("LoginSaveID", strID);
            }
            , OnLoginFailure);
    }



    private void OnLoginSuccess(LoginResult result)
    {
        ShowMessage("�α��� ����");

        GlobalValue.g_Unique_ID = result.PlayFabId;

        

        if (result.InfoResultPayload != null)
        {
            GlobalValue.g_NickName = result.InfoResultPayload.PlayerProfile.DisplayName;
            //�г��� ��������

            //����ġ ��������
            string a_AvatarURL = result.InfoResultPayload.PlayerProfile.AvatarUrl;
            //Json�Ľ�
            if (string.IsNullOrEmpty(a_AvatarURL) == false &&
                a_AvatarURL.Contains("{\"") == true)
            {
                JSONNode parseJson = JSON.Parse(a_AvatarURL);
                if (parseJson["UserExp"] != null)
                {
                    GlobalValue.g_Exp = parseJson["UserExp"].AsInt;                    
                }
                if (parseJson["UserLevel"] != null)
                {
                    GlobalValue.g_Level = parseJson["UserLevel"].AsInt;
                }

                //Debug.Log(GlobalValue.g_Exp + " : " + GlobalValue.g_Level);

            }
            //�÷��̾� ������(Ÿ��Ʋ) �� �޾ƿ���
            int getValue = 0;
            int Idx = -1;   
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
                            //TryParse() �������� ��� out ������ ���� 0�� �����
                            a_IsDifferent = true;
                    }
                    else
                        a_IsDifferent = true;

                    if (Idx < 0 || GlobalValue.g_CurSkillCount.Count <= Idx)
                        a_IsDifferent = true;

                    if (int.TryParse(eachData.Value.Value, out getValue) == false)
                        a_IsDifferent = true;

                    if (a_IsDifferent == true)
                    {
                        ShowMessage("������ ���� �Ľ� ����");
                        continue;
                    }

                    //Debug.Log("idx : " + Idx + ", getValue : " + getValue);
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
        {
            inputNewID.text = "";
            inputNewPw.text = "";
            inputNewNick.text = "";
            createAccPanel.SetActive(false);
        }
            

        if (loginPanel != null)
            loginPanel.SetActive(true);

        if (saveID != null && savePw != null)
        {
            inputID.text = saveID.ToString();
            inputPw.text = savePw.ToString();
        }

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
            inputNewID.text = "";
            return;
        }
        if (!(8 <= strPw.Length && strPw.Length <= 14))   //Pw 8 ~ 14 ������ �ƴ� ���
        {
            ShowMessage("��й�ȣ�� 8���� �̻� 14���� ���Ϸ� �ۼ��� �ּ���.");
            inputNewPw.text = "";
            return;
        }
        if (!(3 <= strNick.Length && strNick.Length <= 8))   //Nick 2 ~ 8 ������ �ƴ� ���
        {
            ShowMessage("�г����� 3���� �̻� 8���� ���Ϸ� �ۼ��� �ּ���.");
            inputNewNick.text = "";
            return;
        }

        if (!CheckEmailAddress(strID))
        {
            ShowMessage("ID�� �̸����������� �ۼ��� �ּ���.");
            inputNewID.text = "";
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

        PlayFabClientAPI.RegisterPlayFabUser(request, 
            (result)=>
            {                
                PresentItem();
                saveID = strID;
                savePw = strPw;
                saveNick = strNick;
            }, 
            RegisterFailure);
            }

    private void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        //Debug.Log("���Լ���");

    }

    void PresentItem()
    {
        Dictionary<string, string> itemList = new Dictionary<string, string>();
        for (int i = 0; i < GlobalValue.g_CurSkillCount.Count; i++)
        {
            //���� �α��� �ϱ� ���̹Ƿ� �ٷ� �۷ι� �������ٰ� �־��ֱ� ���ٴ�
            //��Ʈ��ũ �󿡼��� �־��ְ� 
            //���� ������ ���̵�� �α����� �� �� �� �����͸� �ҷ����� �κп���
            //�۷ι� �����ͷ� �޾ƿ����� �ϴ� ���� ����
            //GlobalValue.g_CurSkillCount[i] = 1;
            //itemList.Add($"Skill_Item_{i}", GlobalValue.g_CurSkillCount[i].ToString());

            itemList.Add($"Skill_Item_{i}", (1).ToString());
        }

        //<�÷��̾� ������(Ÿ��Ʋ)> �� Ȱ�� �ڵ�
        var request = new UpdateUserDataRequest()
        {
            Data = itemList
        };

        isNetworkLock = true;

        PlayFabClientAPI.UpdateUserData(request,
            (result) =>
            {
                ShowMessage("���Լ���! ��� ��ư�� ������ �α����ϼ���.");
                isNetworkLock = false;
            },
            (error) =>
            {
                isNetworkLock = false;
            }
            );
    }

    private void RegisterFailure(PlayFabError error)
    {
        if (error.GenerateErrorReport().Contains("Email address already exists"))
        {
            ShowMessage("���Խ��� :" + "�̹� �����ϴ� �̸����Դϴ�.");
            //���� ����Ʈ ��Ʈ���� �Ʒ��� ����׸� ���� ��� �������� �ߴ��� üũ�ϰ�
            //�׿� �°� �б�ó���� ���־� �ڼ��ϰ� �������ֵ��� ����
            inputNewID.text = "";
        }
        else if (error.GenerateErrorReport().Contains("The display name entered is not available"))
        {
            ShowMessage("���Խ��� :" + "�̹� �����ϴ� �г����Դϴ�.");
            inputNewNick.text = "";
        }
        else
        {
            Debug.Log(error.GenerateErrorReport());
            inputNewID.text = "";
            inputNewPw.text = "";
            inputNewNick.text = "";
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
