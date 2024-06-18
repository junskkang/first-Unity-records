using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //체력 관련
    public float curHp = 100.0f;
    float maxHp = 100.0f;
    public Image hpbar;

    //점수 UI
    public Text scoreText;
    int curScore = 0;
    static int bestScore = 0;


    public static GameManager inst;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int score)
    {
        curScore += score;

        if(scoreText != null)
            scoreText.text = curScore.ToString();

        if (curScore > bestScore)
            bestScore = curScore;
    }

}
