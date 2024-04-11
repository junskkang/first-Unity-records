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
        //���Ͻ�Ű��
        transform.Translate(0.0f, -speed * Time.deltaTime, 0.0f);
        
        //ȭ��� ����
        if (transform.position.y < player.transform.position.y - 10.0f)
        {
            Destroy(gameObject);
        }
    }

    public void InitArrow(float a_PosX)  //�ʱ���ġ ����ֱ�
    {
        player = GameObject.Find("cat");
        transform.position = new(a_PosX * 1.15f, player.transform.position.y + 10.0f, 0.0f);
        // 1.15�� ����� ������ ����� ���߽�Ű�� ���� �� (�������� x��ǥ��)
    }
}
