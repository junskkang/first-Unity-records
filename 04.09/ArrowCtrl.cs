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
            //���ǥ�� ������ ����
            WarningFlicker();

            return;
        }

        if (warningImg.gameObject.activeSelf == true)
            warningImg.gameObject.SetActive(false);


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


        //���ũ ǥ���ϱ�
        //Camera.main.WorldToScreenPoint(); ������ǥ�� UI��ũ����ǥ�� ȯ�� �Լ�
        //Camera.main.ScreenToWorldPoint(); UI��ũ����ǥ�� ������ǥ�� ȯ�� �Լ�
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        
        warningImg.transform.position = new Vector3(screenPos.x,
            warningImg.transform.position.y, warningImg.transform.position.z);
    }

    float alpha = -6.0f; //����(�÷��� ���İ�) ��ȭ �ӵ�

    void WarningFlicker()
    {
        if (warningImg == null)
            return;

        if (warningImg.color.a >= 1.0f)
            alpha = -6.0f;
        else if (warningImg.color.a <= 0.0f)
            alpha = 6.0f;

        //RGB���� 100% ������ ����
        warningImg.color = new Color(1.0f, 1.0f, 1.0f,
            warningImg.color.a + alpha*Time.deltaTime);
    }
}
