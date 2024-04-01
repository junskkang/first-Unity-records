using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public Button m_BackBtn;
    public Text txtLogMsg;
    List<string> m_MsgList = new List<string>();

    public Button m_AttackBtn;
    public Button m_SkillBtn;
    // Start is called before the first frame update
    void Start()
    {
        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        if (m_AttackBtn != null)
            m_AttackBtn.onClick.AddListener(AttackBtnClick);

        if (m_SkillBtn != null)
            m_SkillBtn.onClick.AddListener(SkillBtnClick);
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    private void AttackBtnClick()
    {
        LogMsg("공격 버튼 클릭");
    }

    private void SkillBtnClick()
    {
        LogMsg("스킬 시전");
    }

    public void LogMsg(string msg)
    {
        m_MsgList.Add(msg);
        if (8 < m_MsgList.Count)
            m_MsgList.RemoveAt(0);   //제일 위에 있는 애를 지움

        txtLogMsg.text = "";
        for (int i = 0; i < m_MsgList.Count; i++)
        {
            txtLogMsg.text += m_MsgList[i];
            txtLogMsg.text += "\n";
        }
    }
}
