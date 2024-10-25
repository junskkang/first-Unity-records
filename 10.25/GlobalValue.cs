using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllyType
{
    Warrior,
    Mage,
    Hunter,
    Priest,
    Dancer
}
public class GlobalValue
{
    public static string g_UserID = "";
    public static string g_NickName = "";
    public static int g_Round = 0;
    public static int g_BestScore = 0;

    public static List<Ally_Atrribute> g_AllyList = new List<Ally_Atrribute> ();    //���� ���� �Ʊ� ����Ʈ

    public static void LoadGameData()
    {
        if (g_AllyList.Count <= 0)
        {
            Ally_Atrribute node = new Warrior_Att();
            g_AllyList.Add(node);

            node = new Mage_Att();
            g_AllyList.Add(node);

            node = new Hunter_Att();
            g_AllyList.Add(node);

            node = new Priest_Att();
            g_AllyList.Add(node);

            node = new Dancer_Att();
            g_AllyList.Add(node);
        }

        Debug.Log(g_AllyList.Count);
    }
}
