using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//����ȯ (Casting)
//1. �ڵ� ����ȯ : ���� �ٸ� �������� ������ �����ϰų� ������ �� �� �ڵ����� ����ȯ �Ǵ� ��Ģ
//                 ��, ���� �ٸ� �������� �� �� ū �����θ� ��� ��ȯ�Ǵ� Ư¡�� �ִ�.
// ������ ���� ũ�� ����
// double > float(4Byte) > ulong > long(8Byte) > uint > int > ushort > short > char
// �Ǽ��� > ���������� > ���������� > ������
//2. ���� ����ȯ
public class Test_3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //1. �ڵ�����ȯ
        int ii = 345;
        float ff = ii;  //�ڵ� ����ȯ �Ǿ� ff�� ���� 345.0f
        //int aa = ff;  //C#������ ū���������� �������������� �ڵ����� ���� �Ұ���
        Debug.Log("ii : " + ii + ", ff : " + ff);
        //���ڿ��� ���ڸ� ���ϸ� ���ڸ� ���ڿ��� ��ȯ�Ͽ� ���Ĺ���
        //ii�� ff�� ���������� ���� Debug.Log���� ���ڿ��� ���Ʊ� ������ ���ڿ��� ��ȯ��.
        //"ii : " + "345" + ", ff : " + "345"
        // Console ��°� : ii : 345, ff : 345

        long aaa = 123L; //8Byte¥�� ������ ��� �������� ���� �ڿ� L�� ���̴°� ����
        // �ҹ��ڴ� ����1�� ������ �ȵǴ� �빮��L�� �ٿ��ֵ��� ����
        Debug.Log(aaa + " : " + sizeof(long));
        // "123" + " : " + "8"
        // "123 : 8"
        aaa = ii; //int�� ii�� long�� aaa�� ����ȯ ���� 345L
        //ii = aaa; //long�� int�� ��� ���� �Ұ���
        //aaa = ff; //float�� long�� ��� ���� �Ұ���
        ff = aaa; //long�� float�� ��� ���� ����

        //2. ���� ����ȯ

        float a_ff = 12.34f;
        //int a_ii = a_ff; //int�� float�� ���� �� ����. �ڵ� ����ȯx
        int a_ii = (int)a_ff; // ���� ����ȯ(���� ����ȯ)
        Debug.Log("a_ii : " + a_ii);
        //�Ҽ��� ���� �������� int �������� 12 ���� ����

        //���� ����ȯ Ȱ���ϴ� ��
        //������ ������ �Ҽ��� ���� ��� ���� ��
        float xxx = 123.456f;
        float Myfloat = xxx - (int)xxx; // 123.456f - 123.0f
        Debug.Log(Myfloat);
        //��������ȯ�� ����Ŀ��� �ٷ� ����� ���
        int MyInt = (int)xxx;
        float MyFloat = xxx- MyInt; // 123.456f - 123.0f
        Debug.Log(MyFloat);
        //��������ȯ�� �����Ų ���ο� MyInt��� ������ �����
        //�� ������ ����Ŀ� ����� ���



        //�������� <--> ���ڿ����� ��ȯ
        int ABC = 123;
        string CBA = "123";

        int KKK = 123 + ABC; // ���� 123 + 123 = 246
        string SSS = "123" + ABC; // ���ڿ� "123"+"123" = "123123"
                                  //����ġ�� ������ ������ ���� ������ ���� ����������, ȭ�鿡 ǥ�ô� ���ڿ� ǥ����

        // �������� --> ���ڿ����� ��ȯ
        ABC = 777;
        //CBA = ABC; //int�� ABC�� string�� CBA�� ������ �� ����
        CBA = " " + ABC; //���ڿ� ���ڸ� ���ϴ� ����Ŀ��� �ڵ�����ȯ��.
        CBA = ABC.ToString(); // .ToString���� ���� �������°� ���ڿ� ���·� ��ȯ��


        //���ڿ����� --> ��������
        string EEE = "123";
        //int FFF = EEE; //����ȯ���δ� �ȵǰ� �Լ��� ��� �Ѵ�.
        //int FFF = int.Parse(EEE); // Ư�� ���ڰ� ���ԵǸ� ������ ����.
        //Debug.Log(FFF);

        //int.TryParse(); //Parse();���� ������ �Լ�
        int FFF = 0;
        int.TryParse("123", out FFF);
        Debug.Log(FFF);

        FFF = FFF + 5000; // 123+5000�� �ٽ� FFF�� ��´ٶ�� �ǹ�
        Debug.Log(FFF);

        float ppp = 0.0f;
        float.TryParse("123.456", out ppp);
        //0.0�� ���� ���� ppp�� ���ڿ� "123.456"��
        //������ 123.456���� ��ȯ�Ͽ� ��´ٴ� �ǹ�
        Debug.Log("ppp : "+ ppp);

        
     }

    // Update is called once per frame
    void Update()
    {
        
    }
}
