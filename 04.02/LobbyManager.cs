using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public Button m_GameStartBtn;

    public Transform m_Root_Canvas = null;
    public Text m_SelChrText;

    Chr_Stat m_CurChrStat = null;  //UIǥ�ÿ� ����
    GameObject m_SelHero = null;   //UIǥ�ÿ� ���� ���� ����

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;   //�Ͻ������� ���� �ӵ��� 
        GlobalValue.LoadGameData();

        RefreshChrUI();
        RefreshChrSelUI();

        if (m_GameStartBtn != null)
            m_GameStartBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("InGameScene");
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RefreshChrUI()
    {
        GameObject a_CItemBtn = Resources.Load("ChrItemBtn") as GameObject;
        if (a_CItemBtn != null)
        {
            for(int i = 0; i < GlobalValue.g_MyChrList.Count; i++)
            {
                GameObject obj = Instantiate(a_CItemBtn);       //�ν��Ͻ� ����
                obj.transform.SetParent(m_Root_Canvas, false);  //ĵ������ �ڽ����� ����, �����տ� ����� ��ǥ �״�� ������Ű�� ���� �� false
                Vector3 a_CacPos = obj.transform.localPosition; //�θ� ��ǥ�� ��������
                a_CacPos.x += (i * 200);                        //x��ǥ�� �������ΰ�
                obj.transform.localPosition = a_CacPos;
                ChrItemBtn a_ChrItem = obj.GetComponent<ChrItemBtn>();
                a_ChrItem.InitState(GlobalValue.g_MyChrList[i]);
            }
        }
    }

    public void RefreshChrSelUI()
    {
        if (m_SelChrText == null)   //UI ������� Ȯ��
            return;

        m_SelChrText.text = "���� ���õ� ĳ���� : " + "����";

        if (GlobalValue.g_CurSelCStat == null) //���� ���õ� ĳ���Ͱ� ������
            return;

        Chr_Stat a_ChrStat = GlobalValue.g_CurSelCStat;
        m_SelChrText.text = "���� ���õ� ĳ���� :" + $"{a_ChrStat.m_StrJob}, ({a_ChrStat.m_Name})";

        if (m_CurChrStat != GlobalValue.g_CurSelCStat)
        {
            if (m_SelHero != null)
            {
                Destroy(m_SelHero);
            }

            //ĳ���� ���� ������
            GameObject a_ChrObj = Resources.Load("Hero") as GameObject;  //1. ���������� ����� ������Ʈ�� �ҷ�����
            if(a_ChrObj != null)
            {
                m_SelHero = Instantiate(a_ChrObj);   // 2. Hero spawn = ���̾��Ű�� ����
                if (m_SelHero != null)
                {
                    GlobalValue.g_CurSelCStat.MyAddComponent(m_SelHero);  //3. ������ ĳ������ ��ũ��Ʈ�� Add Component
                    //���� ������Ʈ ���� �� GlobalValue.g_CurSelCStat   <---�� ���� ����

                    Vector3 a_Pos = m_SelHero.transform.position;
                    a_Pos.y -= 2.8f;
                    m_SelHero.transform.position = a_Pos;

                    Vector3 a_Scale = m_SelHero.transform.localScale;
                    a_Scale *= 0.7f;
                    m_SelHero.transform.localScale = a_Scale;
                }
            }
            m_CurChrStat = GlobalValue.g_CurSelCStat;
        }


    }
}
