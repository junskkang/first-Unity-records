using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GawiBawiBo
{
    Gawi = 1,
    Bawi = 2,
    Bo = 3
}

public enum Record
{
    Draw = 0,  //����.
    Win  = 1,  //�̰��.
    Lost = 2   //����.
}

public class Game_Mgr : MonoBehaviour
{
    public Button Gawi_Btn;     //���� ��ư ���� ����
    public Button Bawi_Btn;     //���� ��ư ���� ����
    public Button Bo_Btn;       //�� ��ư ���� ����
    public Button Replay_Btn;   //���Ӵٽ��ϱ� ��ư ���� ����

    public Text UserInfo_Text;  //���� ���� ǥ�� �ؽ�Ʈ
    public Text Result_Text;    //��� ǥ�� �ؽ�Ʈ

    int m_Money = 1000;         //������ ���� �ݾ�
    int m_WinCount = 0;         //�¸� ī��Ʈ
    int m_LostCount = 0;        //�й� ī��Ʈ

    int m_Point = 0;            //�¸� �� 100��, ����ī��Ʈ�� ���� �߰� ���� �ο�
    int m_ContinuousCount = 0;  //���� ī��Ʈ
    bool isUpdate = false;

    [Header("--- ShowUserData ---")]
    public InputField NickNameIF;   //���� �Է� ����
    public Button NickInput_Btn;    //���� �Է� ��ư
    string m_NickName = "";

    [Header("--- Direction Image ---")]
    public Image UserGBB_Img;   //UserSelPanel�� GBB Image
    public Image ComGBB_Img;    //ComSelPanel�� GBB Image

    public Sprite[] m_IconSprite;  //����, ����, �� ��������Ʈ�� ������ �迭 ����

    [Header("--- ShowBestWinCount ---")]
    public Text BestWinCountText;  //�¸� �ְ� ��� ǥ�� Text
    int m_BestWinCount = 0;        //�ְ� ���� ����
    public Button ClearSaveBtn;    //���� ���� �ʱ�ȭ
                                   
    float m_WaitTimer = 0.0f;

    public Text Message_Text;
    float showMessageTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (Gawi_Btn != null) //���⿡ ���� ��ư�� �� ���� �Ǿ� ������....
            Gawi_Btn.onClick.AddListener(()=>
            {
                BtnClickMethod(GawiBawiBo.Gawi);
            });

        if (Bawi_Btn != null)
            Bawi_Btn.onClick.AddListener(()=>
            {
                BtnClickMethod(GawiBawiBo.Bawi);
            });

        if (Bo_Btn != null)
            Bo_Btn.onClick.AddListener(()=>
            {
                BtnClickMethod(GawiBawiBo.Bo);
            });

