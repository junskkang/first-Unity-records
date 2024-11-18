using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageCtrl : MonoBehaviour
{
    public AnimationCurve scaleCurve = new AnimationCurve();
    public AnimationCurve moveCurve = new AnimationCurve();
    public AnimationCurve alphaCurve = new AnimationCurve();

    public Text refText = null;
    Color textColor;
    int damageValue = 0;

    float startTime = 0.0f;
    float curTime = 0.0f;

    Vector3 cacScaleVector = Vector3.zero;
    float cacScale = 0.0f;
    Vector3 cacMoveVector = Vector3.zero;
    float moveOffset = 0.0f;
    float colorAlpha = 1.0f;


    void Start()
    {
        if (refText == null)
            refText = this.gameObject.GetComponentInChildren<Text>();

        if (refText != null)
        {
            //textColor = refText.color;
            if (damageValue < 0)
                refText.text = damageValue.ToString();
            else
                refText.text = "+" +damageValue.ToString();
        }

        startTime = Time.time;

        Destroy(this.gameObject, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        curTime = Time.time;

        cacScale = scaleCurve.Evaluate(curTime - startTime);
        cacScaleVector.x = cacScale;
        cacScaleVector.y = cacScale;
        cacScaleVector.z = 1.0f;
        refText.transform.localScale = cacScaleVector;

        moveOffset = moveCurve.Evaluate(curTime - startTime);
        cacMoveVector.y = moveOffset;
        refText.transform.localPosition = cacMoveVector;

        colorAlpha = alphaCurve.Evaluate(curTime - startTime);
        //textColor = refText.color;
        textColor.a = colorAlpha;
        refText.color = textColor;
    }


    public void DamageSpawn(int damage, Color color)
    {
        damageValue = damage;

        textColor = color;
    }
}


//������ ������ �ؽ�Ʈ
//public class DamageCtrl : MonoBehaviour
//{
//    float effectTime = 0.0f; //���� �ð� ���� ����
//    public Text DamageText = null; //TextUI ���ٿ� ����

//    �ӵ� = �Ÿ�/�ð�
//    float moveVelocity = 1.1f / 1.05f; //1.05�� ���ȿ� 1.1m���ٴ� �ӵ�
//    float alphaVelocity = 1.0f / (1.0f - 0.4f); //alpha�� 0.4�ʺ��� 1�ʱ��� 1�� ���ϴ� �ӵ�

//    Vector3 curPos = Vector3.zero;
//    Color color;

//    void Start()
//    {
//    }

//    Update is called once per frame
//    void Update()
//    {
//        effectTime += Time.deltaTime;

//        if (effectTime < 1.05f)
//        {
//            curPos = DamageText.transform.position;
//            curPos.y += Time.deltaTime * moveVelocity;
//            DamageText.transform.position = curPos;
//        }

//        if (0.4 < effectTime)
//        {
//            color = DamageText.color;
//            color.a -= Time.deltaTime * alphaVelocity;

//            if (color.a < 0)
//                color.a = 0;
//            DamageText.color = color;
//        }

//        if (1.05f < effectTime)
//            Destroy(gameObject);
//    }

//    public void DamageSpawn(int damage, Color color)
//    {
//        if (DamageText == null)
//            DamageText = this.GetComponentInChildren<Text>();

//        if (damage <= 0.0f)
//        {
//            int dmg = (int)Mathf.Abs(damage);  //���밪 �Լ�
//            DamageText.text = "- " + dmg;
//        }
//        else
//        {
//            DamageText.text = "+ " + (int)damage;
//        }

//        color.a = 1.0f;
//        DamageText.color = color;
//    }
//}
