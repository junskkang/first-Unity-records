using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;


//���� : �����͸� �����ϴ� �޸� ����
//������ ���� : �������� ������ = �ʱⰪ;
public class Test_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int age = 30;
        string name;
        float height = 158.3f;

        age = 123; // ���� age�� ���� 123���� �ʱ�ȭ ��.
        age = 1004 + 1000 - 500; //������ ������� age ������ ����
       
        Debug.Log(age);

        float height1 = 160.5f; //C#���� ���� �ڿ� f(float�� f)�� �ٿ������.
        float height2;
        height2 = height1;
        Debug.Log(height2);

        double abc = 567.8;
        //double�� �Ҽ����� ��� ���������̰� 8Byte�� ������. ��,f�� ������ ����.
        //double�� �Ǽ����� �����ϴ� ����������, double���� float������ ������ ũ��.
        //�Ҽ����� ������ ũ�� = �����ϰ� �������� ����.
        //float�� double�� �����Ͽ��� ����� ����
        //���� float�� ǥ���� ������ �Ǽ��� double�� ����� ���� ������ �����̴�.
        //���� = ����ȭ �˸�, �� ���� ����, ���ſ�

        // 1Byte = 8Bit , Bit ��Ʈ�� ��ǻ�� �������� �ּ� ����
        short ABC = 32767; //short���� �ִ�ġ

        bool abab = true; //�� �Ǵ� ������ ����ϴ� ��������
        Debug.Log(abab);
        abab = false;
        Debug.Log(abab);

        int ccc;       //������ ����
        ccc = 10;      //������ �ʱ�ȭ

        int ddd = 10;  //������ ����� ���ÿ� �ʱ�ȭ


        //Debug.Log(ccc);

        //char ó���� ���ĺ� ���ڸ� �����ϱ� ���� �뵵�� �������
        //c, c++ : char 1Byte �ƽ�Ű�ڵ�
        char ccdd = 'k';
        //�ѱ�, ���� �� 1Byte���� ���� �� ���� ���ڵ��� ������
        //c# : char 2Byte �����ڵ�
        ccdd = '��';
        Debug.Log(ccdd);
        Debug.Log(sizeof(char));  //���������� �޸� ������ Ȯ���ϴ� ���

        //string
        string strName;
        strName = "GDragon";
        Debug.Log(strName);

        strName = "�ݰ����� " + "���� " + "���� ���� ���ƿ�.";
        //���ڿ��� ���ڿ��� + ��ȣ�� ���ϸ� ���ڿ��� ������
        //������ �����ε� : �������� �ǹ̸� �������ؼ� ����ϰڴٴ� �ǹ�
        Debug.Log(strName);

        byte ggg = 255; // 0~255���� ���� �� �ִ� 1Byte¥�� ��������

        //var �ʱⰪ�� ���� ���������� �����Ǵ� ��������
        var vv1 = 123;
        var vv2 = 3.14f;
        var vv3 = "seoul";
        var vv4 = true;

        //vv1 = "korea"; ������ ��
        vv3 = "korea";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
