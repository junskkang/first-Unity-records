using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue
{
    public static string g_Unique_ID = "";  //유저의 고유번호

    public static string g_NickName = "";   //유저의 별명
    public static int g_BestScore = 0;      //최고기록
    public static int g_MyPoint = 0;       //누적점수

    public static void ClearGameData()
    {
        g_Unique_ID = "";   //유저 고유번호
        g_NickName = "";   //유저의 별명
        g_BestScore = 0;    //최고기록
        g_MyPoint = 0;    //누적점수

    }//public static void ClearGameData()
}
