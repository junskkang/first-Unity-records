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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitState(int cont, Vector3 spawnPos, Transform canvas, Color color)
    {
        Vector3 startPos = new Vector3(spawnPos.x, spawnPos.y + 2.21f, spawnPos.z);
        transform.SetParent(canvas, false);
        worldPos = spawnPos;
        healValue = cont;

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
                refText.text = "-" + healValue.ToString() + " Dmg";
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
