using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title_Mgr : MonoBehaviour
{
    [Header("--- LoginPanel ---")]
    public GameObject m_LoginPanelObj;
    public Button m_LoginBtn = null;
    public Button m_CreateAccOpenBtn = null;
    public InputField IdInputField;
    public InputField PwInputField;

    [Header("--- CreateAccountPanel ---")]
    public GameObject m_CreatePanelObj;
    public InputField New_IdInputField;
    public InputField New_PwInputField;
    public InputField New_NickInputField;
    public Button m_CreateAccountBtn = null;
    public Button m_CancelBtn = null;

    [Header("--- Message ---")]
    public Text MessageText;
    float ShowMsTimer = 0.0f;

    string LoginUrl = "";
    string CreateUrl = "";

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameData();
        //--- LoginPanel
        if (m_LoginBtn != null)
            m_LoginBtn.onClick.AddListener(LoginBtnClick);

        if (m_CreateAccOpenBtn != null)
            m_CreateAccOpenBtn.onClick.AddListener(OpenCreateAccBtn);

        //--- CreateAccountPanel
        if (m_CancelBtn != null)
            m_CancelBtn.onClick.AddListener(CancelBtnClick);

        if (m_CreateAccountBtn != null)
            m_CreateAccountBtn.onClick.AddListener(CreateAccountBtn);

        LoginUrl = "http://junskk.dothome.co.kr/practice/Login.php";
        CreateUrl = "http://junskk.dothome.co.kr/practice/CreateAccount.php";
    }

    // Update is called once per frame
    void Update()
    {
        if(0.0f < ShowMsTimer)
        {
            ShowMsTimer -= Time.deltaTime;
            if(ShowMsTimer <= 0.0f)
            {
                MessageOnOff("", false);    //메시지 끄기
            }
        }

    }//void Update()

    void LoginBtnClick()
    {
        //SceneManager.LoadScene("Lobby");

        string a_IdStr = IdInputField.text;
        string a_PwStr = PwInputField.text;

        a_IdStr = a_IdStr.Trim();
        a_PwStr = a_PwStr.Trim();

        if(string.IsNullOrEmpty(a_IdStr) == true ||
           string.IsNullOrEmpty(a_PwStr) == true)
        {
            MessageOnOff("Id, Pw는 빈칸 없이 입력해 주세요.");
            return;
        }

        if( !(3 <= a_IdStr.Length && a_IdStr.Length <= 20) )
        {
            MessageOnOff("Id는 3글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }

        if( !(4 <= a_PwStr.Length && a_PwStr.Length <= 20) )
        {
            MessageOnOff("Pw는 4글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }

        StartCoroutine( LoginCo(a_IdStr, a_PwStr) );

    }

    IEnumerator LoginCo(string a_IdStr, string a_PwStr)
    {
        WWWForm form = new WWWForm();

        form.AddField("Input_id", a_IdStr, System.Text.Encoding.UTF8);
        form.AddField("Input_pw", a_PwStr);

        UnityWebRequest a_www = UnityWebRequest.Post(LoginUrl, form);

        yield return a_www.SendWebRequest();    //서버로부터 응답이 올 때까지 대기하기...

        if(a_www.error == null)  //에러가 발생하지 않은 경우
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            //Debug.Log(sz);
            a_www.Dispose();

            if(sz.Contains("Id does not exist.") == true)
            {
                MessageOnOff("아이디가 존재하지 않습니다.");
                yield break;  //코루틴 함수를 즉시 빠져나가는 명령어
            }

            if (sz.Contains("Password does not Match.") == true)
            {
                MessageOnOff("비밀번호가 일치하지 않습니다.");
                yield break;  
            }

            if (sz.Contains("Login_Success!!") == false)
            {
                MessageOnOff("로그인 실패, 잠시 후 다시 시도하거나 운영진에 문의해 주세요." + sz);
                yield break;
            }

            if (sz.Contains("{\"") == false) //JSON 형식이 맞는지 확인 하는 코드
            {
                MessageOnOff("서버의 응답이 정상적이지 않습니다." + sz);
                yield break;
            }

            GlobalValue.g_Unique_ID = a_IdStr; //유저의 고유번호

            string a_GetStr = sz.Substring(sz.IndexOf("{\""));

            //Debug.Log(a_GetStr);
            a_GetStr = a_GetStr.Replace("\nLogin_Success!!","");


            SvRespon response = JsonUtility.FromJson<SvRespon>(a_GetStr); //Json 파싱

            GlobalValue.g_NickName  = response.nick_name;
            GlobalValue.g_BestScore = response.best_score;
            GlobalValue.g_UserGold  = response.game_gold;

            //층정보 불러오기
            if (!string.IsNullOrEmpty(response.floor_info))
            {
                FloorInfo floorInfo = JsonUtility.FromJson<FloorInfo>(response.floor_info);
                if (floorInfo != null)
                {
                    GlobalValue.g_CurFloorNum = floorInfo.CurFloor;
                    GlobalValue.g_BestFloor = floorInfo.BestFloor;
                }
            }

            //ItemList 로딩해 오기...
            if (!string.IsNullOrEmpty(response.info))
            {
                ItemList a_ItList = JsonUtility.FromJson<ItemList>(response.info);
                if (a_ItList != null && a_ItList.SkList != null)
                {
                    for (int i = 0; i < a_ItList.SkList.Length; i++)
                    {
                        if (GlobalValue.g_SkillCount.Length <= i)
                            continue;

                        GlobalValue.g_SkillCount[i] = a_ItList.SkList[i];
                    }//for(int i = 0; i < a_ItList.SkList.Length; i++)
                }//if(a_ItList != null && a_ItList.SkList != null)
                 //ItemList 로딩해 오기...
            }

            SceneManager.LoadScene("Lobby");
        }
        else
        {
            MessageOnOff(a_www.error);
            a_www.Dispose();
        }
    }

    void OpenCreateAccBtn()
    {
        if (m_LoginPanelObj != null)
            m_LoginPanelObj.SetActive(false);

        if(m_CreatePanelObj != null)
            m_CreatePanelObj.SetActive(true);
    }

    void CancelBtnClick()
    {
        if (m_LoginPanelObj != null)
            m_LoginPanelObj.SetActive(true);

        if (m_CreatePanelObj != null)
            m_CreatePanelObj.SetActive(false);
    }

    void CreateAccountBtn()
    {
        string a_IdStr = New_IdInputField.text;
        string a_PwStr = New_PwInputField.text;
        string a_NickStr = New_NickInputField.text;

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

        if (!(3 <= a_IdStr.Length && a_IdStr.Length <= 20))
        {
            MessageOnOff("Id는 3글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }

        if (!(4 <= a_PwStr.Length && a_PwStr.Length <= 20))
        {
            MessageOnOff("Pw는 4글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }

        if (!(2 <= a_NickStr.Length && a_NickStr.Length <= 20))
        {
            MessageOnOff("별명는 2글자 이상 20글자 이하로 작성해 주세요.");
            return;
        }

        StartCoroutine(CreateActCo(a_IdStr, a_PwStr, a_NickStr));
    }

    IEnumerator CreateActCo(string a_IdStr, string a_PwStr, string a_NickStr)
    {
        WWWForm form = new WWWForm();

        form.AddField("Input_id", a_IdStr, System.Text.Encoding.UTF8);
        form.AddField("Input_pw", a_PwStr);
        form.AddField("Input_nick", a_NickStr, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(CreateUrl, form);
        yield return a_www.SendWebRequest(); //응답이 올 때까지 대기하기...

        if(a_www.error == null)  //에러가 없을 때 
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            a_www.Dispose();

            if (sz.Contains("Create Success.") == true)
                MessageOnOff("가입 성공");
            else if (sz.Contains("ID does exist.") == true)
                MessageOnOff("중복된 ID가 존재합니다.");
            else if (sz.Contains("Nickname does exist.") == true)
                MessageOnOff("중복된 별명이 존재합니다.");
            else
                MessageOnOff(sz);
        }
        else
        {
            MessageOnOff("가입 실패 : " + a_www.error);
            a_www.Dispose();
        }
    }

    void MessageOnOff(string Msg = "", bool isOn = true)
    {
        if(isOn == true)
        {
            MessageText.text = Msg;
            MessageText.gameObject.SetActive(true);
            ShowMsTimer = 7.0f;
        }
        else
        {
            MessageText.text = "";
            MessageText.gameObject.SetActive(false);
        }
    }
}
