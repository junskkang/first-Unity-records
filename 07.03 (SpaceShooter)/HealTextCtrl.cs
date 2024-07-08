using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealTextCtrl : MonoBehaviour
{
    Text refText;
    float healValue = 0.0f;
    Vector3 worldPos = Vector3.zero;
    Animator anim;

    //LateUpdate �ڵ带 ����
    Transform refHCanvas;
    RectTransform canvasRect = null;
    Vector3 baseWdPos = Vector3.zero;
    Vector2 screenPos = Vector2.zero;
    Vector2 wdScPos = Vector2.zero;
    Vector3 cacVec = Vector3.zero;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        //������ ��Ʈ�� ���� �� ��ġ���� �ӹ������� �ϱ� ���� ��ǥ ����
        //���� ������ ��Ʈ�� �״�� ĳ���� �̵��� ���� ������� ��.
        // World��ǥ�� UGUI ��ǥ�� ȯ���� �ִ� �ڵ�
        canvasRect = refHCanvas.GetComponent<RectTransform>();

        screenPos = Camera.main.WorldToViewportPoint(baseWdPos);
        wdScPos.x = ((screenPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f));
        wdScPos.y = ((screenPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));

        transform.GetComponent<RectTransform>().anchoredPosition = wdScPos;

        //ī�޶� �ø�
        //View Frustum Culling ī�޶� ���̴� ������ �׸��� �ϴ� ��
        cacVec = baseWdPos - Camera.main.transform.position;    //ī�޶󿡼� ��������Ʈ�� ���ϴ� ����
        if (cacVec.magnitude <= 0.0f)   //�����ִ�.
        {
            //�� �ؽ�Ʈ�� ī�޶� ���� ��ġ�� �־ ���� �ʿ� ����
            if(refText.gameObject.activeSelf)
                refText.gameObject.SetActive(false);
        }
        else if (0.0f < Vector3.Dot(Camera.main.transform.forward, cacVec.normalized))
        {//ī�޶� ���ʿ� �ִٴ� ��
            if (!refText.gameObject.activeSelf)
                refText.gameObject.SetActive(true);
        }
        else //if (Vector3.Dot(Camera.main.transform.forward, cacVec.normalized) <=0.0f);
        {//ī�޶� ���ʿ� �ִٴ� ��
            if (refText.gameObject.activeSelf)
                refText.gameObject.SetActive(false);
        }
        //Vector3.Dot(vec1, vec2) : �� ������ �������� ����Ͽ� ���� �����̸� 1,
        //�����̸� 0, �ݴ�����̸� -1 ���� ���´�
        //�ַ� ����� ��� ���⿡ �ִ��� Ȯ���� �� ������ Ȱ���ϸ� ����.

    }

    public void InitState(int cont, Vector3 spawnPos, Transform canvas, Color color)
    {
        Vector3 startPos = new Vector3(spawnPos.x, spawnPos.y + 2.21f, spawnPos.z);
        transform.SetParent(canvas, false);
        worldPos = spawnPos;
        healValue = cont;

        refHCanvas = canvas;
        baseWdPos = startPos;

        //�ʱ� ��ġ ����ֱ� - Wolrd ��ǥ�� UGUI ��ǥ�� ȯ���� �ִ� �ڵ�
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 screenPos = Camera.main.WorldToViewportPoint(startPos);
        Vector2 worldScreenPos = Vector2.zero;
        worldScreenPos.x = (screenPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f);
        worldScreenPos.y = (screenPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f);
        //canvasRect.sizeDelta�� UI���� ȭ�� ũ�⿡ 16: 9 ����
        this.GetComponent<RectTransform>().anchoredPosition = worldScreenPos;

        refText = this.GetComponentInChildren<Text>();
        if (refText != null)
        {
            if (healValue <= 0)
                refText.text = healValue.ToString() + " Dmg";
            else
                refText.text = "+" + healValue.ToString() + " Heal";

            refText.color = color;
        }

        anim = GetComponentInChildren<Animator>();
        if (anim != null) 
        {
            AnimatorStateInfo animInfo = anim.GetCurrentAnimatorStateInfo(0);
            float lifeTime = animInfo.length;   //�ִϸ��̼� �÷��� �ð�
            Destroy(gameObject, lifeTime);
        }
    }
}
