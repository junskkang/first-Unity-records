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
            //�ִϸ����� 0�� ���̾ ����� ������ ������
            AnimatorStateInfo animStateInfo = m_RefAnimator.GetCurrentAnimatorStateInfo(0);
            //�ش� �ִϸ��̼��� �ѵ��� ���̸� ������
            float lifeTime = animStateInfo.length;
            Destroy(this.gameObject, lifeTime);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //World ��ǥ�� UGUI ��ǥ�� ȯ�����ִ� �ڵ�
        rectCanvas = GameMgr.Inst.m_DamageCanvas.GetComponent<RectTransform>();
        ScreenPos = Camera.main.WorldToViewportPoint(m_BaseWdpos);
        WdScPos.x = ((ScreenPos.x * rectCanvas.sizeDelta.x) - (rectCanvas.sizeDelta.x * 0.5f));
        WdScPos.y = ((ScreenPos.y * rectCanvas.sizeDelta.y) - (rectCanvas.sizeDelta.y * 0.5f));
        transform.GetComponent<RectTransform>().anchoredPosition = WdScPos;

        //ī�޶� �ø�
        m_CacVec = m_BaseWdpos - Camera.main.transform.position;
        if (m_CacVec.magnitude <= 0.0f)
        {
            //������ �ؽ�Ʈ�� ī�޶� ���� ��ġ�� ������� ���� �ʿ䰡 ����
            if (m_RefText.gameObject.activeSelf)
                m_RefText.gameObject.SetActive(false);            
        }
        else if (0.0f < Vector3.Dot(Camera.main.transform.forward, m_CacVec.normalized))    
        {
            //ī�޶� �տ� ��ġ
            if (!m_RefText.gameObject.activeSelf)
                m_RefText.gameObject.SetActive(true);            
        }
        else
        {
            //������ �ؽ�Ʈ�� ī�޶󺸴� �� �� �ʿ� ������� ���� �ʿ䰡 ����
            if (m_RefText.gameObject.activeSelf)
                m_RefText.gameObject.SetActive(false);
        }

    }
}
