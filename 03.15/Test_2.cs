using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�ݺ���
// for��
// for(�ʱ��; ���ǽ�; ������)
// {
//      �ݺ��� �ڵ�
// }

//while��
// while (���ǽ�)
// {
//    �ݺ��� �ڵ�
// }
public class Test_2 : MonoBehaviour
{
    void Start()
    {
        //for�� �ȿ��� a������ ��ȭ���� 0~99 , �ݺ�Ƚ���� 100
        //for ( int a = 0; a < 100; a++)
        //{
        //    Debug.Log(a);
        //}

        //for( int xx = 10; xx > 0; xx--)
        //{ 
        //    Debug.Log(xx); 
        //}

        //Debug.Log("<������ 7��>");
        //for (int i = 1; i<10; i++)
        //{
        //    Debug.Log("7 * " + i + " = " + (7 * i));
        //}

        ////���� for��
        //Debug.Log("<������ 2�ܿ��� 9�ܱ���>");
        //for (int a_Dan = 2; a_Dan < 10; a_Dan++)
        //{
        //    Debug.Log("<������ " + a_Dan + "��>");
        //    for(int idx = 1; idx < 10; idx++)
        //    {
        //        Debug.Log(a_Dan + " * " + idx + " = " + (a_Dan * idx));
        //    }
        //    Debug.Log("---");
        //}

        //while��
        //int a_bb = 1;
        //while(a_bb <= 10)
        //{
        //    Debug.Log("a_bb : " + a_bb);
        //    a_bb++;
        //}

        ////for������ �ٲ㺸��
        //for (int a_cc = 1; a_cc <= 10; a_cc++)
        //    Debug.Log("a_cc : " + a_cc);

        //do ~ while��
        //int a_kk = 10;
        //while(20 < a_kk)
        //{
        //    Debug.Log("while�� a_kk = " + a_kk); 
        //    a_kk++;
        //}

        //a_kk = 10;
        //do
        //{
        //    Debug.Log("Do ~ while�� a_kk = " + a_kk);
        //    a_kk++;
        //}while(20 < a_kk);

        int AAA = 0;
        int BBB = 0;


        AAA = Random.Range(1, 11);
        do
        {
            BBB = Random.Range(1, 11);
        } while (BBB == AAA);

        Debug.Log(AAA + " : " + BBB);



        AAA = Random.Range(1, 11);
        BBB = Random.Range(1, 11);
        while (BBB == AAA)
        {
            BBB = Random.Range(1, 11);
        }

        Debug.Log(AAA + " : " + BBB);


        int ABC = 0;
        //���ѷ���
        while(true)
        {
            ABC++;

            if ((ABC % 2) == 0)
                continue;      // while���� ���� ��ġ�� ���ư��� Ű���� (for���� ����)
            if (10 < ABC)
                break;        //while���� ��� ���� ������ Ű���� (for���� ����)

            Debug.Log("while ���ѷ��� �׽�Ʈ : " + ABC);
        }

        //for���� �̿��� ���ѷ���
        ABC = 0;
        for (; ; )
        {
            ABC++;

            if ((ABC % 2) == 0)
                continue;      // for���� ���� ��ġ�� ���ư��� Ű���� (while���� ����)
            if (10 < ABC)
                break;        //for���� ��� ���� ������ Ű���� (while���� ����)

            Debug.Log("for ���ѷ��� �׽�Ʈ : " + ABC);
        }
    }

    void Update()
    {
        
    }
}
