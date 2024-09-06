using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    int curPoint = 0;
    public Text pointText;

    public static GameManager Inst;

    private void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void AddPoint(int value = 10)
    {
        curPoint += value;

        if (pointText != null)
            pointText.text = $"POINT : {curPoint}";
    }
}
