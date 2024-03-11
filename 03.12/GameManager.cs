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
    public GameObject rule;
    public GameObject round;
    public GameObject roundResult;
    public GameObject result;
    public Text Info;
    public Button[] NumBtn;
    public InputField NumInput;
    public Text[] roundResultText;
    public Text resultText;
    public Text roundCountText;
    public Button restartBtn;
    public Button startBtn;
    public Button throwBtn;
    public Text numInputText;
       

    public string comNum;
    public int comNum1;
    public int comNum2;
    public int comNum3;

    public string playerNum;
    public int[] pNum;

    public int strikeCount;
    public int ballCount;
    public int outCount;
    public int roundCount;

    void Start()
    {
        if (startBtn != null)
            startBtn.onClick.AddListener(GameStart);
        if (restartBtn != null)
            restartBtn.onClick.AddListener(RestartClick);
        if (NumBtn[0] != null)
            NumBtn[0].onClick.AddListener(Num1Click);
        if (NumBtn[1] != null)
            NumBtn[1].onClick.AddListener(Num2Click);
        if (NumBtn[2] != null)
            NumBtn[2].onClick.AddListener(Num3Click);
        if (NumBtn[3] != null)
            NumBtn[3].onClick.AddListener(Num4Click);
        if (NumBtn[4] != null)
            NumBtn[4].onClick.AddListener(Num5Click);
        if (NumBtn[5] != null)
            NumBtn[5].onClick.AddListener(Num6Click);
        if (NumBtn[6] != null)
            NumBtn[6].onClick.AddListener(Num7Click);
        if (NumBtn[7] != null)
            NumBtn[7].onClick.AddListener(Num8Click);
        if (NumBtn[8] != null)
            NumBtn[8].onClick.AddListener(Num9Click);
        if (throwBtn != null)
            throwBtn.onClick.AddListener(ThrowClick);
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
        Debug.Log(pNum[0]); // 100자리 유저가 선택한 숫자
        Debug.Log(pNum[1]); // 10자리 유저가 선택한 숫자
        Debug.Log(pNum[2]); // 1자리 유저가 선택한 숫자

        Judge();

        NumInput.text = "";
    }

    private void Judge()
    {
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
            Debug.Log(comNum);
        }
    }
    private void Num1Click()
    {
        NumInput.text += "1";
    }
    private void Num2Click()
    {
        NumInput.text += "2";
    }
    private void Num3Click()
    {
        NumInput.text += "3";
    }
    private void Num4Click()
    {
        NumInput.text += "4";
    }
    private void Num5Click()
    {
        NumInput.text += "5";
    }
    private void Num6Click()
    {
        NumInput.text += "6";
    }
    private void Num7Click()
    {
        NumInput.text += "7";
    }
    private void Num8Click()
    {
        NumInput.text += "8";
    }
    private void Num9Click()
    {
        NumInput.text += "9";
    }

    private void GameStart()
    {
        rule.gameObject.SetActive(false);
        round.gameObject.SetActive(true);
        roundResult.gameObject.SetActive(true);
        numInputText.gameObject.SetActive(true);

        ComNumber();
    }
    private void RestartClick()
    {
        SceneManager.LoadScene(0);
    }
}
