using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    int curPoint = 0;
    public Text pointText;

    public int round;

    GameObject[] allyLoadObject = new GameObject[5];
    Hero_Ctrl refHero = null;

    public static GameManager Inst;

    private void Awake()
    {
        Inst = this;

        round = 0;
    }

    void Start()
    {
        GlobalValue.LoadGameData();
        for (int i = 0; i < GlobalValue.g_AllyList.Count; i++)
        {
            allyLoadObject[i] = Resources.Load($"Ally_{(int)GlobalValue.g_AllyList[i].type}") as GameObject;

            //Debug.Log(allyLoadObject[i]);
        }
        refHero = GameObject.FindObjectOfType<Hero_Ctrl>();

    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UnitBuild(AllyType.Warrior);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UnitBuild(AllyType.Mage);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UnitBuild(AllyType.Hunter);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UnitBuild(AllyType.Priest);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UnitBuild(AllyType.Dancer);
        }

    }

    public void AddPoint(int value = 10)
    {
        curPoint += value;

        if (pointText != null)
            pointText.text = $"POINT : {curPoint}";
    }

    void UnitBuild(AllyType ally)
    {
        if (allyLoadObject != null)
        {
            GameObject cloneObject = Instantiate(allyLoadObject[(int)ally]);
            if (cloneObject != null)
            {
                AllyUnit refAlly = GlobalValue.g_AllyList[(int)ally].MyAddComponent(cloneObject);
                refAlly.transform.position = refHero.transform.position;
            }
        }
    }
}
