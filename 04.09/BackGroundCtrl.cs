using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class BackGroundCtrl : MonoBehaviour
{
    GameObject player;
    float startY = 12.0f;     //백그라운드의 시작 y높이
    float scroll = 0.2f;      //백그라운드가 위로 올라가는 속도, 

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("cat");    
    }

    // Update is called once per frame
    void Update()
    {
        float scrollPos = startY - player.transform.position.y * scroll; //플레이어의 높이의 20%만큼
        if (scrollPos > 12.0f)  //배경이미지의 최소최댓값 지정
            scrollPos = 12.0f;
        else if (scrollPos < -12.0f)
            scrollPos = -12.0f;

        transform.position = new Vector3(0.0f, player.transform.position.y+scrollPos, 0.0f);
    }
}
