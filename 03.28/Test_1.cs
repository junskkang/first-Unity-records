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
        return $"{m_Name} : 국어({m_Kor}) 영어({m_Eng}) 수학({m_Math}) 총점({m_Total}) 평균({m_Avg:N2})";
    }
}

public enum SortState
{
    AddSort,
    TotalSort,
    KorSort
}

[ExecuteInEditMode]    // 이 옵션을 주게 되면 Unity 플레이를 안해도 onGUI를 볼 수 있다.
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
        LoadList();        //게임에 들어올 때 로딩
    }

    // Update is called once per frame
    void Update()
    {

    }

    string m_InName = "", m_InKor = "", m_InEng = "", m_InMath = "";
    private void OnGUI()   //Update()처럼 계속 호출되는 함수이지만 빈도수가 Update보다 훨씬 많다.
    {


        GUI.Label(new Rect(255, 35, 300, 100),     // 좌표값의 기준이 우측상단 모서리 
            "<color=#00ff00><size=32>" + "학생정보" + "</size></color>");

        GUI.Label(new Rect(210, 100, 300, 100),
            "<size=32>" + "학생 이름 ->" + "</size>");

        GUI.Label(new Rect(210, 145, 300, 100),
            "<size=32>" + "국어 점수 ->" + "</size>");

        GUI.Label(new Rect(210, 190, 300, 100),
            "<size=32>" + "영어 점수 ->" + "</size>");

        GUI.Label(new Rect(210, 235, 300, 100),
            "<size=32>" + "수학 점수 ->" + "</size>");

        GUIStyle temp = new GUIStyle(GUI.skin.textField); //옵션을 저장하고 있는 객체를 생성하여 그 값을 줌 
        temp.fontSize = 30;
        m_InName = GUI.TextField(new Rect(395, 95, 180, 40), m_InName, temp);
        m_InKor = GUI.TextField(new Rect(395, 140, 180, 40), m_InKor, temp);
        m_InEng = GUI.TextField(new Rect(395, 185, 180, 40), m_InEng, temp);
        m_InMath = GUI.TextField(new Rect(395, 230, 180, 40), m_InMath, temp);

        if (GUI.Button(new Rect(40, 50, 130, 40), "<size=23>" + "학생추가" + "</size>") == true)
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
            {// m_InName 값이 빈값이 아니고, m_StDcList에 이름이 등록되어 있지 않다면
                int a_Kor = 0, a_Eng = 0, a_Math = 0;
                int.TryParse(m_InKor, out a_Kor);
                int.TryParse(m_InEng, out a_Eng);
                int.TryParse(m_InMath, out a_Math);
                Student a_Node = new Student(m_InName, a_Kor, a_Eng, a_Math);
                m_StDcList.Add(m_InName, a_Node);  // Dictionary에 노드 추가


                SaveList();  //학생을 추가하였으니 변화가 발생 고로 저장!

                m_InName = ""; m_InKor = ""; m_InEng = ""; m_InMath = "";
            }
        }


        //학생 검색 버튼
        if (GUI.Button(new Rect(40, 130, 130, 40), "<size=23>" + "학생검색" + "</size>") == true)
        {
            m_FindSt = null;

            m_InName = m_InName.Trim();
            //m_InName : 학생 이름 입력 상자에 마지막으로 입력한 학생 이름
            if (string.IsNullOrEmpty(m_InName) == false && m_StDcList.ContainsKey(m_InName) == true)
            {
                m_FindSt = m_StDcList[m_InName];
            }
        }

        //학생 검색 성공시 화면에 출력해 주는 부분
        if (m_FindSt != null)
        {
            string a_StrBuff = m_FindSt.GetInfoStr();
            GUI.Label(new Rect(30, 500, 1000, 100),
                "<color=#ff77ff><size=27>" + a_StrBuff + "</size></color>");
        }


        //검색된 학생 삭제
        if (GUI.Button(new Rect(40, 210, 130, 40), "<size=23>" + "학생 삭제" + "</size>") == true)
        {
            if (m_FindSt != null)
            {
                string a_Key = m_FindSt.m_Name;
                m_StDcList.Remove(a_Key);           //노드 삭제
                m_FindSt = null;

                SaveList();      //학생 삭제로 리스트의 변화가 생겼으므로 저장

                m_InName = ""; m_InKor = ""; m_InEng = ""; m_InMath = "";
            }
        }

        //전체 리스트 삭제
        if (GUI.Button(new Rect(40, 290, 130, 40), "<size=23>" + "전체 삭제" + "</size>") == true)
        {
            if (m_StDcList.Count > 0)
            {
                m_StDcList.Clear();
                m_SortList.Clear();
                m_FindSt = null;
                m_InName = ""; m_InKor = ""; m_InEng = ""; m_InMath = "";

                SaveList();     // 프리팹에 저장된 값은 여기 함수에서 제거해줌

            }
        }

        if (m_sort == SortState.AddSort)
        {
            //학생 리스트 출력해보기
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


        //버튼 클릭에 따른 enum상태 변경
        if (GUI.Button(new Rect(40, 530, 130, 40), "<size=23>" + "추가순 정렬" + "</size>") == true)
        {
            m_sort = SortState.AddSort;
        }

        //총점 순으로 정렬
        if (GUI.Button(new Rect(40, 370, 130, 40), "<size=23>" + "총점순 정렬" + "</size>") == true)
        {
            m_sort = SortState.TotalSort;
        }

        //국어점수 순으로 정렬
        if (GUI.Button(new Rect(40, 450, 130, 40), "<size=23>" + "국어 정렬" + "</size>") == true)
        {
            m_sort = SortState.KorSort;
        }


        //딕셔너리값 리스트로 복사
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
        PlayerPrefs.DeleteAll();    //저장 상태를 모두 제거

        if (m_StDcList.Count <= 0)
            return;

        PlayerPrefs.SetInt("St_Count", m_StDcList.Count);

        KeyValuePair<string, Student> a_PairNode;
        Student a_Node;
        for (int i = 0; i < m_StDcList.Count; i++)
        {
            a_PairNode = m_StDcList.ElementAt(i);
            a_Node = a_PairNode.Value;

            PlayerPrefs.SetString($"ST_{i}_Name", a_Node.m_Name);    //이름저장
            PlayerPrefs.SetInt($"ST_{i}_Kor", a_Node.m_Kor);         //국어점수저장
            PlayerPrefs.SetInt($"ST_{i}_Eng", a_Node.m_Eng);         //영어점수저장
            PlayerPrefs.SetInt($"ST_{i}_Math", a_Node.m_Math);       //수학점수저장
            PlayerPrefs.SetInt($"ST_{i}_Total", a_Node.m_Total);     //총점저장
            PlayerPrefs.SetFloat($"ST_{i}_Avg", a_Node.m_Avg);       //평균저장
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

            a_Node.m_Name = PlayerPrefs.GetString($"ST_{i}_Name", "");   //이름로딩
            a_Node.m_Kor = PlayerPrefs.GetInt($"ST_{i}_Kor", 0);         //국어점수로딩
            a_Node.m_Eng = PlayerPrefs.GetInt($"ST_{i}_Eng", 0);         //영어점수로딩
            a_Node.m_Math = PlayerPrefs.GetInt($"ST_{i}_Math", 0);       //수학점수로딩
            a_Node.m_Total = PlayerPrefs.GetInt($"ST_{i}_Total", 0);     //총점로딩
            a_Node.m_Avg = PlayerPrefs.GetFloat($"ST_{i}_Avg", 0.0f);    //평균로딩

            if (string.IsNullOrEmpty(a_Node.m_Name) == false &&
                m_StDcList.ContainsKey(a_Node.m_Name) == false)
            {
                m_StDcList.Add(a_Node.m_Name, a_Node);  //a_Node의 m_Name을 키값으로 하여 a_Node에 저장된 정보를 Add
            }
        }
    }
        
}
