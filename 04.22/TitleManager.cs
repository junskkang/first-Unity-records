using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AccountNode
{
    public string m_Id;
    public string m_Password;
    public string m_Nickname;

    public AccountNode(string a_Id = "", string a_Pw = "", string a_Nickname = "")
    {//디폴트값을 주어 디폴트생성자 역할까지 하도록 함
        m_Id = a_Id;
        m_Password = a_Pw;
        m_Nickname = a_Nickname;
    }
}

public class TitleManager : MonoBehaviour
{
    [Header("LoginPanel")]
    public GameObject loginPanel;
    public InputField idInputField;
    public InputField pwInputField;
    public Button loginBtn;
    public Button createAccOpenBtn;

    [Header("CreateAccountPanel")]
    public GameObject createAccountPanel;
    public InputField newIdInputField;
    public InputField newPwInputField;
    public InputField newNickInputField;
    public Button createAccountBtn;
    public Button cancelBtn;

    [Header("Main UI")]
    public Button clearBtn;
    public Text helpText;
    public Text printListText;

    List<AccountNode> m_AccList = new List<AccountNode>();

    float showTimer = 0.0f;

    void Start()
    {
        if (createAccOpenBtn != null)
            createAccOpenBtn.onClick.AddListener(OpenCreateAccBtn);

        if (cancelBtn != null)
            cancelBtn.onClick.AddListener(CancelBtnClick);

        if (createAccountBtn != null)
            createAccountBtn.onClick.AddListener(CreateAccountBtnClick);

        if (loginBtn != null)
            loginBtn.onClick.AddListener(LoginBtnClick);

        if (clearBtn != null)
            clearBtn.onClick.AddListener(() =>
            {
                m_AccList.Clear();
                PlayerPrefs.DeleteAll();
                RefreshUIList();               
                //SceneManager.LoadScene("TitleScene");
            });

        LoadList();
        RefreshUIList();
    }
    void Update()
    {
        if (0.0f < showTimer)
        {
            showTimer -= Time.deltaTime;

            if (showTimer <= 0.0f)
            {
                showTimer = 0.0f;
                helpText.text = "";
                helpText.gameObject.SetActive(false);
            }
        }     
    }
    private void LoginBtnClick()
    {
        string a_IdStr = idInputField.text;
        string a_PwStr = pwInputField.text;

        a_IdStr.Trim();
        a_PwStr.Trim();

        if (string.IsNullOrEmpty(a_IdStr) == true || string.IsNullOrEmpty(a_PwStr) == true)
        {
            ShowMessage("ID, Pw는 빈 칸 없이 입력해 주세요.");
            return;
        }
        
        if (!(3 <= a_IdStr.Length) && (a_IdStr.Length <= 15)) // 3 ~ 15의 범위가 아니라면
        {
            ShowMessage("ID는 3글자 이상 15글자 이하로 작성해 주세요.");
            return;
        }

        if (!(4 <= a_PwStr.Length) && (a_PwStr.Length <= 15)) // 4 ~ 15의 범위가 아니라면
        {
            ShowMessage("비밀번호는 4글자 이상 15글자 이하로 작성해 주세요.");
            return;
        }

        AccountNode a_FNode = m_AccList.Find(a_Nd => a_Nd.m_Id == a_IdStr);
        if (a_FNode == null)
        {
            ShowMessage("로그인 실패 : ID가 존재하지 않습니다.");
            return;
        }

        if (a_FNode.m_Password != a_PwStr)
        {
            ShowMessage("로그인 실패 : Password가 일치하지 않습니다.");
            return;
        }

        ShowMessage("로그인 성공");
        GlobalValue.g_Unique_Id = a_IdStr;
        GlobalValue.g_Password = a_FNode.m_Password;
        GlobalValue.g_NickName = a_FNode.m_Nickname;
        SceneManager.LoadScene("LobbyScene");
    }
    private void CreateAccountBtnClick()
    {
        string a_IdStr = newIdInputField.text;
        string a_PwStr = newPwInputField.text;
        string a_NickStr = newNickInputField.text;

        a_IdStr = a_IdStr.Trim();
        a_PwStr = a_PwStr.Trim();
        a_NickStr = a_NickStr.Trim();

        //예외처리
        if (string.IsNullOrEmpty(a_IdStr) == true || string.IsNullOrEmpty(a_PwStr) == true ||
            string.IsNullOrEmpty(a_NickStr) == true)
        {
            ShowMessage("ID, Pw, 별명은 빈 칸 없이 입력해 주세요.");
            return;
        }

        if (!(3 <= a_IdStr.Length) && (a_IdStr.Length <= 15)) // 3 ~ 15의 범위가 아니라면
        {
            ShowMessage("ID는 3글자 이상 15글자 이하로 작성해 주세요.");
            return;
        }

        if (!(4 <= a_PwStr.Length) && (a_PwStr.Length <= 15)) // 4 ~ 15의 범위가 아니라면
        {
            ShowMessage("비밀번호는 4글자 이상 15글자 이하로 작성해 주세요.");
            return;
        }

        if (!(2 <= a_NickStr.Length) && (a_NickStr.Length <= 15)) // 3 ~ 15의 범위가 아니라면
        {
            ShowMessage("별명은 2글자 이상 15글자 이하로 작성해 주세요.");
            return;
        }

        idInputField.text = "";
        //AccountNode a_FNode = null;
        //foreach (AccountNode a_Nd in m_AccList)
        //{
        //    if (a_Nd.m_Id == a_IdStr)
        //    {
        //        a_FNode = a_Nd;
        //        break;
        //    }
        //}

        AccountNode a_FNode = m_AccList.Find(ii => ii.m_Id == a_IdStr);
        if (a_FNode != null)
        {
            ShowMessage("같은 ID가 이미 존재합니다.");
            return;
        }

        AccountNode a_Node = new AccountNode(a_IdStr, a_PwStr, a_NickStr);
        m_AccList.Add(a_Node);

        SaveList();
        RefreshUIList();

        ShowMessage("가입 성공");

        newIdInputField.text = "";
        newPwInputField.text = "";
        newNickInputField.text = "";

        idInputField.text = a_IdStr;
    }

