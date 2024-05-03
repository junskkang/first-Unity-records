using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RT_ItemNode : MonoBehaviour
{
    public Texture[] m_ItemImg = null;

    [HideInInspector] public int m_UniqueID = -1;
    [HideInInspector] public string m_ItemName = "";
    [HideInInspector] public int m_Level = -1;
    [HideInInspector] public bool m_SelectOnOff = false;
    
    public Image m_SelectImg;
    public RawImage m_IconImg;
    public Text m_InfoText;


    void Start()
    {
        m_SelectOnOff = false;

        this.GetComponent<Button>().onClick.AddListener(ItemSelect);
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public void ItemSelect()
    {
        //버튼을 누를 때마다 선택 상태 온/오프 전환
        m_SelectOnOff = !m_SelectOnOff;
        if (m_SelectImg != null)
            m_SelectImg.gameObject.SetActive(m_SelectOnOff);
    }
    public void InitInfo(int a_UniqueID, Item_Type a_ItemType, string a_Name, int a_Level)
    {
        m_UniqueID = a_UniqueID;
        m_ItemName = a_Name;
        m_Level = a_Level;
        m_InfoText.text = "Lv( " + m_Level.ToString() + ")";
        m_IconImg.texture = m_ItemImg[(int)a_ItemType];     //enum 인덱스넘버와 연결해놓은 텍스쳐넘버가 동일함
    }
}
