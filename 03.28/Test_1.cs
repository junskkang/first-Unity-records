using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//Unity C# Dictionary


public class Student
{
    public string m_Name;
    public int m_Kor;
    public int m_Eng;
    public int m_Math;
    public int m_Total;
    public float m_Avg;

    public Student(string a_Name = "", int a_Kor = 0, int a_Eng = 0, int a_Math = 0)
    {
        m_Name = a_Name;
        m_Kor = a_Kor;
        m_Eng = a_Eng;
        m_Math = a_Math;
        m_Total = m_Kor + m_Eng + m_Math;
        m_Avg = m_Total / 3.0f;
    }

    public string GetInfoStr()
    {
        return $"{m_Name} : ����({m_Kor}) ����({m_Eng}) ����({m_Math}) ����({m_Total}) ���({m_Avg:N2})";
    }
}

public enum SortState
{
    AddSort,
    TotalSort,
    KorSort
}

[ExecuteInEditMode]    // �� �ɼ��� �ְ� �Ǹ� Unity �÷��̸� ���ص� onGUI�� �� �� �ִ�.
public class Test_1 : MonoBehaviour
{
    Dictionary<string, Student> m_StDcList = new Dictionary<string, Student>();
    List<Student> m_SortList = new List<Student>();
    Student m_FindSt = null;
    SortState m_sort = SortState.AddSort;
    public int TotalSortM(Student a, Student b)
    {
        return b.m_Total.CompareTo(a.m_Total);
    }

    public int KorSortM(Student a, Student b)
    {
        return b.m_Kor.CompareTo(a.m_Kor);
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadList();        //���ӿ� ���� �� �ε�
    }

    // Update is called once per frame
    void Update()
    {

    }

