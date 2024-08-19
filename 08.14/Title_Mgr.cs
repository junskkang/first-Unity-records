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
    public string info;    //�� �ʵ�� ���ڿ��� �����ϰ�, �߰������� �Ľ��� �ʿ���
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

    //�ּҸ� �޾ƿ� ����
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

        if (string.IsNullOrEmpty(strId) || string.IsNullOrEmpty(strPw)) //�� ���ڿ� ����ó��
        {
            MessageOnOff("ID�� PW�� ��ĭ ���� �Է����ּ���.");
            return;
        }

        if (!(3 <= strId.Length && strId.Length <= 12))
        {
            MessageOnOff("ID�� 3�� �̻� 12�� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }

        if (!(4 <= strPw.Length && strPw.Length <= 10))
        {
            MessageOnOff("��й�ȣ�� 4�� �̻� 10�� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }

        //���� ��� ������ ����ϱ� ���� �ڷ�ƾ���� ����
        //Ȥ�ó� ��� ���ῡ �����̰� ���� ��� �� ���� �𷡽ð質 �ε�ȭ���� ����ְ�
        //���������� �ڷ�ƾ �Լ��� ����ǰ� �ְԲ�
        StartCoroutine(LoginCo(strId, strPw));  
    }
    void OpenCreateBtn()
    {
        m_LoginPanelObj.SetActive(false);
        m_CreateAccPanel.SetActive(true);
        MessageOnOff("���� ������ ���� ������ �Է��ϼ���.");
    }

    void CancelBtn()
    {
        m_CreateAccPanel.SetActive(false);
        m_LoginPanelObj.SetActive(true);
        MessageOnOff("ID�� PW�� �Է� �� �α����ϼ���.");
    }

    void CreateAccountBtn()
    {
        string strId = newIdInput.text.Trim();
        string strPw = newPwInput.text.Trim();
        string strNick = newNickInput.text.Trim();

        if (string.IsNullOrEmpty(strId) || string.IsNullOrEmpty(strPw)
            || string.IsNullOrEmpty(strNick)) //�� ���ڿ� ����ó��
        {
            MessageOnOff("��� �׸��� ��ĭ ���� �Է����ּ���.");
            return;
        }

        if (!(3 <= strId.Length && strId.Length <= 12))
        {
            MessageOnOff("ID�� 3�� �̻� 12�� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }

        if (!(4 <= strPw.Length && strPw.Length <= 10))
        {
            MessageOnOff("��й�ȣ�� 4�� �̻� 10�� ���Ϸ� �ۼ��� �ּ���.");
            return;
        }

        if (!(3 <= strNick.Length && strNick.Length <= 10))
        {
            MessageOnOff("�г����� 3�� �̻� 10�� ���Ϸ� �ۼ��� �ּ���.");
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

        yield return www.SendWebRequest();  //�����κ��� ���� ���

        if (www.error == null)  //������ ���� ��
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            //������ ��û �� �Ѿ�� �����͸� ��Ʈ������ ����
            string sz = enc.GetString(www.downloadHandler.data);
            //Debug.Log(sz);
            www.Dispose();  // Dispose() : www ó���� ��û�� �����ϴ� �Լ�

            if (sz.Contains("Create Success") == true)
            {
                MessageOnOff("���� ����");
            }
            else if (sz.Contains("ID does exist") == true)
            {
                MessageOnOff("�ߺ��� ID�� �����մϴ�.");
            }
            else if (sz.Contains("Nickname does exist") == true)
            {
                MessageOnOff("�ߺ��� �г����� �����մϴ�.");
            }
            else
                MessageOnOff(sz);
        }
        else
        {
            MessageOnOff("���� ���� : " + www.error);
            www.Dispose();
        }

    }

    IEnumerator LoginCo(string id, string pw)
    {
        WWWForm form = new WWWForm();

        //form�� �����Ͽ� �ش� Ű���� �Ű������� �������
        //�ѱ��� ���ԵǾ��� ��츦 ����� ���ڵ�
        form.AddField("input_id", id, System.Text.Encoding.UTF8);
        form.AddField("input_pw", pw);

        //�ش� �ּҿ� form�� ��û
        UnityWebRequest www = UnityWebRequest.Post(LoginUrl, form);

        yield return www.SendWebRequest();  //�����κ��� ������ �� ������ ���

        if (www.error == null) 
        { 
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            //������ ��û �� �Ѿ�� �����͸� ��Ʈ������ ����
            string sz = enc.GetString(www.downloadHandler.data);    
            //Debug.Log(sz);
            www.Dispose();  // Dispose() : www ó���� ��û�� �����ϴ� �Լ�

            if (sz.Contains("ID does not exist"))   //���̵� ���� üũ
            {
                MessageOnOff("���̵� �������� �ʽ��ϴ�.");
                yield break;    //�ڷ�ƾ �Լ��� ��� ���������� ��ɾ�
            }

            if (sz.Contains("Password does not Match")) //��й�ȣ üũ
            {
                MessageOnOff("��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
                yield break;    //�ڷ�ƾ �Լ��� ��� ���������� ��ɾ�
            }

            if (sz.Contains("Login_Success!!") == false) //������ ������ �α��� ������ ���
            {
                MessageOnOff("�α��� ����, ����� �ٽ� �õ��ϰų� ����� �����ϼ���.");
                yield break;    //�ڷ�ƾ �Լ��� ��� ���������� ��ɾ�
            }

            if (sz.Contains("{\"") == false)    //JSON ������ �´��� Ȯ���ϴ� �ڵ�
            {
                MessageOnOff("������ ������ ���������� �ʽ��ϴ�." + sz);
                yield break;
            }


            GlobalValue.g_Unique_ID = id; //������ ������ȣ

            string a_GetStr = sz.Substring(sz.IndexOf("{\""));
            a_GetStr = a_GetStr.Replace("\nLogin_Success!!", "");

            SvResponse response = JsonUtility.FromJson<SvResponse>(a_GetStr); //Json �Ľ�

            Debug.Log("���� : " + response.nick_name);
            Debug.Log("�� : " + response.floor_info);

            GlobalValue.g_NickName = response.nick_name;
            GlobalValue.g_BestScore = response.best_score;
            GlobalValue.g_UserGold = response.game_gold;

            SceneManager.LoadScene("Lobby");
        }
        else
        {
            MessageOnOff(www.error);
            www.Dispose();  // Dispose() : www ó���� ��û�� �����ϴ� �Լ�
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
