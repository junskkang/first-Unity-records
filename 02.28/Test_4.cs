using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_4 : MonoBehaviour
{

    void Start()
    {
        //2�� 28�� ��ũ��Ʈ ��¹� �����ϱ�
        //������ 7���� Console View�� ����Ͻÿ�

        //Debug.Log("<������ 7��>");
        //int a = 7;
        //int sum;
        //for (int i = 1; i < 10; i++)
        //{
        //    sum = a * i;
        //    Debug.Log(a + " * " + i + " = " + sum);
        //}

        //for���� ����� �ٸ� ���
        //for (int x = 1; x <= 9; x++)
        //{
        //    Debug.Log(7 + " * " + x + " = " + (7 * x));
        //}

        //for���� ������� ���� ������
        //Debug.Log("<������ 7��>");
        //int Dan = 7;
        //int Idx = 1;
        //Debug.Log(Dan + " * " + Idx + " = " + (Dan * Idx));
        //Idx = 2;
        //Debug.Log(Dan + " * " + Idx + " = " + (Dan * Idx));
        //Idx = 3;
        //Debug.Log(Dan + " * " + Idx + " = " + (Dan * Idx));
        //Idx = 4;
        //Debug.Log(Dan + " * " + Idx + " = " + (Dan * Idx));
        //Idx = 5;
        //Debug.Log(Dan + " * " + Idx + " = " + (Dan * Idx));
        //Idx = 6;
        //Debug.Log(Dan + " * " + Idx + " = " + (Dan * Idx));
        //Idx = 7;
        //Debug.Log(Dan + " * " + Idx + " = " + (Dan * Idx));
        //Idx = 8;
        //Debug.Log(Dan + " * " + Idx + " = " + (Dan * Idx));
        //Idx = 9;
        //Debug.Log(Dan + " * " + Idx + " = " + (Dan * Idx));









        //������ 2�ܺ��� 9�ܱ��� Console View�� ����Ͻÿ�
        //    int sum;
        //    for(int a = 2; a<10; a++)
        //    {
        //        Debug.Log("<������ " + a + "��>");
        //        for (int i = 1; i < 10; i++)
        //        {
        //            sum = a * i;
        //            Debug.Log(a + " * " + i + " = " + sum);
        //        }

        //    }

        //}

        //1���� 10������ �ձ��ϱ�
        Debug.Log("1���� 10������ �ձ��ϱ�");

        int sum = 0;
        for(int i =1; i<=10; i++)
        {
            sum += i;
            Debug.Log((sum - i) + " + " + i + " = " +sum);

        }
    }
    // Update is called once per frame
   
    void Update()
    { }
}