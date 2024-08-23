using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDrapMgr : MonoBehaviour
{
    public SlotScript[] m_SlotSc;
    public RawImage m_MsObj = null;

    int m_SaveIndex = -1;
    int m_DrtIndex = -1;        //Direction Index
    //bool m_IsPick = false;

    //--- 아이콘 투명하게 사라지게 하기 연출용 변수
    float AniDuring  = 0.8f; //패이드 아웃 연출 시간 설정
    float m_CacTime  = 0.0f;
    float m_AddTimer = 0.0f;
    Color m_Color;
    //--- 아이콘 투명하게 사라지게 하기 연출용 변수

    [Header("--- Display TextUI ---")]
    public Text m_GoldText;
    public Text m_SkillText;

    [Header("--- Help TextUI ---")]
    public Text m_HelpText;
    float m_HelpDuring = 1.5f;  //페이드아웃 연출 시간
    float m_HelpTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //GlobalUserData.LoadGameInfo();

        if(m_GoldText != null)
        {
            if (GlobalUserData.g_UserGold <= 0)
                m_GoldText.text = "x 00";
            else
                m_GoldText.text = "x " + GlobalUserData.g_UserGold.ToString();
        }//if(m_GoldText != null)

        if (m_SkillText != null)
        {
            if (GlobalUserData.g_BombCount <= 0)
                m_SkillText.text = "x 00";
            else
                m_SkillText.text = "x " + GlobalUserData.g_BombCount.ToString();
        }//if (m_SkillText != null)
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) == true)  //왼쪽 마우스 버튼을 클릭한 순간
        {
            BuyMouseBtnDown();
        }

        if(Input.GetMouseButton(0) == true)     //왼쪽 마우스를 누르고 있는 동안
        {
            BuyMouseBtnPress();
        }

        if(Input.GetMouseButtonUp(0) == true)  //왼쪽 마우스를 누르고 있다가 뗀 순간
        {
            BuyMouseBtnUp();
        }

        BuyDirection();
    }

    bool IsCollSlot(GameObject a_CkObj)  //마우스가 UI 슬롯 오브젝트 위에 있으냐? 판단하는 함수
    {
        Vector3[] v = new Vector3[4];
        a_CkObj.GetComponent<RectTransform>().GetWorldCorners(v);

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
            //구매 확정 슬롯에서부터 시작은 않하겠다는 의미로 구매 확정 슬롯(오른쪽 슬롯)인 경우 스킵
            if (i == 1)
                continue;

            if (m_SlotSc[i].ItemImg.gameObject.activeSelf == true &&
                IsCollSlot(m_SlotSc[i].gameObject) == true)
            {
                m_SaveIndex = i;
                m_SlotSc[i].ItemImg.gameObject.SetActive(false);
                m_MsObj.gameObject.SetActive(true);
                m_MsObj.transform.position = Input.mousePosition;
                break;
            }
        }//for(int i = 0; i < m_SlotSc.Length; i++)
    }//private void BuyMouseBtnDown()

    private void BuyMouseBtnPress()
    {
        if(0 <= m_SaveIndex)
        {
            m_MsObj.transform.position = Input.mousePosition;
        }
    }

    //private void BuyMouseBtnUp()  //연습용 코드
    //{
    //    if (m_SaveIndex < 0)
    //        return;

    //    for(int i = 0; i < m_SlotSc.Length; i++)
    //    {
    //        if (m_SlotSc[i].ItemImg.gameObject.activeSelf == false &&
    //            IsCollSlot(m_SlotSc[i].gameObject) == true)
    //        {
    //            m_SlotSc[i].ItemImg.gameObject.SetActive(true);
    //            m_SlotSc[i].ItemImg.color = Color.white;
    //            m_DrtIndex = i;
    //            m_SaveIndex = -1;
    //            m_MsObj.gameObject.SetActive(false);
    //            break;
    //        }
    //    }

    //    if(0 <= m_SaveIndex)
    //    {
    //        m_SlotSc[m_SaveIndex].ItemImg.gameObject.SetActive(true);
    //        m_SaveIndex = -1;
    //        m_MsObj.gameObject.SetActive(false);
    //    }

    //}//private void BuyMouseBtnUp()

    private void BuyMouseBtnUp()
    {
        if (m_SaveIndex < 0)
           return;

        for(int i = 0; i < m_SlotSc.Length; i++)
        {
            if(m_SaveIndex == i)  //자기 자리에 놓은 경우 구매 불가
                continue;

            if (m_SlotSc[i].ItemImg.gameObject.activeSelf == false &&
                IsCollSlot(m_SlotSc[i].gameObject) == true)
            {
                //--- 구매 허가
                if(2000 <= GlobalUserData.g_UserGold)
                {
                    m_SlotSc[i].ItemImg.gameObject.SetActive(true);
                    m_SlotSc[i].ItemImg.color = Color.white;
                    m_DrtIndex = i;
                    m_AddTimer = AniDuring; 
                    //m_SaveIndex = -1;
                    m_MsObj.gameObject.SetActive(false);

                    GlobalUserData.g_UserGold -= 2000;
                    m_GoldText.text = "x " + GlobalUserData.g_UserGold.ToString("N0");

                    Network_Mgr.Inst.PushPacket(PacketType.UserGold);

                    GlobalUserData.g_BombCount += 1;
                    m_SkillText.text = "x " + GlobalUserData.g_BombCount.ToString();

                    //Debug.Log(GlobalUserData.g_BombCount);
                    Network_Mgr.Inst.PushPacket(PacketType.BombCount);

                }//if(100 <= GlobalUserData.g_UserGold)
                else  //구매 불가
                {
                    m_HelpText.gameObject.SetActive(true);
                    m_HelpText.color = Color.white;
                    m_HelpTimer = m_HelpDuring;
                }

                break;
            }//if (m_SlotSc[i].ItemImg.gameObject.activeSelf == false &&
        }//for(int i = 0; i < m_SlotSc.Length; i++)

        //if(0 <= m_SaveIndex)
        {
            m_SlotSc[m_SaveIndex].ItemImg.gameObject.SetActive(true);
            m_SaveIndex = -1;
            m_MsObj.gameObject.SetActive(false);
        }

    }//private void BuyMouseBtnUp()

    void BuyDirection() //구매 연출 함수
    {
        //--- 장착된 아이콘이 서서히 사라지게 처리하는 연출
        if(0.0f < m_AddTimer)
        {
            m_AddTimer -= Time.deltaTime;
            m_CacTime = m_AddTimer / AniDuring;
            m_Color = m_SlotSc[m_DrtIndex].ItemImg.color;
            m_Color.a = m_CacTime;
            m_SlotSc[m_DrtIndex].ItemImg.color = m_Color;

            if(m_AddTimer <= 0.0f)
            {
                m_SlotSc[m_DrtIndex].ItemImg.gameObject.SetActive(false);
            }
        }
        //--- 장착된 아이콘이 서서히 사라지게 처리하는 연출

    }//void BuyDirection() //구매 연출 함수
}
