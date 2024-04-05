using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleController : MonoBehaviour
{
    GameObject player;
    GameObject GameD = null;
    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.Find("player");

        GameD = GameObject.Find("GameDirector");  //GameDirector��� ������Ʈ�� �ҷ���
    }

    // Update is called once per frame
    void Update()
    {
        //�����Ӹ��� ������� ���Ͻ�Ų��
        transform.Translate(0, -0.15f, 0);

        //ȭ�� ������ ������ ������Ʈ�� �Ҹ��Ų��.
        if (transform.position.y < -5.0f)
        {
            Destroy(gameObject);          //�θ������Ʈ�� �ı�
        }

        //�浹 ����
        Vector2 p1 = transform.position;             //ȭ���� �߽���ǥ
        Vector2 p2 = this.player.transform.position; //�÷��̾��� �߽���ǥ
        Vector2 dir = p1 - p2;                       //�÷��̾�� ȭ��� ���ϴ� ���� ��
        float d = dir.magnitude;                     //�����Ÿ�   
        float r1 = 0.5f;                             //ȭ���� �ݰ�
        float r2 = 0.95f;                             //�÷��̾��� �ݰ�

        if (d < r1 + r2)  //�����Ÿ��� �� ������Ʈ�� �ݰ� �պ��� �������
        {
            //GameDirector��ũ��Ʈ ���� ���� �Լ�
            GameD.GetComponent<GameDirector>().GetApple();

            Destroy(gameObject); //�浹�̹Ƿ� �ı�
        }

        if (GameDirector.m_State == GameState.GameEnd)
        {
            Destroy(gameObject);
        }
    }
}
