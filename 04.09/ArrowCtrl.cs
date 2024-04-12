using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowCtrl : MonoBehaviour
{
    GameObject player;
    float speed = 5.0f;

    public Image warningImg;
    float timer = 1.0f;

    void Start()
    {
        player = GameObject.Find("cat");

    }

    // Update is called once per frame
    void Update()
    {

        if (0.0f < timer)
        {
            timer -= Time.deltaTime;
            //경고표시 깜빡임 연출
            WarningFlicker();

            return;
        }

        if (warningImg.gameObject.activeSelf == true)
            warningImg.gameObject.SetActive(false);


        //낙하시키기
        transform.Translate(0.0f, -speed * Time.deltaTime, 0.0f);
        
        //화면밖 제거
        if (transform.position.y < player.transform.position.y - 10.0f)
        {
            Destroy(gameObject);
        }
    }

    public void InitArrow(float a_PosX)  //초기위치 잡아주기
    {
        player = GameObject.Find("cat");
        transform.position = new(a_PosX * 1.15f, player.transform.position.y + 10.0f, 0.0f);
        // 1.15란 상수는 구름의 가운데에 적중시키기 위한 값 (구름들의 x좌표값)


        //경고마크 표시하기
        //Camera.main.WorldToScreenPoint(); 월드좌표를 UI스크린좌표로 환산 함수
        //Camera.main.ScreenToWorldPoint(); UI스크린좌표를 월드좌표로 환산 함수
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        
        warningImg.transform.position = new Vector3(screenPos.x,
            warningImg.transform.position.y, warningImg.transform.position.z);
    }

    float alpha = -6.0f; //투명도(컬러의 알파값) 변화 속도

    void WarningFlicker()
    {
        if (warningImg == null)
            return;

        if (warningImg.color.a >= 1.0f)
            alpha = -6.0f;
        else if (warningImg.color.a <= 0.0f)
            alpha = 6.0f;

        //RGB값은 100% 투명도만 조정
        warningImg.color = new Color(1.0f, 1.0f, 1.0f,
            warningImg.color.a + alpha*Time.deltaTime);
    }
}
