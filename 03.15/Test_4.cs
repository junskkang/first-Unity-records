using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_4 : MonoBehaviour
{

    void Start()
    {
        //1. 아래와 같은 배열이 있을 때 222 값은 몇 번째 인덱스에 있나요?
        // 인덱스를 찾는 코드를 작성하고 인덱스를 출력해 주세요.
        int[] App = { 34, 56, 12, 89, 120, 6, 8, 222, 67, 81, 110 };

        int aa = 0;
        while (App[aa] != 222)
        {
            aa++;
        }
        Debug.Log("해당 배열의 222값은 " + aa + "번째 인덱스에 있습니다.");

        //2. 아래와 같은 배열이 있을 때 33이라는 값은 몇 번 인덱스에 있을까요?
        //또, 99라는 값은 몇 번째 인덱스에 있을까요?
        //찾는 코드를 작성하고 인덱스를 출력해 주세요.
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

        //while문
        int find33 = 0;
        while (VVV[find33] != 33)
        {
            find33++;
        }
        Debug.Log("해당 배열의 33이라는 값은 " + find33 + "번째 인덱스에 있습니다.");
        
        int find99 = 0;
        while (VVV[find99] != 99)
        {
            find99++;
        }
        Debug.Log("해당 배열의 99이라는 값은 " + find99 + "번째 인덱스에 있습니다.");


        //for문
        for(int find = 0; find <= a_idx; find++ )
        {
            if (VVV[find] == 33)
                Debug.Log("해당 배열의 33이라는 값은 " + find + "번째 인덱스에 있습니다.");
            else if (VVV[find] == 99)
                Debug.Log("해당 배열의 99이라는 값은 " + find + "번째 인덱스에 있습니다.");
        }





        //3. 아래와 같은 배열이 있을 때 아래 보기와 같은 연산 후 결과를 출력해 주세요.
        int[] ZZZ = new int[100];
        for(int ii = 0; ii < 100; ii++)
        {
            ZZZ[ii] = ii;
        }

        int[] a_rr = new int[100];
        //보기
        //a_rr[0] = ZZZ[0] + ZZZ[1];
        //a_rr[1] = ZZZ[2] + ZZZ[3];
        //a_rr[2] = ZZZ[4] + ZZZ[5];
        //          :
        //a_rr[?] = ZZZ[98] + ZZZ[99];

        int yy = 0;
        int zz = 0;
        
        while (true)
        {
            a_rr[yy] = ZZZ[zz] + ZZZ[zz + 1];
            Debug.Log("a_rr[" + yy + "] : " + a_rr[yy]);
            yy++;
            zz+=2;
           
            if (zz == 98)
                break;
        }    



        //4. 
        int[] a_kk = { 23, 45, 12, 67, 34, 77, 103, 3, 6, 7, 8, 11, 65, 204, 33, 56 };
        //for문을 돌면서 최대값과 최솟값을 구하고 최대값의 인덱스와 최소값의 인덱스를 같이 출력해 주세요.
        //출력 예시 "최댓값 : ?? (인덱스 ??), 최솟값 : ?? (인덱스 ??)"

    }


    void Update()
    {
        
    }
}
