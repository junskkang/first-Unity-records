using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Skill_0 = 0,        //30% 힐링
    Skill_1,            //수류탄
    Skill_2,            //보호막
    SkCount 
}

[System.Serializable]
public class ItemList
{
    public int[] SkList;
}

[System.Serializable]
public class FloorInfo
{
    public int CurFloor;
    public int BestFloor;
}

[System.Serializable]
public class SvRespon    //ServerResponse
{
    public string nick_name;
    public int best_score;
    public int game_gold;
    public string floor_info;  //이 필드는 문자열로 유지하고, 추가적으로 파싱이 필요함
    public string info;   //이 필드는 문자열로 유지하고, 추가적으로 파싱이 필요함
}

public class GlobalValue 
{
    public static string g_Unique_ID = "";  //유저의 고유번호

    public static string g_NickName = "";   //유저의 별명
    public static int g_BestScore = 0;      //게임점수
    public static int g_UserGold = 0;       //게임머니
    public static int g_Exp = 0;            //경험치 Experience
    public static int g_Level = 0;          //레벨

    public static int[] g_SkillCount = new int[3];  //아이템 보유수

    public static int g_BestFloor = 1;      //최종 도달(클리어) 건물 층수 
    public static int g_CurFloorNum = 1;    //현재 건물 층수

    public static void LoadGameData()
    {
        //PlayerPrefs.SetInt("UserGold", 99999);

        //g_NickName  = PlayerPrefs.GetString("NickName", "SBS영웅");
        //g_BestScore = PlayerPrefs.GetInt("BestScore", 0);
        //g_UserGold  = PlayerPrefs.GetInt("UserGold", 0);

        //string a_MkKey = "";
        //for(int i = 0; i < g_SkillCount.Length; i++)
        //{
        //    a_MkKey = "SkItem_" + i.ToString();
        //    //PlayerPrefs.SetInt(a_MkKey, 3);
        //    g_SkillCount[i] = PlayerPrefs.GetInt(a_MkKey, 1);
        //}

        //PlayerPrefs.SetInt("BestFloorNum", 99);
        //PlayerPrefs.SetInt("CurFloorNum", 99);

        //로컬에 층 로딩, 저장 부분 서버로 옮기기 위해 주석처리 (08/20)
        //g_BestFloor = PlayerPrefs.GetInt("BestFloorNum", 1);
        //g_CurFloorNum = PlayerPrefs.GetInt("CurFloorNum", 1);
    }//public static void LoadGameData()

    public static void ClearGameData()
    {
        g_Unique_ID = "";
        g_NickName = "";
        g_BestScore = 0;
        g_UserGold = 0;
        g_Exp = 0;
        g_Level = 0;

        g_BestFloor = 1;
        g_CurFloorNum = 1;

        for (int i = 0; i < g_SkillCount.Length; i++)
        {
            g_SkillCount[i] = 0;
        }
    }
}
