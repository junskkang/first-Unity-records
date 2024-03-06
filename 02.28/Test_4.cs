using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_4 : MonoBehaviour
{

    void Start()
    {
        //2월 28일 스크립트 출력문 연습하기
        //구구단 7단을 Console View에 출력하시오

        //Debug.Log("<구구단 7단>");
        //int a = 7;
        //int sum;
        //for (int i = 1; i < 10; i++)
        //{
        //    sum = a * i;
        //    Debug.Log(a + " * " + i + " = " + sum);
        //}

        //for문을 사용한 다른 방식
        //for (int x = 1; x <= 9; x++)
        //{
        //    Debug.Log(7 + " * " + x + " = " + (7 * x));
        //}

        //for문을 사용하지 않은 정공법
        //Debug.Log("<구구단 7단>");
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









        //구구단 2단부터 9단까지 Console View에 출력하시오
        //    int sum;
        //    for(int a = 2; a<10; a++)
        //    {
        //        Debug.Log("<구구단 " + a + "단>");
        //        for (int i = 1; i < 10; i++)
        //        {
        //            sum = a * i;
        //            Debug.Log(a + " * " + i + " = " + sum);
        //        }

        //    }

        //}

        //1부터 10까지의 합구하기
        Debug.Log("1부터 10까지의 합구하기");

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