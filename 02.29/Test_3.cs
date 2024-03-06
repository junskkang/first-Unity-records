using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ 
public class Test_3 : MonoBehaviour
{
  
    void Start()
    {
        //1. ���Ŀ�����
        //int a = 88;
        //int b = 22;
        int a = 88, b = 22;  // ���� ���������� ������ �� �޸��� �����Ͽ� ���� ����
        int c = a + b;
        Debug.Log(a+" + "+b +" = "+c);

        c = a - b;
        Debug.Log(a+" - "+b+" = "+c);

        //string str = string.Format("{0} * {1} = {2}", a, b, a * b); //Format���ڿ�
        //������� a�� ���� 0�� ����, b�� ���� 1��, a+b���� 2�� ���� �Լ�
        string str = $"{a} * {b} = {a * b}"; //format���ڿ��� �ٸ� ǥ����
        Debug.Log(str);

        //str = string.Format("{0} / {1} = {2}", a, b, a / b);
       
        Debug.Log($"{a} / {b} = {a / b}");

        //������ ������
        Debug.Log($"{a} % {b} = {a % b}");
        //���� ���ں��� �ϳ� ���� ������ �ݺ��Ǵ� Ư¡�� �ִ�. 
        //ex)3���� ������ �������� 0, 1, 2�� �ݺ���
        //    ��           ������
        // 0 / 2 = 0     0 % 2 = 0
        // 1 / 2 = 0     1 % 2 = 1
        // 2 / 2 = 1     2 % 2 = 0
        // 3 / 2 = 1     3 % 2 = 1
        // 4 / 2 = 2     4 % 2 = 0
        // 5 / 2 = 2     5 % 2 = 1

       
        
        //2. ���������� c, c++, c#

        int cc = 0;
        cc++; // c = c + 1; ������ ����Ͽ� ���׿� �־���
        ++cc; // c = c + 1; �ܵ����� ���� ���� 1 ���� ��Ű��� ��. ���� ����
        Debug.Log("�ܵ� ����� ��� : " + cc);

        cc = 0;
        Debug.Log(string.Format("���� ��ɾ�� ���� ��� �ڿ� ���� �� : {0} ", cc++));
        //cc�� 1����������, ���ڿ��� �������, �ܼ�â�� ������� 3���� ����� ����
        //������ �ڿ� �پ��� ��� �ٸ� ����� ��ġ�� ���� ������ ��
        //������ 1���� ��Ű�� ���� �������̹Ƿ�
        //������� 0
        Debug.Log(cc);
        //���⼭ �ٽ� Debug.Log()�� ����� �� ������� 1
        
        cc = 0;
        Debug.Log(string.Format("���� ��ɾ�� ���� ��� �տ� ���� �� : {0} ", ++cc));
        //������ �տ� �پ��� ��� ���� ���� ������ �ϰ� �ٸ� ����� ������
        //������ 1���� ��Ű�� ���� ����
        //������� 1


        int ff = 10;
        ff--; // ff = ff - 1; 
        --ff; // �ܵ����� ���� ���� ++�� ���������� 1���� ��Ű��� �ǹ�
        Debug.Log(string.Format("ff : {0}", ff)); //������� 8


         
        //3. �Ҵ翬���� = ���� ǥ��
        int a_xx = 10;
        a_xx += 5;  // a_xx = a_xx + 5;
        //������� ���� 3���� ǥ��
        //a_xx = a_xx + 1;    a_xx += 1;    a_xx++;
        a_xx -= 3;  // a_xx = a_xx - 3;

        int a_yy = 10;
        a_yy *= 2;  // a_yy = a_yy * 2;
        a_yy /= 2;  // a_yy = a_yy / 2;
        a_yy %= 2;  // a_yy = a_yy % 2;



        //4. ��������
        int ggg = 50, hhh = 60;
        bool a_Check = ggg > 40 && hhh > 50;   // and ������   ~�̰�, �׸���
        //50�� 40���� ũ�� 60�� 50���� ũ��. �ΰ����� ��� �����ϱ� ������ true
        Debug.Log("ggg > 40 && hhh > 50 : " + a_Check);
        // true  && true  = true
        // ture  && false = false
        // false && true  = false
        // false && false = false

        a_Check = ggg > 40 || hhh > 70;   // or ������  ~�̰ų�, �Ǵ�
        Debug.Log("ggg > 40 || hhh > 50 : " + a_Check);
        //50�� 40���� ũ�ų� 60�� 70���� ũ��. �� �߿� �ϳ��� �����Ͽ��� true
        // true  || true  = true
        // ture  || false = true
        // false || true  = true
        // false || false = false

        a_Check = (ggg > hhh);    // 50�� 60���� ũ��?  
        Debug.Log(a_Check);       // false
        a_Check = !(ggg > hhh);   // ! Not������    ����� ������Ű�� ������
        Debug.Log(a_Check);       // true


        //5. ���迬����
        int AAA = 50;
        int BBB = 60;
        Debug.Log("AAA < BBB : " + (AAA < BBB));     // true
        Debug.Log("AAA > BBB : " + (AAA > BBB));     // false
        Debug.Log("AAA == BBB : " + (AAA == BBB));     // false
        Debug.Log("AAA != BBB : " + (AAA != BBB));     // true
        Debug.Log("AAA <= BBB : " + (AAA <= BBB));     // true
        Debug.Log("AAA >= BBB : " + (AAA >= BBB));     // false


        //6. ��Ʈ������ 10������ 2������ ǥ�����ִ� ���, �� �ڸ������� ��
        int nnn = 5;     // 0101
        int mmm = 10;    // 1010

        int Result = nnn & mmm;    // &&�� �ǹ̿� ����. 0000(2����) ---> 0(10����)
        Debug.Log("nnn & mmm : " + Result); //����� 0


        Result = nnn | mmm;        // ||�� �ǹ̿� ����. 1111(2����) ---> 15(10����) 
        Debug.Log("nnn | mmm : " + Result); //����� 15

        // ���̾� ����ũ �ÿ� Ȱ��
        // 00100010 ��Ʈ������ �ɰ��� �� �ڸ������� ������ �ǹ̸� �ο��ϰ�
        // �� �κ��� true/false�� ����� �� �ִ� ��

        // ^ XOR ������ : �� ���� ������ 0, �� ���� �ٸ��� 1
        Result = nnn ^ mmm;    //  1111(2����)  --->  15(10����)
        Debug.Log("nnn ^ mmm : " + Result); //����� 15

        int kkk = 2357;                                      // 0000 1001 0011 0101
        int a_ScVal = kkk ^ 6789;  // ��ȣȭ   6789�� ��й�ȣ! 0001 1010 1000 0101
        Debug.Log("a_ScVal : " + a_ScVal);  //����� 5040       0001 0011 1011 0000
        int a_MyVal = a_ScVal ^ 6789;  //��ȣȭ   6789�� �Ȱ��� �ѹ� �� �־��ָ�?
        Debug.Log("a_MyVal : "+ a_MyVal);   //����� 2357
    }

    void Update()
    {
        
    }
}
