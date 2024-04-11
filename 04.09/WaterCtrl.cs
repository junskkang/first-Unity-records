using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCtrl : MonoBehaviour
{
    float speed = 1.0f; //1초에 1m를 움직이게 한다는 속도
    GameObject player;
    float distanceItv = 8.0f;  //캐릭터와의 거리가 8m이상 벌어지지 않도록

    void Start()
    {
        player = GameObject.Find("cat");
    }

    // Update is called once per frame
    void Update()
    {
        //캐릭터와의 거리가 너무 먼 경우에 보정
        float a_FollowHeight = player.transform.position.y - distanceItv;
        if(transform.position.y < a_FollowHeight)
            transform.position = new Vector3(0.0f, a_FollowHeight, 0.0f);

        //일정한 속도로 위로 움직이게 하기
        transform.Translate(new Vector3(0.0f, speed*Time.deltaTime, 0.0f));
    }
}
