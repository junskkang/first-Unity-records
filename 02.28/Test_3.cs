using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//형변환 (Casting)
//1. 자동 형변환 : 서로 다른 데이터형 변수에 대입하거나 연산을 할 때 자동으로 형변환 되는 규칙
//                 단, 서로 다른 데이터형 일 때 큰 쪽으로만 상승 변환되는 특징이 있다.
// 데이터 형의 크기 순서
// double > float(4Byte) > ulong > long(8Byte) > uint > int > ushort > short > char
// 실수형 > 음양정수형 > 양의정수형 > 문자형
//2. 수동 형변환
public class Test_3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //1. 자동형변환
        int ii = 345;
        float ff = ii;  //자동 형변환 되어 ff의 값은 345.0f
        //int aa = ff;  //C#에서는 큰데이터형을 작은데이터형에 자동으로 대입 불가능
        Debug.Log("ii : " + ii + ", ff : " + ff);
        //문자열과 숫자를 더하면 숫자를 문자열로 변환하여 합쳐버림
        //ii와 ff는 숫자이지만 위의 Debug.Log에서 문자열과 합쳤기 때문에 문자열로 변환됨.
        //"ii : " + "345" + ", ff : " + "345"
        // Console 출력값 : ii : 345, ff : 345

        long aaa = 123L; //8Byte짜리 정수를 담는 데이터형 숫자 뒤에 L을 붙이는게 관행
        // 소문자는 숫자1과 구분이 안되니 대문자L을 붙여주도록 하자
        Debug.Log(aaa + " : " + sizeof(long));
        // "123" + " : " + "8"
        // "123 : 8"
        aaa = ii; //int인 ii를 long인 aaa에 형변환 가능 345L
        //ii = aaa; //long을 int에 담는 것은 불가능
        //aaa = ff; //float를 long에 담는 것은 불가능
        ff = aaa; //long을 float에 담는 것은 가능

        //2. 수동 형변환

        float a_ff = 12.34f;
        //int a_ii = a_ff; //int에 float를 담을 수 없음. 자동 형변환x
        int a_ii = (int)a_ff; // 수동 형변환(강제 형변환)
        Debug.Log("a_ii : " + a_ii);
        //소수점 값은 버려지고 int 정수형인 12 값만 남음

        //수동 형변환 활용하는 예
        //정수를 제외한 소숫점 값만 얻고 싶을 때
        float xxx = 123.456f;
        float Myfloat = xxx - (int)xxx; // 123.456f - 123.0f
        Debug.Log(Myfloat);
        //수동형변환을 연산식에서 바로 사용한 경우
        int MyInt = (int)xxx;
        float MyFloat = xxx- MyInt; // 123.456f - 123.0f
        Debug.Log(MyFloat);
        //수동형변환을 적용시킨 새로운 MyInt라는 변수를 만들어
        //그 변수를 연산식에 사용한 경우



        //숫자형태 <--> 문자열형태 변환
        int ABC = 123;
        string CBA = "123";

        int KKK = 123 + ABC; // 숫자 123 + 123 = 246
        string SSS = "123" + ABC; // 문자열 "123"+"123" = "123123"
                                  //경험치나 레벨이 오르는 것은 숫자의 값이 증가하지만, 화면에 표시는 문자열 표시임

        // 숫자형태 --> 문자열형태 변환
        ABC = 777;
        //CBA = ABC; //int인 ABC를 string인 CBA에 대입할 수 없음
        CBA = " " + ABC; //문자와 숫자를 더하는 연산식에선 자동형변환됨.
        CBA = ABC.ToString(); // .ToString으로 인해 숫자형태가 문자열 형태로 변환됨


        //문자열형태 --> 숫자형태
        string EEE = "123";
        //int FFF = EEE; //형변환으로는 안되고 함수를 써야 한다.
        //int FFF = int.Parse(EEE); // 특수 문자가 포함되면 에러가 난다.
        //Debug.Log(FFF);

        //int.TryParse(); //Parse();보다 안전한 함수
        int FFF = 0;
        int.TryParse("123", out FFF);
        Debug.Log(FFF);

        FFF = FFF + 5000; // 123+5000을 다시 FFF에 담는다라는 의미
        Debug.Log(FFF);

        float ppp = 0.0f;
        float.TryParse("123.456", out ppp);
        //0.0의 값을 가진 ppp에 문자열 "123.456"을
        //숫자형 123.456으로 변환하여 담는다는 의미
        Debug.Log("ppp : "+ ppp);

        
     }

    // Update is called once per frame
    void Update()
    {
        
    }
}
