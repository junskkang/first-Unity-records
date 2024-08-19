using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;

[System.Serializable]
public class SvResponse     //ServerResponse
{
    public string nick_name;
    public int best_score;
    public int game_gold; 
    public string floor_info;
    public string info;    //이 필드는 문자열로 유지하고, 추가적으로 파싱이 필요함
}   

public class Title_Mgr : MonoBehaviour
{
    [Header("--- LoginPanel ---")]
    public GameObject m_LoginPanelObj;
    public Button m_LoginBtn = null;
    public Button m_CreateAccOpenBtn = null;
    public InputField idInputField = null;
    public InputField pwInputField = null;

    [Header("--- CreatePanel ---")]
    public GameObject m_CreateAccPanel;
    public Button createBtn;
    public Button cancelBtn;
    public InputField newIdInput = null;
    public InputField newPwInput = null;
    public InputField newNickInput = null;

    //주소를 받아올 변수
    string LoginUrl = "";
    string CreateUrl = "";

    [Header("--- Message ---")]
    public Text MessageText;
    float ShowMsTimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameData();
        //--- LoginPanel
        if (m_LoginBtn != null)
            m_LoginBtn.onClick.AddListener(LoginBtnClick);

        if (m_CreateAccOpenBtn != null)
            m_CreateAccOpenBtn.onClick.AddListener(OpenCreateBtn);

        //--- Create Account Panel
        if (cancelBtn != null)
            cancelBtn.onClick.AddListener(CancelBtn);

        if (createBtn != null)
            createBtn.onClick.AddListener(CreateAccountBtn);

        LoginUrl = "http://junskk.dothome.co.kr/practice/Login.php";

