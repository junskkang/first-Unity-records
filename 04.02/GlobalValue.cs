using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue           //글로벌변수로 관리 해야하는 정보들
{
    //유저의 정보
    public static string g_Unique_ID = "";        //유저의 고유번호
    public static string g_NickName = "";         //유저의 별명
    public static int g_BestScore = 0;            //게임점수
    public static int g_UserGold = 0;            //게임머니
    public static int g_Exp = 0;                  //경험치
    public static int g_Level = 0;                //레벨

    public static Chr_Stat g_CurSelCStat = null;  //현재 선택하고 있는 캐릭터 인덱스
    public static List<Chr_Stat> g_MyChrList = new List<Chr_Stat>(); //내가 보유하고 있는 캐릭터 리스트

    public static void LoadGameData()
    {
        if (g_MyChrList.Count <= 0)   //로딩은 한번만 되게 하기 위해서
        {
            //내 인벤토리에 보유하고 있는 캐릭터 목록 로딩 및 추가
            Chr_Stat a_CrNode = new Wizard_Stat("간달프", 70);
            g_MyChrList.Add(a_CrNode);

            a_CrNode = new Barbarian_Stat("바이킹", 10, 50);
            g_MyChrList.Add(a_CrNode );

            a_CrNode = new Archer_Stat("레골라스", 30);
            g_MyChrList.Add(a_CrNode);

            a_CrNode = new Healer_Stat("피오나", 20, 60);
            g_MyChrList.Add (a_CrNode );

        }
    }
}
