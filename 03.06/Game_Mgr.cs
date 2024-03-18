using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum EvenOrOdd
{
    Even,
    Odd
}

public

public class Game_Mgr : MonoBehaviour
{
    public Button Even_Btn;
    public Button Odd_Btn;
    public Button Replay_Btn;

    public Text UserInfo_Text;
    public Text Result_Text;
    
    public Button Back_Btn;

    int m_Money = 1000;     // ������ ���� �ݾ�
    int m_WinCount = 0;     // �¸� ī��Ʈ
    int m_LostCount = 0;    // �й� ī��Ʈ


    [Header("--- Charater Image ---")]   // ����Ƽ�� Inspectorâ�� ��Ÿ���� �ּ� 
    public Image CharacterImg;           // Header�Ʒ����� �ݵ�� public ������ ����Ǿ�� �Ѵ�.
    public Sprite[] ResultImg; 
    //public Sprite WaitImg;
    //public Sprite WinImg;
    //public Sprite LostImg;
    public Image GameOverImg;

    [Header("--- Borrow ---")]
    public InputField NickInputField;
    public InputField BorrowInputField;
    public Button BorrowBtn;
    string m_NickName = "����";

    [Header("--- Timer ---")]
    public Text Timer_Text;

    float m_WaitTimer = 0.0f;   //Ÿ�̸� ���� ����

    // Start is called before the first frame update
    void Start()
    {
        if (Even_Btn != null)       // ��ư�� ������ �� �Ǿ� ������ �����϶�� �ǹ�
            Even_Btn.onClick.AddListener(() => BtnClick(EvenOrOdd.Even));  // ��ư�� ������ �����ϴ� �Լ��� ��� ���� ����

        if (Odd_Btn != null)
            Odd_Btn.onClick.AddListener(() => BtnClick(EvenOrOdd.Odd));

        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(ReplayBtnClick);

        if (Back_Btn != null)
            Back_Btn.onClick.AddListener(BackBtnClick);

        if (BorrowBtn != null)
            BorrowBtn.onClick.AddListener(BorrowBtnClick);
    }//void Start()



    void Update()
    {
        if (m_Money <= 0) //���� ������ �Լ��� ��������
            return;

        if(0.0f < m_WaitTimer)    //Ÿ�̸� ������ ���
        {
            m_WaitTimer -= Time.deltaTime; //Time.deltaTime : �� �������� ���µ� �ɸ��� �ð�
            Timer_Text.text = m_WaitTimer.ToString("F2");

            if (m_WaitTimer <= 0.0f)
            {
                CharacterImg.sprite = WaitImg;
            }    
        }
    }//void Update()

    public void BtnClick(EvenOrOdd a_UserSel)
    {
        if (m_Money <= 0)
            return;

        //�ֻ��� �� ����
        int a_DiceNum = Random.Range(1, 7);
        //�ֻ��� ���� ���� �ѱ� ǥ��
        string a_StrCom = "¦��";
        if ((a_DiceNum % 2) == 1)
            a_StrCom = "Ȧ��";

        if (a_UserSel == (a_DiceNum % 2))
        {
            Result_Text.text = "�ֻ��� ���� (" + a_DiceNum + ") (" + a_StrCom + ") ������ϴ�.";
            Win();
        }
        else  // Ʋ�� ���
        {
            Result_Text.text = "�ֻ��� ���� (" + a_DiceNum + ") (" + a_StrCom + ") Ʋ�Ƚ��ϴ�.";
            Lost();
        }

        //������ ���� UI ����
        UserInfo_Text.text = m_NickName + "�� �����ݾ� : " + m_Money +
            " : ��(" + m_WinCount + ")" +
            " : ��(" + m_LostCount + ")";

        //Ÿ�̸� ����
        m_WaitTimer = 5.0f;  

    }

    void Win()
    {
        m_WinCount++;
        m_Money += 100;

        CharacterImg.sprite = ResultImg[1];
    }

    void Lost()
    {
        m_LostCount++;
        m_Money -= 200;

        CharacterImg.sprite = ResultImg[2];

        if (m_Money <= 0)   // ���� ���� �Ӵϰ� ��� ������ ����
        {
            CharacterImg.gameObject.SetActive(false);
            GameOverImg.gameObject.SetActive(true);

            m_Money = 0;
            Result_Text.text = "Game Over";
        }
    }
    #region
    //private void EvenBtnClick()
    //{

    //    if (m_Money <= 0)  //��ư�� ���� �� ���� �Ӵϰ� ���ٸ�
    //        return;  // ��� �Լ��� ���� ������ ��ɾ� �Ʒ��� �ڵ���� ������� ����

    //    //Debug.Log("¦�� ��ư Ŭ��");
    //    //Result_Text.text = "¦�� ��ư Ŭ��";

