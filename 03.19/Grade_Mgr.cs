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

        

    public void StudentInfo(Student student)
    {
        studentName = student.studentName;
        studentKor = student.studentKor;
        studentEng = student.studentEng;
        studentMath = student.studentMath;
        studentScore = student.studentScore;
        studentAverage = student.studentAverage;
        studentGrade = student.studentGrade;
        
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
            InfoSave_Btn.onClick.AddListener(() => InfoSaveBtn(StudentName_IF.text, StudentScore1_IF.text, StudentScore2_IF.text, StudentScore3_IF.text));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InfoSaveBtn(string Name, string Kor, string Eng, string Math)
    {
  

    }
}
