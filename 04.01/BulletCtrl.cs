using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    float m_Speed = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1.0f);   //��ũ��Ʈ�� �θ��� ���ӿ�����Ʈ �ı�
    }

    // Update is called once per frame
    void Update()
    {
        //�ӵ� = �Ÿ�/�ð�
        //�ӵ� * �ð� = �Ÿ�
        transform.position += Vector3.right * m_Speed * Time.deltaTime; // �������ӵ��� �̵��ϰ� �� �Ÿ�
    }
}
