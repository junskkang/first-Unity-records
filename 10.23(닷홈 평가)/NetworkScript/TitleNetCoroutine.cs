using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TitleNetCoroutine : MonoBehaviour
{
    string LoginUrl = "";
    string CreateUrl = "";

    Title_Mgr m_RefTitleMgr = null;

    // Start is called before the first frame update
    void Start()
    {
        LoginUrl = "http://junskk.dothome.co.kr/GawiBawiBo/Login.php";
        CreateUrl = "http://junskk.dothome.co.kr/GawiBawiBo/CreateAccount.php";
    }

    public void TitleStart(Title_Mgr a_RefTMgr)
    {
        m_RefTitleMgr = a_RefTMgr;
    }

    //// Update is called once per frame
    //void Update()
    //{
    //}

    public IEnumerator LoginCo(string a_IdStr, string a_PwStr)
    {
        WWWForm form = new WWWForm();

        form.AddField("Input_id", a_IdStr, System.Text.Encoding.UTF8);
        form.AddField("Input_pw", a_PwStr);

        UnityWebRequest a_www = UnityWebRequest.Post(LoginUrl, form);

        //Ÿ�Ӿƿ� ���� (�� ������ ����, ��: 3��)
        bool isTimeOut = false;
        float startTime = Time.unscaledTime;

        NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

        yield return a_www.SendWebRequest();    //�����κ��� ������ �� ������ ����ϱ�...

        //������ ���ų� Ÿ�Ӿƿ��� �߻��� ������ ���
        while (!a_www.isDone && !isTimeOut)
        {
            if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
                isTimeOut = true;

            yield return null; //���� �����ӱ��� ���
        }//while(!a_www.isDone && !isTimeOut)

        //Ÿ�Ӿƿ� ó��
        if (isTimeOut == true)
        {
            a_www.Abort();  //��û�� �ߴ�
            NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
            yield break;
        }

        bool IsExit = false;
        if (a_www.error == null)  //������ �߻����� ���� ���
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            //Debug.Log(sz);

            if (sz.Contains("Id does not exist.") == true)
            {
                if(m_RefTitleMgr != null)
                    m_RefTitleMgr.MessageOnOff("���̵� �������� �ʽ��ϴ�.");
                IsExit = true;
            }

            else if (sz.Contains("Password does not Match.") == true)
            {
                if (m_RefTitleMgr != null)
                    m_RefTitleMgr.MessageOnOff("��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
                IsExit = true;
            }

            else if (sz.Contains("Login_Success!!") == false)
            {
                if (m_RefTitleMgr != null)
                    m_RefTitleMgr.MessageOnOff("�α��� ����, ��� �� �ٽ� �õ��ϰų� ����� ������ �ּ���.");
                IsExit = true;
            }

            else if (sz.Contains("{\"") == false) //JSON ������ �´��� Ȯ�� �ϴ� �ڵ�
            {
                if (m_RefTitleMgr != null)
                    m_RefTitleMgr.MessageOnOff("������ ������ ���������� �ʽ��ϴ�.");
                IsExit = true;
            }

            if(IsExit == true)
            {
                a_www.Dispose();
                NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
                yield break;  //�ڷ�ƾ �Լ��� ��� ���������� ��ɾ�
            }

            GlobalValue.g_Unique_ID = a_IdStr; //������ ������ȣ

            string a_GetStr = sz.Substring(sz.IndexOf("{\""));
            a_GetStr = a_GetStr.Replace("\nLogin_Success!!", "");

            SvRespon response = JsonUtility.FromJson<SvRespon>(a_GetStr); //Json �Ľ�

            GlobalValue.g_NickName = response.nick_name;
            GlobalValue.g_BestScore = response.best_score;
            GlobalValue.g_MyPoint = response.mypoint;

            //Debug.Log(sz);

            if (m_RefTitleMgr != null)
                m_RefTitleMgr.LobbyLoadScene();
        }
        else
        {
            if (m_RefTitleMgr != null)
                m_RefTitleMgr.MessageOnOff(a_www.error);
        }

        a_www.Dispose();
        NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
    }

    public IEnumerator CreateActCo(string a_IdStr, string a_PwStr, string a_NickStr)
    {
        WWWForm form = new WWWForm();

        form.AddField("Input_id", a_IdStr, System.Text.Encoding.UTF8);
        form.AddField("Input_pw", a_PwStr);
        form.AddField("Input_nick", a_NickStr, System.Text.Encoding.UTF8);

        UnityWebRequest a_www = UnityWebRequest.Post(CreateUrl, form);

        //Ÿ�Ӿƿ� ���� (�� ������ ����, ��: 3��)
        bool isTimeOut = false;
        float startTime = Time.unscaledTime;

        NetworkMgr.Inst.m_NetWaitTimer = NetworkMgr.m_Timeout;

        a_www.SendWebRequest(); //������ �� ������ ����ϱ�...

        //������ ���ų� Ÿ�Ӿƿ��� �߻��� ������ ���
        while (!a_www.isDone && !isTimeOut)
        {
            if (Time.unscaledTime - startTime > NetworkMgr.m_Timeout)
                isTimeOut = true;

            yield return null; //���� �����ӱ��� ���
        }//while(!a_www.isDone && !isTimeOut)

        //Ÿ�Ӿƿ� ó��
        if (isTimeOut == true)
        {
            a_www.Abort();  //��û�� �ߴ�
            NetworkMgr.Inst.m_NetWaitTimer = 0.0f;
            yield break;
        }

        if (a_www.error == null)  //������ ���� �� 
        {
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            string sz = enc.GetString(a_www.downloadHandler.data);
            //a_www.Dispose();

            if (m_RefTitleMgr != null)
            {
                if (sz.Contains("Create Success.") == true)
                {
                    m_RefTitleMgr.MessageOnOff("���� ����! �α��� �� ������ ����ּ���.");
                    m_RefTitleMgr.IdInputField.text = a_IdStr;
                    m_RefTitleMgr.PwInputField.text = a_PwStr;
                }
                else if (sz.Contains("ID does exist.") == true)
                    m_RefTitleMgr.MessageOnOff("�ߺ��� ID�� �����մϴ�.");
                else if (sz.Contains("Nickname does exist.") == true)
                    m_RefTitleMgr.MessageOnOff("�ߺ��� ������ �����մϴ�.");
                else
                    m_RefTitleMgr.MessageOnOff(sz);
            }//if (m_RefTitleMgr != null)
        }
        else
        {
            if (m_RefTitleMgr != null)
                m_RefTitleMgr.MessageOnOff("���� ���� : " + a_www.error);
            //a_www.Dispose();
        }

        a_www.Dispose();
        NetworkMgr.Inst.m_NetWaitTimer = 0.0f;

    }//IEnumerator CreateActCo(string a_IdStr, string a_PwStr, string a_NickStr)
}
