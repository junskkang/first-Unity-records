using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMove : MonoBehaviour
{
    //��ũ�� �̵� �� ȸ�� �ӵ��� ��Ÿ���� ����
    public float moveSpeed = 20.0f;
    public float rotSpeed = 50.0f;

    //������ ������Ʈ�� �Ҵ��� ����
    private Rigidbody rbody;
    private Transform tr;

    //Ű���� �Է°� ����
    private float h, v;
    void Start()
    {
        //������Ʈ �Ҵ�
        rbody = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();

        //Rigidbody�� �����߽��� ���� ����
        rbody.centerOfMass = new Vector3(0.0f, -2.5f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //ȸ���� �̵� ó��
        tr.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);
        tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
    }
}
