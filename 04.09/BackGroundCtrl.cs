using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class BackGroundCtrl : MonoBehaviour
{
    GameObject player;
    float startY = 12.0f;     //��׶����� ���� y����
    float scroll = 0.2f;      //��׶��尡 ���� �ö󰡴� �ӵ�, 

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("cat");    
    }

    // Update is called once per frame
    void Update()
    {
        float scrollPos = startY - player.transform.position.y * scroll; //�÷��̾��� ������ 20%��ŭ
        if (scrollPos > 12.0f)  //����̹����� �ּ��ִ� ����
            scrollPos = 12.0f;
        else if (scrollPos < -12.0f)
            scrollPos = -12.0f;

        transform.position = new Vector3(0.0f, player.transform.position.y+scrollPos, 0.0f);
    }
}
