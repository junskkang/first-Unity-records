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
                    unitDescription.text = $"������ \n" +
                                           $"��  ��  �� : �ڡڡڡڡ� \n" +
                                           $"���ݼӵ� : �ڡڡڡ١� \n" +
                                           $"���ݹ��� : �ڡڡ١١� \n\n" +
                                           $"<size=20>���� �̿��Ͽ� �����ϰ� ���� �����ϴ� �뺴</size>\n\n" +
                                           $"Ư����ų : �����ϼ�";
                    unitIdx = 0;
                    break;
                case "Mage":
                    unitDescription.text = $"������ \n" +
                                           $"��  ��  �� : �ڡڡڡڡ� \n" +
                                           $"���ݼӵ� : �ڡ١١١� \n" +
                                           $"���ݹ��� : �ڡڡڡڡ� \n\n" +
                                           $"<size=20>4�� �Ӽ��� ������ ��������� �ٷ�� ������</size>\n\n" +
                                           $"Ư����ų : ���׿�";
                    unitIdx = 1;
                    break;
                case "Hunter":
                    unitDescription.text = $"���� \n" +
                                           $"��  ��  �� : �ڡڡ١١� \n" +
                                           $"���ݼӵ� : �ڡڡڡڡ� \n" +
                                           $"���ݹ��� : �ڡڡڡڡ� \n\n" +
                                           $"<size=20>���Ÿ� ������ ������ �ڿ��� ��ȣ�ϴ� ����</size>\n\n" +
                                           $"Ư����ų : �������� ��";
                    unitIdx = 2;
                    break;
                case "Priest":
                    unitDescription.text = $"������Ʈ \n" +
                                           $"��  ��  �� : �ڡ١١١� \n" +
                                           $"���ݼӵ� : �ڡڡڡ١� \n" +
                                           $"���ݹ��� : �ڡڡ١١� \n\n" +
                                           $"<size=20>���� �������� �Ʊ��� ��ȣ�ϴ� ����</size>\n\n" +
                                           $"Ư����ų : ����Ŭ���ý�";
                    unitIdx = 3;
                    break;
                case "Dancer":
                    unitDescription.text = $"�� \n" +
                                           $"��  ��  �� : �ڡ١١١� \n" +
                                           $"���ݼӵ� : �ڡڡ١١� \n" +
                                           $"���ݹ��� : �ڡڡڡڡ� \n\n" +
                                           $"<size=20>ȯ������ �������� ��θ� ��Ȥ�ϴ� ����</size>\n\n" +                                           
                                           $"Ư����ų : ��ÿ������";
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
            if (GlobalValue.g_Gold < GlobalValue.g_AllyList[idx].unlockCost)    //���źҰ�
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
        //�� ������ �ڽ�Ʈ���� �����ͼ� �۷ι������ ��尪�� �� �ڽ�Ʈ���� ��
        if (idx >= 0) 
        {
            if (GlobalValue.g_Gold < GlobalValue.g_AllyList[idx].buildCost)    //���źҰ�
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
