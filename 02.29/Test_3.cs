using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 연산자 
public class Test_3 : MonoBehaviour
{
  
    void Start()
    {
        //1. 수식연산자
        //int a = 88;
        //int b = 22;
        int a = 88, b = 22;  // 같은 데이터형을 선언할 때 콤마로 연속하여 선언 가능
        int c = a + b;
        Debug.Log(a+" + "+b +" = "+c);

        c = a - b;
        Debug.Log(a+" - "+b+" = "+c);

        //string str = string.Format("{0} * {1} = {2}", a, b, a * b); //Format문자열
        //순서대로 a의 값이 0에 들어가고, b의 값이 1에, a+b값이 2로 들어가는 함수
        string str = $"{a} * {b} = {a * b}"; //format문자열의 다른 표현법
        Debug.Log(str);

        //str = string.Format("{0} / {1} = {2}", a, b, a / b);
       
        Debug.Log($"{a} / {b} = {a / b}");

        //나머지 연산자
        Debug.Log($"{a} % {b} = {a % b}");
        //나눈 숫자보다 하나 작은 값까지 반복되는 특징이 있다. 
        //ex)3으로 나누면 나머지는 0, 1, 2를 반복함
        //    목           나머지
        // 0 / 2 = 0     0 % 2 = 0
        // 1 / 2 = 0     1 % 2 = 1
        // 2 / 2 = 1     2 % 2 = 0
        // 3 / 2 = 1     3 % 2 = 1
        // 4 / 2 = 2     4 % 2 = 0
        // 5 / 2 = 2     5 % 2 = 1

       
        
        //2. 증감연산자 c, c++, c#

        int cc = 0;
        cc++; // c = c + 1; 우항을 계산하여 좌항에 넣어줘
        ++cc; // c = c + 1; 단독으로 쓰일 때는 1 증가 시키라는 뜻. 위와 같음
        Debug.Log("단독 사용일 경우 : " + cc);

        cc = 0;
        Debug.Log(string.Format("복합 명령어로 사용될 경우 뒤에 붙일 때 : {0} ", cc++));
        //cc를 1증가시켜줘, 문자열로 만들어줘, 콘솔창에 출력해줘 3가지 명령이 존재
        //증감이 뒤에 붙었을 경우 다른 명령을 마치고 나서 증감을 함
        //때문에 1증가 시키는 것이 마지막이므로
        //결과값이 0
        Debug.Log(cc);
        //여기서 다시 Debug.Log()를 찍었을 땐 결과값이 1
        
        cc = 0;
        Debug.Log(string.Format("복합 명령어로 사용될 경우 앞에 붙일 때 : {0} ", ++cc));
        //증감이 앞에 붙었을 경우 가장 먼저 증감을 하고 다른 명령을 수행함
        //때문에 1증가 시키는 것이 먼저
        //결과값이 1


        int ff = 10;
        ff--; // ff = ff - 1; 
        --ff; // 단독으로 쓰일 때는 ++과 마찬가지로 1감소 시키라는 의미
        Debug.Log(string.Format("ff : {0}", ff)); //결과값은 8


         
        //3. 할당연산자 = 줄임 표현
        int a_xx = 10;
        a_xx += 5;  // a_xx = a_xx + 5;
        //결과값이 같은 3가지 표현
        //a_xx = a_xx + 1;    a_xx += 1;    a_xx++;
        a_xx -= 3;  // a_xx = a_xx - 3;

        int a_yy = 10;
        a_yy *= 2;  // a_yy = a_yy * 2;
        a_yy /= 2;  // a_yy = a_yy / 2;
        a_yy %= 2;  // a_yy = a_yy % 2;



        //4. 논리연산자
        int ggg = 50, hhh = 60;
        bool a_Check = ggg > 40 && hhh > 50;   // and 연산자   ~이고, 그리고
        //50이 40보다 크고 60이 50보다 크다. 두가지를 모두 만족하기 때문에 true
        Debug.Log("ggg > 40 && hhh > 50 : " + a_Check);
        // true  && true  = true
        // ture  && false = false
        // false && true  = false
        // false && false = false

        a_Check = ggg > 40 || hhh > 70;   // or 연산자  ~이거나, 또는
        Debug.Log("ggg > 40 || hhh > 50 : " + a_Check);
        //50이 40보다 크거나 60이 70보다 크다. 둘 중에 하나만 만족하여도 true
        // true  || true  = true
        // ture  || false = true
        // false || true  = true
        // false || false = false

        a_Check = (ggg > hhh);    // 50이 60보다 크다?  
        Debug.Log(a_Check);       // false
        a_Check = !(ggg > hhh);   // ! Not연산자    결과를 반전시키는 연산자
        Debug.Log(a_Check);       // true


        //5. 관계연산자
        int AAA = 50;
        int BBB = 60;
        Debug.Log("AAA < BBB : " + (AAA < BBB));     // true
        Debug.Log("AAA > BBB : " + (AAA > BBB));     // false
        Debug.Log("AAA == BBB : " + (AAA == BBB));     // false
        Debug.Log("AAA != BBB : " + (AAA != BBB));     // true
        Debug.Log("AAA <= BBB : " + (AAA <= BBB));     // true
        Debug.Log("AAA >= BBB : " + (AAA >= BBB));     // false


        //6. 비트연산자 10진수를 2진수로 표현해주는 방법, 각 자리수끼리 비교
        int nnn = 5;     // 0101
        int mmm = 10;    // 1010

        int Result = nnn & mmm;    // &&의 의미와 같음. 0000(2진수) ---> 0(10진수)
        Debug.Log("nnn & mmm : " + Result); //결과값 0


        Result = nnn | mmm;        // ||의 의미와 같음. 1111(2진수) ---> 15(10진수) 
        Debug.Log("nnn | mmm : " + Result); //결과값 15

        // 레이어 마스크 시에 활용
        // 00100010 비트단위로 쪼개서 각 자릿수마다 각각의 의미를 부여하고
        // 각 부분을 true/false로 사용할 수 있는 것

        // ^ XOR 연산자 : 두 값이 같으면 0, 두 값이 다르면 1
        Result = nnn ^ mmm;    //  1111(2진수)  --->  15(10진수)
        Debug.Log("nnn ^ mmm : " + Result); //결과값 15

        int kkk = 2357;                                      // 0000 1001 0011 0101
        int a_ScVal = kkk ^ 6789;  // 암호화   6789가 비밀번호! 0001 1010 1000 0101
        Debug.Log("a_ScVal : " + a_ScVal);  //결과값 5040       0001 0011 1011 0000
        int a_MyVal = a_ScVal ^ 6789;  //복호화   6789를 똑같이 한번 더 넣어주면?
        Debug.Log("a_MyVal : "+ a_MyVal);   //결과값 2357
    }

    void Update()
    {
        
    }
}