        if (ClearSaveBtn != null)
            ClearSaveBtn.onClick.AddListener(() =>
            {
                //PlayerPrefs.DeleteAll();
                //GlobalValue.ClearGameData();
                SceneManager.LoadScene("LobbyScene"); 
            });

        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("GameScene");
            });

        //--- User Data Loading        
        if (NickNameIF != null)
            NickNameIF.text = GlobalValue.g_NickName;

        //m_BestWinCount = PlayerPrefs.GetInt("BestWinCount", 0);

        ////--- ���� ���� UI ����
        //RefreshUserUI();
        ////--- ���� ���� UI ����
        //--- User Data Loading

        if (NickInput_Btn != null)
            NickInput_Btn.onClick.AddListener(NickNameBtnClick);

        isUpdate = false;

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        if(0.0f < m_WaitTimer)
        {
            m_WaitTimer -= Time.deltaTime;
            if(m_WaitTimer <= 0.0f)
            {
                UserGBB_Img.gameObject.SetActive(false);
            }
        }//if(0.0f < m_WaitTimer)

        if (0.0f < showMessageTimer)
        {
            showMessageTimer -= Time.deltaTime;
            if (showMessageTimer <= 0.0f)
            {
                MessageOnOff("", false);    //�޽��� ����
            }
        }

        RefreshUI();

    }//void Update()


    void BtnClickMethod(GawiBawiBo a_UserSel)
    {
        if (m_Money <= 0)
            return;

        GawiBawiBo a_ComSel = (GawiBawiBo)Random.Range((int)GawiBawiBo.Gawi,
                                                       (int)GawiBawiBo.Bo + 1);

        string a_strUser = "����";
        if (a_UserSel == GawiBawiBo.Bawi)
            a_strUser = "����";
        else if (a_UserSel == GawiBawiBo.Bo)
            a_strUser = "��";

        string a_strCom = "����";
        if (a_ComSel == GawiBawiBo.Bawi)
            a_strCom = "����";
        else if (a_ComSel == GawiBawiBo.Bo)
            a_strCom = "��";

        Result_Text.text = "User(" + a_strUser + ") : Com(" + a_strCom + ")";

        //--- ����
        Record IsWin = Record.Draw;
        if (a_UserSel == a_ComSel)
        {
            Result_Text.text += " �����ϴ�.";
            IsWin = Record.Draw;
        }
        else if ((a_UserSel == GawiBawiBo.Gawi && a_ComSel == GawiBawiBo.Bo) ||
                 (a_UserSel == GawiBawiBo.Bawi && a_ComSel == GawiBawiBo.Gawi) ||
                 (a_UserSel == GawiBawiBo.Bo && a_ComSel == GawiBawiBo.Bawi))
        {
            Result_Text.text += " �̰���ϴ�.";
            IsWin = Record.Win;
        }
        else
        {
            Result_Text.text += " �����ϴ�.";
            IsWin = Record.Lost;
        }
        //--- ����

        //--- ����
        if (IsWin == Record.Win) //�̰��� �� 
        {
            m_WinCount++;
            m_Money += 100;
            m_Point += 100 * (1+ m_ContinuousCount);
            m_ContinuousCount++;
        }
        else if(IsWin == Record.Lost) //���� �� 
        {
            m_LostCount++;
            m_Money -= 200;
            m_ContinuousCount = 0;
            if(m_Money <= 0)
            {
                m_Money = 0;
                Result_Text.text = "Game Over ";
                if (isUpdate == true)
                    Result_Text.text += "����� ���� �Ǿ����ϴ�. ��ŷ�� Ȯ���غ�����!";

            }
        }
        //--- ����

        //--- �ְ� �¸� �� ����
        if (GlobalValue.g_BestScore < m_WinCount)
        {
            GlobalValue.g_BestScore = m_WinCount;
            NetworkMgr.Inst.PushPacket(PacketType.BestScore);
            //PlayerPrefs.SetInt("BestWinCount", m_BestWinCount);
            isUpdate = true;
        }
        //--- �ְ� �¸� �� ����

        if (GlobalValue.g_MyPoint < m_Point)
        {
            GlobalValue.g_MyPoint = m_Point;
            NetworkMgr.Inst.PushPacket(PacketType.MyPoint);
            isUpdate = true;
        }

        ////--- ���� ���� UI ����
        //RefreshUserUI();
        ////--- ���� ���� UI ����

        //--- ���� ���¿� ���� �̹��� ��ü �ڵ�
        UserGBB_Img.sprite = m_IconSprite[(int)a_UserSel - 1];
        UserGBB_Img.gameObject.SetActive(true);

        ComGBB_Img.sprite = m_IconSprite[(int)a_ComSel - 1];
        ////--- ���� ���¿� ���� �̹��� ��ü �ڵ�
        
        m_WaitTimer = 3.0f;  //<-- Ÿ�̸� ����
    }
 
    private void NickNameBtnClick()
    {
        if (m_Money <= 0)
            return;

        
        string a_Nick = NickNameIF.text.Trim();

        if (a_Nick == "" || a_Nick.Length < 3)
        {
            MessageOnOff("�г����� ������� 3���� �̻� �Է����ּ���.", true);
            return;
        }

        if (!string.IsNullOrEmpty(a_Nick) && a_Nick != GlobalValue.g_NickName)
        { 
            NetworkMgr.Inst.m_NickCgBuff = a_Nick;
            NetworkMgr.Inst.PushPacket(PacketType.NickUpdate);
        }      
                

        ////--- ���� ���� UI ����
        //RefreshUserUI();
        ////--- ���� ���� UI ����
    }

    void RefreshUI()
    {
        //--- ���� ���� UI ����
        if (UserInfo_Text != null)
            UserInfo_Text.text = GlobalValue.g_NickName + "�� �����ݾ� : " + m_Money +
                                " : ��(" + m_WinCount + ")" +
                                " : ��(" + m_LostCount + ")";
        //--- ���� ���� UI ����

        //--- �̹� �� ���� UI ����
        if (BestWinCountText != null)
            BestWinCountText.text = "�̱�Ƚ�� : (" + m_WinCount + ")�� \n"
                                    +"���� : (" + m_Point + ")��";
                                    
        //--- �ְ� ���� UI ����
    }

    public void MessageOnOff(string Msg = "", bool isOn = true)
    {
        if (isOn == true)
        {
            Message_Text.text = Msg;
            Message_Text.gameObject.SetActive(true);
            showMessageTimer = 5.0f;
        }
        else
        {
            Message_Text.text = "";
            Message_Text.gameObject.SetActive(false);
        }
    }

}
