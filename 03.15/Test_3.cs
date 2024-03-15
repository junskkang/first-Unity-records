using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Value Type의 변수 : int, float, double .... struct(구조체)
//Reference Type의 변수 : array, class객체

public class Test_3 : MonoBehaviour
{
    //유니티에서 배열을 맴버 변수로 선언해서 사용하는 2가지 방법
    int[] m_Brr = new int[5];     //맴버 배열 변수 첫번째 사용 예
    public int[] Crr;          // 맴버 배열 변수 두번째 사용 예 갯수를 정해주지 않음
   
  
    void Start()
    {
        //    int AA0 = 0;
        //    int AA1 = 0;    
        //    int AA2 = 0;

        //    int[] arr = new int[100];   //C# 배열 변수 선언

        int[] arr = new int[5];
        for (int a = 0; a < arr.Length; a++)
            arr[a] = 10 + a;

        for (int i = 0; i < 5; i++)
        {
            Debug.Log(arr[i]);
        }

        Debug.Log("arr 크기 : " + arr.Length);


        // 맴버 배열 변수 첫번째 사용 예시
        m_Brr[0] = 11;
        m_Brr[1] = 12;
        m_Brr[2] = 13;
        m_Brr[3] = 14;
        m_Brr[4] = 15;
        for(int ii = 0; ii < m_Brr.Length; ii++)
        {
            Debug.Log("m_Brr[" + ii + "] : " + m_Brr[ii]);
        }


        // 유니티에서 배열의 갯수와 값을 지정해준 사용 예시
        for(int ii = 0; ii< Crr.Length; ii++)
        {
            Debug.Log("Crr[" + ii + "] : " + Crr[ii]);
        }


        int[] a_AAA = new int[10];
        int[] a_BBB;                 //선언만 해놓는 것은 상관없음
        a_BBB = new int[10];       //단 사용하기 전에 무조건 몇 개를 사용할 것인지 값을 줘야함
        a_BBB[0] = 11;               //
        
        for(int ii = 0; ii< a_AAA.Length; ii++)
            Debug.Log(a_AAA[ii]);



        //암시적 선언
        int[] a_CCC = { 10, 20, 30, 40, 50, 60 };
        //배열의 선언과 동시에 인덱스 순서대로 초기화값을 지정해줌

        for(int ii = 0;ii< a_CCC.Length; ii++)
            Debug.Log(a_CCC[ii]);

        //int[] a_XXX;
        //a_XXX = { 1, 2, 3, 4, 5, 6 };

        //명시적+암시적 선언
        int[] a_EEE = new int[] { 10, 20, 30, 40, 50, 60 };
        int[] a_FFF;
        a_FFF = new int[] { 11, 12, 13, 14, 15 };
        for(int ii = 0;ii < a_FFF.Length; ii++)
            Debug.Log(a_FFF[ii]);



        int AAAAA = 1000;
        int BBBBB = AAAAA;   // 일반 변수들은 값이 복사된다라는 뜻에서 Value Type
        BBBBB = 99;
        // AAAAA = 1000, BBBBB = 99

        int[] CCCCC = { 1000 };
        int[] DDDDD = CCCCC;
        DDDDD[0] = 99;      //배열변수는 원본 저장공간을 참조한다는 뜻에서 Reference Type
        Debug.Log("CCCCC[0] : " + CCCCC[0] + ", DDDDD[0] " + DDDDD[0]);
        // CCCCC = 99, DDDDD = 99
        int[] EEEEE = CCCCC;
        // CCCCC = DDDDD = EEEEE 모두 하나의 공간을 공유하게 됨.

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
