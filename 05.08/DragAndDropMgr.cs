using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDropMgr : MonoBehaviour
{
    public SlotScript[] m_SlotSc;
    public RawImage MouseImg;

    int m_SaveIndex = -1;
    int m_DrtIndex = -1;        //Direction Index
    //bool m_IsPick = false;

    //아이콘 투명하게 사라지게 하기 연출용 변수
    float AniDuring = 0.8f; // fade out 연출시간 설정
    float m_CacTime = 0.0f;
    float m_AddTimer = 0.0f;
    Color m_Color;

    [Header("Display TextUI")]
    public Text m_GoldText;
    public Text m_SkillText;

    [Header("Help TextUI")]
    public Text m_HelpText;
    float m_HelpDuring = 1.5f;
    float m_HelpTimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        m_SlotSc = gameObject.GetComponentsInChildren<SlotScript>();

        GlobalUserData.LoadGameInfo();      //골드값, 폭탄값 로딩하기 위해

        if (m_GoldText != null)
        {
            if (GlobalUserData.g_UserGold <= 0)
                m_GoldText.text = "x 00";
            else
                m_GoldText.text = "x " + GlobalUserData.g_UserGold.ToString();
        }

        if (m_SkillText != null)
        {
            if (GlobalUserData.g_BombCount <= 0)
                m_SkillText.text = "x 00";
            else
                m_SkillText.text = "x " + GlobalUserData.g_BombCount.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (MouseImg != null)
        {
            if(Input.GetMouseButtonDown(0) == true)
            {                
                BuyMouseBtnDown();                
            }

            if (Input.GetMouseButton(0) == true)
            {
                BuyMouseBtnDrag();     
            }

            if(Input.GetMouseButtonUp(0) == true)
            {
                BuyItemBtnUp();                
            }

            BuyDirection();

        }
    }

    bool IsCollSlot(GameObject a_CheckObject)       //마우스가 UI슬롯 오브젝트 위에 있는지 판단하는 함수
    {
        Vector3[] v = new Vector3[4];
        a_CheckObject.GetComponent<RectTransform>().GetWorldCorners(v);         //UI의 꼭지점을 구해주는 함수

        if (v[0].x <= Input.mousePosition.x && Input.mousePosition.x <= v[2].x &&
            v[0].y <= Input.mousePosition.y && Input.mousePosition.y <= v[2].y)
        {
            return true;
        }

        return false;
    }

    private void BuyMouseBtnDown()
    {     
        m_SaveIndex = -1;

        for(int i = 0; i < m_SlotSc.Length; i++)
        {
            //반드시 구매창에서 출발해야만 동작하도록
            if (i == 1) continue;

            if (m_SlotSc[i].ItemImg.gameObject.activeSelf == true &&
                IsCollSlot(m_SlotSc[i].gameObject) == true)
            {
                m_SaveIndex = i;
                m_SlotSc[i].ItemImg.gameObject.SetActive(false);
                MouseImg.gameObject.SetActive(true);
                break;
            }
        }
    }

    private void BuyMouseBtnDrag()
    {
        if (0 <= m_SaveIndex)
        {
            MouseImg.transform.position = Input.mousePosition;
        }
        
    }

    private void BuyMouseBtnUp()
    {
        if (m_SaveIndex < 0) return;

        for (int i = 0; i < m_SlotSc.Length; i++)
        {
            if (m_SlotSc[i].ItemImg.gameObject.activeSelf == false &&
                IsCollSlot(m_SlotSc[i].gameObject) == true)
            {
                m_SlotSc[i].ItemImg.gameObject.SetActive(true);
                m_SlotSc[i].ItemImg.color = Color.white;
                m_DrtIndex = i;     //도착한 곳의 인덱스
                m_SaveIndex = -1;
                MouseImg.gameObject.SetActive(false);
                break;
            }
        }

        if (0 <= m_SaveIndex)       //슬롯에 넣지 않고 아이템을 다른 곳에 드랍했을 때 원래 있었던 위치로 돌아가게끔 하는 코드
        {
            m_SlotSc[m_SaveIndex].ItemImg.gameObject.SetActive(true);
            m_SaveIndex = -1;
            MouseImg.gameObject.SetActive(false);
        }
        
    }

    private void BuyItemBtnUp()
    {
        if (m_SaveIndex < 0) return;    //잡은 아이템이 없을 때

        for (int i = 0; i < m_SlotSc.Length; i++)
        {
            if (m_SaveIndex == i) continue; //자기 자리에 놓은 경우에 구매 불가

            if (m_SlotSc[i].ItemImg.gameObject.activeSelf == false && IsCollSlot(m_SlotSc[i].gameObject) == true)
            {
                //구매 허가
                if (300 <= GlobalUserData.g_UserGold)
                {
                    m_SlotSc[i].ItemImg.gameObject.SetActive(true);
                    m_SlotSc[i].ItemImg.color= Color.white;
                    m_DrtIndex = i;
                    m_AddTimer = AniDuring;
                    m_SaveIndex = -1;
                    MouseImg.gameObject.SetActive(false);

                    GlobalUserData.g_UserGold -= 300;
                    m_GoldText.text = "x " + GlobalUserData.g_UserGold.ToString("N0");
                    PlayerPrefs.SetInt("GoldCount", GlobalUserData.g_UserGold); //골드 값 저장

                    GlobalUserData.g_BombCount++;
                    m_SkillText.text = "x " + GlobalUserData.g_BombCount.ToString();
                    PlayerPrefs.SetInt("BombCount", GlobalUserData.g_BombCount);//폭탄 값 저장
                }
                else //구매 불가
                {
                    m_HelpText.gameObject.SetActive(true);
                    m_HelpText.color = Color.white;
                    m_HelpTimer = m_HelpDuring;
                }

                break;

            }
        }

        //if (0 <= m_SaveIndex)         //슬롯에 넣지 않고 아이템을 다른 곳에 드랍했을 때 원래 있었던 위치로 돌아가게끔 하는 코드
        {
            m_SlotSc[0].ItemImg.gameObject.SetActive(true);
            m_SaveIndex = -1;
            MouseImg.gameObject.SetActive(false);
        }


    }

    void BuyDirection()
    {
        //장착된 아이콘이 서서히 사라지게 처리하는 연출
        if (0.0f < m_AddTimer)
        {
            m_AddTimer -= Time.deltaTime;
            m_CacTime = m_AddTimer / AniDuring;
            m_Color = m_SlotSc[m_DrtIndex].ItemImg.color;
            m_Color.a = m_CacTime;
            m_SlotSc[m_DrtIndex].ItemImg.color = m_Color;

            if (m_AddTimer <= 0.0f)
            {
                m_SlotSc[m_DrtIndex].ItemImg.gameObject.SetActive(false);
            }
        }
    }
}
