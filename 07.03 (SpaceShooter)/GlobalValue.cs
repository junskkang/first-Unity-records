using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Skill_0 = 0,    //30% 체력 힐링
    Skill_1,        //수류탄
    Skill_2,        //보호막
    SkCount
}

public class GlobalValue
{
    public static string g_UniqueID = "";   //유저의 고유번호

    public static string g_NickName = "";   //유저의 별명. 유저가 변경 가능
    public static int g_BestScore = 0;
    public static int g_UserGold = 0;
    public static int g_Exp = 0;
    public static int g_Level = 0;

    public static int[] g_SkillCount = new int[3];  //아이템 보유 수
    public static void LoadGameDate()
    {
        g_NickName = PlayerPrefs.GetString("NickName", "뉴비");
        g_BestScore = PlayerPrefs.GetInt("BestScore", 0);
        g_UserGold = PlayerPrefs.GetInt("UserGold", 0);

        string a_MkKey = "";
        for (int i = 0; i < g_SkillCount.Length; i++)
        {
            a_MkKey = "SkItem_" + i.ToString();

            //테스트용 갯수 충전
            PlayerPrefs.SetInt(a_MkKey, 10);
            g_SkillCount[i] = PlayerPrefs.GetInt(a_MkKey, 1);
        }
    }

}
