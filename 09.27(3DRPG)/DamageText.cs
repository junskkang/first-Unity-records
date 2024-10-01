using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    [HideInInspector] public Text m_RefText = null;
    [HideInInspector] public float m_DamageVal = 0.0f;
    [HideInInspector] public Vector3 m_BaseWdpos = Vector3.zero;

    Animator m_RefAnimator = null;

    RectTransform rectCanvas;
    Vector2 ScreenPos = Vector2.zero;
    Vector2 WdScPos = Vector2.zero;

    Vector3 m_CacVec = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        m_RefText = this.gameObject.GetComponentInChildren<Text>();
        if (m_RefText != null)
        {
            m_RefText.text = "-" + m_DamageVal.ToString("N0");
        }

        m_RefAnimator = GetComponentInChildren<Animator>();
        if (m_RefAnimator != null)
        {
            //애니메이터 0번 레이어에 저장된 정보를 가져옴
            AnimatorStateInfo animStateInfo = m_RefAnimator.GetCurrentAnimatorStateInfo(0);
            //해당 애니메이션의 한동작 길이를 가져옴
            float lifeTime = animStateInfo.length;
            Destroy(this.gameObject, lifeTime);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //World 좌표를 UGUI 좌표로 환산해주는 코드
        rectCanvas = GameMgr.Inst.m_DamageCanvas.GetComponent<RectTransform>();
        ScreenPos = Camera.main.WorldToViewportPoint(m_BaseWdpos);
        WdScPos.x = ((ScreenPos.x * rectCanvas.sizeDelta.x) - (rectCanvas.sizeDelta.x * 0.5f));
        WdScPos.y = ((ScreenPos.y * rectCanvas.sizeDelta.y) - (rectCanvas.sizeDelta.y * 0.5f));
        transform.GetComponent<RectTransform>().anchoredPosition = WdScPos;

        //카메라 컬링
        m_CacVec = m_BaseWdpos - Camera.main.transform.position;
        if (m_CacVec.magnitude <= 0.0f)
        {
            //데미지 텍스트와 카메라가 같은 위치에 있을경우 보일 필요가 없음
            if (m_RefText.gameObject.activeSelf)
                m_RefText.gameObject.SetActive(false);            
        }
        else if (0.0f < Vector3.Dot(Camera.main.transform.forward, m_CacVec.normalized))    
        {
            //카메라 앞에 위치
            if (!m_RefText.gameObject.activeSelf)
                m_RefText.gameObject.SetActive(true);            
        }
        else
        {
            //데미지 텍스트가 카메라보다 더 뒤 쪽에 있을경우 보일 필요가 없음
            if (m_RefText.gameObject.activeSelf)
                m_RefText.gameObject.SetActive(false);
        }

    }
}
