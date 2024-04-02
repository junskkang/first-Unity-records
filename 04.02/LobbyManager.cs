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

    Chr_Stat m_CurChrStat = null;  //UI표시용 변수
    GameObject m_SelHero = null;   //UI표시용 영웅 선택 변수

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;   //일시정지를 원래 속도로 
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
                GameObject obj = Instantiate(a_CItemBtn);       //인스턴스 스폰
                obj.transform.SetParent(m_Root_Canvas, false);  //캔버스의 자식으로 붙임, 프리팹에 저장된 좌표 그대로 스폰시키고 싶을 때 false
                Vector3 a_CacPos = obj.transform.localPosition; //부모 좌표를 기준으로
                a_CacPos.x += (i * 200);                        //x좌표에 간격을두고
                obj.transform.localPosition = a_CacPos;
                ChrItemBtn a_ChrItem = obj.GetComponent<ChrItemBtn>();
                a_ChrItem.InitState(GlobalValue.g_MyChrList[i]);
            }
        }
    }

    public void RefreshChrSelUI()
    {
        if (m_SelChrText == null)   //UI 연결상태 확인
            return;

        m_SelChrText.text = "현재 선택된 캐릭터 : " + "없음";

        if (GlobalValue.g_CurSelCStat == null) //지금 선택된 캐릭터가 없으면
            return;

        Chr_Stat a_ChrStat = GlobalValue.g_CurSelCStat;
        m_SelChrText.text = "현재 선택된 캐릭터 :" + $"{a_ChrStat.m_StrJob}, ({a_ChrStat.m_Name})";

        if (m_CurChrStat != GlobalValue.g_CurSelCStat)
        {
            if (m_SelHero != null)
            {
                Destroy(m_SelHero);
            }

            //캐릭터 외형 랜더링
            GameObject a_ChrObj = Resources.Load("Hero") as GameObject;  //1. 프리팹으로 저장된 오브젝트를 불러오고
            if(a_ChrObj != null)
            {
                m_SelHero = Instantiate(a_ChrObj);   // 2. Hero spawn = 하이어라키로 복사
                if (m_SelHero != null)
                {
                    GlobalValue.g_CurSelCStat.MyAddComponent(m_SelHero);  //3. 선택한 캐릭터의 스크립트를 Add Component
                    //직업 컴포넌트 빙의 및 GlobalValue.g_CurSelCStat   <---이 스탯 전달

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
