using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RockScissorsPaper : MonoBehaviour
{
    [SerializeField]
    Button buttonRock;
    [SerializeField]
    Button buttonScissors;
    [SerializeField]
    Button buttonPaper;
    [SerializeField]
    TextMeshProUGUI tmpPc;
    [SerializeField]
    TextMeshProUGUI records;

    const int ROCK = 0;
    const int SCISSORS = 1;
    const int PAPER = 2;
    
    string[] intToString = {"ROCK", "SCISSORS", "PAPER"};

    int win = 0;
    int lose = 0;
    int draw = 0;

    private void Start()
    {
        buttonRock.onClick.AddListener(Rock);
        buttonScissors.onClick.AddListener(Scissors);
        buttonPaper.onClick.AddListener(Paper);
    }

    void PlayGameLogic(int myTurn)
    {
        int pc = GetRandom();

        if (myTurn == pc)
        {
            draw++;
        }
        else
        {
            switch(myTurn)
            {
                case ROCK:
                    if(pc == SCISSORS)
                    {
                        win++;
                    }
                    else
                    {
                        lose++;
                    }
                    break;

                case SCISSORS:
                    if (pc == PAPER)
                    {
                        win++;
                    }
                    else
                    {
                        lose++;
                    }
                    break;

                case PAPER:
                    if (pc == ROCK)
                    {
                        win++;
                    }
                    else
                    {
                        lose++;
                    }
                    break;
            }
        }
        tmpPc.text = "You : " + intToString[myTurn] + "     vs     PC : " + intToString[pc];
        records.text = "Win : " + win.ToString() + " Lose : " + lose.ToString() + " Draw : " + draw.ToString();
    }

    int GetRandom()
    {
        return Random.Range(0, 3);
    }

    void Rock()
    {
        PlayGameLogic(ROCK);
    }

    void Scissors()
    {
        PlayGameLogic(SCISSORS);
    }

    void Paper()
    {
        PlayGameLogic(PAPER);
    }
}
