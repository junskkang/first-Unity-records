using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCtrl : MonoBehaviour
{
    float speed = 1.0f; //1�ʿ� 1m�� �����̰� �Ѵٴ� �ӵ�
    GameObject player;
    float distanceItv = 8.0f;  //ĳ���Ϳ��� �Ÿ��� 8m�̻� �������� �ʵ���

    void Start()
    {
        player = GameObject.Find("cat");
    }

    // Update is called once per frame
    void Update()
    {
        //ĳ���Ϳ��� �Ÿ��� �ʹ� �� ��쿡 ����
        float a_FollowHeight = player.transform.position.y - distanceItv;
        if(transform.position.y < a_FollowHeight)
            transform.position = new Vector3(0.0f, a_FollowHeight, 0.0f);

        //������ �ӵ��� ���� �����̰� �ϱ�
        transform.Translate(new Vector3(0.0f, speed*Time.deltaTime, 0.0f));
    }
}
