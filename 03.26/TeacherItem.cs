using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CItem
{
    public static int g_CurUniqId = 0; //게임 시작시 로컬에서 로딩해 올 값
    //string[] m_Item = { "드래곤의검", "엘프의반지", "사자의이빨", "팔라독의활", "고양이의발톱",
    //"상어의단검"};

    string[] m_Item2 = { "드래곤", "엘프", "전사", "사자", "팔라독", "고양이", "강아지", "상어", "마법사" };
    string[] m_Item3 = {"검", "활", "단검", "지팡이", "발톱", "반지", "갑옷"};
    public int m_ItemUId = -1;
    public string m_Name = "";
    public int m_Level = 1;
    public int m_Grade = 7;
    public int m_Price = 1000;

    public CItem()                        //디폴트 생성자 함수 이름없이 자동생성
    {
        //int a_Idx = Random.Range(0, m_Item.Length + 1);
        //m_Name = m_Item[a_Idx];
        //m_Level = Random.Range(1, 9);
        //m_Grade = 7 - Random.Range(0, 2);
        //m_Price = Random.Range(100, 1001);

    }

    public CItem(string a_Name)           //생성자 오버로딩 함수
    {
        m_Name = a_Name;
        m_ItemUId = g_CurUniqId;
        g_CurUniqId++;
        PlayerPrefs.SetInt("CurUniqId", g_CurUniqId);
        m_Level = Random.Range(1, 9);
        m_Grade = 7 - Random.Range(0, 2);
        m_Price = Random.Range(100, 1001);
    }

    public void InitItem()
    {
        int a_idx2 = Random.Range(0, m_Item2.Length);
        int a_idx3 = Random.Range(0, m_Item3.Length);
        m_Name = m_Item2[a_idx2] + "의" + m_Item3[a_idx3];
        m_ItemUId = g_CurUniqId;
        g_CurUniqId++;
        PlayerPrefs.SetInt("CurUniqId", g_CurUniqId);
        m_Level = Random.Range(1, 9);
        m_Grade = 7 - Random.Range(0, 2);
        m_Price = Random.Range(100, 1001);
    }
}
public class TeacherItem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
