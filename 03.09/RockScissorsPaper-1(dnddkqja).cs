using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RockScissorsPaper : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    Button buttonStone;
    [SerializeField]
    Button buttonShear;
    [SerializeField]
    Button buttonPaper;

    [Header("UI")]
    [SerializeField]
    TextMeshProUGUI tmpCom;
    [SerializeField]
    TextMeshProUGUI records;

    const int STONE = 0;
    const int SHEAR = 1;
    const int PAPER = 2;
    
    string[] intToString = { "STONE", "SHEAR", "PAPER" };

    int beat = 0;
    int lose = 0;
    int draw = 0;

    void Start()
    {
        buttonStone.onClick.AddListener(() => PlayGameLogic(STONE));
        buttonShear.onClick.AddListener(() => PlayGameLogic(SHEAR));
        buttonPaper.onClick.AddListener(() => PlayGameLogic(PAPER));
    }

    void PlayGameLogic(int playerPick)
    {
        int comPick = Random.Range(0, 3);

        if (playerPick == comPick)
        {
            draw++;
        }
        else if ((playerPick == STONE && comPick == SHEAR) ||
                 (playerPick == SHEAR && comPick == PAPER) ||
                 (playerPick == PAPER && comPick == STONE))
        {
            beat++;
        }
        else
        {
            lose++;
        }
        tmpCom.text = "You : " + intToString[playerPick] + "     vs     PC : " + intToString[comPick];
        records.text = "Win : " + beat.ToString() + " Lose : " + lose.ToString() + " Draw : " + draw.ToString();
    }      
}
