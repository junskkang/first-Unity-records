using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCtrl : MonoBehaviour
{
    GameObject player;
    float speed = 5.0f;
    void Start()
    {
        player = GameObject.Find("cat");

    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
