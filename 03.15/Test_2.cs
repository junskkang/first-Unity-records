using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//반복문
// for문
// for(초기식; 조건식; 증감식)
// {
//      반복될 코드
// }

//while문
// while (조건식)
// {
//    반복될 코드
// }
public class Test_2 : MonoBehaviour
{
    void Start()
    {
        //for문 안에서 a변수의 변화값은 0~99 , 반복횟수는 100
        //for ( int a = 0; a < 100; a++)
        //{
        //    Debug.Log(a);
        //}

        //for( int xx = 10; xx > 0; xx--)
        //{ 
        //    Debug.Log(xx); 
        //}

        //Debug.Log("<구구단 7단>");
        //for (int i = 1; i<10; i++)
        //{
        //    Debug.Log("7 * " + i + " = " + (7 * i));
        //}

        ////이중 for문
        //Debug.Log("<구구단 2단에서 9단까지>");
        //for (int a_Dan = 2; a_Dan < 10; a_Dan++)
        //{
        //    Debug.Log("<구구단 " + a_Dan + "단>");
        //    for(int idx = 1; idx < 10; idx++)
        //    {
        //        Debug.Log(a_Dan + " * " + idx + " = " + (a_Dan * idx));
        //    }
        //    Debug.Log("---");
        //}

        //while문
        //int a_bb = 1;
        //while(a_bb <= 10)
        //{
        //    Debug.Log("a_bb : " + a_bb);
        //    a_bb++;
        //}

        ////for문으로 바꿔보자
        //for (int a_cc = 1; a_cc <= 10; a_cc++)
        //    Debug.Log("a_cc : " + a_cc);

        //do ~ while문
        //int a_kk = 10;
        //while(20 < a_kk)
        //{
        //    Debug.Log("while문 a_kk = " + a_kk); 
        //    a_kk++;
        //}

        //a_kk = 10;
        //do
        //{
        //    Debug.Log("Do ~ while문 a_kk = " + a_kk);
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
        //무한루프
        while(true)
        {
            ABC++;

            if ((ABC % 2) == 0)
                continue;      // while문의 시작 위치로 돌아가는 키워드 (for문도 동일)
            if (10 < ABC)
                break;        //while문을 즉시 빠져 나가는 키워드 (for문도 동일)

            Debug.Log("while 무한루프 테스트 : " + ABC);
        }

        //for문을 이용한 무한루프
        ABC = 0;
        for (; ; )
        {
            ABC++;

            if ((ABC % 2) == 0)
                continue;      // for문의 시작 위치로 돌아가는 키워드 (while문도 동일)
            if (10 < ABC)
                break;        //for문을 즉시 빠져 나가는 키워드 (while문도 동일)

            Debug.Log("for 무한루프 테스트 : " + ABC);
        }
    }

    void Update()
    {
        
    }
}
