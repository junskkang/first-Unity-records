using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMark : MonoBehaviour
{
    HeroCtrl m_RefHero = null;
    Vector3 m_CacVLen = Vector3.zero;
    float m_AddTimer = 0.0f;
    bool m_IsOnOff = true;
    Renderer m_RefRender;
    Color32 m_WtColor = new Color32(255, 255, 255, 200);
    Color32 m_BrColor = new Color32(0, 130, 255, 200);

    void Start()
    {
        m_RefRender = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //깜빡임 연출
        if (m_RefRender != null)
        {
            m_AddTimer += Time.deltaTime;   
            if (0.25f <= m_AddTimer)
            {
                m_IsOnOff = !m_IsOnOff;
                if (m_IsOnOff == true)
                    m_RefRender.material.SetColor("_TintColor", m_WtColor);
                else
                    m_RefRender.material.SetColor("_TintColor", m_BrColor);
                m_AddTimer = 0.0f;
            }
        }
    }

    public void PlayEff(Vector3 a_PickVec, HeroCtrl a_RefHero)
    {
        m_RefHero = a_RefHero;  //HeroCtrl을 받아놓을 수 있는 함수

        transform.position = new Vector3(a_PickVec.x, 0.0f, a_PickVec.z);
        this.gameObject.SetActive(true);

    }

    public void ClickMarkOnOff(bool val)
    {
        gameObject.SetActive(val);
    }
}
