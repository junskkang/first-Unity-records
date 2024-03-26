using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CItem
{
    public static int g_CurUniqId = 0; //���� ���۽� ���ÿ��� �ε��� �� ��
    //string[] m_Item = { "�巡���ǰ�", "�����ǹ���", "�������̻�", "�ȶ���Ȱ", "������ǹ���",
    //"����Ǵܰ�"};

    string[] m_Item2 = { "�巡��", "����", "����", "����", "�ȶ�", "�����", "������", "���", "������" };
    string[] m_Item3 = {"��", "Ȱ", "�ܰ�", "������", "����", "����", "����"};
    public int m_ItemUId = -1;
    public string m_Name = "";
    public int m_Level = 1;
    public int m_Grade = 7;
    public int m_Price = 1000;

    public CItem()                        //����Ʈ ������ �Լ� �̸����� �ڵ�����
    {
        //int a_Idx = Random.Range(0, m_Item.Length + 1);
        //m_Name = m_Item[a_Idx];
        //m_Level = Random.Range(1, 9);
        //m_Grade = 7 - Random.Range(0, 2);
        //m_Price = Random.Range(100, 1001);

    }

    public CItem(string a_Name)           //������ �����ε� �Լ�
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
        m_Name = m_Item2[a_idx2] + "��" + m_Item3[a_idx3];
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
