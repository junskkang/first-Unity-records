using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 제어문
// if : 조건문, 분기문
// if(조건식)
// {
//     실행될 코드
// }
// else if(조건식)
// {
//     실행될 코드
// }
// else
// {
//     실행될 코드
// }
public class Test_1 : MonoBehaviour
{
    void Start()
    {
        int x = 1;
        if(x == 1)
        {
            int y = 2;
            Debug.Log(x);
            Debug.Log(y);
        }
        // [지역변수의 범위]
        // y = 11; //y는 if문 안에서 선언된 변수이므로 그 if안에서만 사용가능한 
        // 지역변수이므로 자기 소속 중괄호를 벗어난 y의 사용은 에러가 난다.

        x = 8;
        if (x < 5)
        {
            Debug.Log("x는 5보다 작습니다.");
        }
        // if문의 실행될 코드가 한 줄이면 { }중괄호를 생략할 수 있다.
        else if (x < 10)
        {
            Debug.Log("x는 5보다 크거나 같고");
            Debug.Log("x는 10보다 작습니다.");
        }
        else if(x < 15)
        {
            Debug.Log("x는 10보다 크거나 같고");
            Debug.Log("x는 15보다 작습니다.");
        }
        else
        {
            Debug.Log("x는 15보다 크거나 같습니다");
        }


        //if문의 3가지 패턴
        Debug.Log("---- if문의 첫번째 패턴");
        int xyz = 15;
        if (xyz == 4) 
        {
            Debug.Log("xyz는 4입니다.");
        }
        if (xyz == 5) 
        {
            Debug.Log("xyz는 5입니다.");
        }
        if (xyz == 6)
        {
            Debug.Log("xyz는 5입니다.");
        }
        if (xyz == 5)
        {
            Debug.Log("xyz는 5가 확실합니다.");
        }

        Debug.Log("---- if문의 두번째 패턴");
        if (xyz == 4)
            Debug.Log("xyz는 4입니다.");
        else if (xyz == 5)
            Debug.Log("xyz는 5입니다.");
        else if (xyz == 6)
            Debug.Log("xyz는 6입니다.");
        else if (xyz == 5)
            Debug.Log("xyz는 5가 확실합니다.");

        Debug.Log("---- if문의 세번째 패턴");
        if (xyz == 4)
            Debug.Log("xyz는 4입니다.");
        else if (xyz == 5)
            Debug.Log("xyz는 5입니다.");
        else if (xyz == 6)
            Debug.Log("xyz는 6입니다.");
        else
            Debug.Log("xyz는 4, 5, 6이 아닙니다.");

        //switch ~ case 문
        //switch (조건식)           // 조건식 결과로 나올 수 있는 값 : 정수형, 문자, 문자열
        //{
        //     case 상수 :
        //          실행될 코드;
        //     break;
        //
        //     case 상수 :
        //          실행될 코드;
        //     break;
        //
        //     default:             // if문에서 else로 끝나는 구문에 해당함.
        //          실행될 코드;    // if문의 else처럼 있어도 없어도 상관없음.
        //     break;
        //}

        int a_ii = 7;
        switch(a_ii % 2)
        {
            case 0:
                Debug.Log("짝수입니다.");
                break;
            case 1:
                Debug.Log("홀수입니다.");
                break;
        }
        //switch case문도 마찬가지로 위에서부터 순서대로 처리함.

        if ((a_ii % 2) == 0)
            Debug.Log("짝수입니다.");
        else if ((a_ii % 2) == 1)
            Debug.Log("홀수입니다.");
        //위의 switch case와 동일한 결과가 나오는 if문


        // switch ~ case 문과 if ~else if문으로 구현해 보세요.

        // a_Day = '월'; --> 오늘은 월요일입니다.
        // a_Day = '화'; --> 오늘은 화요일입니다.
        //....
        // a_Day = '글'; --> 해당하는 요일을 정확히 입력해 주세요.

        char a_Day = '화';    // c#에서 char형은 2byte(한글 한글자도 저장할 수 있다.)

        if (a_Day == '월')
        {
            Debug.Log("오늘은 월요일 입니다.");
        }
        else if (a_Day == '화')
            Debug.Log("오늘은 화요일 입니다.");
        else if (a_Day == '수')
            Debug.Log("오늘은 수요일 입니다.");
        else if (a_Day == '목')
            Debug.Log("오늘은 목요일 입니다.");
        else if (a_Day == '금')
            Debug.Log("오늘은 금요일 입니다.");
        else if (a_Day == '토')
            Debug.Log("오늘은 토요일 입니다.");
        else if (a_Day == '일')
            Debug.Log("오늘은 일요일 입니다.");
        else
            Debug.Log("해당하는 요일을 정확히 입력해 주세요.");

        switch(a_Day)
        {
            case '월':
                Debug.Log("오늘은 월요일 입니다");
                break;
            case '화':
                Debug.Log("오늘은 화요일 입니다");
                break;
            case '수':
                Debug.Log("오늘은 수요일 입니다");
                break;
            case '목':
                Debug.Log("오늘은 목요일 입니다");
                break;
            case '금':
                Debug.Log("오늘은 금요일 입니다");
                break;
            case '토':
                Debug.Log("오늘은 토요일 입니다");
                break;
            case '일':
                Debug.Log("오늘은 일요일 입니다");
                break;
            default:
                Debug.Log("해당하는 요일을 정확히 입력해 주세요.");
                break;
        }//switch(a_Day)

        switch(a_Day)
        {
            case '월':
            case '화':
                Debug.Log("오늘은 월요일 일수도, 화요일 일수도...");
                break;
        }

        if (a_Day == '월' || a_Day == '화')
            Debug.Log("오늘은 월요일 일수도, 화요일 일수도...");

    }//void Start()

    void Update()
    {
        //유니티 c#에서 랜덤값을 얻어오는 방법
        if(Input.GetKeyDown(KeyCode.R) == true) 
        {
            //int a_Rand = Random.Range(1, 101);   
            // Random.Range(최솟값, 최댓값);
            //1부터 100까지 랜덤한 숫자를 발생시켜줌
            // 최솟값보다 같거나 크고 최댓값보다는 작은 범위
            //Debug.Log(a_Rand);

            float a_FRd = Random.Range(0.0f, 10.0f);
            //0.0f ~ 10.0f 까지 랜덤한 숫자를 발생시켜 줌
            Debug.Log(a_FRd);
            //int와 다르게 float에서는 최댓값도 범위에 포함된다.
            //딱! 10까지만 출력되고 10.xxxx 소숫점은 나오지 않는다.
        }
        
    }
}