    string m_InName = "", m_InKor = "", m_InEng = "", m_InMath = "";
    private void OnGUI()   //Update()ó�� ��� ȣ��Ǵ� �Լ������� �󵵼��� Update���� �ξ� ����.
    {


        GUI.Label(new Rect(255, 35, 300, 100),     // ��ǥ���� ������ ������� �𼭸� 
            "<color=#00ff00><size=32>" + "�л�����" + "</size></color>");

        GUI.Label(new Rect(210, 100, 300, 100),
            "<size=32>" + "�л� �̸� ->" + "</size>");

        GUI.Label(new Rect(210, 145, 300, 100),
            "<size=32>" + "���� ���� ->" + "</size>");

        GUI.Label(new Rect(210, 190, 300, 100),
            "<size=32>" + "���� ���� ->" + "</size>");

        GUI.Label(new Rect(210, 235, 300, 100),
            "<size=32>" + "���� ���� ->" + "</size>");

        GUIStyle temp = new GUIStyle(GUI.skin.textField); //�ɼ��� �����ϰ� �ִ� ��ü�� �����Ͽ� �� ���� �� 
        temp.fontSize = 30;
        m_InName = GUI.TextField(new Rect(395, 95, 180, 40), m_InName, temp);
        m_InKor = GUI.TextField(new Rect(395, 140, 180, 40), m_InKor, temp);
        m_InEng = GUI.TextField(new Rect(395, 185, 180, 40), m_InEng, temp);
        m_InMath = GUI.TextField(new Rect(395, 230, 180, 40), m_InMath, temp);

        if (GUI.Button(new Rect(40, 50, 130, 40), "<size=23>" + "�л��߰�" + "</size>") == true)
        {
            m_InName = m_InName.Trim();
            m_InKor = m_InKor.Trim();
            m_InEng = m_InEng.Trim();
            m_InMath = m_InMath.Trim();

            if (string.IsNullOrEmpty(m_InName) == false &&
                string.IsNullOrEmpty(m_InKor) == false &&
                string.IsNullOrEmpty(m_InEng) == false &&
                string.IsNullOrEmpty(m_InMath) == false &&
                m_StDcList.ContainsKey(m_InName) == false)
            {// m_InName ���� ���� �ƴϰ�, m_StDcList�� �̸��� ��ϵǾ� ���� �ʴٸ�
                int a_Kor = 0, a_Eng = 0, a_Math = 0;
                int.TryParse(m_InKor, out a_Kor);
                int.TryParse(m_InEng, out a_Eng);
                int.TryParse(m_InMath, out a_Math);
                Student a_Node = new Student(m_InName, a_Kor, a_Eng, a_Math);
                m_StDcList.Add(m_InName, a_Node);  // Dictionary�� ��� �߰�


                SaveList();  //�л��� �߰��Ͽ����� ��ȭ�� �߻� ��� ����!

                m_InName = ""; m_InKor = ""; m_InEng = ""; m_InMath = "";
            }
        }


        //�л� �˻� ��ư
        if (GUI.Button(new Rect(40, 130, 130, 40), "<size=23>" + "�л��˻�" + "</size>") == true)
        {
            m_FindSt = null;

            m_InName = m_InName.Trim();
            //m_InName : �л� �̸� �Է� ���ڿ� ���������� �Է��� �л� �̸�
            if (string.IsNullOrEmpty(m_InName) == false && m_StDcList.ContainsKey(m_InName) == true)
            {
                m_FindSt = m_StDcList[m_InName];
            }
        }

        //�л� �˻� ������ ȭ�鿡 ����� �ִ� �κ�
        if (m_FindSt != null)
        {
            string a_StrBuff = m_FindSt.GetInfoStr();
            GUI.Label(new Rect(30, 500, 1000, 100),
                "<color=#ff77ff><size=27>" + a_StrBuff + "</size></color>");
        }


        //�˻��� �л� ����
        if (GUI.Button(new Rect(40, 210, 130, 40), "<size=23>" + "�л� ����" + "</size>") == true)
        {
            if (m_FindSt != null)
            {
                string a_Key = m_FindSt.m_Name;
                m_StDcList.Remove(a_Key);           //��� ����
                m_FindSt = null;

                SaveList();      //�л� ������ ����Ʈ�� ��ȭ�� �������Ƿ� ����

                m_InName = ""; m_InKor = ""; m_InEng = ""; m_InMath = "";
            }
        }

        //��ü ����Ʈ ����
        if (GUI.Button(new Rect(40, 290, 130, 40), "<size=23>" + "��ü ����" + "</size>") == true)
        {
            if (m_StDcList.Count > 0)
            {
                m_StDcList.Clear();
                m_SortList.Clear();
                m_FindSt = null;
                m_InName = ""; m_InKor = ""; m_InEng = ""; m_InMath = "";

                SaveList();     // �����տ� ����� ���� ���� �Լ����� ��������

            }
        }

        if (m_sort == SortState.AddSort)
        {
            //�л� ����Ʈ ����غ���
            if (0 < m_StDcList.Count)
            {
                int a_Index = 0;
                string a_StrBuff;
                foreach (KeyValuePair<string, Student> a_Node in m_StDcList)
                {
                    // string Key = a_Node.Key;
                    // Student Value = a_Node.Value;

                    a_StrBuff = a_Node.Value.GetInfoStr();
                    GUI.Label(new Rect(600, 25 + (a_Index * 32), 1000, 100), "<color=#ffff00><size=25>" +
                        a_StrBuff + "</size></color>");
                    a_Index++;
                }
            }
        }
        else if (m_sort == SortState.TotalSort)
        {
            TotalSort();
        }
        else if (m_sort == SortState.KorSort)
        {
            KorSort();
        }


        //��ư Ŭ���� ���� enum���� ����
        if (GUI.Button(new Rect(40, 530, 130, 40), "<size=23>" + "�߰��� ����" + "</size>") == true)
        {
            m_sort = SortState.AddSort;
        }

        //���� ������ ����
        if (GUI.Button(new Rect(40, 370, 130, 40), "<size=23>" + "������ ����" + "</size>") == true)
        {
            m_sort = SortState.TotalSort;
        }

        //�������� ������ ����
        if (GUI.Button(new Rect(40, 450, 130, 40), "<size=23>" + "���� ����" + "</size>") == true)
        {
            m_sort = SortState.KorSort;
        }


        //��ųʸ��� ����Ʈ�� ����
        if (m_StDcList.Count > 0 && m_sort == SortState.TotalSort || m_sort == SortState.KorSort)
        {
            KeyValuePair<string, Student> a_PairNode;
            Student a_Node;
            for (int i = 0; i < m_StDcList.Count; i++)
            {
                a_PairNode = m_StDcList.ElementAt(i);
                a_Node = a_PairNode.Value;

                for (int ii = 0; ii < m_SortList.Count; ii++)
                {
                    if (a_Node.m_Name == m_SortList[ii].m_Name)
                        return;
                }
                m_SortList.Add(a_Node);
            }
        }


    }

