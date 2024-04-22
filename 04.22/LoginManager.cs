using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Account
{
    public string m_Name;
    public int m_Pass;
    public string m_Nick;

    public Account()
    {

    }

    public Account(string a_Name, int a_Pass, string a_Nick)
    {
        m_Name = a_Name;
        m_Pass = a_Pass;
        m_Nick = a_Nick;
    }
}

public class LoginManager : MonoBehaviour
{
    [Header("First UI")]
    public GameObject FirstDisplay;
    public Text IDListText;
    public Text InfoText;
    public Button ClearBtn;     //완료

    [Header("Login UI")]
    public RawImage LoginImg;
    public InputField IDInput;
    public InputField PassInput;
    public Button LoginBtn;     //로그인체크부분 해야함
    public Button NewAccountBtn;    //람다식 완료

    [Header("New Account UI")]
    public RawImage NewAccountImg;
    public InputField NewIDInput;
    public InputField NewPwInput;
    public InputField NewNickInput;
    public Button AddAcountBtn; //구현 필요
    public Button CancleBtn;    //완료

    [Header("UserInfo UI")]
    public GameObject AccountInfo;
    public Text IDInfo;
    public Text PWInfo;
    public Text NickInfo;
    public Button LogoutBtn;        //완료

    float m_Timer = 0.0f;

    List<Account> UserList = new List<Account>();
    // Start is called before the first frame update
    void Start()
    {
        LoadData();
        ListUIUpdate();

        if (ClearBtn != null)
            ClearBtn.onClick.AddListener(() =>
            {
                PlayerPrefs.DeleteAll();
                UserList.Clear();
                ListUIUpdate();
                LoadData();

                InfoText.text = $"모든 계정이 삭제되었습니다. 새로운 계정을 생성해주세요.";
                m_Timer = 3.0f;
            });
            
        if (LoginBtn != null)
            LoginBtn.onClick.AddListener(LoginBtnClick);

        if (NewAccountBtn != null)
            NewAccountBtn.onClick.AddListener(() =>
            {
                LoginImg.gameObject.SetActive(false);
                NewAccountImg.gameObject.SetActive(true);
                InfoText.text = $"사용할 ID, 비밀번호, 별명을 입력해주세요.";
                m_Timer = 3.0f;
            });

        if (CancleBtn != null)
            CancleBtn.onClick.AddListener(() =>
            {
                NewAccountImg.gameObject.SetActive(false);
                LoginImg.gameObject.SetActive(true);
                InfoText.text = $"이미 계정이 있다면 ID와 PW를 입력하고, \r\n계정이 없다면 계정생성 버튼을 눌러주세요.";
                m_Timer = 3.0f;
            });

        if (AddAcountBtn != null)
            AddAcountBtn.onClick.AddListener(AddAcountBtnClick);

    }



    // Update is called once per frame
    void Update()
    {
        //UI 표시용 타이머
        if (m_Timer > 0.0f)
        {
            m_Timer -= Time.deltaTime;
            if(m_Timer <= 0.0f)
            {
                m_Timer = 0.0f;
                InfoText.text = "";
            }
               
        }

    }

    private void LoginBtnClick()
    {
        string a_ID = IDInput.text.Trim();
        string a_PwStr = PassInput.text.Trim();

        if(UserList.Count <= 0)
        {
            InfoText.text = $"생성된 계정이 없습니다.\n계정을 먼저 생성해주세요.";
            m_Timer = 3.0f;
            return;
        }    

        if(string.IsNullOrEmpty(a_PwStr) == true)
        {
            InfoText.text = $"공백없이 비밀번호를 입력해주세요.";
            m_Timer = 3.0f;
            return;
        }

        if (string.IsNullOrEmpty(a_ID) == true)
        {
            InfoText.text = $"공백없이 ID를 입력해주세요.";
            m_Timer = 3.0f;
            return;
        }

        if (string.IsNullOrEmpty(a_ID) == true && string.IsNullOrEmpty(a_PwStr) == true)
        {
            InfoText.text = $"공백없이 ID와 비밀번호를 입력해주세요.";
            m_Timer = 3.0f;
            return;
        }

        int.TryParse(a_PwStr, out int a_Pw);

        //리스트에 계정이 있는지 확인
        for(int i = 0; i < UserList.Count; i++)
        {
            if (a_ID != UserList[i].m_Name)
            {
                InfoText.text = $"해당 계정이 존재하지 않습니다. \n계정을 먼저 생성해주세요.";
                m_Timer = 3.0f;
                continue;
            }

            if (a_ID == UserList[i].m_Name && a_Pw != UserList[i].m_Pass)
            {
                InfoText.text = $"비밀번호가 일치하지 않습니다 \n올바른 비밀번호를 입력해주세요.";
                m_Timer = 3.0f;
                break;
            }

            if (a_ID == UserList[i].m_Name && a_Pw == UserList[i].m_Pass)
            {
                InfoText.text = $"{UserList[i].m_Name}님 로그인에 성공하였습니다.";
                m_Timer = 3.0f;
                FirstDisplay.gameObject.SetActive(false);
                AccountInfo.gameObject.SetActive(true);

                LoginSuccess(a_ID);

                if (LogoutBtn != null)
                    LogoutBtn.onClick.AddListener(() =>
                    {
                        AccountInfo.gameObject.SetActive(false);
                        FirstDisplay.gameObject.SetActive(true);

                        IDInput.text = "";
                        PassInput.text = "";

                        InfoText.text = $"이미 계정이 있다면 ID와 PW를 입력하고, \r\n계정이 없다면 계정생성 버튼을 눌러주세요.";
                        m_Timer = 3.0f;
                    });
                return;
            }
        }
    }

