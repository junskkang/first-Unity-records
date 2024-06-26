using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    Text m_RefText = null;

    public AnimationCurve scaleCurve = new AnimationCurve(
        new Keyframe[] { new Keyframe(0.0f, 0.03f), new Keyframe(0.2f, 0.061f) });
    //캐싱버그에 의해서 public의 값이 인스펙터 창에 안바뀐다면..
    //1. 유니티를 재실행한다.
    //2. 스크립트 컴포넌트를 제거했다가 다시 붙인다.

    public AnimationCurve moveCurve = new AnimationCurve(
        new Keyframe[] { new Keyframe(0.19f, 0.0f), new Keyframe(0.65f, 2.8f) });

    public AnimationCurve alphaCurve = new AnimationCurve(
        new Keyframe[] { new Keyframe(0.4f, 1.0f), new Keyframe(1.0f, 0.0f) });

    //연출 계산용 변수
    float m_StartTime = 0.0f;
    float m_CurTime  = 0.0f;

    Vector3 m_CacScVec = Vector3.zero;
    float m_CacScale = 0.0f;

    Vector3 m_CacCurPos = Vector3.zero;
    float m_MvOffset = 0.0f;

    Color m_Color = new Color32(200, 0, 0, 255);
    float m_Alpha = 0.0f;

    //데미지 값
    float m_DamageVal = 10.0f;
    
    void Start()
    {
        if(m_RefText == null)
            m_RefText = this.gameObject.GetComponentInChildren<Text>();

        if (m_RefText != null)
        {
            m_Color = m_RefText.color;
            m_RefText.text = "-" + m_DamageVal.ToString();
        }

        m_StartTime = Time.time;    //스폰된 시간

        Destroy(this.gameObject, 1.5f);
            
    }

    
    void Update()
    {
        m_CurTime = Time.time;      //스폰된 이후 흐른 시간

        //펀칭 효과 연출
        m_CacScale = scaleCurve.Evaluate(m_CurTime - m_StartTime);
        m_CacScVec.x = m_CacScale;
        m_CacScVec.y = m_CacScale;
        m_CacScVec.z = 1.0f;
        m_RefText.transform.localScale = m_CacScVec;

        //이동 효과 연출
        m_MvOffset = moveCurve.Evaluate(m_CurTime - m_StartTime);
        m_CacCurPos.z = m_MvOffset;
        m_RefText.transform.localPosition = m_CacCurPos;

        //투명 효과 연출
        m_Alpha = alphaCurve.Evaluate(m_CurTime - m_StartTime);
        m_Color = m_RefText.color;
        m_Color.a = m_Alpha;
        m_RefText.color = m_Color;
    }

    public void DamageTxtSpawn(float a_Damage, Color a_Color)
    {
        m_DamageVal = a_Damage;
        m_RefText = this.gameObject.GetComponentInChildren<Text>();
        if (m_RefText != null)
        {
            m_RefText.color = a_Color;
            m_RefText.text = "-" + m_DamageVal.ToString();
        }
    }
}
