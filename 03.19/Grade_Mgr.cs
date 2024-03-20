using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Student
{
    public string studentName;
    public int studentKor;
    public int studentEng;
    public int studentMath;
    public int studentScore;
    public float studentAverage;
    public string studentGrade;

    public string StudentScore()
    {
        studentScore = studentKor + studentEng + studentMath;
        studentAverage = studentScore / 3;

        string score = studentScore.ToString();
        string average = studentAverage.ToString("F2");

        return $"총점 ({score}) 평균({average})";
    }
    public string StudentGrade()
    {
        if (studentAverage > 80)
            studentGrade = "A등급";
        else if (studentAverage > 60)
            studentGrade = "B등급";
        else if (studentAverage > 40)
            studentGrade = "C등급";
        else if (studentAverage > 20)
            studentGrade = "D등급";
        else
            studentGrade = "외계인";

        return studentGrade;
    }
}
public class Grade_Mgr : MonoBehaviour
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
    // Start is called before the first frame update
    void Start()
    {
        if (InfoSave_Btn != null)
            InfoSave_Btn.onClick.AddListener(() => InfoSaveBtn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InfoSaveBtn()
    {
        Student student = new Student();
        {
            student.studentName = StudentName_IF.text;
            int.TryParse(StudentScore1_IF.text, out student.studentKor);
            int.TryParse(StudentScore2_IF.text, out student.studentEng);
            int.TryParse(StudentScore3_IF.text, out student.studentMath);
        }

        StudentInfo_Text.text = $"{student.studentName} : 국어({student.studentKor}) 영어({student.studentEng}) 수학({student.studentMath})";
        StudentScore_Text.text = student.StudentScore();
        StudentGrade_Text.text = student.StudentGrade();
    }
}