    private void AddAcountBtnClick()
    {
        

        string newID;
        string newPwStr;
        string newNick;
        if (NewIDInput != null && NewPwInput != null && NewNickInput != null)
        {
            if (NewIDInput.text.Trim() == "" || NewPwInput.text.Trim() == "" || NewNickInput.text.Trim() == "")
            {
                InfoText.text = $"ID와 비밀번호, 별명 모두 빠짐 없이 입력해주세요";
                m_Timer = 3.0f;

                return;
            }
            else
            {
                //입력한 내용 리스트에 추가 
                newID = NewIDInput.text.Trim();
                newPwStr = NewPwInput.text.Trim();
                int.TryParse(newPwStr, out int newPw);
                newNick = NewNickInput.text.Trim();
                Account newAccount = new Account(newID, newPw, newNick);
                for (int i = 0; i < UserList.Count; i++)
                {
                    if (newAccount.m_Name == UserList[i].m_Name)
                    {
                        InfoText.text = $"동일한 ID가 존재합니다. 다른 ID를 입력해주세요.";
                        NewIDInput.text = "";
                        m_Timer = 3.0f;

                        return;
                    }
                    else if (newAccount.m_Nick == UserList[i].m_Nick)
                    {
                        InfoText.text = $"동일한 별명이 존재합니다. 다른 별명을 입력해주세요.";
                        NewNickInput.text = "";
                        m_Timer = 3.0f;

                        return;
                    }
                    
                }
                UserList.Add(newAccount);
                //인풋필드 비워주기
                NewIDInput.text = "";
                NewPwInput.text = "";
                NewNickInput.text = "";

                //안내 텍스트
                InfoText.text = $"새로운 계정이 생성되었습니다. \n취소 버튼을 누르고 로그인 화면으로 돌아가 로그인해주세요.";
                m_Timer = 3.0f;
            }
        }

        ListUIUpdate();
        SaveData();

        //계정 생성 함수 리스트에 추가, 프리퍼런스에 추가
    }

    void ListUIUpdate()
    {

        if (IDListText == null)
            return;

        IDListText.text = "";

        if (UserList.Count <= 0)
            return;

        string a_Str = "";
        for(int i = 0; i < UserList.Count; i++)
        {
            a_Str += $"ID({UserList[i].m_Name}) Pass({UserList[i].m_Pass}) Nick({UserList[i].m_Nick}) \n";
        }

        IDListText.text = a_Str;
    }

    void SaveData()
    {
        PlayerPrefs.DeleteAll();


        if (UserList.Count <= 0)
            return;

        PlayerPrefs.SetInt("UserCount", UserList.Count);    //아이템 수 저장

        Account a_Node;
        for (int i = 0; i < UserList.Count; i++)
        {
            a_Node = UserList[i];
            PlayerPrefs.SetString($"User_{i}_Name", a_Node.m_Name);
            PlayerPrefs.SetInt($"User_{i}_Pass", a_Node.m_Pass);
            PlayerPrefs.SetString($"User_{i}_Nick", a_Node.m_Nick);
        }
    }

    void LoadData()
    {
        int a_UserCount = PlayerPrefs.GetInt("UserCount", 0);

        if (a_UserCount <= 0)
            return;

        Account a_Node;
        for (int i = 0; i < a_UserCount; i++)
        {
            a_Node = new Account();
            a_Node.m_Name = PlayerPrefs.GetString($"User_{i}_Name", "");
            a_Node.m_Pass = PlayerPrefs.GetInt($"User_{i}_Pass", 0);
            a_Node.m_Nick = PlayerPrefs.GetString($"User_{i}_Nick", "");

            UserList.Add(a_Node);
        }
    }

    void LoginSuccess(string a_Name)
    {
        if (a_Name == null)
            return;

        Account a_Find = UserList.Find((a_Node) => a_Node.m_Name == a_Name);

        if (a_Find != null)
        {
            IDInfo.text = $"ID              : {a_Find.m_Name}";
            PWInfo.text = $"Password : {a_Find.m_Pass}";
            NickInfo.text = $"Nickname : {a_Find.m_Nick}";
        }
        
    }
}
