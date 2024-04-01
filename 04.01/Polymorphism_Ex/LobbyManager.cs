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

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;   //�Ͻ������� ���� �ӵ��� 

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
}
