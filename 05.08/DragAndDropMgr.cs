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

    //������ �����ϰ� ������� �ϱ� ����� ����
    float AniDuring = 0.8f; // fade out ����ð� ����
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

        GlobalUserData.LoadGameInfo();      //��尪, ��ź�� �ε��ϱ� ����

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

    bool IsCollSlot(GameObject a_CheckObject)       //���콺�� UI���� ������Ʈ ���� �ִ��� �Ǵ��ϴ� �Լ�
    {
        Vector3[] v = new Vector3[4];
        a_CheckObject.GetComponent<RectTransform>().GetWorldCorners(v);         //UI�� �������� �����ִ� �Լ�

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
            //�ݵ�� ����â���� ����ؾ߸� �����ϵ���
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
                m_DrtIndex = i;     //������ ���� �ε���
                m_SaveIndex = -1;
                MouseImg.gameObject.SetActive(false);
                break;
            }
        }

        if (0 <= m_SaveIndex)       //���Կ� ���� �ʰ� �������� �ٸ� ���� ������� �� ���� �־��� ��ġ�� ���ư��Բ� �ϴ� �ڵ�
        {
            m_SlotSc[m_SaveIndex].ItemImg.gameObject.SetActive(true);
            m_SaveIndex = -1;
            MouseImg.gameObject.SetActive(false);
        }
        
    }

    private void BuyItemBtnUp()
    {
        if (m_SaveIndex < 0) return;    //���� �������� ���� ��

        for (int i = 0; i < m_SlotSc.Length; i++)
        {
            if (m_SaveIndex == i) continue; //�ڱ� �ڸ��� ���� ��쿡 ���� �Ұ�

            if (m_SlotSc[i].ItemImg.gameObject.activeSelf == false && IsCollSlot(m_SlotSc[i].gameObject) == true)
            {
                //���� �㰡
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
                    PlayerPrefs.SetInt("GoldCount", GlobalUserData.g_UserGold); //��� �� ����

                    GlobalUserData.g_BombCount++;
                    m_SkillText.text = "x " + GlobalUserData.g_BombCount.ToString();
                    PlayerPrefs.SetInt("BombCount", GlobalUserData.g_BombCount);//��ź �� ����
                }
                else //���� �Ұ�
                {
                    m_HelpText.gameObject.SetActive(true);
                    m_HelpText.color = Color.white;
                    m_HelpTimer = m_HelpDuring;
                }

                break;

            }
        }

        //if (0 <= m_SaveIndex)         //���Կ� ���� �ʰ� �������� �ٸ� ���� ������� �� ���� �־��� ��ġ�� ���ư��Բ� �ϴ� �ڵ�
        {
            m_SlotSc[0].ItemImg.gameObject.SetActive(true);
            m_SaveIndex = -1;
            MouseImg.gameObject.SetActive(false);
        }


    }

    void BuyDirection()
    {
        //������ �������� ������ ������� ó���ϴ� ����
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
