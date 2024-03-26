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

    int m_Money = 1000;     // 유저의 보유 금액
    int m_WinCount = 0;     // 승리 카운트
    int m_LostCount = 0;    // 패배 카운트


    [Header("--- Charater Image ---")]   // 유니티상 Inspector창에 나타나는 주석 
    public Image CharacterImg;           // Header아래에는 반드시 public 변수가 선언되어야 한다.
    public Sprite[] ResultImg; 
    //public Sprite WaitImg;
    //public Sprite WinImg;
    //public Sprite LostImg;
    public Image GameOverImg;

    [Header("--- Borrow ---")]
    public InputField NickInputField;
    public InputField BorrowInputField;
    public Button BorrowBtn;
    string m_NickName = "유저";

    [Header("--- Timer ---")]
    public Text Timer_Text;

    float m_WaitTimer = 0.0f;   //타이머 변수 선언

    // Start is called before the first frame update
    void Start()
    {
        if (Even_Btn != null)       // 버튼이 연결이 잘 되어 있으면 동작하라는 의미
            Even_Btn.onClick.AddListener(() => BtnClick(EvenOrOdd.Even));  // 버튼을 누르면 반응하는 함수를 대기 시켜 놓음

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
        if (m_Money <= 0) //게임 오버시 함수를 빠져나감
            return;

        if(0.0f < m_WaitTimer)    //타이머 돌리는 방법
        {
            m_WaitTimer -= Time.deltaTime; //Time.deltaTime : 한 프레임이 도는데 걸리는 시간
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

        //주사위 값 생성
        int a_DiceNum = Random.Range(1, 7);
        //주사위 값에 따른 한글 표기
        string a_StrCom = "짝수";
        if ((a_DiceNum % 2) == 1)
            a_StrCom = "홀수";

        if (a_UserSel == (a_DiceNum % 2))
        {
            Result_Text.text = "주사위 값은 (" + a_DiceNum + ") (" + a_StrCom + ") 맞췄습니다.";
            Win();
        }
        else  // 틀린 경우
        {
            Result_Text.text = "주사위 값은 (" + a_DiceNum + ") (" + a_StrCom + ") 틀렸습니다.";
            Lost();
        }

        //유저의 정보 UI 갱신
        UserInfo_Text.text = m_NickName + "의 보유금액 : " + m_Money +
            " : 승(" + m_WinCount + ")" +
            " : 패(" + m_LostCount + ")";

        //타이머 갱신
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

        if (m_Money <= 0)   // 보유 게임 머니가 모두 소진된 상태
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

    //    if (m_Money <= 0)  //버튼을 누를 때 게임 머니가 없다면
    //        return;  // 즉시 함수를 빠져 나가는 명령어 아래의 코드들이 실행되지 않음

    //    //Debug.Log("짝수 버튼 클릭");
    //    //Result_Text.text = "짝수 버튼 클릭";

    //    int a_UserSel = 0;  //유저의 선택 0번 짝수, 1번 홀수
    //    int a_DiceNum = Random.Range(1, 7);   // 1 ~ 6까지 랜덤값 발생

    //    string a_StrCom = "짝수";
    //    if ((a_DiceNum % 2) == 1)
    //        a_StrCom = "홀수";

    //    //판정
    //    if (a_UserSel == (a_DiceNum % 2))  // 맞춘 경우
    //    {
    //        Result_Text.text = "주사위 값은 (" + a_DiceNum + ") (" + a_StrCom + ") 맞췄습니다.";
    //        m_WinCount++;
    //        m_Money += 100;

    //        CharacterImg.sprite = WinImg;
    //    }
    //    else  // 틀린 경우
    //    {
    //        Result_Text.text = "주사위 값은 (" + a_DiceNum + ") (" + a_StrCom + ") 틀렸습니다.";
    //        m_LostCount++;
    //        m_Money -= 200;

    //        CharacterImg.sprite = LostImg;

    //        if (m_Money <= 0)   // 보유 게임 머니가 모두 소진된 상태
    //        {
    //            CharacterImg.gameObject.SetActive(false);
    //            GameOverImg.gameObject.SetActive(true);

    //            m_Money = 0;
    //            Result_Text.text = "Game Over";
    //        }
    //    }

    //    //유저의 정보 UI 갱신
    //    UserInfo_Text.text = m_NickName + "의 보유금액 : " + m_Money + 
    //        " : 승(" + m_WinCount + ")" + 
    //        " : 패(" + m_LostCount + ")";


    //    m_WaitTimer = 5.0f;

    //}//private void EvenBtnClick()

    //private void OddBtnClick()
    //{

    //    if (m_Money <= 0)  //버튼을 누를 때 게임 머니가 없다면
    //        return;  // 즉시 함수를 빠져 나가는 명령어 아래의 코드들이 실행되지 않음

    //    //Debug.Log("홀수 버튼을 눌렀어요.");
    //    //Result_Text.text = "홀수 버튼을 눌렀어요.";

    //    int a_UserSel = 1;  //유저의 선택 0번 짝수, 1번 홀수
    //    int a_DiceNum = Random.Range(1, 7);   // 1 ~ 6까지 랜덤값 발생

    //    string a_StrCom = "짝수";
    //    if ((a_DiceNum % 2) == 1)
    //        a_StrCom = "홀수";

    //    //판정
    //    if (a_UserSel == (a_DiceNum % 2))  // 맞춘 경우
    //    {
    //        Result_Text.text = "주사위 값은 (" + a_DiceNum + ") (" + a_StrCom + ") 맞췄습니다.";
    //        m_WinCount++;
    //        m_Money += 100;

    //        CharacterImg.sprite = WinImg;
    //    }
    //    else  // 틀린 경우
    //    {
    //        Result_Text.text = "주사위 값은 (" + a_DiceNum + ") (" + a_StrCom + ") 틀렸습니다.";
    //        m_LostCount++;
    //        m_Money -= 200;

    //        CharacterImg.sprite = LostImg;

    //        if (m_Money <= 0)   // 보유 게임 머니가 모두 소진된 상태
    //        {
    //            CharacterImg.gameObject.SetActive(false);
    //            GameOverImg.gameObject.SetActive(true);

    //            m_Money = 0;
    //            Result_Text.text = "Game Over";
    //        }
    //    }

    //    //유저의 정보 UI 갱신
    //    UserInfo_Text.text = m_NickName + "의 보유금액 : " + m_Money +
    //        " : 승(" + m_WinCount + ")" +
    //        " : 패(" + m_LostCount + ")";

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
        if (m_Money <= 0)  //게임오버 상태인데 돈을 빌리면 안되니까 리턴시키기
            return;

        string a_BValue = BorrowInputField.text;

        int a_CacBr = 0;
        int.TryParse(a_BValue, out a_CacBr);
        m_Money += a_CacBr;

        string a_Nick = NickInputField.text;
        if (a_Nick == "")
            m_NickName = "유저";
        else
            m_NickName = a_Nick;

        //유저의 정보 UI 갱신
        UserInfo_Text.text = m_NickName + "의 보유금액 : " + m_Money +
            " : 승(" + m_WinCount + ")" +
            " : 패(" + m_LostCount + ")";

    }//private void BorrowBtnClick()
}
