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
    Color32 m_WtColor = new Color32(255, 247, 119, 60);
    Color32 m_BrColor = new Color32(0, 130, 255, 60);

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

        //클릭마크 끄기
        if (m_RefHero == null)  //주인공 사망시에도 꺼줘야 함.
        {//오브젝트가 Destroy가 되면 null이 됨.
            gameObject.SetActive(false);
            return;
        }

        if (gameObject.activeSelf == true)
        {
            if (m_RefHero.m_bMoveOnOff == false)    //마우스 클릭 이동이 취소 되었을 때
                gameObject.SetActive(false);

            m_CacVLen = m_RefHero.transform.position - transform.position;
            m_CacVLen.y = 0.0f; //3D환경에서는 높이값까지 고려해야 하니 이 부분을 주석 걸어버리면 된다.
            if (m_CacVLen.magnitude < 1.0f) //주인공이 클릭 마크에 도착하였을 때
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void PlayEff(Vector3 a_PickVec, HeroCtrl a_RefHero)
    {
        m_RefHero = a_RefHero;  //HeroCtrl을 받아놓을 수 있는 함수

        transform.position = new Vector3(a_PickVec.x, 0.8f, a_PickVec.z);
        gameObject.SetActive(true);

    }

    public void ClickMarkOnOff(bool val)
    {
        gameObject.SetActive(val);
    }
}
