using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Value Type�� ���� : int, float, double .... struct(����ü)
//Reference Type�� ���� : array, class��ü

public class Test_3 : MonoBehaviour
{
    //����Ƽ���� �迭�� �ɹ� ������ �����ؼ� ����ϴ� 2���� ���
    int[] m_Brr = new int[5];     //�ɹ� �迭 ���� ù��° ��� ��
    public int[] Crr;          // �ɹ� �迭 ���� �ι�° ��� �� ������ �������� ����
   
  
    void Start()
    {
        //    int AA0 = 0;
        //    int AA1 = 0;    
        //    int AA2 = 0;

        //    int[] arr = new int[100];   //C# �迭 ���� ����

        int[] arr = new int[5];
        for (int a = 0; a < arr.Length; a++)
            arr[a] = 10 + a;

        for (int i = 0; i < 5; i++)
        {
            Debug.Log(arr[i]);
        }

        Debug.Log("arr ũ�� : " + arr.Length);


        // �ɹ� �迭 ���� ù��° ��� ����
        m_Brr[0] = 11;
        m_Brr[1] = 12;
        m_Brr[2] = 13;
        m_Brr[3] = 14;
        m_Brr[4] = 15;
        for(int ii = 0; ii < m_Brr.Length; ii++)
        {
            Debug.Log("m_Brr[" + ii + "] : " + m_Brr[ii]);
        }


        // ����Ƽ���� �迭�� ������ ���� �������� ��� ����
        for(int ii = 0; ii< Crr.Length; ii++)
        {
            Debug.Log("Crr[" + ii + "] : " + Crr[ii]);
        }


        int[] a_AAA = new int[10];
        int[] a_BBB;                 //���� �س��� ���� �������
        a_BBB = new int[10];       //�� ����ϱ� ���� ������ �� ���� ����� ������ ���� �����
        a_BBB[0] = 11;               //
        
        for(int ii = 0; ii< a_AAA.Length; ii++)
            Debug.Log(a_AAA[ii]);



        //�Ͻ��� ����
        int[] a_CCC = { 10, 20, 30, 40, 50, 60 };
        //�迭�� ����� ���ÿ� �ε��� ������� �ʱ�ȭ���� ��������

        for(int ii = 0;ii< a_CCC.Length; ii++)
            Debug.Log(a_CCC[ii]);

        //int[] a_XXX;
        //a_XXX = { 1, 2, 3, 4, 5, 6 };

        //�����+�Ͻ��� ����
        int[] a_EEE = new int[] { 10, 20, 30, 40, 50, 60 };
        int[] a_FFF;
        a_FFF = new int[] { 11, 12, 13, 14, 15 };
        for(int ii = 0;ii < a_FFF.Length; ii++)
            Debug.Log(a_FFF[ii]);



        int AAAAA = 1000;
        int BBBBB = AAAAA;   // �Ϲ� �������� ���� ����ȴٶ�� �濡�� Value Type
        BBBBB = 99;
        // AAAAA = 1000, BBBBB = 99

        int[] CCCCC = { 1000 };
        int[] DDDDD = CCCCC;
        DDDDD[0] = 99;      //�迭������ ���� ��������� �����Ѵٴ� �濡�� Reference Type
        Debug.Log("CCCCC[0] : " + CCCCC[0] + ", DDDDD[0] " + DDDDD[0]);
        // CCCCC = 99, DDDDD = 99
        int[] EEEEE = CCCCC;
        // CCCCC = DDDDD = EEEEE ��� �ϳ��� ������ �����ϰ� ��.

        int[] ZZZ = new int[3];
        ZZZ[0] = 10;
        ZZZ[1] = 20;
        ZZZ[2] = 30;

        ZZZ = new int[5];
        ZZZ[3] = 999;
        ZZZ[4] = 1000;
        for(int ii = 0; ii < ZZZ.Length; ii++)
            Debug.Log(ZZZ[ii]);
       
    }


    void Update()
    {
        
    }
}
