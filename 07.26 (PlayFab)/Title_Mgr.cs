using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//이메일 형식이 맞는지 확인하는 방법을 위한 네임스페이스 추가
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

    bool invalidEmailType = false; // 이메일 포맷이 올바른지 체크
    bool isValidFormat = false; // 올바른 형식인지 아닌지 체크


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
        Sound_Mgr.Inst.m_AudioSrc.clip = null;  //배경음 플레이 끄기
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
        //Debug.Log("버튼을 클릭 했어요.");
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
            ShowMessage("id, pw를 빈칸 없이 입력해 주세요.");
            return;
        }

        if (!(6 <= strID.Length && strID.Length <= 20))   //ID 6 ~ 20 범위가 아닐 경우
        {
            ShowMessage("id는 6글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }
        if (!(8 <= strPw.Length && strPw.Length <= 14))   //Pw 8 ~ 14 범위가 아닐 경우
        {
            ShowMessage("비밀번호는 8글자 이상 14글자 이하로 작성해 주세요.");
            return;
        }

        if (!CheckEmailAddress(strID))
        {
            ShowMessage("ID는 이메일형식으로 작성해 주세요.");
            return;
        }

        //로그인 성공시 어떤 유저 정보를 가져올지를 설정하는 옵션 객체 생성
        //요청한 것만 넘겨줌
        var option = new GetPlayerCombinedInfoRequestParams()
        {
            //DisplayName(닉네임)을 가져오기 위한 옵션
            GetPlayerProfile = true,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,     //DisplayName(닉네임)
            },

            GetUserData = true,  //플레이어 데이터(타이틀)에 저장한 옵션을 가져오기 위한 설정

            GetPlayerStatistics = true  //플레이어 통계에 저장한 옵션을 가져오기 위한 설정
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
        ShowMessage("로그인 성공");

        GlobalValue.g_Unique_ID = result.PlayFabId;

        Debug.Log(GlobalValue.g_Unique_ID);

        if (result.InfoResultPayload != null)
        {
            GlobalValue.g_NickName = result.InfoResultPayload.PlayerProfile.DisplayName;
            //닉네임 가져오기

            //플레이어 데이터(타이틀) 값 받아오기
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
                    string[] strArr = eachData.Key.Split('_');      // _를 기준으로 나누어 저장
                    if (3 <= strArr.Length)         //Skill_Item_1 이런식으로 되어있기에 3덩어리로 나올것
                    {
                        //2번째 인덱스에 있는 것은 아이템 넘버인데 트라이파세가 실패했다? 비정상          
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
                        ShowMessage("아이템 정보 파싱 실패");
                        continue;
                    }

                    Debug.Log("idx : " + Idx + ", getValue : " + getValue);
                    GlobalValue.g_CurSkillCount[Idx] = getValue;

                    //GlobalValue.g_CurSkillCount[Idx] = getValue;
                }
            }

            //통계값 가져오기
            //옵션 설정에 의해 LoginWithEmailAdress()만으로도 유저 통계값(순위)를 불러올 수 있다.
            foreach (var eachStat in result.InfoResultPayload.PlayerStatistics)
            {
                if (eachStat.StatisticName == "BestScore")
                {
                    GlobalValue.g_BestScore = eachStat.Value;
                }
            }
        }

        //Debug.Log("버튼을 클릭 했어요.");
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
            ShowMessage("로그인 실패 : 해당 ID가 존재하지 않습니다.");
        }
        else if (error.GenerateErrorReport().Contains("Invalid email address or password"))
        {
            ShowMessage("로그인 실패 : 패스워드가 일치하지 않습니다.");
        }
        else
        {
            ShowMessage("로그인 실패 :" + error.GenerateErrorReport());
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
            ShowMessage("id, pw, nick 빈칸 없이 입력해 주세요.");
            return;
        }

        if (!(6 <= strID.Length && strID.Length <= 20))   //ID 6 ~ 20 범위가 아닐 경우
        {
            ShowMessage("id는 6글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }
        if (!(8 <= strPw.Length && strPw.Length <= 14))   //Pw 8 ~ 14 범위가 아닐 경우
        {
            ShowMessage("비밀번호는 8글자 이상 14글자 이하로 작성해 주세요.");
            return;
        }
        if (!(3 <= strNick.Length && strNick.Length <= 8))   //Nick 2 ~ 8 범위가 아닐 경우
        {
            ShowMessage("닉네임은 3글자 이상 8글자 이하로 작성해 주세요.");
            return;
        }

        if (!CheckEmailAddress(strID))
        {
            ShowMessage("ID는 이메일형식으로 작성해 주세요.");
            return;
        }

        //if (!isPassCheck(strPw))
        //{
        //    Debug.Log("비밀번호는 영문1자, 숫자1자, 특수문자1자를 반드시 포함해 주세요.");
        //    return;
        //}

        var request = new RegisterPlayFabUserRequest()
        {
            Email = strID,
            Password = strPw,
            DisplayName = strNick,

            RequireBothUsernameAndEmail = false //이메일을 기본 ID로 사용하겠다는 옵션
        };

        //위에 방식을 풀어서 쓰면 아래와 같다.
        //var test = new RegisterPlayFabUserRequest();
        //test.Email = strID;
        //test.Password = strPw;
        //test.DisplayName = strNick;
        //test.RequireBothUsernameAndEmail = false;

        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFailure);
    }

    private void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        //Debug.Log("가입성공");
        ShowMessage("가입성공");
    }

    private void RegisterFailure(PlayFabError error)
    {
        if (error.GenerateErrorReport().Contains("Email address already exists"))
        {
            ShowMessage("가입실패 :" + "이미 존재하는 이메일입니다.");
            //에러 리포트 스트링은 아래의 디버그를 찍어보아 어떠한 내용으로 뜨는지 체크하고
            //그에 맞게 분기처리를 해주어 자세하게 설명해주도록 하자            
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



    //길이 10자 이상, 숫자1이상, 영문1이상, 특수문자1이상
    public bool isPassCheck(string pass)
    {
        //10자리 이상
        if (pass != null && pass.Length < 10) return false;

        //숫자1이상, 영문1이상, 특수문자1이상
        Regex regexPass = new Regex(@"^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{9,}$", RegexOptions.IgnorePatternWhitespace);
        return regexPass.IsMatch(pass);
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
