using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public UnitNode[] unitBuildBtn;
    public Image mouseImg;
    public Image isPossibleImg;
    bool isPossible;

    Color possibleColor = new Color(0.26f, 0.89f, 0.32f, 0.72f);
    Color impossibleColor = new Color(1f, 0f, 0f, 0.72f);

    int saveIdx = -1;
    Sprite saveSprite;
    Vector3 saveVec = Vector3.zero;

    Vector3 origin = new Vector3(0, 0);
    Vector3 hide = new Vector3(0, -200);
    float hideSpeed = 400.0f;

   
    
    public static BuildManager instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        unitBuildBtn = gameObject.GetComponentsInChildren<UnitNode>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseImg != null)
        {
            if (Input.GetMouseButtonDown(0))
                UnitBuildDown();
            if (Input.GetMouseButton(0))
                UnitBuildDrag();
            if (Input.GetMouseButtonUp(0))
                UnitBuildUp();
        }
    }

    bool IsCollNode(GameObject check)
    {
        Vector3[] v = new Vector3[4];
        check.GetComponent<RectTransform>().GetWorldCorners(v);

        if (v[0].x <= Input.mousePosition.x && Input.mousePosition.x <= v[2].x &&
            v[0].y <= Input.mousePosition.y && Input.mousePosition.y <= v[2].y)
        {
            return true;
        }

        return false;
    }

    void UnitBuildDown()
    {
        
        saveIdx = -1;
        for (int i = 0; i < unitBuildBtn.Length; i++)
        {
            if (unitBuildBtn[i].gameObject.activeSelf &&
                IsCollNode(unitBuildBtn[i].gameObject))
            {
                //�ر� ��
                if (unitBuildBtn[i].unlockedPossible && !unitBuildBtn[i].isUnlocked)
                {
                    GameManager.Inst.GetGold(-GlobalValue.g_AllyList[i].unlockCost);
                    unitBuildBtn[i].isUnlocked = true;
                    GameManager.Inst.notifyObject.SetActive(true);
                    GameManager.Inst.notifyObject.GetComponent<NotifyCtrl>().NotifyOn(GlobalValue.g_AllyList[i].unitName);

                    return;
                }


                //�ر� ��
                if (unitBuildBtn[i].isUnlocked && unitBuildBtn[i].buildPossible)
                {                    
                    saveIdx = i;
                    mouseImg.sprite = unitBuildBtn[i].sprite;
                    isPossibleImg.gameObject.SetActive(true);
                    break;
                }

            }
        }
    }

    void UnitBuildDrag()
    {
        if (0 <= saveIdx)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float mouseX = Mathf.RoundToInt(mousePos.x);
            float mouseY = Mathf.RoundToInt(mousePos.y);
            Vector3 newVec = new Vector3(mouseX, mouseY, 0f);
            saveVec = newVec;
            //Debug.Log(newVec);

            if (GameManager.Inst.BuildPossible(newVec))
            {
                isPossibleImg.color = possibleColor;
                isPossible = true;
            }
            else
            {
                isPossibleImg.color = impossibleColor;
                isPossible = false;
            }

            
            Vector3 view = Camera.main.WorldToScreenPoint(newVec);
            isPossibleImg.transform.position = view;

            GameManager.Inst.unitBuildBack.GetComponent<RectTransform>().anchoredPosition = 
                Vector2.MoveTowards(GameManager.Inst.unitBuildBack.GetComponent<RectTransform>().anchoredPosition, hide, hideSpeed * Time.deltaTime);
        }
            
    }

    void UnitBuildUp()
    {
        if (saveIdx < 0) return;

        GameManager.Inst.unitBuildBack.GetComponent<RectTransform>().anchoredPosition =
            Vector2.Lerp(GameManager.Inst.unitBuildBack.GetComponent<RectTransform>().anchoredPosition,
            origin, hideSpeed * Time.deltaTime);

        if (isPossible == false)
        {
            isPossibleImg.gameObject.SetActive(false);
            saveIdx = -1;
            saveVec = Vector3.zero;
            return;
        }                   
        
        GameManager.Inst.UnitBuild((AllyType)saveIdx, saveVec);
        GameManager.Inst.GetGold(-GlobalValue.g_AllyList[saveIdx].buildCost);
        isPossibleImg.gameObject.SetActive(false);
        saveIdx = -1;
        saveVec = Vector3.zero;
    }
}
