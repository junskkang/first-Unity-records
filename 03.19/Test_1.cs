using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

//�޼����� �����ε� (�������̵��� �ٸ� ����!)
//�ϳ��� �޼��� �̸����� ���� ���� �޼��带 ���� �ϴ� ��
//�Ű������� ���ĸ� �ٸ��� ���� �޼��� �̸��� ����� �� �ִ�
//�⺻�뵵�� �ϳ��� �Լ� �̸����� �پ��� �������� �Ű������� ���ϰ� ����ϱ� ���ؼ� ���Ǵ� ����

public class Test_1 : MonoBehaviour
{
    int Plus(int a, int b)
    { 
        return a + b;
    }

    double Plus(double a, double b) //1. �޼����� �����ε��� ��
    {
        return (a + b); 
    }

    int Plus(int a, int b, int c)  //2. �޼����� �����ε� ��
    {
        return a+b+c;
    }

    double Plus(int a, double b)  //3. �޼����� �����ε��� ��
    {
        return (a + b); 
    }

    // Start is called before the first frame update
    void Start()
    {
        int ret = Plus(45, 70);     // ù��° Plus()�� ����

        Debug.Log(Plus(1.8, 2.4));  // �ι�° Plus()�� ����
        Debug.Log(Plus(4, 7, 9));   // ����° Plus()�� ����
        Debug.Log(Plus(1, 2.4));    // �׹�° Plus()�� ����


        //����Ƽ C#���� �����ϴ� �޼��� �����ε��� ��
        int iRan = Random.Range(1, 7);            // 1~6
        float fRan = Random.Range(1.5f, 3.14f);   // 1.5~ 3.14
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
