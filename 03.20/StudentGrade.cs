using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Student1
{
    public string m_Name;
    public int m_Kor;
    public int m_Eng;
    public int m_Math;
    public int m_Total;
    public float m_Average;

    public Student1(string a_Name = "", int a_Kor = 0, int a_Eng = 0, int a_Math = 0)
    {
        m_Name = a_Name;
        m_Kor = a_Kor;
        m_Eng = a_Eng;
        m_Math = a_Math;

        m_Total = m_Kor + m_Eng + m_Math;
        m_Average = m_Total / 3.0f;
    }

    public string PrintInfo()
    {
        string a_Result = "";

        a_Result = $"{m_Name} : ����({m_Kor}) ����({m_Eng}) ����({m_Math})\n\n";
        a_Result += $"����({m_Total}) ���({m_Average.ToString("F2")})";
           
              
        return a_Result;
    }
}
public class StudentGrade : MonoBehaviour
{

    public InputField StName_If;
    public InputField Kor_If;
    public InputField Eng_If;
    public InputField Math_If;

    public Button OK_Btn;
    public Text Result_Text;


    void Start()
    {
        if (OK_Btn != null)
            OK_Btn.onClick.AddListener(OKBtnClick);
    }

    void Update()
    {

    }

    private void OKBtnClick()
    {
        string a_StName = StName_If.text.Trim();   //.Trim() ������ �������ִ� �Լ�

        // �̸� �Է� ���ڿ� ���� ��� ������...
        if (string.IsNullOrEmpty(a_StName) == true)
            return;

        int a_Kor = 0, a_Eng = 0, a_Math = 0;
        bool a_IsKorOk = int.TryParse(Kor_If.text.Trim(), out a_Kor);
        bool a_IsEngOk = int.TryParse(Eng_If.text.Trim(), out a_Eng);
        bool a_IsMathOk = int.TryParse(Math_If.text.Trim(), out a_Math);

        //���� ���� ���� �Է� ���ڿ� ���� ���ų� �������°� �ƴϸ�...
        if (a_IsKorOk == false || a_IsEngOk == false || a_IsMathOk == false)
            return;

        //������ 0�� �̸��̳� 100�� �̻��� ������ ���
        if ((a_Kor < 0 || 100 < a_Kor) || (a_Eng < 0 || 100 < a_Eng) || (a_Math < 0 || 100 < a_Math))
            return;

        Student1 a_Std = new Student1(a_StName, a_Kor, a_Eng, a_Math);
        Result_Text.text = a_Std.PrintInfo();
    }
}
