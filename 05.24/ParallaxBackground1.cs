using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground1 : MonoBehaviour
{
    [SerializeField] private Transform target;      //���� ���� �̾��� �����
    [SerializeField] private float scrollAmount;    //�� ����� �Ÿ�
    [SerializeField] private float moveSpeed;       //�̵��ӵ�
    [SerializeField] private Vector3 moveDirection; //�̵�����
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //���� ��� moveDirection�� �������� moveSpeed�� ������ �̵�
        this.transform.position += moveDirection * moveSpeed * Time.deltaTime;

        //ĳ���ʹ� ���������� �����ϴ� ������ �ֱ� ���� ����� ����(-x��)�������� �̵���ų ��
        //�׷��� �� �������� �Ÿ� ���� -�� �ٿ��ִ� ��!
        if (transform.position.x <= -scrollAmount)
        {
            transform.position = target.position - moveDirection * scrollAmount;
            //ex) ����� ��ġ�� = 40 - (-1 * 40) = ���� ��ġ�� 80�� �� ��
        }
    }
}
