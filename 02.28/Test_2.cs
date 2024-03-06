using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;


//변수 : 데이터를 저장하는 메모리 공간
//변수의 선언 : 데이터형 변수명 = 초기값;
public class Test_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int age = 30;
        string name;
        float height = 158.3f;

        age = 123; // 변수 age의 값을 123으로 초기화 함.
        age = 1004 + 1000 - 500; //우항의 결과값을 age 값으로 대입
       
        Debug.Log(age);

        float height1 = 160.5f; //C#에서 숫자 뒤에 f(float의 f)를 붙여줘야함.
        float height2;
        height2 = height1;
        Debug.Log(height2);

        double abc = 567.8;
        //double도 소수점을 담는 데이터형이고 8Byte를 차지함. 단,f를 붙이지 않음.
        //double도 실수값을 저장하는 공간이지만, double형이 float형보다 범위가 크다.
        //소수점의 범위가 크다 = 정밀하고 오차값이 적다.
        //float은 double에 대입하여도 허용은 해줌
        //굳이 float로 표현이 가능한 실수를 double로 만드는 것은 데이터 낭비이다.
        //낭비 = 최적화 똥망, 렉 유발 가능, 무거움

        // 1Byte = 8Bit , Bit 비트는 컴퓨터 데이터의 최소 단위
        short ABC = 32767; //short형의 최대치

        bool abab = true; //참 또는 거짓을 출력하는 데이터형
        Debug.Log(abab);
        abab = false;
        Debug.Log(abab);

        int ccc;       //변수의 선언
        ccc = 10;      //변수의 초기화

        int ddd = 10;  //변수의 선언과 동시에 초기화


        //Debug.Log(ccc);

        //char 처음엔 알파벳 문자를 저장하기 위한 용도로 만들어짐
        //c, c++ : char 1Byte 아스키코드
        char ccdd = 'k';
        //한글, 한자 등 1Byte에는 담을 수 없는 문자들이 많아짐
        //c# : char 2Byte 유니코드
        ccdd = '글';
        Debug.Log(ccdd);
        Debug.Log(sizeof(char));  //데이터형의 메모리 사이즈 확인하는 방법

        //string
        string strName;
        strName = "GDragon";
        Debug.Log(strName);

        strName = "반가워요 " + "서울 " + "오늘 날씨 좋아요.";
        //문자열과 문자열을 + 기호로 더하면 문자열이 합쳐짐
        //연산자 오버로딩 : 연산자의 의미를 재정의해서 사용하겠다는 의미
        Debug.Log(strName);

        byte ggg = 255; // 0~255까지 담을 수 있는 1Byte짜리 데이터형

        //var 초기값에 따라서 데이터형이 결정되는 데이터형
        var vv1 = 123;
        var vv2 = 3.14f;
        var vv3 = "seoul";
        var vv4 = true;

        //vv1 = "korea"; 에러가 남
        vv3 = "korea";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