    void KorSort()
    {
        if (m_sort == SortState.KorSort && m_SortList != null)
        {
            List<Student> a_CloneList = m_SortList.ToList();
            m_SortList.Clear();
            a_CloneList.Sort(KorSortM);

            string a_StrBuff;
            for (int i = 0; a_CloneList.Count > i; i++)
            {
                a_StrBuff = a_CloneList[i].GetInfoStr();
                GUI.Label(new Rect(600, 25 + (i * 32), 1000, 100), "<color=#ffff00><size=25>" + a_StrBuff + "</size></color>");
                Debug.Log(a_StrBuff);
            }
        }
    }

    void TotalSort()
    {
        if (m_sort == SortState.TotalSort && m_SortList != null)
        {
            List<Student> a_CloneList = m_SortList.ToList();
            m_SortList.Clear();
            a_CloneList.Sort(TotalSortM);

            string a_StrBuff;
            for (int i = 0; a_CloneList.Count > i; i++)
            {
                a_StrBuff = a_CloneList[i].GetInfoStr();
                GUI.Label(new Rect(600, 25 + (i * 32), 1000, 100), "<color=#ffff00><size=25>" + a_StrBuff + "</size></color>");
                //Debug.Log(a_StrBuff);
            }
        }
    }
    void SaveList()
    {
        PlayerPrefs.DeleteAll();    //���� ���¸� ��� ����

        if (m_StDcList.Count <= 0)
            return;

        PlayerPrefs.SetInt("St_Count", m_StDcList.Count);

        KeyValuePair<string, Student> a_PairNode;
        Student a_Node;
        for (int i = 0; i < m_StDcList.Count; i++)
        {
            a_PairNode = m_StDcList.ElementAt(i);
            a_Node = a_PairNode.Value;

            PlayerPrefs.SetString($"ST_{i}_Name", a_Node.m_Name);    //�̸�����
            PlayerPrefs.SetInt($"ST_{i}_Kor", a_Node.m_Kor);         //������������
            PlayerPrefs.SetInt($"ST_{i}_Eng", a_Node.m_Eng);         //������������
            PlayerPrefs.SetInt($"ST_{i}_Math", a_Node.m_Math);       //������������
            PlayerPrefs.SetInt($"ST_{i}_Total", a_Node.m_Total);     //��������
            PlayerPrefs.SetFloat($"ST_{i}_Avg", a_Node.m_Avg);       //�������
        }
    }
    void LoadList()
    {
        int a_StCount = PlayerPrefs.GetInt("St_Count", 0);

        if (a_StCount <= 0)
            return;

        Student a_Node;
        for (int i = 0; i < a_StCount; i++)
        {
            a_Node = new Student();

            a_Node.m_Name = PlayerPrefs.GetString($"ST_{i}_Name", "");   //�̸��ε�
            a_Node.m_Kor = PlayerPrefs.GetInt($"ST_{i}_Kor", 0);         //���������ε�
            a_Node.m_Eng = PlayerPrefs.GetInt($"ST_{i}_Eng", 0);         //���������ε�
            a_Node.m_Math = PlayerPrefs.GetInt($"ST_{i}_Math", 0);       //���������ε�
            a_Node.m_Total = PlayerPrefs.GetInt($"ST_{i}_Total", 0);     //�����ε�
            a_Node.m_Avg = PlayerPrefs.GetFloat($"ST_{i}_Avg", 0.0f);    //��շε�

            if (string.IsNullOrEmpty(a_Node.m_Name) == false &&
                m_StDcList.ContainsKey(a_Node.m_Name) == false)
            {
                m_StDcList.Add(a_Node.m_Name, a_Node);  //a_Node�� m_Name�� Ű������ �Ͽ� a_Node�� ����� ������ Add
            }
        }
    }
        
}
