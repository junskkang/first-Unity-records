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

    bool invalidEmailType = false; // 이메일 포맷이 올바른지 체크
    bool isValidFormat = false; // 올바른 형식인지 아닌지 체크

    bool isNetworkLock = false; 


    string saveID;
    string savePw;
    string saveNick;

    //탭으로 입력필드 이동
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
        Sound_Mgr.Inst.m_AudioSrc.clip = null;  //배경음 플레이 끄기

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
                ShowAvatarUrl = true,       //AvatarURL을 가져오기 위한 설정
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
        ShowMessage("로그인 성공");

        GlobalValue.g_Unique_ID = result.PlayFabId;

        

        if (result.InfoResultPayload != null)
        {
            GlobalValue.g_NickName = result.InfoResultPayload.PlayerProfile.DisplayName;
            //닉네임 가져오기

            //경험치 가져오기
            string a_AvatarURL = result.InfoResultPayload.PlayerProfile.AvatarUrl;
            //Json파싱
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
            //플레이어 데이터(타이틀) 값 받아오기
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
                    string[] strArr = eachData.Key.Split('_');      // _를 기준으로 나누어 저장
                    if (3 <= strArr.Length)         //Skill_Item_1 이런식으로 되어있기에 3덩어리로 나올것
                    {
                        //2번째 인덱스에 있는 것은 아이템 넘버인데 트라이파세가 실패했다? 비정상          
                        if (int.TryParse(strArr[2], out Idx) == false)  
                            //TryParse() 실패했을 경우 out 변수의 값은 0이 저장됨
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
                        ShowMessage("아이템 정보 파싱 실패");
                        continue;
                    }

                    //Debug.Log("idx : " + Idx + ", getValue : " + getValue);
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
            ShowMessage("id, pw, nick 빈칸 없이 입력해 주세요.");
            return;
        }

        if (!(6 <= strID.Length && strID.Length <= 20))   //ID 6 ~ 20 범위가 아닐 경우
        {
            ShowMessage("id는 6글자 이상 20글자 이하로 작성해 주세요.");
            inputNewID.text = "";
            return;
        }
        if (!(8 <= strPw.Length && strPw.Length <= 14))   //Pw 8 ~ 14 범위가 아닐 경우
        {
            ShowMessage("비밀번호는 8글자 이상 14글자 이하로 작성해 주세요.");
            inputNewPw.text = "";
            return;
        }
        if (!(3 <= strNick.Length && strNick.Length <= 8))   //Nick 2 ~ 8 범위가 아닐 경우
        {
            ShowMessage("닉네임은 3글자 이상 8글자 이하로 작성해 주세요.");
            inputNewNick.text = "";
            return;
        }

        if (!CheckEmailAddress(strID))
        {
            ShowMessage("ID는 이메일형식으로 작성해 주세요.");
            inputNewID.text = "";
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
        //Debug.Log("가입성공");

    }

    void PresentItem()
    {
        Dictionary<string, string> itemList = new Dictionary<string, string>();
        for (int i = 0; i < GlobalValue.g_CurSkillCount.Count; i++)
        {
            //아직 로그인 하기 전이므로 바로 글로벌 변수에다가 넣어주기 보다는
            //네트워크 상에서만 넣어주고 
            //새로 가입한 아이디로 로그인할 때 웹 상 데이터를 불러오는 부분에서
            //글로벌 데이터로 받아오도록 하는 것이 낫다
            //GlobalValue.g_CurSkillCount[i] = 1;
            //itemList.Add($"Skill_Item_{i}", GlobalValue.g_CurSkillCount[i].ToString());

            itemList.Add($"Skill_Item_{i}", (1).ToString());
        }

        //<플레이어 데이터(타이틀)> 값 활용 코드
        var request = new UpdateUserDataRequest()
        {
            Data = itemList
        };

        isNetworkLock = true;

        PlayFabClientAPI.UpdateUserData(request,
            (result) =>
            {
                ShowMessage("가입성공! 취소 버튼을 누르고 로그인하세요.");
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
            ShowMessage("가입실패 :" + "이미 존재하는 이메일입니다.");
            //에러 리포트 스트링은 아래의 디버그를 찍어보아 어떠한 내용으로 뜨는지 체크하고
            //그에 맞게 분기처리를 해주어 자세하게 설명해주도록 하자
            inputNewID.text = "";
        }
        else if (error.GenerateErrorReport().Contains("The display name entered is not available"))
        {
            ShowMessage("가입실패 :" + "이미 존재하는 닉네임입니다.");
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