    //    int a_UserSel = 0;  //������ ���� 0�� ¦��, 1�� Ȧ��
    //    int a_DiceNum = Random.Range(1, 7);   // 1 ~ 6���� ������ �߻�

    //    string a_StrCom = "¦��";
    //    if ((a_DiceNum % 2) == 1)
    //        a_StrCom = "Ȧ��";

    //    //����
    //    if (a_UserSel == (a_DiceNum % 2))  // ���� ���
    //    {
    //        Result_Text.text = "�ֻ��� ���� (" + a_DiceNum + ") (" + a_StrCom + ") ������ϴ�.";
    //        m_WinCount++;
    //        m_Money += 100;

    //        CharacterImg.sprite = WinImg;
    //    }
    //    else  // Ʋ�� ���
    //    {
    //        Result_Text.text = "�ֻ��� ���� (" + a_DiceNum + ") (" + a_StrCom + ") Ʋ�Ƚ��ϴ�.";
    //        m_LostCount++;
    //        m_Money -= 200;

    //        CharacterImg.sprite = LostImg;

    //        if (m_Money <= 0)   // ���� ���� �Ӵϰ� ��� ������ ����
    //        {
    //            CharacterImg.gameObject.SetActive(false);
    //            GameOverImg.gameObject.SetActive(true);

    //            m_Money = 0;
    //            Result_Text.text = "Game Over";
    //        }
    //    }

    //    //������ ���� UI ����
    //    UserInfo_Text.text = m_NickName + "�� �����ݾ� : " + m_Money + 
    //        " : ��(" + m_WinCount + ")" + 
    //        " : ��(" + m_LostCount + ")";


    //    m_WaitTimer = 5.0f;

    //}//private void EvenBtnClick()

    //private void OddBtnClick()
    //{

    //    if (m_Money <= 0)  //��ư�� ���� �� ���� �Ӵϰ� ���ٸ�
    //        return;  // ��� �Լ��� ���� ������ ��ɾ� �Ʒ��� �ڵ���� ������� ����

    //    //Debug.Log("Ȧ�� ��ư�� �������.");
    //    //Result_Text.text = "Ȧ�� ��ư�� �������.";

    //    int a_UserSel = 1;  //������ ���� 0�� ¦��, 1�� Ȧ��
    //    int a_DiceNum = Random.Range(1, 7);   // 1 ~ 6���� ������ �߻�

    //    string a_StrCom = "¦��";
    //    if ((a_DiceNum % 2) == 1)
    //        a_StrCom = "Ȧ��";

    //    //����
    //    if (a_UserSel == (a_DiceNum % 2))  // ���� ���
    //    {
    //        Result_Text.text = "�ֻ��� ���� (" + a_DiceNum + ") (" + a_StrCom + ") ������ϴ�.";
    //        m_WinCount++;
    //        m_Money += 100;

    //        CharacterImg.sprite = WinImg;
    //    }
    //    else  // Ʋ�� ���
    //    {
    //        Result_Text.text = "�ֻ��� ���� (" + a_DiceNum + ") (" + a_StrCom + ") Ʋ�Ƚ��ϴ�.";
    //        m_LostCount++;
    //        m_Money -= 200;

    //        CharacterImg.sprite = LostImg;

    //        if (m_Money <= 0)   // ���� ���� �Ӵϰ� ��� ������ ����
    //        {
    //            CharacterImg.gameObject.SetActive(false);
    //            GameOverImg.gameObject.SetActive(true);

    //            m_Money = 0;
    //            Result_Text.text = "Game Over";
    //        }
    //    }

    //    //������ ���� UI ����
    //    UserInfo_Text.text = m_NickName + "�� �����ݾ� : " + m_Money +
    //        " : ��(" + m_WinCount + ")" +
    //        " : ��(" + m_LostCount + ")";

    //    m_WaitTimer = 5.0f;

    //}//private void OddBtnClick()
    #endregion
    private void ReplayBtnClick()
    {
        SceneManager.LoadScene("EvenOddGame");
    }//private void ReplayBtnClick()



    private void BackBtnClick()
    {
        SceneManager.LoadScene("LobbyScene");
    }//private void BackBtnClick()

    private void BorrowBtnClick()
    {
        if (m_Money <= 0)  //���ӿ��� �����ε� ���� ������ �ȵǴϱ� ���Ͻ�Ű��
            return;

        string a_BValue = BorrowInputField.text;

        int a_CacBr = 0;
        int.TryParse(a_BValue, out a_CacBr);
        m_Money += a_CacBr;

        string a_Nick = NickInputField.text;
        if (a_Nick == "")
            m_NickName = "����";
        else
            m_NickName = a_Nick;

        //������ ���� UI ����
        UserInfo_Text.text = m_NickName + "�� �����ݾ� : " + m_Money +
            " : ��(" + m_WinCount + ")" +
            " : ��(" + m_LostCount + ")";

    }//private void BorrowBtnClick()
}
