using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GawiBawiBo   // 
{
    Gawi = 1,
    Bowi = 2,
    Bo = 3    //마지막 원소에는 쉼표를 써도 생략해도 가능
}

public enum City  //원소들에 값을 주지 않을 경우
{
    Seoul,        //0   자동으로 순서대로 0부터 시작
    Inchoen,      //1
    Busan = 5,    //5   값을 준 곳부터 다시 1씩 증가
    Gwangju,      //6
    Jeju ,     //7
    Shinchon,     //8
    Count         //9 
}

public class Test_1 : MonoBehaviour
{

    void Start()
    {
        City MyCity = City.Busan;
        Debug.Log("My City : "+ MyCity + " : Index(" + (int)MyCity + ")");
        Debug.Log("Shinchon : " + " : Index(" + (int)City.Shinchon + ")");

        //int a_UserSel = 1;   //1이면 가위를 의미함
        //int a_ComSel = Random.Range(1, (3 + 1));

        //if (a_UserSel == a_ComSel)
        //    Debug.Log("비겼습니다");
        //else if (a_UserSel == 1 && a_ComSel == 3)
        //    Debug.Log("이겼습니다");
        //else
        //    Debug.Log("졌습니다");


        //위의 코드를 enum형으로 
        GawiBawiBo a_UserSel = GawiBawiBo.Gawi;
        GawiBawiBo a_ComSel = (GawiBawiBo)Random.Range((int)GawiBawiBo.Gawi,
             (int)GawiBawiBo.Bo + 1);

        if (a_UserSel == a_ComSel)
            Debug.Log("비겼습니다");
        else if (a_UserSel == GawiBawiBo.Gawi && a_ComSel == GawiBawiBo.Bo)
            Debug.Log("이겼습니다");
        else
            Debug.Log("졌습니다");
    }

    void Update()
    {
        
    }
}
