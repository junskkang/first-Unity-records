using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_4 : MonoBehaviour
{

    void Start()
    {
        //1. �Ʒ��� ���� �迭�� ���� �� 222 ���� �� ��° �ε����� �ֳ���?
        // �ε����� ã�� �ڵ带 �ۼ��ϰ� �ε����� ����� �ּ���.
        int[] App = { 34, 56, 12, 89, 120, 6, 8, 222, 67, 81, 110 };


        //��
        int aa = 0;
        while (App[aa] != 222)
        {
            aa++;
        }
        Debug.Log("�ش� �迭�� 222���� " + (aa + 1) + "��° �ε����� �ֽ��ϴ�.");

        //2. �Ʒ��� ���� �迭�� ���� �� 33�̶�� ���� �� �� �ε����� �������?
        //��, 99��� ���� �� ��° �ε����� �������?
        //ã�� �ڵ带 �ۼ��ϰ� �ε����� ����� �ּ���.
        int a_idx = 0;
        int[] VVV = new int[100];
        for(int ii= 0; ii < 100; ii++)
        {
            if((ii %3 ) == 0)
            {
                VVV[a_idx] = ii;
                a_idx++;
            }
        }


        //��
        //while��
        int find33 = 0;
        while (VVV[find33] != 33)
        {
            find33++;
        }
        Debug.Log("�ش� �迭�� 33�̶�� ���� " + (find33 + 1) + "��° �ε����� �ֽ��ϴ�.");
        
        int find99 = 0;
        while (VVV[find99] != 99)
        {
            find99++;
        }
        Debug.Log("�ش� �迭�� 99�̶�� ���� " + (find99 + 1) + "��° �ε����� �ֽ��ϴ�.");


        //for��
        for(int find = 0; find <= a_idx; find++ )
        {
            if (VVV[find] == 33)
                Debug.Log("�ش� �迭�� 33�̶�� ���� " + (find + 1) + "��° �ε����� �ֽ��ϴ�.");
            else if (VVV[find] == 99)
                Debug.Log("�ش� �迭�� 99�̶�� ���� " + (find + 1) + "��° �ε����� �ֽ��ϴ�.");
        }





        //3. �Ʒ��� ���� �迭�� ���� �� �Ʒ� ����� ���� ���� �� ����� ����� �ּ���.
        int[] ZZZ = new int[100];
        for(int ii = 0; ii < 100; ii++)
        {
            ZZZ[ii] = ii;
        }

        int[] a_rr = new int[100];
        //����
        //a_rr[0] = ZZZ[0] + ZZZ[1];
        //a_rr[1] = ZZZ[2] + ZZZ[3];
        //a_rr[2] = ZZZ[4] + ZZZ[5];
        //          :
        //a_rr[?] = ZZZ[98] + ZZZ[99];


        //��
        int yy = 0;
        int zz = 0;
        
        while (true)
        {
            a_rr[yy] = ZZZ[zz] + ZZZ[zz + 1];
            Debug.Log("a_rr[" + yy + "] : " + a_rr[yy]);

            if (zz == 98)
                break;

            yy++;
            zz+=2;
        }    



        //4. 
        int[] a_kk = { 23, 45, 12, 67, 34, 77, 103, 3, 6, 7, 8, 11, 65, 204, 33, 56 };
        //for���� ���鼭 �ִ밪�� �ּڰ��� ���ϰ� �ִ밪�� �ε����� �ּҰ��� �ε����� ���� ����� �ּ���.
        //��� ���� "�ִ� : ?? (�ε��� ??), �ּڰ� : ?? (�ε��� ??)"

        //��
        int min = a_kk[0];
        int max = a_kk[0];

        for (int count = 0; count < a_kk.Length; count++)
        {
            if (a_kk[count] < min)
                min = a_kk[count];
            else if (a_kk[count] > max)
                max = a_kk[count];
        }

        int xx = 0;
        int idxMin = 0;
        int idxMax = 0;

        while (true)
        {
            if (xx == a_kk.Length)
                break;
            else if (a_kk[xx] == min)
                idxMin = xx+1;
            else if (a_kk[xx] == max)
                idxMax = xx+1;

            xx++;
        }
        
        Debug.Log("�ּڰ� : " + min + "(" + idxMin + ")��° �ε���, �ִ� : " + max + "(" + idxMax + "��° �ε���)");


    }


    void Update()
    {
        
    }
}