    private void OpenCreateAccBtn()
    {
        //인풋필드 초기화
        newIdInputField.text = "";
        newPwInputField.text = "";
        newNickInputField.text = "";

        if (loginPanel != null) 
            loginPanel.SetActive(false);

        if(createAccountPanel != null)
            createAccountPanel.SetActive(true);
    }
    private void CancelBtnClick()
    {
        //idInputField.text = "";
        pwInputField.text = "";

        if (loginPanel != null)
            loginPanel.SetActive(true);

        if (createAccountPanel != null)
            createAccountPanel.SetActive(false);
    }
    private void LoadList()
    {
        int a_AccCount = PlayerPrefs.GetInt("AccountCount", 0);

        if (a_AccCount <= 0)
            return;

        AccountNode a_Node;
        for(int i = 0; i < a_AccCount; i++)
        {
            a_Node = new AccountNode();
            a_Node.m_Id = PlayerPrefs.GetString($"Account_{i}_ID", "");
            a_Node.m_Password = PlayerPrefs.GetString($"Account_{i}_Pw", "");
            a_Node.m_Nickname = PlayerPrefs.GetString($"Account_{i}_Nick", "");
            m_AccList.Add(a_Node);
        }
    }
    private void SaveList()
    {
        PlayerPrefs.DeleteAll();

        if (m_AccList.Count <= 0)
            return;

        PlayerPrefs.SetInt("AccountCount", m_AccList.Count);    //계정 수 저장

        AccountNode a_Node;
        for(int i = 0; i < m_AccList.Count; i++)
        {
            a_Node = m_AccList[i];
            PlayerPrefs.SetString($"Account_{i}_ID", a_Node.m_Id);
            PlayerPrefs.SetString($"Account_{i}_Pw", a_Node.m_Password);
            PlayerPrefs.SetString($"Account_{i}_Nick", a_Node.m_Nickname);
        }
    }
    private void RefreshUIList()
    {
        string a_StrBuff = "";

        for (int i = 0; i < m_AccList.Count; i++)
        {
            a_StrBuff += $"ID({m_AccList[i].m_Id}) Pw({m_AccList[i].m_Password})" +
                            $" Nick({m_AccList[i].m_Nickname})";

            a_StrBuff += "\n";
        }


        if (printListText != null)
            printListText.text = a_StrBuff;
    }
    void ShowMessage(string a_Str)
    {
        if (helpText == null)
            return;

        helpText.gameObject.SetActive(true);
        helpText.text = a_Str;
        showTimer = 3.0f;
    }
}