        CreateUrl = "http://junskk.dothome.co.kr/practice/CreateAccount.php";
    }


    // Update is called once per frame
    void Update()
    {
        if (ShowMsTimer > 0.0f)
        {
            ShowMsTimer -= Time.deltaTime;

            if (ShowMsTimer <= 0.0f)
            {
                MessageOnOff();
            }
        }


    }

    void LoginBtnClick()
    {
        //SceneManager.LoadScene("Lobby");

        string strId = idInputField.text.Trim();
        string strPw = pwInputField.text.Trim();

        if (string.IsNullOrEmpty(strId) || string.IsNullOrEmpty(strPw)) //빈 문자열 예외처리
        {
            MessageOnOff("ID와 PW는 빈칸 없이 입력해주세요.");
            return;
        }

        if (!(3 <= strId.Length && strId.Length <= 12))
        {
            MessageOnOff("ID는 3자 이상 12자 이하로 작성해 주세요.");
            return;
        }

        if (!(4 <= strPw.Length && strPw.Length <= 10))
        {
            MessageOnOff("비밀번호는 4자 이상 10자 이하로 작성해 주세요.");
            return;
        }

        //각종 통신 오류에 대비하기 위해 코루틴으로 구현
        //혹시나 통신 연결에 딜레이가 생길 경우 그 동안 모래시계나 로딩화면을 띄워주고
        //내부적으로 코루틴 함수는 진행되고 있게끔
        StartCoroutine(LoginCo(strId, strPw));  
    }
    void OpenCreateBtn()
    {
        m_LoginPanelObj.SetActive(false);
        m_CreateAccPanel.SetActive(true);
        MessageOnOff("계정 생성을 위한 정보를 입력하세요.");
    }

    void CancelBtn()
    {
        m_CreateAccPanel.SetActive(false);
        m_LoginPanelObj.SetActive(true);
        MessageOnOff("ID와 PW를 입력 후 로그인하세요.");
    }

    void CreateAccountBtn()
    {
        string strId = newIdInput.text.Trim();
        string strPw = newPwInput.text.Trim();
        string strNick = newNickInput.text.Trim();

        if (string.IsNullOrEmpty(strId) || string.IsNullOrEmpty(strPw)
            || string.IsNullOrEmpty(strNick)) //빈 문자열 예외처리
        {
            MessageOnOff("모든 항목은 빈칸 없이 입력해주세요.");
            return;
        }

        if (!(3 <= strId.Length && strId.Length <= 12))
        {
            MessageOnOff("ID는 3자 이상 12자 이하로 작성해 주세요.");
            return;
        }

        if (!(4 <= strPw.Length && strPw.Length <= 10))
        {
            MessageOnOff("비밀번호는 4자 이상 10자 이하로 작성해 주세요.");
            return;
        }

        if (!(3 <= strNick.Length && strNick.Length <= 10))
        {
            MessageOnOff("닉네임은 3자 이상 10자 이하로 작성해 주세요.");
            return;
        }

        StartCoroutine(CreateAccCo(strId, strPw, strNick));
    }

    IEnumerator CreateAccCo(string strId, string strPw, string strNick)
    {
        WWWForm form = new WWWForm();

        form.AddField("input_id", strId, System.Text.Encoding.UTF8);
        form.AddField("input_pw", strPw);
        form.AddField("input_nick", strNick, System.Text.Encoding.UTF8);

        UnityWebRequest www = UnityWebRequest.Post(CreateUrl, form);

        yield return www.SendWebRequest();  //서버로부터 응답 대기

        if (www.error == null)  //에러가 없을 때
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            //서버에 요청 후 넘어온 데이터를 스트링으로 저장
            string sz = enc.GetString(www.downloadHandler.data);
            //Debug.Log(sz);
            www.Dispose();  // Dispose() : www 처리한 요청을 삭제하는 함수

            if (sz.Contains("Create Success") == true)
            {
                MessageOnOff("가입 성공");
            }
            else if (sz.Contains("ID does exist") == true)
            {
                MessageOnOff("중복된 ID가 존재합니다.");
            }
            else if (sz.Contains("Nickname does exist") == true)
            {
                MessageOnOff("중복된 닉네임이 존재합니다.");
            }
            else
                MessageOnOff(sz);
        }
        else
        {
            MessageOnOff("가입 실패 : " + www.error);
            www.Dispose();
        }

    }

    IEnumerator LoginCo(string id, string pw)
    {
        WWWForm form = new WWWForm();

        //form을 생성하여 해당 키값에 매개변수를 집어넣음
        //한글이 포함되었을 경우를 대비해 인코딩
        form.AddField("input_id", id, System.Text.Encoding.UTF8);
        form.AddField("input_pw", pw);

        //해당 주소에 form을 요청
        UnityWebRequest www = UnityWebRequest.Post(LoginUrl, form);

        yield return www.SendWebRequest();  //서버로부터 응답이 올 때까지 대기

        if (www.error == null) 
        { 
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            //서버에 요청 후 넘어온 데이터를 스트링으로 저장
            string sz = enc.GetString(www.downloadHandler.data);    
            //Debug.Log(sz);
            www.Dispose();  // Dispose() : www 처리한 요청을 삭제하는 함수

            if (sz.Contains("ID does not exist"))   //아이디 유무 체크
            {
                MessageOnOff("아이디가 존재하지 않습니다.");
                yield break;    //코루틴 함수를 즉시 빠져나가는 명령어
            }

            if (sz.Contains("Password does not Match")) //비밀번호 체크
            {
                MessageOnOff("비밀번호가 일치하지 않습니다.");
                yield break;    //코루틴 함수를 즉시 빠져나가는 명령어
            }

            if (sz.Contains("Login_Success!!") == false) //모종의 이유로 로그인 실패할 경우
            {
                MessageOnOff("로그인 실패, 잠시후 다시 시도하거나 운영진에 문의하세요.");
                yield break;    //코루틴 함수를 즉시 빠져나가는 명령어
            }

            if (sz.Contains("{\"") == false)    //JSON 형식이 맞는지 확인하는 코드
            {
                MessageOnOff("서버의 응답이 정상적이지 않습니다." + sz);
                yield break;
            }


            GlobalValue.g_Unique_ID = id; //유저의 고유번호

            string a_GetStr = sz.Substring(sz.IndexOf("{\""));
            a_GetStr = a_GetStr.Replace("\nLogin_Success!!", "");

            SvResponse response = JsonUtility.FromJson<SvResponse>(a_GetStr); //Json 파싱

            Debug.Log("별명 : " + response.nick_name);
            Debug.Log("층 : " + response.floor_info);

            GlobalValue.g_NickName = response.nick_name;
            GlobalValue.g_BestScore = response.best_score;
            GlobalValue.g_UserGold = response.game_gold;

            SceneManager.LoadScene("Lobby");
        }
        else
        {
            MessageOnOff(www.error);
            www.Dispose();  // Dispose() : www 처리한 요청을 삭제하는 함수
        }

        
    }

    void MessageOnOff(string msg = "")
    {
        if (msg != "")
        {
            MessageText.gameObject.SetActive(true);

            MessageText.text = msg;

            ShowMsTimer = 3.0f;
        }
        else
        {
            ShowMsTimer = 0.0f;

            MessageText.text = msg;

            MessageText.gameObject.SetActive(false);            
        }
    }
}
