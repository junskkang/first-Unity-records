using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitNode : MonoBehaviour
{
    private Image image;
    public Sprite sprite;
    private Animator anim;
    private RectTransform rect;
    int unitIdx = -1;
    Vector2 origin;
    Vector2 selectSize = new Vector2(120, 120);

    Color originColor = Color.white;
    Color disableColor = Color.black;
    Color notEnoughGoldColor = new Color(0.27f, 0.27f, 0.27f);

    [HideInInspector] public bool buildPossible = false;
    [HideInInspector] public bool unlockedPossible = false;
    [HideInInspector] public bool isUnlocked = false;

    private Text unitDescription;
    private RawImage descriptionBack;
    public Text priceText;

    private void Start()
    {
        image = this.GetComponent<Image>();
        if (image != null )
            sprite = image.sprite;

        anim = GetComponent<Animator>();
        if (anim != null )
            anim.speed = 0;

        rect = GetComponent<RectTransform>();
        if (rect != null)
            origin = rect.sizeDelta;

        descriptionBack = GetComponentInChildren<RawImage>(true);
        unitDescription = GetComponentInChildren<Text>(true);

        if (unitDescription != null)
        {
            string name = this.gameObject.name;
            
            name = name.Substring(8);
            //Debug.Log(name);
            switch (name)
            {
                case "Warrior":
                    unitDescription.text = $"워리어 \n" +
                                           $"공  격  력 : ★★★★☆ \n" +
                                           $"공격속도 : ★★★☆☆ \n" +
                                           $"공격범위 : ★★☆☆☆ \n\n" +
                                           $"<size=20>검을 이용하여 강력하게 적을 제압하는 용병</size>\n\n" +
                                           $"특수스킬 : 벽력일섬";
                    unitIdx = 0;
                    break;
                case "Mage":
                    unitDescription.text = $"메이지 \n" +
                                           $"공  격  력 : ★★★★★ \n" +
                                           $"공격속도 : ★☆☆☆☆ \n" +
                                           $"공격범위 : ★★★★☆ \n\n" +
                                           $"<size=20>4대 속성의 마법을 자유자재로 다루는 마법사</size>\n\n" +
                                           $"특수스킬 : 메테오";
                    unitIdx = 1;
                    break;
                case "Hunter":
                    unitDescription.text = $"헌터 \n" +
                                           $"공  격  력 : ★★☆☆☆ \n" +
                                           $"공격속도 : ★★★★★ \n" +
                                           $"공격범위 : ★★★★★ \n\n" +
                                           $"<size=20>원거리 공격이 가능한 자연을 수호하는 엘프</size>\n\n" +
                                           $"특수스킬 : 독수리의 눈";
                    unitIdx = 2;
                    break;
                case "Priest":
                    unitDescription.text = $"프리스트 \n" +
                                           $"공  격  력 : ★☆☆☆☆ \n" +
                                           $"공격속도 : ★★★☆☆ \n" +
                                           $"공격범위 : ★★☆☆☆ \n\n" +
                                           $"<size=20>신의 은총으로 아군을 수호하는 사제</size>\n\n" +
                                           $"특수스킬 : 에피클레시스";
                    unitIdx = 3;
                    break;
                case "Dancer":
                    unitDescription.text = $"댄서 \n" +
                                           $"공  격  력 : ★☆☆☆☆ \n" +
                                           $"공격속도 : ★★☆☆☆ \n" +
                                           $"공격범위 : ★★★★☆ \n\n" +
                                           $"<size=20>환상적인 몸짓으로 모두를 유혹하는 집시</size>\n\n" +                                           
                                           $"특수스킬 : 아첼레란도";
                    unitIdx = 4;
                    break;
            }
        }
    }

    private void Update()
    {
        if (!isUnlocked)
            UnlockedUnit(unitIdx);
        else if (isUnlocked)
            BuildEnable(unitIdx);
    }

    public void OnPointerEnter()
    {
        if (NotifyCtrl.isNotify) return;

        if (!unlockedPossible) return;

        if (isUnlocked)
            anim.speed = 1f;
        else
            anim.speed = 0.3f;

        rect.sizeDelta = selectSize;
        descriptionBack.gameObject.SetActive(true);
    }

    public void OnPointerExit()
    {
        anim.speed = 0;
        rect.sizeDelta = origin;
        descriptionBack.gameObject.SetActive(false);

    }

    void UnlockedUnit(int idx)
    {
        if (idx >= 0)
        {
            if (GlobalValue.g_Gold < GlobalValue.g_AllyList[idx].unlockCost)    //구매불가
            {
                image.color = disableColor;
                priceText.text = GlobalValue.g_AllyList[idx].unlockCost.ToString();
                priceText.color = Color.red;
                unlockedPossible = false;
            }
            else
            {
                image.color = disableColor;
                priceText.text = GlobalValue.g_AllyList[idx].unlockCost.ToString();
                priceText.color = Color.white;
                unlockedPossible = true;
            }
        }
    }
    void BuildEnable(int idx)
    {
        //각 유닛의 코스트값을 가져와서 글로벌밸류의 골드값이 그 코스트값과 비교
        if (idx >= 0) 
        {
            if (GlobalValue.g_Gold < GlobalValue.g_AllyList[idx].buildCost)    //구매불가
            {
                image.color = notEnoughGoldColor;
                priceText.text = GlobalValue.g_AllyList[idx].buildCost.ToString();
                priceText.color = Color.red;
                buildPossible = false;
            }
            else
            {
                image.color = originColor;
                priceText.text = GlobalValue.g_AllyList[idx].buildCost.ToString();
                priceText.color = Color.white;
                buildPossible = true;
            }
        }
            
    }
    
}
