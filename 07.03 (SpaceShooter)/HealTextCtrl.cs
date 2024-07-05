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

        //초기 위치 잡아주기 - Wolrd 좌표를 UGUI 좌표로 환산해 주는 코드
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 screenPos = Camera.main.WorldToViewportPoint(startPos);
        Vector2 worldScreenPos = Vector2.zero;
        worldScreenPos.x = (screenPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f);
        worldScreenPos.y = (screenPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f);
        //canvasRect.sizeDelta는 UI기준 화면 크기에 16: 9 비율
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
            float lifeTime = animInfo.length;   //애니메이션 플레이 시간
            Destroy(gameObject, lifeTime);
        }
    }
}
