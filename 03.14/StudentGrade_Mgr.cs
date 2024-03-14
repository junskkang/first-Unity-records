using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StudentGrade_Mgr : MonoBehaviour
{
    [Header("Student Info")]
    public InputField StudentName_IF;
    public InputField StudentScore1_IF;
    public InputField StudentScore2_IF;
    public InputField StudentScore3_IF;
    public Button InfoSave_Btn;

    [Header("Result")]
    public Text StudentInfo_Text;
    public Text StudentScore_Text;
    public Text StudentGrade_Text;

    void Start()
    {
        if (InfoSave_Btn == null)
            InfoSave_Btn.onClick.AddListener(InfoSaveBtnClick);
    }

    void Update()
    {
        
    }

    private void InfoSaveBtnClick()
    {
        int.TryParse(StudentScore1_IF.text, out int a_KoreanScore);
        int.TryParse(StudentScore2_IF.text, out int a_EnglishScore);
        int.TryParse(StudentScore3_IF.text, out int a_MathScore);

        int Sum = a_KoreanScore + a_EnglishScore + a_MathScore;
        int Average = (a_KoreanScore + a_EnglishScore + a_MathScore) / 3;

        StudentInfo_Text.text = StudentName_IF.text + " : 국어(" + a_KoreanScore + 
            ") 영어(" + a_EnglishScore + ") 수학(" + a_MathScore + ")";
        StudentScore_Text.text = "총점 : " + Sum + "  평균 : " + Average;

        if (Average > 80)
            StudentGrade_Text.text = "당신은 A등급 입니다.";
        else if (Average > 60)
            StudentGrade_Text.text = "당신은 B등급 입니다.";
        else if (Average > 40)
            StudentGrade_Text.text = "당신은 C등급 입니다.";
        else if (Average > 20)
            StudentGrade_Text.text = "당신은 D등급 입니다.";
        else
            StudentGrade_Text.text = "당신은 외계인입니다.";
    }

}
