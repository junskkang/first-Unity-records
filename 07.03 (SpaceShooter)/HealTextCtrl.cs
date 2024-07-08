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

    //LateUpdate 코드를 위해
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
        //데미지 폰트가 떴던 그 위치에서 머무르도록 하기 위한 좌표 갱신
        //하지 않으면 폰트가 그대로 캐릭터 이동에 따라 따라오게 됨.
        // World좌표를 UGUI 좌표로 환산해 주는 코드
        canvasRect = refHCanvas.GetComponent<RectTransform>();

        screenPos = Camera.main.WorldToViewportPoint(baseWdPos);
        wdScPos.x = ((screenPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f));
        wdScPos.y = ((screenPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));

        transform.GetComponent<RectTransform>().anchoredPosition = wdScPos;

        //카메라 컬링
        //View Frustum Culling 카메라에 보이는 영역만 그리게 하는 것
        cacVec = baseWdPos - Camera.main.transform.position;    //카메라에서 데미지폰트를 향하는 벡터
        if (cacVec.magnitude <= 0.0f)   //겹쳐있다.
        {
            //힐 텍스트와 카메라가 같은 위치에 있어도 보일 필요 없음
            if(refText.gameObject.activeSelf)
                refText.gameObject.SetActive(false);
        }
        else if (0.0f < Vector3.Dot(Camera.main.transform.forward, cacVec.normalized))
        {//카메라 앞쪽에 있다는 뜻
            if (!refText.gameObject.activeSelf)
                refText.gameObject.SetActive(true);
        }
        else //if (Vector3.Dot(Camera.main.transform.forward, cacVec.normalized) <=0.0f);
        {//카메라 뒤쪽에 있다는 뜻
            if (refText.gameObject.activeSelf)
                refText.gameObject.SetActive(false);
        }
        //Vector3.Dot(vec1, vec2) : 두 벡터의 내적값을 계산하여 같은 방향이면 1,
        //직각이면 0, 반대방향이면 -1 값이 나온다
        //주로 대상이 어느 방향에 있는지 확인할 때 내적을 활용하면 좋다.

    }

    public void InitState(int cont, Vector3 spawnPos, Transform canvas, Color color)
    {
        Vector3 startPos = new Vector3(spawnPos.x, spawnPos.y + 2.21f, spawnPos.z);
        transform.SetParent(canvas, false);
        worldPos = spawnPos;
        healValue = cont;

        refHCanvas = canvas;
        baseWdPos = startPos;

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
                refText.text = healValue.ToString() + " Dmg";
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
