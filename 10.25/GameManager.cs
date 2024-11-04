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

        monsterRoadLayer = 1 << LayerMask.NameToLayer("InstallImpossible");
        existUnit = 1 << LayerMask.NameToLayer("Unit");
    }

    
    void Update()
    {
        MouseClick();

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
        if (Physics2D.Raycast(refHero.transform.position, -refHero.transform.forward, Mathf.Infinity, monsterRoadLayer.value))
        {
            Debug.Log("해당 위치에 유닛을 설치할 수 없습니다.");
            return;
        }

        worldPos = refHero.transform.position;
        Vector3 worldToCellPos = tileMap.WorldToCell(worldPos);
        worldToCellPos.x += 0.5f;
        worldToCellPos.y += 0.5f;
        
        if (Physics2D.Raycast(worldToCellPos, -refHero.transform.forward, Mathf.Infinity, existUnit.value))
        {
            Debug.Log("해당 위치에 유닛이 이미 설치되어 있습니다..");
            return;
        }

        if (allyLoadObject != null)
        {
            GameObject cloneObject = Instantiate(allyLoadObject[(int)ally]);
            if (cloneObject != null)
            {
                AllyUnit refAlly = GlobalValue.g_AllyList[(int)ally].MyAddComponent(cloneObject);                              



                refAlly.transform.position = worldToCellPos;
            }
        }
    }

    void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject target = null;

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(mousePos, Vector2.zero);
            hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, existUnit.value);
            //Debug.Log(mousePos);

            if (hit)
            {
                target = hit.collider.gameObject;
                target.GetComponent<AllyUnit>().UnitClick();
            }


            if (Physics2D.Raycast(mousePos, mousePos.normalized, Mathf.Infinity, existUnit.value))
            {
                Debug.Log("유닛에 히트");
            }

            
        }
    }

    
}
