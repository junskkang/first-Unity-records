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
    Vector2 origin;
    Vector2 selectSize = new Vector2(120, 120);

    private Text unitDescription;
    private RawImage descriptionBack;

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
            Debug.Log(name);
            switch (name)
            {
                case "Warrior":
                    unitDescription.text = $"������ \n" +
                                           $"��  ��  �� : �ڡڡڡڡ� \n" +
                                           $"���ݼӵ� : �ڡڡڡ١� \n" +
                                           $"���ݹ��� : �ڡڡ١١� \n\n" +
                                           $"<size=20>���� �̿��Ͽ� �����ϰ� ���� �����ϴ� �뺴</size>\n\n" +
                                           $"Ư����ų : �����ϼ�";                                                
                    break;
                case "Mage":
                    unitDescription.text = $"������ \n" +
                                           $"��  ��  �� : �ڡڡڡڡ� \n" +
                                           $"���ݼӵ� : �ڡ١١١� \n" +
                                           $"���ݹ��� : �ڡڡڡڡ� \n\n" +
                                           $"<size=20>4�� �Ӽ��� ������ ��������� �ٷ�� ������</size>\n\n" +
                                           $"Ư����ų : ���׿�";
                    break;
                case "Hunter":
                    unitDescription.text = $"���� \n" +
                                           $"��  ��  �� : �ڡڡ١١� \n" +
                                           $"���ݼӵ� : �ڡڡڡڡ� \n" +
                                           $"���ݹ��� : �ڡڡڡڡ� \n\n" +
                                           $"<size=20>�ڿ��� ��ȣ�ϴ� ����</size>\n\n" +
                                           $"Ư����ų : �������� ��";
                    break;
                case "Priest":
                    unitDescription.text = $"������Ʈ \n" +
                                           $"��  ��  �� : �ڡ١١١� \n" +
                                           $"���ݼӵ� : �ڡڡڡ١� \n" +
                                           $"���ݹ��� : �ڡڡ١١� \n\n" +
                                           $"<size=20>���� �������� �Ʊ��� ��ȣ�ϴ� ����</size>\n\n" +
                                           $"Ư����ų : ����Ŭ���ý�";
                    break;
                case "Dancer":
                    unitDescription.text = $"�� \n" +
                                           $"��  ��  �� : �ڡ١١١� \n" +
                                           $"���ݼӵ� : �ڡڡ١١� \n" +
                                           $"���ݹ��� : �ڡڡڡڡ� \n\n" +
                                           $"<size=20>ȯ������ �������� ��θ� ��Ȥ�ϴ� ����</size>\n\n" +                                           
                                           $"Ư����ų : ��ÿ������";
                    break;
            }
        }
    }

    public void OnPointerEnter()
    {
        anim.speed = 1;
        rect.sizeDelta = selectSize;
        descriptionBack.gameObject.SetActive(true);
    }

    public void OnPointerExit()
    {
        anim.speed = 0;
        rect.sizeDelta = origin;
        descriptionBack.gameObject.SetActive(false);

    }
    
}
