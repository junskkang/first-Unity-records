using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteController : MonoBehaviour
{
    float rotSpeed = 0;    //ȸ���ӵ�

    void Start()
    {
        Application.targetFrameRate = 60;   //�������� 60���� ����
        QualitySettings.vSyncCount = 0;  
        //����� �ֻ���(��������)�� �ٸ� ��ǻ���� ��� ĳ���� ���۽� ������ ������ �� �ִ�.
    }

    // Update is called once per frame
    void Update()
    {
        //Ŭ���ϸ� ȸ���ӵ��� ����
        if (Input.GetMouseButtonDown(0)) //���� ���콺��ư�� Ŭ���ϸ�
        {
            this.rotSpeed = 10;
        }


        //ȸ���ӵ���ŭ �귿 ȸ��  1�����ӿ� 10����
        transform.Rotate(0,0, this.rotSpeed);

        //Rotate()�� ����
        //Vector3 a_Rot = transform.transform.eulerAngles;  // eulerAngels = 0~360�� ��
        //a_Rot.z += this.rotSpeed;
        //transform.transform.eulerAngles = a_Rot;

        //transform.eulerAngels.z : 0~359.999f�� ������ ȯ���Ͽ� ������. ���̳ʽ��� 540���� 

        //�귿 �ӵ� ����
        this.rotSpeed *= 0.98f;
    }
}
