using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue
{
    public static string g_UniqueID = "";   //������ ������ȣ

    public static string g_NickName = "";   //������ ����. ������ ���� ����
    public static int g_BestScore = 0;
    public static int g_UserGold = 0;
    public static int g_Exp = 0;
    public static int g_Level = 0;

    public static void LoadGameDate()
    {
        g_NickName = PlayerPrefs.GetString("NickName", "����");
        g_BestScore = PlayerPrefs.GetInt("BestScore", 0);
        g_UserGold = PlayerPrefs.GetInt("UserGold", 0);
    }

}
