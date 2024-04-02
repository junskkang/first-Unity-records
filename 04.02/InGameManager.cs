using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    CUnit m_RefHero = null;

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

        SpawnCharacter();
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    private void AttackBtnClick()
    {
        //LogMsg("공격 버튼 클릭");

        if (m_RefHero == null)   //지금 선택된 캐릭터가 없다는 뜻
            return;    

        m_RefHero.Attack();
    }

    private void SkillBtnClick()
    {
        //LogMsg("스킬 시전");

        if (m_RefHero == null)   //지금 선택된 캐릭터가 없다는 뜻
            return;

        m_RefHero.UseSkill();
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

    void SpawnCharacter()   //인게임화면에서 선택한 캐릭터 UI 스폰
    {
        if (GlobalValue.g_CurSelCStat == null)  //선택 값이 존재하지 않으면 리턴
            return;

        GameObject a_ChrObj = Resources.Load("Hero") as GameObject;
        if(a_ChrObj != null)
        {
            GameObject a_CloneObj = Instantiate(a_ChrObj);     //Hero 스폰
            if(a_CloneObj != null)
            {
                m_RefHero = GlobalValue.g_CurSelCStat.MyAddComponent(a_CloneObj);
                //직업 컴포넌트 빙의 및 GlobalValue.g_CurSelCStat <--이 스탯 전달

            }
        }
    }
}
