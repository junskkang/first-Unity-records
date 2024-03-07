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
        RuleNCount_Text.text = "진행 횟수 : 20번 중 " + m_Count + "번";
        Com_Text.text = "당신이 생각한 숫자는 " + m_UserNum + "인가요?";
    }//private void GameStartBtnClick()

    private void AnswerBtnClick()
    {
        Answer_Btn.gameObject.SetActive(false);
        Up_Btn.gameObject.SetActive(false);
        Down_Btn.gameObject.SetActive(false);
        Restart_Btn.gameObject.SetActive(true);
        Result_Text.gameObject.SetActive(true);
        CharImg.sprite = AnswerImg;

        Com_Text.text = "게임을 다시 하려면 RESTART 버튼을 Click!";
        Result_Text.text = "당신이 생각한 숫자는 " + m_UserNum + "입니다!";
    }//private void AnswerBtnClick()

    private void UpBtnClick()
    {
        // Character Change
        if (m_Count >= 5)
            CharImg.sprite = CharImg2;

        // 1보다 작은 수를 생각하지 않았을 때 Liar버튼이 활성화 되는 경우 방지
        DownLiar_Btn.gameObject.SetActive(false);
        Down_Btn.gameObject.SetActive(true);
       
        // Click Count
        m_Count++;
        RuleNCount_Text.text = "진행 횟수 : 20번 중 " + m_Count + "번";

        // Judge
        m_UNMin = m_UserNum + 1;
        m_UserNum = Random.Range(m_UNMin, m_UNMax);
        Com_Text.text = "당신이 생각한 숫자는 " + m_UserNum + "인가요?";

        // 100이 출력되었는데 또 다시 Up버튼을 누른 경우
        if (m_UserNum >= 101)   
        {
            Com_Text.text = "게임을 다시 하려면 RESTART 버튼을 Click!";
            Result_Text.text = "당신은 거짓말쟁이에요!";

            CharImg.sprite = CharLoseImg;

            Answer_Btn.gameObject.SetActive(false);
            Up_Btn.gameObject.SetActive(false);
            Down_Btn.gameObject.SetActive(false);
            Restart_Btn.gameObject.SetActive(true);
            Result_Text.gameObject.SetActive(true);
        }
        
        // 진행횟수가 20회 도달하면 컴퓨터 Lose
        if (m_Count >= 20)   
        {
            Com_Text.text = "게임을 다시 하려면 RESTART 버튼을 Click!";
            Result_Text.text = "제가 졌어요 ㅠㅠ  모르겠어요!";

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
        RuleNCount_Text.text = "진행 횟수 : 20번 중 " + m_Count + "번";

        // Judge
        m_UNMax = m_UserNum;
        m_UserNum = Random.Range(m_UNMin, m_UNMax);
        Com_Text.text = "당신이 생각한 숫자는 " + m_UserNum + "인가요?";

        // 1보다 작은 수를 생각했을 때 Liar_Btn 활성화
        if (m_UserNum == 1)
        {
            Com_Text.text = "당신이 생각한 숫자는 " + m_UserNum + "인가요?";
            DownLiar_Btn.gameObject.SetActive(true);
            Down_Btn.gameObject.SetActive(false);
        }

        // 진행횟수가 20회 도달하면 컴퓨터 Lose
        if (m_Count >= 20)    
        {
            Com_Text.text = "게임을 다시 하려면 RESTART 버튼을 Click!";
            Result_Text.text = "제가 졌어요 ㅠㅠ  모르겠어요!";

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
        // 1이 출력되었는데 또 다시 Down버튼을 누를 경우
        if (m_UserNum == 1)
        {
            Com_Text.text = "게임을 다시 하려면 RESTART 버튼을 Click!";
            Result_Text.text = "당신은 거짓말쟁이에요!";

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


