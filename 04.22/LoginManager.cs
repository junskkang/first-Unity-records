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
    public Button ClearBtn;     //�Ϸ�

    [Header("Login UI")]
    public RawImage LoginImg;
    public InputField IDInput;
    public InputField PassInput;
    public Button LoginBtn;     //�α���üũ�κ� �ؾ���
    public Button NewAccountBtn;    //���ٽ� �Ϸ�

    [Header("New Account UI")]
    public RawImage NewAccountImg;
    public InputField NewIDInput;
    public InputField NewPwInput;
    public InputField NewNickInput;
    public Button AddAcountBtn; //���� �ʿ�
    public Button CancleBtn;    //�Ϸ�

    [Header("UserInfo UI")]
    public GameObject AccountInfo;
    public Text IDInfo;
    public Text PWInfo;
    public Text NickInfo;
    public Button LogoutBtn;        //�Ϸ�

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

                InfoText.text = $"��� ������ �����Ǿ����ϴ�. ���ο� ������ �������ּ���.";
                m_Timer = 3.0f;
            });
            
        if (LoginBtn != null)
            LoginBtn.onClick.AddListener(LoginBtnClick);

        if (NewAccountBtn != null)
            NewAccountBtn.onClick.AddListener(() =>
            {
                LoginImg.gameObject.SetActive(false);
                NewAccountImg.gameObject.SetActive(true);
                InfoText.text = $"����� ID, ��й�ȣ, ������ �Է����ּ���.";
                m_Timer = 3.0f;
            });

        if (CancleBtn != null)
            CancleBtn.onClick.AddListener(() =>
            {
                NewAccountImg.gameObject.SetActive(false);
                LoginImg.gameObject.SetActive(true);
                InfoText.text = $"�̹� ������ �ִٸ� ID�� PW�� �Է��ϰ�, \r\n������ ���ٸ� �������� ��ư�� �����ּ���.";
                m_Timer = 3.0f;
            });

        if (AddAcountBtn != null)
            AddAcountBtn.onClick.AddListener(AddAcountBtnClick);

    }



    // Update is called once per frame
    void Update()
    {
        //UI ǥ�ÿ� Ÿ�̸�
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
            InfoText.text = $"������ ������ �����ϴ�.\n������ ���� �������ּ���.";
            m_Timer = 3.0f;
            return;
        }    

        if(string.IsNullOrEmpty(a_PwStr) == true)
        {
            InfoText.text = $"������� ��й�ȣ�� �Է����ּ���.";
            m_Timer = 3.0f;
            return;
        }

        if (string.IsNullOrEmpty(a_ID) == true)
        {
            InfoText.text = $"������� ID�� �Է����ּ���.";
            m_Timer = 3.0f;
            return;
        }

        if (string.IsNullOrEmpty(a_ID) == true && string.IsNullOrEmpty(a_PwStr) == true)
        {
            InfoText.text = $"������� ID�� ��й�ȣ�� �Է����ּ���.";
            m_Timer = 3.0f;
            return;
        }

        int.TryParse(a_PwStr, out int a_Pw);

        //����Ʈ�� ������ �ִ��� Ȯ��
        for(int i = 0; i < UserList.Count; i++)
        {
            if (a_ID != UserList[i].m_Name)
            {
                InfoText.text = $"�ش� ������ �������� �ʽ��ϴ�. \n������ ���� �������ּ���.";
                m_Timer = 3.0f;
                continue;
            }

            if (a_ID == UserList[i].m_Name && a_Pw != UserList[i].m_Pass)
            {
                InfoText.text = $"��й�ȣ�� ��ġ���� �ʽ��ϴ� \n�ùٸ� ��й�ȣ�� �Է����ּ���.";
                m_Timer = 3.0f;
                break;
            }

            if (a_ID == UserList[i].m_Name && a_Pw == UserList[i].m_Pass)
            {
                InfoText.text = $"{UserList[i].m_Name}�� �α��ο� �����Ͽ����ϴ�.";
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

                        InfoText.text = $"�̹� ������ �ִٸ� ID�� PW�� �Է��ϰ�, \r\n������ ���ٸ� �������� ��ư�� �����ּ���.";
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
                InfoText.text = $"ID�� ��й�ȣ, ���� ��� ���� ���� �Է����ּ���";
                m_Timer = 3.0f;

                return;
            }
            else
            {
                //�Է��� ���� ����Ʈ�� �߰� 
                newID = NewIDInput.text.Trim();
                newPwStr = NewPwInput.text.Trim();
                int.TryParse(newPwStr, out int newPw);
                newNick = NewNickInput.text.Trim();
                Account newAccount = new Account(newID, newPw, newNick);
                for (int i = 0; i < UserList.Count; i++)
                {
                    if (newAccount.m_Name == UserList[i].m_Name)
                    {
                        InfoText.text = $"������ ID�� �����մϴ�. �ٸ� ID�� �Է����ּ���.";
                        NewIDInput.text = "";
                        m_Timer = 3.0f;

                        return;
                    }
                    else if (newAccount.m_Nick == UserList[i].m_Nick)
                    {
                        InfoText.text = $"������ ������ �����մϴ�. �ٸ� ������ �Է����ּ���.";
                        NewNickInput.text = "";
                        m_Timer = 3.0f;

                        return;
                    }
                    
                }
                UserList.Add(newAccount);
                //��ǲ�ʵ� ����ֱ�
                NewIDInput.text = "";
                NewPwInput.text = "";
                NewNickInput.text = "";

                //�ȳ� �ؽ�Ʈ
                InfoText.text = $"���ο� ������ �����Ǿ����ϴ�. \n��� ��ư�� ������ �α��� ȭ������ ���ư� �α������ּ���.";
                m_Timer = 3.0f;
            }
        }

        ListUIUpdate();
        SaveData();

        //���� ���� �Լ� ����Ʈ�� �߰�, �����۷����� �߰�
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

        PlayerPrefs.SetInt("UserCount", UserList.Count);    //������ �� ����

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
