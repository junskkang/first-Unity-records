using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //���� ��ġ�� ���� Ÿ�� ��ǥ
    public Tilemap tileMap;
    Vector3 worldPos;
    LayerMask monsterRoadLayer = -1;
    LayerMask existUnit = -1;

    //���콺 Ŭ���� ���� ����
    Vector2 mousePos;
    RaycastHit2D hit;

    //UI
    [SerializeField] private Canvas canvas;
    [HideInInspector] public Transform unitBuildBack;
    private bool isBuild = false;
    
    //���� �����г�
    Transform unitInfoPanel;
    AllyUnit clickUnit = null;
    public Text infoText;
    public Button unitLevelUpBtn;
    public Button unitRemoveBtn;

    

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
        unitInfoPanel = canvas.transform.Find("UnitInfo");
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
            if (clickUnit != null) return;

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
            Debug.Log("�ش� ��ġ�� ������ ��ġ�� �� �����ϴ�.");
            Debug.Log(buildPos);
            return false;
        }

        if (Physics2D.Raycast(buildPos, -Vector3.forward, Mathf.Infinity, existUnit.value))
        {
            Debug.Log("�ش� ��ġ�� ������ �̹� ��ġ�Ǿ� �ֽ��ϴ�..");
            return false;
        }

        return true;
    }
    void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(mousePos, Vector2.zero);
            hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, existUnit.value);
            //Debug.Log(mousePos);

            if (clickUnit != null)
            {               
                clickUnit.GetComponent<AllyUnit>().UnitClick();
                clickUnit = null;
                unitInfoPanel.gameObject.SetActive(false);
            }
                

            if (hit)
            {
                clickUnit = hit.collider.gameObject.GetComponent<AllyUnit>();
                clickUnit.GetComponent<AllyUnit>().UnitClick();
                unitInfoPanel.gameObject.SetActive(true);
                UnitInfo(clickUnit);

                if (unitBuildBack.gameObject.activeSelf)
                {
                    isBuild = !isBuild;
                    unitBuildBack.gameObject.SetActive(isBuild);
                }
            }        
        }
    }    
    
    void UnitInfo(AllyUnit unit)
    {
        if (infoText != null)
        {
            infoText.text = "";

            infoText.text = $"���� : {unit.ally_Attribute.unitName}\n\n" +
                            $"���� : {unit.curLevel}\n\n" +
                            $"ü�� : {unit.curHp.ToString("N0")}\n\n" +
                            $"���ݷ� : {unit.curAttDamage.ToString("N0")}\n" +
                            $"���ݼӵ� : {unit.curAttSpeed.ToString("N1")}\n" +
                            $"���ݹ��� : {unit.curAttRange.ToString("N1")}\n\n" +
                            $"��ų���ݷ� : {unit.skillDamage.ToString("N0")}\n" +
                            $"��ų���� : {unit.skillRange.ToString("N1")}\n";
        }

        if (unitLevelUpBtn != null)
        {
            unitLevelUpBtn.onClick.RemoveAllListeners();
            unitLevelUpBtn.onClick.AddListener(() =>
            {
                unit.Levelup();

                infoText.text = $"���� : {unit.ally_Attribute.unitName}\n\n" +
                $"���� : {unit.curLevel}\n\n" +
                $"ü�� : {unit.curHp.ToString("N0")}\n\n" +
                $"���ݷ� : {unit.curAttDamage.ToString("N0")}\n" +
                $"���ݼӵ� : {unit.curAttSpeed.ToString("N1")}\n" +
                $"���ݹ��� : {unit.curAttRange.ToString("N1")}\n\n" +
                $"��ų���ݷ� : {unit.skillDamage.ToString("N0")}\n" +
                $"��ų���� : {unit.skillRange.ToString("N1")}\n";
            });
        }


        if (unitRemoveBtn != null)
        {
            unitRemoveBtn.onClick.RemoveAllListeners();
            unitRemoveBtn.onClick.AddListener(() =>
            {
                unitInfoPanel.gameObject.SetActive(false);
                Destroy(unit.gameObject);
            });
        }
    }
}
