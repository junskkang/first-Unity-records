using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_Mgr : MonoBehaviour
{
    public Button Gawi_Btn;      //���� ��ư ���� ���� ����
    public Button Bawi_Btn;      //���� ��ư ���� ���� ����
    public Button Bo_Btn;        //�� ��ư ���� ���� ����
    public Button Replay_Btn;    //���Ӵٽ��ϱ� ��ư ���� ���� ����

    public Text UserInfo_text;   //���� ���� ǥ�� �ؽ�Ʈ
    public Text Result_Text;     //��� ǥ�� �ؽ�Ʈ

    int m_Money = 1000;          //������ ���� �ݾ�
    int m_WinCount = 0;          //�¸� ī��Ʈ
    int m_LostCount = 0;         //�й� ī��Ʈ
    float m_Timer = 3.0f;         //Ÿ�̸� ī��Ʈ


    [Header("--- ShowUserData ---")]
    public InputField NickNameIF;  //���� �Է� ����
    public Button NickInput_Btn;   //���� �Է� ��ư
    string m_NickName = "����";

    [Header("--- Direction Image ---")]
    public Image UserGBB_Img;     //UserSelPanel�� GBB Image
    public Image ComGBB_Img;      //ComSelPanel�� GBB Image

    public Sprite GawiIcon;       //���� �ؽ���
    public Sprite BawiIcon;       //���� �ؽ���
    public Sprite BoIcon;         //�� �ؽ���

    void Start()
    {
        if (Gawi_Btn != null)     //�ش� ������ ������Ʈ�� ����Ǿ� �ִٸ�(���� �־����ٸ�)
            Gawi_Btn.onClick.AddListener(GawiBtnClick);
        if (Bawi_Btn != null)
            Bawi_Btn.onClick.AddListener(BawiBtnClick);
        if (Bo_Btn != null)
            Bo_Btn.onClick.AddListener(BoBtnClick);
        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() =>
            {
                    SceneManager.LoadScene("SampleScene");
            });

        // User Data Loading
        //if (NickNameIF != null)
        //    NickNameIF.text = ;

        if (NickInput_Btn != null)
            NickInput_Btn.onClick.AddListener(NickBtnClick);
    }

    void Update()
    {
        if (m_Money <= 0)
            return;

        if (m_Timer > 0.0f)
        {
            m_Timer -= Time.deltaTime;
            if (m_Timer <= 0.0f)
                UserGBB_Img.gameObject.SetActive(false);
        }
        
    }

    
    private void GawiBtnClick()
    {

        if (m_Money <= 0)  //���ӸӴϰ� �����Ǿ����� ������ �۵����� �ʵ��� �ϴ� ����
            return;

        m_Timer = 3.0f;


        // 1, ����     2, ����     3, ��
        int a_ComSel = UnityEngine.Random.Range(1, 4);     // Random.Range(�ּڰ��̻�, �ִ񰪹̸�)

        string a_strCom = "����";
        if (a_ComSel == 2)
            a_strCom = "����";
        else if (a_ComSel == 3)
            a_strCom = "��";

        Result_Text.text = "User(����) : " + "Com(" + a_strCom + ") ";


        //����   ������ ���� ���·� ����
        if(1 == a_ComSel)  // ��� ���
        {
            Result_Text.text += "�����ϴ�.";
        }
        else if (3 == a_ComSel)   // ������ �̱� ���
        {
            Result_Text.text += "�̰���ϴ�.";

            m_WinCount++;
            m_Money += 100;
        }
        else   // �� ���
        {
            Result_Text.text += "�����ϴ�.";

            m_LostCount++;
            m_Money -= 200;

            //Game Over
            if (m_Money <= 0 )  //���ӸӴϰ� ��� ���� �ƴٸ�
            {
                m_Money = 0;
                Result_Text.text = "Game Over";
            }
               
        }

        //���� ���� UI ����
        if (UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "�� �����ݾ� : " + m_Money +
                " : ��(" + m_WinCount +
                ") : ��(" + m_LostCount + ")";
        }

        //���ÿ� ���� �̹��� ��ü
        UserGBB_Img.gameObject.SetActive(true);   //��Ȱ��ȭ �Ǿ� �ִ� ������Ʈ�� Ȱ��ȭ �ϴ� �ڵ�
        UserGBB_Img.sprite = GawiIcon;

        if (a_ComSel == 1)         //��ǻ���� ������ ����
            ComGBB_Img.sprite = GawiIcon;
        else if (a_ComSel == 2)    //��ǻ���� ������ ����
            ComGBB_Img.sprite = BawiIcon;
        else if (a_ComSel == 3)    //��ǻ���� ������ ��
            ComGBB_Img.sprite = BoIcon;
        
        
    }
    private void BawiBtnClick()
    {
        if (m_Money <= 0)
            return;

        m_Timer = 3.0f;

        // 1, ����     2, ����     3, ��
        int a_ComSel = UnityEngine.Random.Range(1, 4);     // Random.Range(�ּڰ��̻�, �ִ񰪹̸�)

        string a_strCom = "����";
        if (a_ComSel == 2)
            a_strCom = "����";
        else if (a_ComSel == 3)
            a_strCom = "��";

        Result_Text.text = "User(����) : " + "Com(" + a_strCom + ") ";


        //����   ������ ���� ���·� ����
        if (2 == a_ComSel)  // ��� ���
        {
            Result_Text.text += "�����ϴ�.";
        }
        else if (1 == a_ComSel)   // ������ �̱� ���
        {
            Result_Text.text += "�̰���ϴ�.";

            m_WinCount++;
            m_Money += 100;
        }
        else   // �� ���
        {
            Result_Text.text += "�����ϴ�.";

            m_LostCount++;
            m_Money -= 200;

            //Game Over
            if (m_Money <= 0)  //���ӸӴϰ� ��� ���� �ƴٸ�
            {
                m_Money = 0;
                Result_Text.text = "Game Over";
            }

        }

        //���� ���� UI ����
        if (UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "�� �����ݾ� : " + m_Money +
                " : ��(" + m_WinCount +
                ") : ��(" + m_LostCount + ")";
        }

        //���ÿ� ���� �̹��� ��ü
        UserGBB_Img.gameObject.SetActive(true);   //��Ȱ��ȭ �Ǿ� �ִ� ������Ʈ�� Ȱ��ȭ �ϴ� �ڵ�
        UserGBB_Img.sprite = BawiIcon;

        if (a_ComSel == 1)         //��ǻ���� ������ ����
            ComGBB_Img.sprite = GawiIcon;
        else if (a_ComSel == 2)    //��ǻ���� ������ ����
            ComGBB_Img.sprite = BawiIcon;
        else if (a_ComSel == 3)    //��ǻ���� ������ ��
            ComGBB_Img.sprite = BoIcon;

    }
    private void BoBtnClick()
    {
        if (m_Money <= 0)
            return;

        m_Timer = 3.0f;

        // 1, ����     2, ����     3, ��
        int a_ComSel = UnityEngine.Random.Range(1, 4);     // Random.Range(�ּڰ��̻�, �ִ񰪹̸�)

        string a_strCom = "����";
        if (a_ComSel == 2)
            a_strCom = "����";
        else if (a_ComSel == 3)
            a_strCom = "��";

        Result_Text.text = "User(��) : " + "Com(" + a_strCom + ") ";


        //����   ������ �� ���·� ����
        if (3 == a_ComSel)  // ��� ���
        {
            Result_Text.text += "�����ϴ�.";
        }
        else if (2 == a_ComSel)   // ������ �̱� ���
        {
            Result_Text.text += "�̰���ϴ�.";

            m_WinCount++;
            m_Money += 100;
        }
        else   // �� ���
        {
            Result_Text.text += "�����ϴ�.";

            m_LostCount++;
            m_Money -= 200;

            //Game Over
            if (m_Money <= 0)  //���ӸӴϰ� ��� ���� �ƴٸ�
            {
                m_Money = 0;
                Result_Text.text = "Game Over";
            }

        }

        //���� ���� UI ����
        if (UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "�� �����ݾ� : " + m_Money +
                " : ��(" + m_WinCount +
                ") : ��(" + m_LostCount + ")";
        }

        //���ÿ� ���� �̹��� ��ü
        UserGBB_Img.gameObject.SetActive(true);   //��Ȱ��ȭ �Ǿ� �ִ� ������Ʈ�� Ȱ��ȭ �ϴ� �ڵ�
        UserGBB_Img.sprite = BoIcon;

        if (a_ComSel == 1)         //��ǻ���� ������ ����
            ComGBB_Img.sprite = GawiIcon;
        else if (a_ComSel == 2)    //��ǻ���� ������ ����
            ComGBB_Img.sprite = BawiIcon;
        else if (a_ComSel == 3)    //��ǻ���� ������ ��
            ComGBB_Img.sprite = BoIcon;

    }
    //private void ReplayBtnClick()
    //{
    //    SceneManager.LoadScene("SampleScene");
    //}

    private void NickBtnClick()
    {
        if (m_Money <= 0)
            return;

        string a_Nick = NickNameIF.text;
        if (a_Nick == "")
            m_NickName = "����";
        else
            m_NickName = a_Nick;

        if(UserInfo_text != null)
        {
            UserInfo_text.text = m_NickName + "�� �����ݾ� : " + m_Money +
                " : ��(" + m_WinCount + ")" +
                " : ��(" + m_LostCount + ")";
        }
    }
}
