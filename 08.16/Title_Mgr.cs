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
                MessageOnOff("", false);    //�޽��� ����
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
            MessageOnOff("Id, Pw�� ��ĭ ���� �Է��� �ּ���.");
            return;
        }

        if( !(3 <= a_IdStr.Length && a_IdStr.Length <= 20) )
        {
            MessageOnOff("Id�� 3���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }

        if( !(4 <= a_PwStr.Length && a_PwStr.Length <= 20) )
        {
            MessageOnOff("Pw�� 4���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
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

        yield return a_www.SendWebRequest();    //�����κ��� ������ �� ������ ����ϱ�...

        if(a_www.error == null)  //������ �߻����� ���� ���
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            //Debug.Log(sz);
            a_www.Dispose();

            if(sz.Contains("Id does not exist.") == true)
            {
                MessageOnOff("���̵� �������� �ʽ��ϴ�.");
                yield break;  //�ڷ�ƾ �Լ��� ��� ���������� ��ɾ�
            }

            if (sz.Contains("Password does not Match.") == true)
            {
                MessageOnOff("��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
                yield break;  
            }

            if (sz.Contains("Login_Success!!") == false)
            {
                MessageOnOff("�α��� ����, ��� �� �ٽ� �õ��ϰų� ����� ������ �ּ���." + sz);
                yield break;
            }

            if (sz.Contains("{\"") == false) //JSON ������ �´��� Ȯ�� �ϴ� �ڵ�
            {
                MessageOnOff("������ ������ ���������� �ʽ��ϴ�." + sz);
                yield break;
            }

            GlobalValue.g_Unique_ID = a_IdStr; //������ ������ȣ

            string a_GetStr = sz.Substring(sz.IndexOf("{\""));

            //Debug.Log(a_GetStr);
            a_GetStr = a_GetStr.Replace("\nLogin_Success!!","");


            SvRespon response = JsonUtility.FromJson<SvRespon>(a_GetStr); //Json �Ľ�

            GlobalValue.g_NickName  = response.nick_name;
            GlobalValue.g_BestScore = response.best_score;
            GlobalValue.g_UserGold  = response.game_gold;

            //������ �ҷ�����
            if (!string.IsNullOrEmpty(response.floor_info))
            {
                FloorInfo floorInfo = JsonUtility.FromJson<FloorInfo>(response.floor_info);
                if (floorInfo != null)
                {
                    GlobalValue.g_CurFloorNum = floorInfo.CurFloor;
                    GlobalValue.g_BestFloor = floorInfo.BestFloor;
                }
            }

            //ItemList �ε��� ����...
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
                 //ItemList �ε��� ����...
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
            MessageOnOff("Id, Pw, ������ ��ĭ ���� �Է��� �ּ���.");
            return;
        }

        if (!(3 <= a_IdStr.Length && a_IdStr.Length <= 20))
        {
            MessageOnOff("Id�� 3���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }

        if (!(4 <= a_PwStr.Length && a_PwStr.Length <= 20))
        {
            MessageOnOff("Pw�� 4���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }

        if (!(2 <= a_NickStr.Length && a_NickStr.Length <= 20))
        {
            MessageOnOff("����� 2���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
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
        yield return a_www.SendWebRequest(); //������ �� ������ ����ϱ�...

        if(a_www.error == null)  //������ ���� �� 
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            a_www.Dispose();

            if (sz.Contains("Create Success.") == true)
                MessageOnOff("���� ����");
            else if (sz.Contains("ID does exist.") == true)
                MessageOnOff("�ߺ��� ID�� �����մϴ�.");
            else if (sz.Contains("Nickname does exist.") == true)
                MessageOnOff("�ߺ��� ������ �����մϴ�.");
            else
                MessageOnOff(sz);
        }
        else
        {
            MessageOnOff("���� ���� : " + a_www.error);
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
