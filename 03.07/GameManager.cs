using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("---  Text  ---")]
    public Text RuleNCount_Text;
    public Text Com_Text;
    public Text Result_Text;

    [Header("---  Button  ---")]
    public Button GameStart_Btn;
    public Button Answer_Btn;
    public Button Up_Btn;
    public Button Down_Btn;
    public Button DownLiar_Btn;
    public Button Restart_Btn;

    [Header("---  Image  ---")]
    public RawImage ClickImg;
    public Image CharImg;
    public Sprite CharImg2;
    public Sprite CharLoseImg;
    public Sprite AnswerImg;

    int m_Count = 0;
    int m_UNMin = 1;
    int m_UNMax = 101;
    int m_UserNum;


    void Start()
    {
        if (GameStart_Btn != null)
            GameStart_Btn.onClick.AddListener(GameStartBtnClick);

        if (Answer_Btn != null)
            Answer_Btn.onClick.AddListener(AnswerBtnClick);

        if (Up_Btn != null)
            Up_Btn.onClick.AddListener(UpBtnClick);

        if (Down_Btn != null)
            Down_Btn.onClick.AddListener(DownBtnClick);

        if (DownLiar_Btn != null)
            DownLiar_Btn.onClick.AddListener(DownLiarBtnClick);

        if (Restart_Btn != null)
            Restart_Btn.onClick.AddListener(RestartBtnClick);
    }//void Start()




    //    void Update()
    //    {

    //    }


    private void GameStartBtnClick()
    {
        Answer_Btn.gameObject.SetActive(true);
        Up_Btn.gameObject.SetActive(true);
        Down_Btn.gameObject.SetActive(true);
        GameStart_Btn.gameObject.SetActive(false);
        ClickImg.gameObject.SetActive(false);
        CharImg.gameObject.SetActive(true);

        m_UserNum = Random.Range(m_UNMin, m_UNMax);

        // UI Update
        RuleNCount_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";
        Com_Text.text = "����� ������ ���ڴ� " + m_UserNum + "�ΰ���?";
    }//private void GameStartBtnClick()

    private void AnswerBtnClick()
    {
        Answer_Btn.gameObject.SetActive(false);
        Up_Btn.gameObject.SetActive(false);
        Down_Btn.gameObject.SetActive(false);
        Restart_Btn.gameObject.SetActive(true);
        Result_Text.gameObject.SetActive(true);
        CharImg.sprite = AnswerImg;

        Com_Text.text = "������ �ٽ� �Ϸ��� RESTART ��ư�� Click!";
        Result_Text.text = "����� ������ ���ڴ� " + m_UserNum + "�Դϴ�!";
    }//private void AnswerBtnClick()

    private void UpBtnClick()
    {
        // Character Change
        if (m_Count >= 5)
            CharImg.sprite = CharImg2;

        // 1���� ���� ���� �������� �ʾ��� �� Liar��ư�� Ȱ��ȭ �Ǵ� ��� ����
        DownLiar_Btn.gameObject.SetActive(false);
        Down_Btn.gameObject.SetActive(true);
       
        // Click Count
        m_Count++;
        RuleNCount_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

        // Judge
        m_UNMin = m_UserNum + 1;
        m_UserNum = Random.Range(m_UNMin, m_UNMax);
        Com_Text.text = "����� ������ ���ڴ� " + m_UserNum + "�ΰ���?";

        // 100�� ��µǾ��µ� �� �ٽ� Up��ư�� ���� ���
        if (m_UserNum >= 101)   
        {
            Com_Text.text = "������ �ٽ� �Ϸ��� RESTART ��ư�� Click!";
            Result_Text.text = "����� ���������̿���!";

            CharImg.sprite = CharLoseImg;

            Answer_Btn.gameObject.SetActive(false);
            Up_Btn.gameObject.SetActive(false);
            Down_Btn.gameObject.SetActive(false);
            Restart_Btn.gameObject.SetActive(true);
            Result_Text.gameObject.SetActive(true);
        }
        
        // ����Ƚ���� 20ȸ �����ϸ� ��ǻ�� Lose
        if (m_Count >= 20)   
        {
            Com_Text.text = "������ �ٽ� �Ϸ��� RESTART ��ư�� Click!";
            Result_Text.text = "���� ����� �Ф�  �𸣰ھ��!";

            CharImg.sprite = CharLoseImg;

            Answer_Btn.gameObject.SetActive(false);
            Up_Btn.gameObject.SetActive(false);
            Down_Btn.gameObject.SetActive(false);
            Restart_Btn.gameObject.SetActive(true);
            Result_Text.gameObject.SetActive(true);
        }

    }//private void UpBtnClick()

    private void DownBtnClick()
    {
        // Character Change
        if (m_Count >= 5)
            CharImg.sprite = CharImg2;

        // Click Count 
        m_Count++;
        RuleNCount_Text.text = "���� Ƚ�� : 20�� �� " + m_Count + "��";

        // Judge
        m_UNMax = m_UserNum;
        m_UserNum = Random.Range(m_UNMin, m_UNMax);
        Com_Text.text = "����� ������ ���ڴ� " + m_UserNum + "�ΰ���?";

        // 1���� ���� ���� �������� �� Liar_Btn Ȱ��ȭ
        if (m_UserNum == 1)
        {
            Com_Text.text = "����� ������ ���ڴ� " + m_UserNum + "�ΰ���?";
            DownLiar_Btn.gameObject.SetActive(true);
            Down_Btn.gameObject.SetActive(false);
        }

        // ����Ƚ���� 20ȸ �����ϸ� ��ǻ�� Lose
        if (m_Count >= 20)    
        {
            Com_Text.text = "������ �ٽ� �Ϸ��� RESTART ��ư�� Click!";
            Result_Text.text = "���� ����� �Ф�  �𸣰ھ��!";

            CharImg.sprite = CharLoseImg;

            Answer_Btn.gameObject.SetActive(false);
            Up_Btn.gameObject.SetActive(false);
            Down_Btn.gameObject.SetActive(false);
            Restart_Btn.gameObject.SetActive(true);
            Result_Text.gameObject.SetActive(true);
        }
    }//private void DownBtnClick()

    private void DownLiarBtnClick()
    {
        // 1�� ��µǾ��µ� �� �ٽ� Down��ư�� ���� ���
        if (m_UserNum == 1)
        {
            Com_Text.text = "������ �ٽ� �Ϸ��� RESTART ��ư�� Click!";
            Result_Text.text = "����� ���������̿���!";

            CharImg.sprite = CharLoseImg;

            Answer_Btn.gameObject.SetActive(false);
            Up_Btn.gameObject.SetActive(false);
            DownLiar_Btn.gameObject.SetActive(false);
            Restart_Btn.gameObject.SetActive(true);
            Result_Text.gameObject.SetActive(true);
        }
    }//private void DownLiarBtnClick()

    private void RestartBtnClick()
    {
        SceneManager.LoadScene("UpDownGame");
    }//private void RestartBtnClick()
}


