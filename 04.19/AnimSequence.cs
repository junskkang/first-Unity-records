using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    Idle,
    Front_Walk,
    Back_Walk,
    Left_Walk,
    Right_Walk,
    Attack
   
}

public class AnimSequence : MonoBehaviour
{
    Renderer m_RefRender = null;

    public Texture[] m_Frt_Idle = null;
    public Texture[] m_Front_Wk = null;
    public Texture[] m_Back_Wk = null;
    public Texture[] m_Left_Wk = null;
    public Texture[] m_Right_Wk = null;

    int m_FrameCount = 0;
    float m_EachAniDelay = 0.1f;
    float m_AniTickCount = 0.0f;
    int m_CurAniInx = 0;
    Texture[] m_NowAniSocket = null;

    UnitState currentState = UnitState.Idle;
    
    void Start()
    {
        m_RefRender = gameObject.GetComponent<Renderer>();

        m_EachAniDelay = 0.5f;
        m_NowAniSocket = m_Frt_Idle;
        if (m_NowAniSocket != null && 0 < m_NowAniSocket.Length)
        {
            m_CurAniInx = 0;
            if (m_RefRender != null)
            {
                m_RefRender.material.SetTexture("_MainTex", m_NowAniSocket[m_CurAniInx]);
            }
        }
    }

    
    void Update()
    {
        
    }
}
