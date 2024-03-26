using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Start Scene")]
    public GameObject rule;
    public Button startBtn;

    [Header("Play Scene")]
    public GameObject round;
    public GameObject roundResult;
    public Text Info;
    public Text numInputText;
    public Text[] roundResultText;
    public Button[] NumBtn;
    public Button resetBtn;
    public Button throwBtn;
    public InputField NumInput;

    [Header("Result Scene")]
    public GameObject result;
    public Text resultText;
    public Text roundCountText;
    public Button restartBtn;

    //computer number
    string comNum;
    int comNum1;
    int comNum2;
    int comNum3;

    //player number
    string playerNum;
    int[] pNum;

    //count
    int strikeCount;
    int ballCount;
    int outCount;
    int roundCount;

    //number keypad count
    const int numKeypad = 9; 

    void Start()
    {
        if (startBtn != null)
            startBtn.onClick.AddListener(GameStart);
        if (restartBtn != null)
            restartBtn.onClick.AddListener(RestartClick);

        //for문과 람다식을 이용한 매개변수를 갖는 함수 만들기
        for (int i = 0; i < numKeypad; i++)
        {
            if (NumBtn[i] != null)
            {
                int num = i;
                NumBtn[i].onClick.AddListener(() => NumClick(num));


                Debug.Log("NumBtn[" + i + "] = " + NumBtn[i]);
            }
        }


        //if (NumBtn[1] != null)
        //    NumBtn[1].onClick.AddListener(Num2Click);
        //if (NumBtn[2] != null)
        //    NumBtn[2].onClick.AddListener(Num3Click);
        //if (NumBtn[3] != null)
        //    NumBtn[3].onClick.AddListener(Num4Click);
        //if (NumBtn[4] != null)
        //    NumBtn[4].onClick.AddListener(Num5Click);
        //if (NumBtn[5] != null)
        //    NumBtn[5].onClick.AddListener(Num6Click);
        //if (NumBtn[6] != null)
        //    NumBtn[6].onClick.AddListener(Num7Click);
        //if (NumBtn[7] != null)
        //    NumBtn[7].onClick.AddListener(Num8Click);
        //if (NumBtn[8] != null)
        //    NumBtn[8].onClick.AddListener(Num9Click);

        if (throwBtn != null)
            throwBtn.onClick.AddListener(ThrowClick);
        if (resetBtn != null)
            resetBtn.onClick.AddListener(resetClick);
    }



    void Update()
    {

    }

    private void ThrowClick()
    {
        playerNum = NumInput.text;
        pNum = new int[playerNum.Length];
        for (int i = 0; i < pNum.Length; i++)
        {
            pNum[i] = (int)Char.GetNumericValue(playerNum[i]);
        }
        //Debug.Log(pNum[0]); // 100자리 유저가 선택한 숫자
        //Debug.Log(pNum[1]); // 10자리 유저가 선택한 숫자
        //Debug.Log(pNum[2]); // 1자리 유저가 선택한 숫자

        Judge();

        NumKeypadOn();

        NumInput.text = "";   
    }

    private void Judge()
    {
        //숫자, 자릿수 모두 단 하나도 맞지 않는 경우
        if (pNum[0] != comNum1 && pNum[0] != comNum2 && pNum[0] != comNum3
            && pNum[1] != comNum1 && pNum[1] != comNum2 && pNum[1] != comNum3
            && pNum[2] != comNum1 && pNum[2] != comNum2 && pNum[2] != comNum3)
        {
            outCount++;
            roundResultText[roundCount].text = pNum[0].ToString() + pNum[1].ToString() + pNum[2].ToString() + "    S" + strikeCount + " B" + ballCount + " O" + outCount;
            Info.text = outCount + "아웃!";
        }
        else
        {
            //100의 자리수 비교
            if (pNum[0] == comNum1)
            {
                strikeCount++;
                roundResultText[roundCount].text = pNum[0].ToString() + pNum[1].ToString() + pNum[2].ToString() + "    S" + strikeCount + " B" + ballCount + " O" + outCount;
            }
            else if (pNum[0] == comNum2 || pNum[0] == comNum3)
            {
                ballCount++;
                roundResultText[roundCount].text = pNum[0].ToString() + pNum[1].ToString() + pNum[2].ToString() + "    S" + strikeCount + " B" + ballCount + " O" + outCount;
            }

            //10의 자리수 비교
            if (pNum[1] == comNum2)
            {
                strikeCount++;
                roundResultText[roundCount].text = pNum[0].ToString() + pNum[1].ToString() + pNum[2].ToString() + "    S" + strikeCount + " B" + ballCount + " O" + outCount;
            }
            else if (pNum[1] == comNum1 || pNum[1] == comNum3)
            {
                ballCount++;
                roundResultText[roundCount].text = pNum[0].ToString() + pNum[1].ToString() + pNum[2].ToString() + "    S" + strikeCount + " B" + ballCount + " O" + outCount;
            }

            //1의 자리수 비교
            if (pNum[2] == comNum3)
            {
                strikeCount++;
                roundResultText[roundCount].text = pNum[0].ToString() + pNum[1].ToString() + pNum[2].ToString() + "    S" + strikeCount + " B" + ballCount + " O" + outCount;
            }
            else if (pNum[2] == comNum1 || pNum[2] == comNum2)
            {
                ballCount++;
                roundResultText[roundCount].text = pNum[0].ToString() + pNum[1].ToString() + pNum[2].ToString() + "    S" + strikeCount + " B" + ballCount + " O" + outCount;
            }

            if (strikeCount > 0 && ballCount > 0)
                Info.text = strikeCount + "스트~라잌! " + ballCount + "볼!";
            else if (strikeCount == 0)
                Info.text = ballCount + "볼!";
            else if (ballCount == 0)
                Info.text = strikeCount + "스트~라잌!";
        }



        roundCount++;

        if (strikeCount == 3)
            PlayerWin();
        else if (outCount == 3)
            ThreeOut();
        else if (roundCount == 12)
            PlayerLose();


        strikeCount = 0;
        ballCount = 0;
    }
    private void PlayerWin()
    {
        round.gameObject.SetActive(false);
        roundResult.gameObject.SetActive(false);
        result.gameObject.SetActive(true);
        Info.text = "COM의 숫자 : " + comNum;
        roundCountText.text = "진행 라운드 " + roundCount + "회";
    }
    private void ThreeOut()
    {
        round.gameObject.SetActive(false);
        roundResult.gameObject.SetActive(false);
        result.gameObject.SetActive(true);
        resultText.text = "삼진아웃! LOL";
        roundCountText.text = "진행 라운드 " + roundCount + "회";
        Info.text = "COM의 숫자 : " + comNum;
    }

    private void PlayerLose()
    {
        round.gameObject.SetActive(false);
        roundResult.gameObject.SetActive(false);
        result.gameObject.SetActive(true);
        resultText.text = "GAME OVER";
        roundCountText.text = "진행 라운드 " + roundCount + "회";
        Info.text = "COM의 숫자 : " + comNum;
    }

    private void ComNumber()
    {
        comNum1 = UnityEngine.Random.Range(1, 10);
        comNum2 = UnityEngine.Random.Range(1, 10);
        comNum3 = UnityEngine.Random.Range(1, 10);

        if (comNum1 == comNum2 || comNum1 == comNum3 || comNum2 == comNum3)
            ComNumber();
        else 
        {
            comNum = comNum1.ToString() + comNum2.ToString() + comNum3.ToString();
            //Debug.Log(comNum);
        }
    }

    public void NumClick(int num)
    {
        //람다식 사용
        NumInput.text += (num + 1).ToString();
        NumBtn[num].enabled = false;

        //유니티 상에서 On Click() 연결 + if(매개변수 값)
        //if (num == 1)
        //{
        //    NumInput.text += "1";
        //    NumBtn[0].enabled = false;
        //}
        //else if (num == 2)
        //{
        //    NumInput.text += "2";
        //    NumBtn[1].enabled = false;
        //}
        //else if (num == 3)
        //{
        //    NumInput.text += "3";
        //    NumBtn[2].enabled = false;
        //}
        //else if (num == 4)
        //{
        //    NumInput.text += "4";
        //    NumBtn[3].enabled = false;
        //}
        //else if (num == 5)
        //{
        //    NumInput.text += "5";
        //    NumBtn[4].enabled = false;
        //}
        //else if (num == 6)
        //{
        //    NumInput.text += "6";
        //    NumBtn[5].enabled = false;
        //}
        //else if (num == 7)
        //{
        //    NumInput.text += "7";
        //    NumBtn[6].enabled = false;
        //}
        //else if (num == 8)
        //{
        //    NumInput.text += "8";
        //    NumBtn[7].enabled = false;
        //}
        //else if (num == 9)
        //{
        //    NumInput.text += "9";
        //    NumBtn[8].enabled = false;
        //}

        //유니티 상에서 On Click() 연결 + Switch(매개변수) case :
        //switch (num)
        //{
        //    case 1:
        //        NumInput.text += "1";
        //        NumBtn[0].enabled = false;
        //        break;

        //    case 2:
        //        NumInput.text += "2";
        //        NumBtn[1].enabled = false;
        //        break;

        //    case 3:
        //        NumInput.text += "3";
        //        NumBtn[2].enabled = false;
        //        break;

        //    case 4:
        //        NumInput.text += "4";
        //        NumBtn[3].enabled = false;
        //        break;

        //    case 5:
        //        NumInput.text += "5";
        //        NumBtn[4].enabled = false;
        //        break;

        //    case 6:
        //        NumInput.text += "6";
        //        NumBtn[5].enabled = false;
        //        break;

        //    case 7:
        //        NumInput.text += "7";
        //        NumBtn[6].enabled = false;
        //        break;

        //    case 8:
        //        NumInput.text += "8";
        //        NumBtn[7].enabled = false;
        //        break;

        //    case 9:
        //        NumInput.text += "9";
        //        NumBtn[8].enabled = false;
        //        break;
        //}
    }

    //private void Num1Click()
    //{
    //    NumInput.text += "1";
    //    NumBtn[0].enabled = false;
    //}
    //private void Num2Click()
    //{
    //    NumInput.text += "2";
    //    NumBtn[1].enabled = false;
    //}
    //private void Num3Click()
    //{
    //    NumInput.text += "3";
    //    NumBtn[2].enabled = false;
    //}
    //private void Num4Click()
    //{
    //    NumInput.text += "4";
    //    NumBtn[3].enabled = false;
    //}
    //private void Num5Click()
    //{
    //    NumInput.text += "5";
    //    NumBtn[4].enabled = false;
    //}
    //private void Num6Click()
    //{
    //    NumInput.text += "6";
    //    NumBtn[5].enabled = false;
    //}
    //private void Num7Click()
    //{
    //    NumInput.text += "7";
    //    NumBtn[6].enabled = false;
    //}
    //private void Num8Click()
    //{
    //    NumInput.text += "8";
    //    NumBtn[7].enabled = false;
    //}
    //private void Num9Click()
    //{
    //    NumInput.text += "9";
    //    NumBtn[8].enabled = false;
    //}


    private void NumKeypadOn()
    {
        for (int i = 0; i < numKeypad; i++)
        {
            NumBtn[i].enabled = true;
        }
    }
    private void GameStart()
    {
        rule.gameObject.SetActive(false);
        round.gameObject.SetActive(true);
        roundResult.gameObject.SetActive(true);
        numInputText.gameObject.SetActive(true);

        ComNumber();
        NumKeypadOn();
    }
    private void resetClick()
    {
        NumKeypadOn();

        NumInput.text = "";
    }
    private void RestartClick()
    {
        SceneManager.LoadScene(0);
    }
}
