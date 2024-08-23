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

    //8.23 능력단위 평가 관련 변수 추가
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

    [Header("Normal")]
    public Text messageText;
    float showMsTimer = 0.0f;

    [Header("FacebookLogin")]
    public Button faceboookLoginBtn;


    bool invalidEmailType = false;       // 이메일 포맷이 올바른지 체크
    bool isValidFormat = false;          // 올바른 형식인지 아닌지 체크

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
        //Debug.Log("버튼을 클릭 했어요.");
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
            MessageOnOff("Id, Pw 빈칸 없이 입력해 주세요.");
        }

        if (!(6 <= a_IdStr.Length && a_IdStr.Length <= 20))  // 6 ~ 20
        {
            MessageOnOff("Id는 6글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!(6 <= a_PwStr.Length && a_PwStr.Length <= 20))
        {
            MessageOnOff("비밀번호는 6글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!CheckEmailAddress(a_IdStr))
        {
            MessageOnOff("Email 형식이 맞지 않습니다.");
            return;
        }

        //--- 로그인 성공시 어떤 유저 정보를 가져올지를 설정하는 옵션 객체 생성
        var option = new GetPlayerCombinedInfoRequestParams()
        {
            //--- DisplayName(닉네임)을 가져오기 위한 옵션
            GetPlayerProfile = true,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,  //DisplayName(닉네임) 가져오기 위한 요청 옵션
                ShowAvatarUrl = true     //아바 URL을 가져오는 옵션
            },
            //--- DisplayName(닉네임)을 가져오기 위한 옵션

            GetPlayerStatistics = true,
            //--- 이 옵션으로 통계값(순위표에 관여하는)을 불러올 수 있다.
            GetUserData = true
            //--- 이 옵션으로 < 플레이어 테이터(타이틀) > 값을 불러올 수 있다.
        };

        svIdStr = a_IdStr;    //로그인 아이디 저장

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
        MessageOnOff("로그인 성공");

        GlobalUserData.g_Unique_ID = result.PlayFabId;

        if (result.InfoResultPayload != null)
        {
            //닉네임 가져오기
            GlobalUserData.g_NickName = result.InfoResultPayload.PlayerProfile.DisplayName;
            

            //--- < 플레이어 데이터(타이틀) > 값 받아오기
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
            //--- < 플레이어 데이터(타이틀) >값 받아오기

        }//if (result.InfoResultPayload != null)


        //로그인 성공시에 ...
        if (saveIdToggle.isOn == true)  //체크 버튼이 켜져 있으면
        {
            PlayerPrefs.SetString("MySave_Id", svIdStr);
        }
        else  //체크 버튼이 꺼져 있으면
        {
            PlayerPrefs.DeleteKey("MySave_Id");
        }

        //Debug.Log("버튼을 클릭 했어요.");
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
            MessageOnOff("로그인 실패 : 해당 Id가 존재하지 않습니다.");
        }
        else if (error.GenerateErrorReport().Contains("Invalid email address or password") == true)
        {
            MessageOnOff("로그인 실패 : 패스워드가 일치하지 않습니다.");
        }
        else
        {
            MessageOnOff("로그인 실패 : " + error.GenerateErrorReport());
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

    void CreateAccountBtn() //계정 생성 요청 함수
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
            MessageOnOff("Id, Pw, 별명은 빈칸 없이 입력해 주세요.");
            return;
        }

        if (!(6 <= a_IdStr.Length && a_IdStr.Length <= 20))  // 6 ~ 20
        {
            MessageOnOff("Id는 6글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!(6 <= a_PwStr.Length && a_PwStr.Length <= 20))
        {
            MessageOnOff("비밀번호는 6글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!(3 <= a_NickStr.Length && a_NickStr.Length <= 20))
        {
            MessageOnOff("별명은 3글자부터 20글자까지 작성해 주세요.");
            return;
        }

        if (!CheckEmailAddress(a_IdStr))
        {
            MessageOnOff("Email 형식이 맞지 않습니다.");
            return;
        }

        svNewIdStr = a_IdStr;
        svNewPwStr = a_PwStr;

        var request = new RegisterPlayFabUserRequest()
        {
            Email = a_IdStr,
            Password = a_PwStr,
            DisplayName = a_NickStr,

            RequireBothUsernameAndEmail = false //Email 을 기본 Id로 사용하겠다는 옵션
        };

        MessageOnOff("회원 가입 중... 잠시만 기다려 주세요.");
        showMsTimer = 300.0f;

        PlayFabClientAPI.RegisterPlayFabUser(request,
            (result)=>
            {
                MessageOnOff("가입 성공! 뒤로가기 버튼을 누르고 로그인 해주세요.");
                idInputField.text = svNewIdStr;
                passInputField.text = svNewPwStr;
            }, 
            RegisterFailure);
    }

    void RegisterFailure(PlayFabError error)
    {
        if (error.GenerateErrorReport().Contains("Email address already exists") == true)
        {
            MessageOnOff("가입 실패 : " + "이미 존재하는 Id 입니다.");
        }
        else if (error.GenerateErrorReport().Contains("The display name entered is not available") == true)
        {
            MessageOnOff("가입 실패 : " + "이미 존재하는 별명 입니다.");
        }
        else
        {
            MessageOnOff("가입 실패 : " + error.GenerateErrorReport());
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

    //----------------- 이메일형식이 맞는지 확인하는 방법 스크립트
    //https://blog.naver.com/rlawndks4204/221591566567
    // <summary>
    /// 올바른 이메일인지 체크.
    /// </summary>
    private bool CheckEmailAddress(string EmailStr)
    {
        if (string.IsNullOrEmpty(EmailStr)) isValidFormat = false;

        EmailStr = Regex.Replace(EmailStr, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
        if (invalidEmailType) isValidFormat = false;

        // true 로 반환할 시, 올바른 이메일 포맷임.
        isValidFormat = Regex.IsMatch(EmailStr,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase);
        return isValidFormat;
    }

    /// <summary>
    /// 도메인으로 변경해줌.
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
    //----------------- 이메일형식이 맞는지 확인하는 방법 스크립트
}
