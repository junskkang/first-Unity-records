using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //유닛 설치를 위한 타일 좌표
    public Tilemap tileMap;
    Vector3 worldPos;
    LayerMask monsterRoadLayer = -1;
    LayerMask existUnit = -1;

    //마우스 클릭을 위한 변수
    Vector2 mousePos;
    RaycastHit2D hit;

    //UI
    [SerializeField] private Canvas canvas;
    [HideInInspector] public Transform unitBuildBack;
    private bool isBuild = false;

    GameObject clickUnit = null;

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
        unitBuildBack = canvas.transform.Find("UnitBuildBack");
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

        monsterRoadLayer = 1 << LayerMask.NameToLayer("InstallImpossible");
        existUnit = 1 << LayerMask.NameToLayer("Unit");
    }

    
    void Update()
    {
        MouseClick();

        KeyboardControl();

    }
    void KeyboardControl()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBuild = !isBuild;
            unitBuildBack.gameObject.SetActive(isBuild);
        }

    }
    public void AddPoint(int value = 10)
    {
        curPoint += value;

        if (pointText != null)
            pointText.text = $"POINT : {curPoint}";
    }

    public void UnitBuild(AllyType ally, Vector2 buildPos)
    {      
        if (allyLoadObject != null)
        {
            GameObject cloneObject = Instantiate(allyLoadObject[(int)ally]);
            if (cloneObject != null)
            {
                AllyUnit refAlly = GlobalValue.g_AllyList[(int)ally].MyAddComponent(cloneObject);                              



                refAlly.transform.position = buildPos;
            }
        }
    }
    public bool BuildPossible(Vector2 buildPos)
    {
        if (Physics2D.Raycast(buildPos, -Vector3.forward, Mathf.Infinity, monsterRoadLayer.value))
        {
            Debug.Log("해당 위치에 유닛을 설치할 수 없습니다.");
            Debug.Log(buildPos);
            return false;
        }

        if (Physics2D.Raycast(buildPos, -Vector3.forward, Mathf.Infinity, existUnit.value))
        {
            Debug.Log("해당 위치에 유닛이 이미 설치되어 있습니다..");
            return false;
        }

        return true;
    }
    void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {           
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(mousePos, Vector2.zero);
            hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, existUnit.value);
            //Debug.Log(mousePos);

            if (hit)
            {
                clickUnit = hit.collider.gameObject;
                clickUnit.GetComponent<AllyUnit>().UnitClick();
            }
            else if (!hit && clickUnit != null)
            {
                clickUnit.GetComponent<AllyUnit>().UnitClick();
                clickUnit = null;

            }          
        }
    }    
}
