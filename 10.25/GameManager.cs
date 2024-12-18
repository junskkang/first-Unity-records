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
    
    //유닛 정보패널
    Transform unitInfoPanel;
    AllyUnit clickUnit = null;
    public Text infoText;
    public Button unitLevelUpBtn;
    public Button unitRemoveBtn;

    //알림창
    public GameObject notifyObject;

    //줌인 카메라
    CameraResolution cam;

    //게임오버 판넬
    public GameObject gameOverPanel;

    //배속 재생을 위한 변수
    [HideInInspector] public int isFast = 1;

    //데미지 텍스트 관련 변수
    public Transform damageCanvas = null;   //유니티 연결용
    public GameObject damagePrefab = null;  //유니티 연결용
    GameObject damageClone;                 //복사본
    DamageCtrl damageText;                  //컴포넌트 받아오기용
    Vector3 startPos = Vector3.zero;

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
        cam = Camera.main.transform.GetComponent < CameraResolution>();
        GetGold(30);
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

        //Time.timeScale = 15.0f;
    }

    
    void Update()
    {
        if (NotifyCtrl.isNotify) return;

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

        if (Input.GetKeyDown(KeyCode.G))
        {
            GlobalValue.g_Gold += 10;
        }

        if (Input.GetKeyDown(KeyCode.R)) 
        {
            isFast++;            

            Time.timeScale = isFast;

            if (isFast == 3) isFast = 0;
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
            //Debug.Log(buildPos);
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
                cam.zoomTarget = null;
            }
                

            if (hit)
            {
                clickUnit = hit.collider.gameObject.GetComponent<AllyUnit>();
                clickUnit.GetComponent<AllyUnit>().UnitClick();
                unitInfoPanel.gameObject.SetActive(true);
                UnitInfo(clickUnit);
                cam.zoomTarget = hit.collider.gameObject;

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

            infoText.text = $"직업 : {unit.ally_Attribute.unitName}\n\n" +
                            $"레벨 : {unit.curLevel}\n\n" +
                            $"체력 : {unit.curHp.ToString("N0")}\n\n" +
                            $"공격력 : {unit.curAttDamage.ToString("N0")}\n" +
                            $"공격속도 : {unit.curAttSpeed.ToString("N1")}\n" +
                            $"공격범위 : {unit.curAttRange.ToString("N1")}\n\n" +
                            $"스킬공격력 : {unit.skillDamage.ToString("N0")}\n" +
                            $"스킬범위 : {unit.skillRange.ToString("N1")}\n";
        }

        if (unitLevelUpBtn != null)
        {
            if (unit.curLevel >= 10)
            {
                unitLevelUpBtn.GetComponentInChildren<Text>().text = "최대 레벨 달성";
                unitLevelUpBtn.interactable = false;
            }
            else
            {
                unitLevelUpBtn.GetComponentInChildren<Text>().text = $"레벨 상승 : {unit.levelUpCost}골";
                unitLevelUpBtn.interactable = true;
            }

            unitLevelUpBtn.onClick.RemoveAllListeners();
            unitLevelUpBtn.onClick.AddListener(() =>
            {
                unit.LevelUp();
                if (unit.curLevel >= 10)
                {
                    unitLevelUpBtn.GetComponentInChildren<Text>().text = "최대 레벨 달성";
                    unitLevelUpBtn.interactable = false;
                }
                else
                {
                    unitLevelUpBtn.GetComponentInChildren<Text>().text = $"레벨 상승 : {unit.levelUpCost}골";
                    unitLevelUpBtn.interactable = true;
                }

                infoText.text = $"직업 : {unit.ally_Attribute.unitName}\n\n" +
                $"레벨 : {unit.curLevel}\n\n" +
                $"체력 : {unit.curHp.ToString("N0")}\n\n" +
                $"공격력 : {unit.curAttDamage.ToString("N0")}\n" +
                $"공격속도 : {unit.curAttSpeed.ToString("N1")}\n" +
                $"공격범위 : {unit.curAttRange.ToString("N1")}\n\n" +
                $"스킬공격력 : {unit.skillDamage.ToString("N0")}\n" +
                $"스킬범위 : {unit.skillRange.ToString("N1")}\n";
            });
        }


        if (unitRemoveBtn != null)
        {
            unitRemoveBtn.GetComponentInChildren<Text>().text = $"유닛 판매 : {(int)(unit.resellCost * 0.7f)}골";
            unitRemoveBtn.onClick.RemoveAllListeners();
            unitRemoveBtn.onClick.AddListener(() =>
            {
                GlobalValue.g_Gold += (int)(unit.resellCost * 0.7f);
                unitInfoPanel.gameObject.SetActive(false);
                Destroy(unit.gameObject);
            });
        }
    }

    public void GetGold(int value)
    {
        if (value < 0 && GlobalValue.g_Gold < Mathf.Abs(value)) return;


        GlobalValue.g_Gold += value;

        if (pointText != null)
            pointText.text = $"GOLD : {GlobalValue.g_Gold}";
    }
    float alpha = 0.0f;
    Color alphaColor = Color.white;
    public IEnumerator GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.gameObject.SetActive(true);

            while (alpha <= 1.0f)
            {
                alpha += Time.unscaledDeltaTime * 10;
                Debug.Log(alpha);

                if (alpha >= 1.0f)
                    alpha = 1.0f;

                alphaColor = new Color(1.0f, 1.0f, 1.0f, alpha);
                gameOverPanel.GetComponent<Image>().color = alphaColor;
                if (alpha >= 1.0f)
                    yield break;

                yield return new WaitForSecondsRealtime(0.1f);
            }
        }
    }

    public void DamageText(int Value, Vector3 ownerPos, Color ownerColor)
    {
        damageClone = Instantiate(damagePrefab);
        if (damageClone != null && damageCanvas != null)
        {
            startPos = new Vector3(ownerPos.x, ownerPos.y + 0.2f, ownerPos.z);
            damageClone.transform.SetParent(damageCanvas);
            damageText = damageClone.GetComponent<DamageCtrl>();
            damageText.DamageSpawn(Value, ownerColor);
            damageClone.transform.position = startPos;
        }
    }
}
