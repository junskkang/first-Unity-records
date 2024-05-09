using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData
{
    public static int gameGold = 0;

    public static void SaveGold()
    {
        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetInt("GameGold", gameGold);
    }

    public static void LoadGold()
    {
        PlayerPrefs.GetInt("GameGold", 0);
    }
}
