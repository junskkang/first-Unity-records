using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    GameObject player;

    public Text CurHeight_Text;
    public Text BestHeight_Text;

    float m_Height = 0;                        //현재높이
    // Scene이 넘어가도 해당 값을 사용하기 위해 static 선언
    public static float m_CurBHeight = 0.0f;   //현재 최고 높이
    public static float m_BestHeight = 0.0f;   //최고 기록 높이

    
    // Start is called before the first frame update
    void Start()
    {
        Load();

        player = GameObject.Find("cat");
        m_CurBHeight = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_Height = player.transform.position.y;
        if (m_Height <= 0.0f)
            m_Height = 0.0f;

        if (m_CurBHeight < m_Height)      //현재 트라이 최고기록 
            m_CurBHeight = m_Height;

        if (m_BestHeight < m_CurBHeight)  //최고 기록
        {
            m_BestHeight = m_CurBHeight;
            Save();
        }

        if (CurHeight_Text != null)
        {
            CurHeight_Text.text = $"현재높이 : {m_Height.ToString("N2")}";
        }

        if (BestHeight_Text != null)
        {
            BestHeight_Text.text = $"최고기록 : {m_BestHeight.ToString("N2")}";
        }
    }

    public static void Save()
    {
        PlayerPrefs.SetFloat("HighScore", m_BestHeight);
    }

    public static void Load()
    {
        m_BestHeight = PlayerPrefs.GetFloat("HighScore", 0.0f);
    }

}
